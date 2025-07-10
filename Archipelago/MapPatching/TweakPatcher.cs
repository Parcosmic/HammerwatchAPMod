using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using ARPGGame;
using HammerwatchAP.Util;

namespace HammerwatchAP.Archipelago
{
    public static class TweakPatcher
    {
        static Random random;
        static ArchipelagoData archipelagoData;

        const int MAX_ITEM_NAME_LENGTH_TO_DISPLAY = 30;
        const float IMPERFECT_OBFUSCATE_CHANCE = 0.5f;

        public static void EditTweaks(string assetsPath, string mapTweakDir, XElement[] sharedUpgrades, bool shopsanity, int upgradeShuffleMode, float minCostMod, float maxCostMod, ArchipelagoData generateArchipelagoData, Random generateRandom)
        {
            random = generateRandom;
            archipelagoData = generateArchipelagoData;
            //Read info from tweak files
            string[] tweakFiles = Directory.GetFiles(Path.Combine(assetsPath, "tweak"));
            int obfuscationMode = archipelagoData.GetOption(SlotDataKeys.shopsanityTrapObfuscation);
            foreach (string file in tweakFiles)
            {
                string fileName = Path.GetFileName(file);
                if (fileName.Contains("shared") || fileName.Contains("general")) continue; //We already handled the shared files, and we don't touch the general tweaks
                XDocument doc = XDocument.Load(file);
                XElement upgradeRootNode = doc.Root.Element("upgrades");
                //Add shared tweaks to this class node
                foreach (XElement sharedUpgrade in sharedUpgrades)
                {
                    upgradeRootNode.Add(new XElement(sharedUpgrade));
                }
                if (shopsanity)
                {
                    SetTweaksShopsanity(fileName, upgradeRootNode, obfuscationMode);
                }
                else
                {
                    if (upgradeShuffleMode == 1)
                        ShuffleTweakFileKeepGroups(upgradeRootNode, random);
                    else if (upgradeShuffleMode == 2)
                        ShuffleTweakFile(upgradeRootNode, random);
                }
                RandomizeShopCosts(upgradeRootNode, minCostMod, maxCostMod, random);
                doc.Save(Path.Combine(mapTweakDir, fileName));
            }
        }
        private static XElement ShuffleTweakFileKeepGroups(XElement upgradeRootNode, Random random)
        {
            //Init category list
            Dictionary<string, string> groupCategories = new Dictionary<string, string>();
            //Iterate through upgrades and add group categories to the dict
            XElement[] upgradeNodes = upgradeRootNode.Elements().ToArray();
            foreach (XElement upgrade in upgradeNodes)
            {
                string category = upgrade.Attribute("cat").Value;
                string categoryType = category == "power" ? category : category.Substring(0, category.Length - 1);
                string upgradeName = upgrade.Attribute("id").Value;
                string groupName = upgradeName.Substring(0, upgradeName.Length - 1);
                groupCategories[groupName] = categoryType;
            }
            //Distribute categories
            List<string> categories = new List<string>();
            foreach (string category in groupCategories.Values)
            {
                categories.Add(category);
            }
            string[] groupNames = groupCategories.Keys.ToArray();
            foreach (string group in groupNames)
            {
                int categoryIndex = random.Next(0, categories.Count);
                groupCategories[group] = categories[categoryIndex];
                categories.RemoveAt(categoryIndex);
            }
            //Randomize categories
            foreach (XElement upgrade in upgradeNodes)
            {
                string groupName = upgrade.Attribute("id").Value;
                groupName = groupName.Substring(0, groupName.Length - 1);
                string groupCategory = groupCategories[groupName];
                XAttribute catAttribute = upgrade.Attribute("cat");
                if (groupCategory != "power")
                {
                    string last = catAttribute.Value[catAttribute.Value.Length - 1].ToString();
                    int level = 1;
                    if (int.TryParse(last, out int parse))
                    {
                        level = parse;
                    }
                    groupCategory += level.ToString();
                }
                catAttribute.Value = groupCategory;
            }
            return upgradeRootNode;
        }
        private static XElement ShuffleTweakFile(XElement upgradeRootNode, Random random)
        {
            //Init category list
            Dictionary<int, List<string>> categoryLevels = new Dictionary<int, List<string>>();
            for (int i = 0; i < 5; i++)
            {
                categoryLevels[i] = new List<string>();
            }
            //Iterate through upgrades and add them to the dict
            XElement[] upgradeNodes = upgradeRootNode.Elements().ToArray();
            foreach (XElement upgrade in upgradeNodes)
            {
                string category = upgrade.Attribute("cat").Value;
                string last = category[category.Length - 1].ToString();
                int level = 1;
                if (int.TryParse(last, out int parse)) //The upgrade might come from the power shop, so we need to check if the last character is a letter
                {
                    level = parse;
                }
                categoryLevels[level - 1].Add(category);
            }
            //Randomize categories
            foreach (XElement upgrade in upgradeNodes)
            {
                XAttribute catAttribute = upgrade.Attribute("cat");
                string last = catAttribute.Value[catAttribute.Value.Length - 1].ToString();
                int level = 1;
                if (int.TryParse(last, out int parse))
                {
                    level = parse;
                }
                int randomIndex = random.Next(0, categoryLevels[level - 1].Count);
                catAttribute.Value = categoryLevels[level - 1][randomIndex];
                categoryLevels[level - 1].RemoveAt(randomIndex);
            }
            return upgradeRootNode;
        }
        private static void SetTweaksShopsanity(string fileName, XElement upgradeRootNode, int obfuscationMode)
        {
            if (!APData.classFromString.TryGetValue(fileName, out PlayerClass playerClass))
                return;
            Dictionary<string, XElement> upgradeIdToNode = new Dictionary<string, XElement>();
            foreach (XElement upgrade in upgradeRootNode.Elements())
            {
                upgradeIdToNode[upgrade.Attribute("id").Value] = upgrade;
            }
            foreach (XElement upgrade in upgradeRootNode.Elements().ToArray())
            {
                XElement newTweak = RandomizeFromTweak(playerClass, upgrade, upgradeIdToNode, obfuscationMode);
                if (newTweak == null) continue;
                upgradeRootNode.Add(newTweak);
            }
        }
        private static XElement RandomizeFromTweak(PlayerClass playerClass, XElement upgrade, Dictionary<string, XElement> upgradeIdToNode, int obfuscationMode)
        {
            string upgradeId = upgrade.Attribute("id").Value;
            int locId = APData.GetLocIdFromUpgradeId(playerClass, upgradeId);
            if (locId == -1)
                return null;
            int vanillaCost = int.Parse(upgrade.Attribute("cost").Value);
            string category = upgrade.Attribute("cat").Value;
            NetworkItem item = archipelagoData.GetItemFromLoc(locId);
            if (item.Item == -1)
                return null;
            if (archipelagoData.GetOption(SlotDataKeys.shopsanityBalanceCosts) > 0)
            {
                switch (item.Flags)
                {
                    case ItemFlags.NeverExclude:
                        vanillaCost = (int)(vanillaCost * 0.5f);
                        break;
                    case ItemFlags.None:
                        vanillaCost = (int)(vanillaCost * 0.1f);
                        break;
                }
            }
            if (item.Flags.HasFlag(ItemFlags.Trap) && obfuscationMode > 0)
            {
                item = GetFakeItem(item, obfuscationMode);
            }
            else
            {
                //If the obfuscation mode is set to mystery ALL items need to be obscured
                if (obfuscationMode != 5)
                    obfuscationMode = 0;
            }

            string itemName = ModifyShopItemName(ArchipelagoManager.GetItemName(item), obfuscationMode);
            string displayName = itemName;
            if (displayName.Length > MAX_ITEM_NAME_LENGTH_TO_DISPLAY)
            {
                displayName = displayName.Substring(0, MAX_ITEM_NAME_LENGTH_TO_DISPLAY) + "...";
            }

            string descriptionHeader = "";
            if (item.Player != ArchipelagoManager.connectionInfo.playerId)
            {
                descriptionHeader = $"{ArchipelagoManager.connectionInfo.GetPlayerName(item.Player)}'s item{GetItemItemFlagsString(item)}\n";
            }
            if (itemName.Length > MAX_ITEM_NAME_LENGTH_TO_DISPLAY) //If the name would be too large for the name box put it in the description too
            {
                descriptionHeader += $"{itemName}\n";
            }
            string actualDescription = APData.GetShopItemDescription(item);
            string description = descriptionHeader + ModifyShopItemDesc(actualDescription, obfuscationMode);

            //Remove category info from vanilla upgrade so it doesn't appear in the shop
            upgrade.Attribute("cat").Value = "";

            XElement newTweak = NodeHelper.CreateTweakNode($"ap-{locId}", vanillaCost, category, displayName, description, true);
            return newTweak;
        }
        private static XElement RandomizeShopCosts(XElement upgradeRootNode, float minCostMod, float maxCostMod, Random random)
        {
            XElement[] upgradeNodes = upgradeRootNode.Elements().ToArray();
            foreach (XElement upgrade in upgradeNodes)
            {
                XAttribute costAttribute = upgrade.Attribute("cost");
                int cost = int.Parse(costAttribute.Value);
                float mod = (float)random.NextDouble() * (maxCostMod - minCostMod) + minCostMod;
                costAttribute.Value = ((int)(cost * mod)).ToString();
            }
            return upgradeRootNode;
        }
        private static string GetItemItemFlagsString(NetworkItem item)
        {
            string itemFlagsString = "";
            if (item.Flags.HasFlag(ItemFlags.Advancement))
            {
                itemFlagsString += "P";
            }
            if (item.Flags.HasFlag(ItemFlags.NeverExclude))
            {
                itemFlagsString += "U";
            }
            if (item.Flags.HasFlag(ItemFlags.Trap))
            {
                itemFlagsString += "T";
            }
            if (item.Flags == ItemFlags.None)
            {
                itemFlagsString += "F";
            }
            if (itemFlagsString.Length > 0)
            {
                itemFlagsString = " (" + itemFlagsString + ")";
            }
            return itemFlagsString;
        }
        public static NetworkItem GetFakeItem(NetworkItem trueItem, int obfuscationMode)
        {
            NetworkItem fakeItem = trueItem;
            List<NetworkItem> otherPlayerItems = new List<NetworkItem>();
            foreach (NetworkItem otherItem in archipelagoData.locationToItem.Values)
            {
                if (fakeItem.Player == otherItem.Player && otherItem.Flags != ItemFlags.Trap)
                    otherPlayerItems.Add(otherItem);
            }
            if (otherPlayerItems.Count > 0)
            {
                fakeItem = otherPlayerItems[random.Next(otherPlayerItems.Count)];
                fakeItem.Location = trueItem.Location;
                if (obfuscationMode == 2) //If obfuscation mode is imperfect switch the item classification
                    fakeItem.Flags = MiscHelper.RandomFromList(random, itemFlags);
            }
            else
            {
                GameData itemGameData = ArchipelagoManager.gameData[ArchipelagoManager.connectionInfo.GetPlayerGame(trueItem.Player)];
                fakeItem.Item = itemGameData.ItemLookup.Values.ToArray()[random.Next(itemGameData.ItemLookup.Count)];
                fakeItem.Flags = MiscHelper.RandomFromList(random, itemFlags);
            }
            return fakeItem;
        }
        private static List<ItemFlags> itemFlags = new List<ItemFlags>() { ItemFlags.None, ItemFlags.NeverExclude, ItemFlags.Advancement };

        public static string ModifyShopItemName(string name, int obfuscationMode)
        {
            string newName = name;
            newName = newName.Replace("Progressive", "Prog.");
            newName = ObfuscateString(newName, obfuscationMode, false);
            return newName;
        }
        public static string ModifyShopItemDesc(string desc, int obfuscationMode)
        {
            string newName = desc;
            newName = ObfuscateString(newName, obfuscationMode, true);
            return newName;
        }
        private static string ObfuscateString(string text, int obfuscationMode, bool description)
        {
            Dictionary<string, string> replaceRules = new Dictionary<string, string>();
            switch (obfuscationMode)
            {
                case 2: //Imperfect
                    Dictionary<char, char> charReplaceRules = new Dictionary<char, char>
                    {
                        ['l'] = 'I',
                        ['.'] = ' ',
                        ['b'] = 'd',
                        ['p'] = 'q',
                        ['u'] = 'w',
                        ['I'] = 'l',
                        [' '] = '_',
                        ['d'] = 'b',
                        ['q'] = 'p',
                        ['w'] = 'u'
                    };
                    string newText = "";
                    foreach (char c in text)
                    {
                        if (charReplaceRules.TryGetValue(c, out char newChar) && random.Next() < IMPERFECT_OBFUSCATE_CHANCE)
                        {
                            newText += newChar;
                            continue;
                        }
                        newText += c;
                    }
                    break;
                case 3: //Leetspeak
                    replaceRules["a"] = "4";
                    replaceRules["e"] = "3";
                    replaceRules["g"] = "6";
                    replaceRules["i"] = "1";
                    replaceRules["o"] = "0";
                    replaceRules["s"] = "5";
                    replaceRules["t"] = "7";
                    replaceRules["A"] = "4";
                    replaceRules["E"] = "3";
                    replaceRules["G"] = "6";
                    replaceRules["I"] = "1";
                    replaceRules["O"] = "0";
                    replaceRules["S"] = "5";
                    replaceRules["T"] = "7";
                    break;
                case 4: //Owo
                    replaceRules["r"] = "w";
                    replaceRules["l"] = "w";
                    replaceRules["R"] = "W";
                    replaceRules["L"] = "W";
                    break;
                case 5: //Mystery
                    if (description)
                    {
                        string[] lines = text.Split('\n');
                        text = lines[0] + "\n";
                        return text + "Does something?";
                    }
                    else
                    {
                        return "???";
                    }
            }
            foreach (var replaceRule in replaceRules)
            {
                text = text.Replace(replaceRule.Key, replaceRule.Value);
            }
            return text;
        }
    }
}
