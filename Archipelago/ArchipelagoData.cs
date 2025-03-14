using Archipelago.MultiClient.Net.Models;
using ARPGGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using HammerwatchAP.Game;
using HammerwatchAP.Util;

namespace HammerwatchAP.Archipelago
{
    public class ArchipelagoData
    {
        public string seed;
        public int itemIndexCounter;
        public int itemsReceived;
        public Dictionary<string, object> slotData;
        public string[] playerGames;

        public string mapFileName;

        //Check data
        public List<NetworkItem> itemsToReceive = new List<NetworkItem>();
        public List<long> checkedLocations = new List<long>();
        public HashSet<long> allLocalLocations = new HashSet<long>();
        public Dictionary<long, NetworkItem> locationToItem = new Dictionary<long, NetworkItem>();
        public bool completedGoal;

        //In game data
        public MapType mapType;
        public GoalType goalType;
        public string currentLevelName;
        public int currentAct;
        public Dictionary<string, bool> gameModifiers = new Dictionary<string, bool>();

        //Archipelago mod variables
        public int planks;
        public int panFragments;
        public int leverFragments;
        public int pickaxeFragments;
        public int hammerFragments;

        public int plankHuntRequirement;
        public int totalPanFragments;
        public int totalLeverFragments;
        public int totalPickaxeFragments;
        public int totalHammerFragments;

        public int[,] actKeys = new int[4, 4];
        public bool[,] hasMasterFloorKeys = new bool[4, 12];
        public bool[] killedBosses;
        public int[] bossRunes;
        public int pofRaiseLevel;
        public int wormCounter = 0;
        public int neededPlayers;
        public PlayerClass?[] shopsanityClasses;
        public string saveFileName;

        public enum MapType
        {
            Castle,
            Temple,
        }
        public enum GoalType
        {
            BeatAllBosses,
            PlankHunt,
            FullCompletion,
            Alternate,
        }
        public void CheckLocation(Vector2 pos, bool showPickupMessage = false)
        {
            if (!ArchipelagoManager.playingArchipelagoSave) return;
            long locId = GetLocationIdFromPos(pos, currentLevelName);
            CheckLocation(locId, showPickupMessage);
        }
        public void CheckLocation(long locationID, bool showPickupMessage)
        {
            if (locationID == -1 || !ArchipelagoManager.playingArchipelagoSave) return;
            if (allLocalLocations != null && !allLocalLocations.Contains(locationID)) return; //This item is a randomized location, but it isn't active this game
            if (!checkedLocations.Contains(locationID))
            {
                Console.WriteLine($"Found check: {locationID}");
                checkedLocations.Add(locationID);
                if (ArchipelagoManager.connectionInfo.ConnectionActive)
                {
                    ArchipelagoManager.connectionInfo.session.Locations.CompleteLocationChecksAsync(new long[] { locationID });
                }
            }
            //Show a pickup popup if the item belongs to someone else
            if (showPickupMessage)
            {
                NetworkItem item = GetItemFromLoc(locationID);
                ArchipelagoManager.AddPickupMessageToQueue(item);
            }
        }

        public void Update(ArchipelagoData otherData)
        {
            throw new NotImplementedException();
        }
        public void PickupItemEffectsXml(string xmlName, bool receive) //For item effects based on the xml name of the item
        {
            //Logging.GameLog("PickupItemEffectsXml receive: " + xmlName);
            bool hasPickaxe = false;
            bool hasFryingPan = false;
            bool hasPumpsLever = false;
            bool hasHammer = false;
            int keyAct = -1;
            int keyType = -1;
            int keyCount = 1;

            switch (xmlName)
            {
                case "items/tool_pickaxe.xml": //Pickaxe
                    hasPickaxe = true;
                    break;
                case "items/tool_pan.xml": //Frying Pan
                    hasFryingPan = true;
                    break;
                case "items/tool_lever.xml": //Pumps Lever
                    hasPumpsLever = true;
                    break;
                case "items/tool_hammer.xml": //Hammer
                    ++hammerFragments;
                    hasHammer = true;
                    break;
                case "items/tool_pickaxe_fragment.xml": //Pickaxe Fragment
                    if (++pickaxeFragments >= totalPickaxeFragments)
                        hasPickaxe = true;
                    break;
                case "items/tool_pan_fragment.xml": //Frying Pan Fragment
                    if (++panFragments >= totalPanFragments)
                        hasFryingPan = true;
                    break;
                case "items/tool_lever_fragment.xml": //Pumps Lever Fragment
                    if (++leverFragments >= totalLeverFragments)
                        hasPumpsLever = true;
                    break;
                case "items/tool_hammer_fragment.xml": //Hammer Fragment
                    if (++hammerFragments >= totalHammerFragments)
                        hasHammer = true;
                    break;
                case "items/collectable_2.xml": //Strange Plank
                    if (++planks <= 12)
                        GameInterface.SetGlobalFlag($"l{planks}_plank", true);
                    if (!receive)
                        ArchipelagoManager.AddTranslatedMessageToQueue("ig.secret-plank");
                    if (goalType == GoalType.PlankHunt && planks >= plankHuntRequirement)
                    {
                        ArchipelagoManager.CompleteGoal(GoalType.PlankHunt);
                    }
                    break;
            }
            if (xmlName.Contains("key"))
            {
                if (xmlName.Contains("prison"))
                    keyAct = 0;
                else if (xmlName.Contains("armory"))
                    keyAct = 1;
                else if (xmlName.Contains("archives"))
                    keyAct = 2;
                else if (xmlName.Contains("chambers"))
                    keyAct = 3;
                if (xmlName.Contains("bronze"))
                    keyType = 0;
                else if (xmlName.Contains("silver"))
                    keyType = 1;
                else if (xmlName.Contains("gold"))
                    keyType = 2;
                else if (xmlName.Contains("bonus"))
                    keyType = 3;
                if (xmlName.Contains("big"))
                    keyCount = 3;
                //Logging.GameLog("Key item. keyAct: " + keyAct + " | keyType: " + keyType + " | keyCount: " + keyCount);
                if (keyAct != -1 && actKeys[0, keyType] != -1)
                {
                    actKeys[keyAct, keyType] += keyCount;
                    int keyIndex = keyType == 3 ? 10 : keyType;
                    if (keyAct != currentAct - 1)
                    {
                        int keys = GameBase.Instance.Players[0].GetKey(keyIndex);

                        for (int k = 0; k < keyCount; k++)
                        {
                            if (keys == 0)
                            {
                                GameBase.Instance.Players[0].SetKeys(keyIndex, -keyCount + k);
                                break;
                            }
                            else
                            {
                                GameBase.Instance.Players[0].ConsumeKey(keyIndex);
                                keys--;
                            }
                        }
                    }
                }
            }

            if (hasPickaxe)
            {
                GameInterface.SetGlobalFlag("quest_rocks_solved", true);
                GameInterface.SetGlobalFlag("quest_pickaxe_icon_updated", true);

                ArchipelagoManager.AddTranslatedMessageToQueue("d.quest.pickaxe");
            }
            if (hasFryingPan)
            {
                GameInterface.SetGlobalFlag("quest_cookingmama_solved", true);
                GameInterface.SetGlobalFlag("quest_pan_icon_updated", true);
                //SetGlobalFlag.GlobalFlags["quest_pan_text_updated"] = true;

                ArchipelagoManager.AddTranslatedMessageToQueue("d.quest.pan");
            }
            if (hasPumpsLever)
            {
                GameInterface.SetGlobalFlag("quest_pumps_solved", true);
                GameInterface.SetGlobalFlag("quest_lever_icon_updated", true);
                //SetGlobalFlag.GlobalFlags["quest_lever_text_updated"] = true;

                ArchipelagoManager.AddTranslatedMessageToQueue("d.quest.lever");
            }
            if (hasHammer)
            {
                GameInterface.SetGlobalFlag("has_hammer", true);

                ArchipelagoManager.AddTranslatedMessageToQueue("Found a sturdy hammer...");
            }
        }
        public MapType GetMapType(int goal)
        {
            switch (goal / 10)
            {
                case 0:
                    return MapType.Castle;
                case 1:
                    return MapType.Temple;
                default:
                    return MapType.Castle;
            }
        }
        public void SetMapType()
        {
            mapType = GetMapType(GetOption("goal"));
        }
        public void SetGoalType()
        {
            switch (GetOption("goal") % 10)
            {
                case 0:
                    goalType = GoalType.BeatAllBosses;
                    break;
                case 1:
                    goalType = GoalType.PlankHunt;
                    break;
                case 2:
                    goalType = GoalType.FullCompletion;
                    break;
                case 3:
                    goalType = GoalType.Alternate;
                    break;
            }
        }
        public void SetSlotData(Dictionary<string, object> newSlotData)
        {
            slotData = newSlotData;

            SetMapType();
            SetGoalType();

            plankHuntRequirement = GetOption("planks_required_count");
            totalPanFragments = GetOption("pan_fragments");
            totalLeverFragments = GetOption("lever_fragments");
            totalPickaxeFragments = GetOption("pickaxe_fragments");
            totalHammerFragments = GetOption("hammer_fragments");

            int keyMode = GetOption("key_mode");
            if (keyMode != 1 || mapType == MapType.Temple)
            {
                actKeys[0, 0] = -1;
                actKeys[0, 1] = -1;
                actKeys[0, 2] = -1;
                if (!(keyMode == 2 && GetOption("randomize_bonus_keys") == 0) || mapType == MapType.Temple)
                {
                    actKeys[0, 3] = -1;
                }
            }
            killedBosses = new bool[GetActCount()];
            gameModifiers = GetSlotJObject<Dictionary<string, bool>>("game_modifiers");
            neededPlayers = 1;
            shopsanityClasses = new PlayerClass?[4];
            for (int p = 0; p < 4; p++)
            {
                int shopsanity = GetOption($"shopsanity_p{p + 1}");
                if (shopsanity == 0) continue;
                neededPlayers = p + 1;
                shopsanityClasses[p] = (PlayerClass)(shopsanity - 1);
            }
        }
        public object GetSlotObj(string key)
        {
            //Console.WriteLine(key);
            return slotData[key];
        }
        public T GetSlotValue<T>(string key)
        {
            return (T)slotData[key];
        }
        public int GetSlotInt(string key)
        {
            return (int)(long)GetSlotObj(key);
        }
        //public static string GetSlotString(string key)
        //{
        //    if (!TryGetSlotObj(key, out object objValue))
        //        ResourceContext.Log($"Failed to get slot data key: {key}");
        //    return (string)objValue;
        //}
        public Newtonsoft.Json.Linq.JObject GetSlotJObject(string key)
        {
            return GetSlotValue<Newtonsoft.Json.Linq.JObject>(key);
        }
        public T GetSlotJObject<T>(string key)
        {
            return GetSlotJObject(key).ToObject<T>();
        }
        public int GetOption(string option)
        {
            return GetSlotInt(option);
        }
        public int GetRandomLocation(string rLoc)
        {
            return (int)(long)GetSlotJObject<Dictionary<string, object>>("Random Locations")[rLoc];
        }
        public bool TryGetRandomLocation(string rLoc, out int rLocValue)
        {
            if (GetSlotJObject<Dictionary<string, object>>("Random Locations").TryGetValue(rLoc, out object rLocObj))
            {
                rLocValue = (int)(long)rLocObj;
                return true;
            }
            rLocValue = 0;
            return false;
        }
        public int GetShopLocation(string shopLoc)
        {
            object val = GetSlotJObject<Dictionary<string, object>>("Shop Locations")[shopLoc];
            return (int)(long)val;
        }
        public int GetEnemyShuffleMode()
        {
            return GetOption("enemy_shuffle_mode");
        }
        public bool IsShopSanityOn()
        {
            if (GetOption("shopsanity_p1") > 0 || GetOption("shopsanity_p2") > 0 || GetOption("shopsanity_p3") > 0 || GetOption("shopsanity_p4") > 0)
                return true;
            return false;
        }

        public Difficulty GetDifficulty()
        {
            int difficultyIndex = GetOption("difficulty");
            return (Difficulty)difficultyIndex;
        }
        public int GetActCount()
        {
            switch (mapType)
            {
                case MapType.Castle:
                    return 4;
                case MapType.Temple:
                    return 3;
                default:
                    return 4;
            }
        }

        public string GetStartExitCode()
        {
            object startExitObj = GetSlotObj("Start Exit");
            string startCode = APData.exitIdToCode[(int)(long)startExitObj];
            return startCode;
        }
        public bool IsPositionLinked(string levelFile, Vector2 pos)
        {
            switch (mapType)
            {
                case MapType.Castle:
                    if (APData.castleLinkedPositions.ContainsKey(levelFile) && APData.castleLinkedPositions[levelFile].ContainsKey(pos))
                    {
                        return true;
                    }
                    break;
                case MapType.Temple:
                    if (APData.templeLinkedPositions.ContainsKey(levelFile) && APData.templeLinkedPositions[levelFile].ContainsKey(pos))
                    {
                        return true;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return false;
        }
        public Vector2 GetLinkedPos(string levelFile, Vector2 pos, out string linkedLevel)
        {
            linkedLevel = levelFile;
            switch (mapType)
            {
                case MapType.Castle:
                    if (APData.castleLinkedPositions.ContainsKey(levelFile) && APData.castleLinkedPositions[levelFile].ContainsKey(pos))
                    {
                        linkedLevel = APData.castleLinkedPositions[levelFile][pos].Item1 ?? levelFile;
                        return APData.castleLinkedPositions[levelFile][pos].Item2;
                    }
                    break;
                case MapType.Temple:
                    return pos; //Yes we have a linked positions list, it's not needed
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return pos; //We return the original position if it isn't linked
        }
        public long GetLocationIdFromPos(Vector2 pos, string levelId)
        {
            long locId = -1;
            switch (mapType)
            {
                case MapType.Castle:
                    if (APData.castlePosToLocationId.ContainsKey(levelId) && APData.castlePosToLocationId[levelId].ContainsKey(pos))
                    {
                        locId = APData.castlePosToLocationId[levelId][pos];
                        locId += APData.castleStartID;
                    }
                    break;
                case MapType.Temple:
                    if (APData.templePosToLocationId.ContainsKey(levelId) && APData.templePosToLocationId[levelId].ContainsKey(pos))
                    {
                        locId = APData.templePosToLocationId[levelId][pos];
                        locId += APData.templeStartID;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (locId == -1)
                return -1;
            return locId;
        }
        public long GetGenLocationIdFromPos(Vector2 pos, string levelFile)
        {
            pos = GetLinkedPos(levelFile, pos, out string linkedLevel); //Returns pos if position is not linked
            return GetLocationIdFromPos(pos, linkedLevel);
        }
        public NetworkItem GetItemFromLoc(long locId)
        {
            if (locationToItem.TryGetValue(locId, out NetworkItem item))
                return item;
            return new NetworkItem() { Item = -1 };
        }
        public NetworkItem GetItemFromPos(Vector2 pos, string levelId)
        {
            long locId = GetLocationIdFromPos(pos, levelId);
            return GetItemFromLoc(locId);
        }
        public NetworkItem GetGenItemFromPos(Vector2 pos, string levelId)
        {
            long locId = GetGenLocationIdFromPos(pos, levelId);
            return GetItemFromLoc(locId);
        }

        /// <summary>
        /// Gets the xml name of the item that is located at the given position
        /// </summary>
        /// <param name="levelFile"></param>
        /// <param name="pos"></param>
        /// <returns>The xml name of the item at the location. Alternatively returns null if the location was not found and the item should stay the same, or "" if the item has been collected already.</returns>
        public string GetItemXmlNameFromPos(string levelFile, Vector2 pos, bool trueItem = false)
        {
            string xmlName = null;
            long locationId = GetGenLocationIdFromPos(pos, levelFile);
            //If this location was checked (likely by someone playing the same slot beforehand) there shouldn't be an item there
            if (checkedLocations.Contains(locationId) && ArchipelagoManager.IsItemOurs(GetItemFromLoc(locationId), false)) return "";
            if (locationToItem.ContainsKey(locationId))
            {
                xmlName = ArchipelagoManager.GetItemXmlName(locationToItem[locationId], false, trueItem);
            }
            return xmlName;
        }
        public string GetItemNameFromPos(string levelFile, Vector2 pos)
        {
            NetworkItem item = GetItemFromPos(pos, levelFile);
            if (item.Item == -1)
                return null;
            return ArchipelagoManager.GetItemName(item);
        }

        public void GameUpdate(int ms)
        {
            //Item receiving
            if (itemsToReceive.Count > itemsReceived)
            {
                ArchipelagoMessageManager.receivedItemPopupItems = itemsToReceive.Count - itemsReceived;
            }
            while (itemsReceived < itemsToReceive.Count)
            {
                int itemsToGetCounter = Math.Min(ArchipelagoMessageManager.MAX_RECEIVE_ITEM_UPDATE_COUNT, itemsToReceive.Count - itemsReceived);
                if (itemsToGetCounter-- <= 0)
                    break;
                NetworkItem itemToReceive = itemsToReceive[itemsReceived++];
                ArchipelagoMessageManager.announceMessageQueue.Add(ArchipelagoManager.GetReceiveItemMessage(itemToReceive));
                ArchipelagoManager.ReceiveItem(itemToReceive);
            }
        }
    }
}
