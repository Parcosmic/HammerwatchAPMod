using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using OpenTK;
using TiltedEngine;
using ARPGGame;
using HammerwatchAP.Util;

namespace HammerwatchAP.Archipelago
{
    public static class APMapPatcher
    {
        static Random random;
        static ArchipelagoData archipelagoData;

        public const string AP_ASSETS_FOLDER = "archipelago-assets";

        private static Dictionary<string, int> treasureCounts;

        private static Dictionary<string, string> gateTypes;
        private static bool exitRando = false;
        private static Dictionary<string, string> exitSwaps;
        private static Dictionary<string, char> exitCodeToActChar = new Dictionary<string, char>();

        private static List<int> globalScriptNodesToTriggerOnLoad;
        private static List<int> globalScriptNodesToTriggerOnceOnLoad;

        private static readonly HashSet<string> treasureNames = new HashSet<string>()
        {
            "items/valuable_1.xml",
            "items/valuable_2.xml",
            "items/valuable_3.xml",
            "items/valuable_4.xml",
            "items/valuable_5.xml",
            "items/valuable_6.xml",
            "items/valuable_7.xml",
            "items/valuable_8.xml",
            "items/valuable_9.xml",
            "items/breakable_barrel.xml",
            "items/breakable_barrel_v2.xml",
            "items/breakable_barrel_b.xml",
            "items/breakable_barrel_b_v2.xml",
            "items/breakable_crate.xml",
            "items/breakable_crate_v2.xml",
            "items/breakable_crate_b.xml",
            "items/breakable_vase.xml",
            "items/breakable_vase_v2.xml",
            "items/breakable_vase_v3.xml",
            "items/breakable_vase_v4.xml",
            "items/vgt_plant.xml",
        };
        private static readonly Dictionary<string, string> spawnerDoodads = new Dictionary<string, string>
        {
            { "actors/spawners/archer_1.xml", "doodads/special/marker_grave.xml" },
            { "actors/spawners/archer_2.xml", "doodads/special/marker_grave.xml" },

            { "actors/spawners/skeleton_1.xml", "doodads/special/marker_grave.xml" },
            { "actors/spawners/skeleton_2.xml", "doodads/special/marker_grave.xml" },

            { "actors/spawners/maggot_1.xml", "doodads/special/marker_maggot_1.xml" },
        };
        private static readonly Dictionary<string, Dictionary<string, string>> spawnerDoodadPositionCorrections = new Dictionary<string, Dictionary<string, string>>()
        {
            { "level_12.xml", new Dictionary<string, string>{
                { "25 -82.375", "25 -82.5" },
            }
            }
        };
        private static readonly Dictionary<string, Dictionary<string, string>> floorSignExits = new Dictionary<string, Dictionary<string, string>>()
        {
            {"level_1.xml", new Dictionary<string, string>{
            { "-47.5 -41.5", "esc_3|1"}, //Not an entrance yet
            { "34.5 -52.5", "3|10"},
            { "49.5 -45.5", "2|0"},
            { "-37.5 -2", "3|1"},
            { "11.5 -29.5", "2|2"},
            { "16.5 -7.5", "2|1"},
            }},
            {"level_2.xml", new Dictionary<string, string>{
            { "-16.5 -33.5", "3|0"},
            { "49.5 -44.5", "1|1"},
            { "8.5 -27.5", "1|3"},
            { "16.5 -6.5", "1|2"},
            }},
            {"level_3.xml", new Dictionary<string, string>{
            { "-12.5 -33.5", "2|3"},
            { "-38.5 -1.5", "1|4"},
            { "7.5 39.5", "boss_1|0"},
            { "14.5 58.5", "1|10"},
            }},
            {"level_bonus_1.xml", new Dictionary<string, string>{
            }},
            {"level_boss_1.xml", new Dictionary<string, string>{
            { "-1.5 -56.5", "4|0"},
            { "-22.5 19.5", "3|100"},
            }},
            {"level_4.xml", new Dictionary<string, string>{
            { "-94.5 -47.5", "esc_2|1"}, //Not an entrance
            { "-81.75 -9.5", "esc_4|0"}, //Not an entrance
            { "-62.5 24.5", "6|0"},
            { "-50.5 17.5", "5|1"},
            { "-30.5 29.5", "5|0"},
            { "-38.5 17.5", "boss_2|0"},
            { "-44.5 50.5", "boss_1|1"},
            }},
            {"level_5.xml", new Dictionary<string, string>{
            { "-44.5 -67.5", "6|2"},
            { "-53.5 17.5", "4|1"},
            { "-30.5 29.5", "4|5"},
            { "-12.5 8.5", "6|1"},
            }},
            {"level_bonus_2.xml", new Dictionary<string, string>{
            }},
            {"level_6.xml", new Dictionary<string, string>{
            { "-47.5 -67.5", "5|3"},
            { "-62.5 24.5", "4|6"},
            { "-15.5 9.5", "5|2"},
            }},
            {"level_boss_2.xml", new Dictionary<string, string>{
            { "2.5 -58.5", "7|0"},
            { "-14.25 6.5", "4|100"},
            }},
            {"level_7.xml", new Dictionary<string, string>{
            { "-42.25 -74.5", "esc_1|1"}, //Not an entrance
            { "-79.25 -49.5", "esc_2|0"}, //Not an entrance
            { "0.75 11.5", "boss_2|1"},
            { "-0.5 36.5", "8|0"},
            { "20.5 34.5", "8|1"},
            }},
            {"level_8.xml", new Dictionary<string, string>{
            { "-17.5 0.5", "9|0"},
            { "2.25 0.5", "7|1"},
            { "22.5 -1.5", "7|2"},
            { "90.5 -57.5", "9|250"},
            }},
            {"level_9.xml", new Dictionary<string, string>{
            { "-36 -45.5", "boss_3|0"},
            { "-10.5 9.5", "8|2"},
            { "90.75 -57.5", "8|200"},
            }},
            {"level_bonus_3.xml", new Dictionary<string, string>{
            }},
            {"level_boss_3.xml", new Dictionary<string, string>{
            { "2.5 -39.5", "10|0"},
            { "-3.25 20.5", "9|100"},
            }},
            {"level_10.xml", new Dictionary<string, string>{
            { "74 -13.5", "boss_3|1"},
            { "-45.625 33.5", "10b|0"},
            { "5.75 35.5", "11|0"},
            { "27.75 42.5", "boss_4|1"},
            { "53.75 50.5", "esc_3|0"}, //The doodad name is a 4 >:|
            }},
            {"level_10_special.xml", new Dictionary<string, string>{
            }},
            {"level_11.xml", new Dictionary<string, string>{
            { "65 -105.5", "12|0"},
            { "52.5 -105.5", "12|54"},
            { "17.75 34.5", "10|100"},
            }},
            {"level_bonus_4.xml", new Dictionary<string, string>{
            }},
            {"level_12.xml", new Dictionary<string, string>{
            { "43 -84.5", "11|105"},
            { "55 -84.5", "11|45"},
            }},
            {"level_boss_4.xml", new Dictionary<string, string>{
            { "31.5 -27.5", "esc_1|0"},
            }},
            {"level_esc_1.xml", new Dictionary<string, string>{
            { "5.75 35.5", "11|0"},
            { "27.75 42.5", "boss_4|1"},
            { "53.75 50.5", "esc_3|0"}, //The doodad name is a 4 >:|
            }},
            {"level_esc_2.xml", new Dictionary<string, string>{
            { "-42.25 -74.5", "esc_1|1"}, //Not an entrance
            { "-79.25 -49.5", "esc_2|0"}, //Not an entrance
            }},
            {"level_esc_3.xml", new Dictionary<string, string>{
            { "-94.5 -47.5", "esc_2|1"}, //Not an entrance
            { "-81.75 -9.5", "esc_4|0"}, //Not an entrance
            { "-62.5 24.5", "6|0"},
            { "-50.5 17.5", "5|1"},
            { "-38.5 17.5", "boss_2|0"},
            }},
            {"level_esc_4.xml", new Dictionary<string, string>{
            { "-47.5 -41.5", "esc_3|1"}, //Not an entrance yet
            { "-37.5 -2", "3|1"},
            }},
        };

        public static string GetAPMapFileName(string seed, int slotId)
        {
            return seed != null ? $"archipelago-{seed}-{slotId}.hwm" : null;
        }
        private static string GetMapFileWithAssetDefault(string assetPath, string mapPath, string filePath)
        {
            string finalPath = Path.Combine(mapPath, filePath);
            if (!File.Exists(finalPath))
            {
                finalPath = Path.Combine(assetPath, filePath);
            }
            return finalPath;
        }

        /// <summary>
        /// Generates a map file from Archipelago
        /// </summary>
        /// <returns>True if a save file was generated or exists, False if the save failed to generate</returns>
        public static bool CreateAPMapFile(string baseFile, string seed, int slotId, out string mapFileName, ConnectionInfo connectionInfo, ArchipelagoData generateArchipelagoData)
        {
            archipelagoData = generateArchipelagoData;

            slotId = connectionInfo.playerId;
            seed = archipelagoData.seed;
            ArchipelagoData.MapType mapType = archipelagoData.mapType;

            string dir = Directory.GetCurrentDirectory();
            string apAssetPath = Path.Combine(dir, "archipelago-assets");
            string editorDir = Path.Combine(dir, "editor");

            mapFileName = GetAPMapFileName(seed, slotId);

            //Check if the map file already exists
            if (APSaveManager.SaveExists(mapFileName))
            {
                Logging.Log($"Map file {mapFileName} already exists, skipping map generation");
                return true;
            }

            Logging.Log("Starting map file generation");

            long longSeed = long.Parse(seed.Substring(Math.Max(seed.Length - int.MaxValue.ToString().Length, 0)));
            int intSeed = (int)longSeed;
            random = new Random(intSeed);

            if(!LaunchMapExtractor(editorDir, editorDir, "map " + Path.Combine("..", baseFile)))
                return false;

            Logging.Log("Moving extracted files into new folder");
            //Create a new folder for our generated instance
            string oldMapFolder = editorDir + baseFile.Substring(6).Replace(".hwm", "");
            string mapFolder = Path.Combine(editorDir, "archipelago");
            if (Directory.Exists(mapFolder))
                Directory.Delete(mapFolder, true);
            Directory.Move(oldMapFolder, mapFolder);

            string assetsPath = Path.Combine(editorDir, "assets");
            //If the assets directory doesn't exist, we gotta run the extractor
            if (!Directory.Exists(assetsPath))
            {
                Logging.Log("Extracting assets");
                Directory.CreateDirectory(assetsPath);

                if (!LaunchMapExtractor(editorDir, assetsPath, "..\\..\\assets.bin"))
                    return false;
            }
            PatchHammerwatchAssets(assetsPath, apAssetPath, mapFolder);

            Logging.Log("Copying archipelago assets into map folder");
            //Copy folders from ap assets folder into the map
            MiscHelper.DeepCopy(Path.Combine(apAssetPath, "common"), mapFolder);
            string mapAssetFolder;
            switch (mapType)
            {
                case ArchipelagoData.MapType.Castle:
                    mapAssetFolder = "castle";
                    break;
                case ArchipelagoData.MapType.Temple:
                    mapAssetFolder = "temple";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            MiscHelper.DeepCopy(Path.Combine(apAssetPath, mapAssetFolder), mapFolder);

            Logging.Log("Edit map info file");
            //Edit starting lives count through the info file
            string infoFileName = Path.Combine(mapFolder, "info.xml");
            StreamReader infoReader = new StreamReader(infoFileName);
            string infoText = infoReader.ReadToEnd();
            infoReader.Close();
            string[] infoStrs = infoText.Split(new[] { "<lives>" }, StringSplitOptions.RemoveEmptyEntries);
            string newText = $"{infoStrs[0]}<lives>{archipelagoData.GetOption(SlotDataKeys.startingLifeCount)}{infoStrs[1].Substring(1)}";
            File.WriteAllText(infoFileName, newText);

            Logging.Log("Edit level info file");
            string levelsFileName = Path.Combine(mapFolder, "levels.xml");
            XDocument levelDoc;
            using (StreamReader levelsReader = new StreamReader(levelsFileName))
            {
                levelDoc = XDocument.Parse(levelsReader.ReadToEnd());
                //Set the starting level
                string startCode = APData.exitIdToCode[archipelagoData.GetSlotInt("Start Exit")];
                string[] startSplits = startCode.Split('|');
                levelDoc.Root.SetAttributeValue("start", startSplits[0]);
            }
            //Add the archipelago hub, I know it's not useful in the temple campaign but the BK zone is fun :)
            XElement hubLevelElement = new XElement("level");
            hubLevelElement.SetAttributeValue("id", "ap_hub");
            hubLevelElement.SetAttributeValue("res", "levels/ap_hub.xml");
            hubLevelElement.SetAttributeValue("name", "Portal Hub");
            XElement actElement = new XElement("act");
            actElement.SetAttributeValue("name", "ARCHIPELAGO");
            actElement.Add(hubLevelElement);
            levelDoc.Root.Add(actElement);
            levelDoc.Save(levelsFileName);

            //Shop upgrade and cost shuffle
            Logging.Log("Tweaking tweaks");
            string mapTweakDir = Path.Combine(mapFolder, "tweak");
            //Copy shared tweak info to each class and remove from file
            string sharedPath = GetMapFileWithAssetDefault(assetsPath, mapFolder, Path.Combine("tweak", "shared.xml"));
            XDocument sharedDoc = XDocument.Load(sharedPath);
            XElement sharedUpgradeRoot = sharedDoc.Root.Element("upgrades");
            XElement[] sharedUpgrades = sharedUpgradeRoot.Elements().ToArray();
            foreach (XElement upgrade in sharedUpgrades)
            {
                upgrade.Remove();
            }
            Directory.CreateDirectory(mapTweakDir);
            sharedDoc.Save(Path.Combine(mapTweakDir, "shared.xml"));
            bool shopsanity = archipelagoData.IsShopSanityOn();
            int category_shuffle = archipelagoData.GetOption(SlotDataKeys.shopUpgradeCategoryShuffle);
            float minCostMod = archipelagoData.GetOption(SlotDataKeys.shopCostMin) / 100f;
            float maxCostMod = archipelagoData.GetOption(SlotDataKeys.shopCostMax) / 100f;
            TweakPatcher.EditTweaks(assetsPath, mapTweakDir, sharedUpgrades, shopsanity, category_shuffle, minCostMod, maxCostMod, archipelagoData, random);

            Logging.Log("Loading slot data dictionaries");
            exitRando = archipelagoData.GetOption(SlotDataKeys.exitRandomization) > 0;
            Newtonsoft.Json.Linq.JObject gateTypesObj = archipelagoData.GetSlotJObject("Gate Types");
            Newtonsoft.Json.Linq.JObject exitSwapsObj = archipelagoData.GetSlotJObject("Exit Swaps");
            Dictionary<string, long> optimizedGateTypes = gateTypesObj.ToObject<Dictionary<string, long>>();
            gateTypes = new Dictionary<string, string>(optimizedGateTypes.Count);
            foreach (var pair in optimizedGateTypes)
            {
                gateTypes[APData.gateIdToCode[int.Parse(pair.Key)]] = GetKeyNameFromCode((int)pair.Value);
            }
            Dictionary<string, long> optimizedExitSwaps = exitSwapsObj.ToObject<Dictionary<string, long>>();
            exitSwaps = new Dictionary<string, string>(optimizedExitSwaps.Count);
            foreach (var pair in optimizedExitSwaps)
            {
                exitSwaps[APData.exitIdToCode[int.Parse(pair.Key)]] = APData.exitIdToCode[(int)pair.Value];
            }
            //Create dictionary of exit code sign to act for the Castle campaign
            if (mapType == ArchipelagoData.MapType.Castle)
            {
                foreach (string level in floorSignExits.Keys)
                {
                    int act = APData.GetActFromLevelFileName(level, archipelagoData);
                    char actChar = (char)('a' + act - 1);
                    foreach (string exitCode in floorSignExits[level].Values)
                    {
                        exitCodeToActChar[exitCode] = actChar;
                    }
                }
            }

            Logging.Log("Reading level xml files");
            //Modify each level in the save
            string levelsDir = Path.Combine(mapFolder, "levels");
            string[] levels = Directory.GetFiles(levelsDir);
            Dictionary<string, XDocument> docs = new Dictionary<string, XDocument>();
            foreach (string levelPath in levels)
            {
                if (levelPath.EndsWith("ap_hub.xml")) continue;
                docs[levelPath] = XDocument.Parse(File.ReadAllText(levelPath));
            }
            //Count enemies and treasure
            Logging.Log("Counting enemies and treasure");
            if (archipelagoData.raceMode.HasValue && archipelagoData.raceMode.Value)
                EnemyShuffler.SetRandomSeed(random.Next());
            else
                EnemyShuffler.SetRandomSeed();
            EnemyShuffler.CountEnemies(docs, archipelagoData);
            CountTreasure(docs);
            Logging.Log("Shuffling enemies");
            int shuffleMode = archipelagoData.GetOption(SlotDataKeys.enemyShuffleMode);
            int actRange = archipelagoData.GetOption(SlotDataKeys.enemyShuffleActRange);
            EnemyShuffler.ShuffleEnemies(actRange, shuffleMode, archipelagoData);
            Logging.Log("Modifying level xml files");
            foreach (string levelPath in docs.Keys)
            {
                Logging.Log($"    Modifying {Path.GetFileName(levelPath)}");
                XDocument doc = docs[levelPath];
                File.WriteAllText(levelPath, EditLevel(levelPath, doc, connectionInfo));
            }

            Logging.Log("Packing xml files into map file");
            if (!LaunchMapPacker(editorDir))
                return false;

            Logging.Log("Moving file map file into levels folder");
            //Move the generated map file into the correct levels directory
            string endFilePath = Path.Combine(dir, "levels", mapFileName);
            File.Move(Path.Combine(editorDir, "archipelago.hwm"), endFilePath);

            Logging.Log("Loading the level into Hammerwatch's resources");
            //Load the save into Hammerwatch's resources
            GameBase.Instance.modList.Load(Path.Combine("levels", mapFileName));

            Logging.Log("Generation complete!");
            return true;
        }
        private static bool LaunchMapExtractor(string editorDir, string workingDir, string args)
        {
            Logging.Log("Launching resource extractor");
            ProcessStartInfo processInfo = new ProcessStartInfo(Path.Combine(editorDir, "ResourceExtractor.exe"))
            {
                WorkingDirectory = workingDir,
                Arguments = args,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process extractorProcess = Process.Start(processInfo);
            while (!extractorProcess.HasExited)
            {
                //Wait for the process to finish
            }
            if (extractorProcess.ExitCode != 0)
            {
                //The extractor failed to extract! This should never happen we have a major issue here
                Logging.Log($"Resource extractor exited with exit code: {extractorProcess.ExitCode}");
                return false;
            }
            return true;
        }
        private static bool LaunchMapPacker(string editorDir)
        {
            //Pack the level
            ProcessStartInfo packerInfo = new ProcessStartInfo(Path.Combine(editorDir, "LevelPacker.exe"))
            {
                WorkingDirectory = editorDir,
                Arguments = "archipelago",
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process packerProcess = Process.Start(packerInfo);
            while (!packerProcess.HasExited)
            {
                //Wait for the packer to finish
            }
            if (packerProcess.ExitCode != 0)
            {
                //The packer failed to pack, likely we screwed up some formatting
                Logging.Log($"Level packer exited with exit code: {packerProcess.ExitCode}");
                return false;
            }
            return true;
        }
        private static void PatchHammerwatchAssets(string hwAssetPath, string apAssetsPath, string templeMapPath)
        {
            string campaignAssetPath;
            switch (archipelagoData.mapType)
            {
                case ArchipelagoData.MapType.Castle:
                    campaignAssetPath = Path.Combine(apAssetsPath, "castle");
                    break;
                case ArchipelagoData.MapType.Temple:
                    campaignAssetPath = Path.Combine(apAssetsPath, "temple");
                    break;
                default:
                    campaignAssetPath = "";
                    break;
            }

            //Bonus door and key types
            string[] bonusAssetsToChange = new[] {
               "bonus_door_h_32.xml",
               "bonus_door_v_32.xml",
               "bonus_key.xml",
            };
            foreach (string bonusItem in bonusAssetsToChange)
            {
                string bonusItemApAssetPath = Path.Combine(apAssetsPath, "common", "items", bonusItem);
                if (File.Exists(bonusItemApAssetPath)) continue;
                string bonusAssetPath = Path.Combine(hwAssetPath, "items", bonusItem);
                XDocument bonusDoc = XDocument.Parse(File.ReadAllText(bonusAssetPath));
                bonusDoc.Root.Element("behavior").Element("dictionary").Element("entry").Element("int").Value = "10";
                File.WriteAllText(bonusItemApAssetPath, bonusDoc.ToString());
            }

            //Modify menus to show bonus key, strange plank, and tool fragment count
            string hudFile = Path.Combine("menus", "gui", "hud.xml");
            string guiFilePath = Path.Combine(hwAssetPath, hudFile);
            string apGuiPath = Path.Combine(campaignAssetPath, "menus", "gui");
            string plankOffset = "-80 20";
            if (archipelagoData.mapType == ArchipelagoData.MapType.Temple)
            {
                guiFilePath = Path.Combine(templeMapPath, hudFile);
                plankOffset = "-34 20";
            }
            string apGuiFilePath = Path.Combine(apGuiPath, "hud.xml");
            if (!File.Exists(apGuiFilePath))
            {
                XElement bonusKeyMenuNode = NodeHelper.CreateGUISpriteNode("bonus-key", "-24 1", "menus/hud-extended.xml:bonus-key", true);
                bonusKeyMenuNode.Add(NodeHelper.CreateGUITextNode("keys-10", "16 0", "", "menus/px-20.xml"));
                XElement plankMenuNode = NodeHelper.CreateGUISpriteNode("strange-plank", plankOffset, "menus/hud-extended.xml:strange-plank", false);
                plankMenuNode.Add(NodeHelper.CreateGUITextNode("planks", "20 0", "", "menus/px-20.xml"));

                XDocument hudFileDoc = XDocument.Parse(File.ReadAllText(guiFilePath));
                XElement hudBaseNode = hudFileDoc.Root.Element("base");

                XElement hudBaseSpriteNode = hudBaseNode.Element("sprite");
                hudBaseSpriteNode.Add(bonusKeyMenuNode);
                XElement extraInfoNode = hudBaseNode.Elements("group").Where(group => group.Attribute("id").Value == "extra-info").First();
                extraInfoNode.Add(plankMenuNode);

                string hammerMenuOffset;
                switch (archipelagoData.mapType)
                {
                    case ArchipelagoData.MapType.Temple:
                        hammerMenuOffset = "212";
                        break;
                    default:
                        hammerMenuOffset = "151";
                        break;
                }
                XElement hammerMenuNode = NodeHelper.CreateGUISpriteNode("hammer", $"{hammerMenuOffset} 1", "menus/hud-extended.xml:hammer", true);
                hammerMenuNode.Add(NodeHelper.CreateGUITextNode("hammer-fragments", "17 0", "", "menus/px-20.xml"));
                hudBaseSpriteNode.Add(hammerMenuNode);

                //Tool fragment display
                if (archipelagoData.mapType == ArchipelagoData.MapType.Temple)
                {
                    XElement fragmentsGroup = new XElement("group");
                    fragmentsGroup.SetAttributeValue("id", "extra-info-2");
                    fragmentsGroup.SetAttributeValue("anchor", "100% 50%");
                    string[] toolFragments = new[]
                    {
                        "lever",
                        "pan",
                        "pickaxe",
                    };
                    for (int t = 0; t < toolFragments.Length; t++)
                    {
                        fragmentsGroup.Add(NodeHelper.CreateGUITextNode($"{toolFragments[t]}-fragments", $"-13 {22 - 13 * t}", "", "menus/px-20.xml", false));
                    }
                    hudBaseNode.Add(fragmentsGroup);
                }

                Directory.CreateDirectory(apGuiPath);
                File.WriteAllText(apGuiFilePath, hudFileDoc.ToString());
            }

            if (archipelagoData.mapType == ArchipelagoData.MapType.Castle)
            {
                //Remove lower collider from dragon corpse
                string dragonCorpseDoodadPath = Path.Combine(apAssetsPath, "castle", "actors", "boss_dragon", "boss_dragon_razed.xml");
                if (!File.Exists(dragonCorpseDoodadPath))
                {
                    string dragonCorpseAssetPath = Path.Combine(hwAssetPath, "actors", "boss_dragon", "boss_dragon_razed.xml");
                    XDocument dragonCorpseActorDoc = XDocument.Parse(File.ReadAllText(dragonCorpseAssetPath));
                    XElement test1 = dragonCorpseActorDoc.Root;
                    XElement test2 = test1.Element("collision");
                    XElement test3 = test2.Element("polygon");
                    test3.Remove();

                    Directory.CreateDirectory(Path.Combine(apAssetsPath, "castle", "actors", "boss_dragon"));
                    File.WriteAllText(dragonCorpseDoodadPath, dragonCorpseActorDoc.ToString());
                }
            }
            else
            {
                //Modify secret room prefab to remove spawn item nodes
                string roomSecretPrefabPath = Path.Combine(campaignAssetPath, "prefabs", "theme_e", "parts", "secret_random_1.xml");
                string roomSecretBlankPrefabPath = Path.Combine(campaignAssetPath, "prefabs", "theme_e", "parts", "secret_random_1_blank.xml");
                if (!File.Exists(roomSecretPrefabPath) || !File.Exists(roomSecretBlankPrefabPath))
                {
                    string randomRoomPrefabPath = Path.Combine(hwAssetPath, "prefabs", "theme_e", "parts", "secret_random_1.xml");
                    XDocument secretBlankPrefabDoc = XDocument.Parse(File.ReadAllText(randomRoomPrefabPath));
                    XElement secretScriptNodeBase = secretBlankPrefabDoc.Root.Element("dictionary").Elements().ToArray()[4].Element("array");
                    Dictionary<string, XElement> idToNode = new Dictionary<string, XElement>();
                    foreach (XElement scriptNode in secretScriptNodeBase.Elements().ToArray())
                    {
                        string scriptNodeId = scriptNode.Element("int").Value;
                        idToNode[scriptNodeId] = scriptNode;
                    }
                    int secretNodeCounter = 200000;
                    AddHammerNodesToBreakableWall(ref secretNodeCounter, idToNode, "104658", secretScriptNodeBase);
                    XDocument secretPrefabDoc = new XDocument(secretBlankPrefabDoc);
                    idToNode["104667"].Remove();
                    Directory.CreateDirectory(Path.Combine(campaignAssetPath, "prefabs", "theme_e", "parts"));
                    File.WriteAllText(roomSecretPrefabPath, secretPrefabDoc.ToString());
                    File.WriteAllText(roomSecretBlankPrefabPath, secretBlankPrefabDoc.ToString());
                }

                //Modify bonus level pillar
                string bonusPillarPrefabPath = Path.Combine(campaignAssetPath, "prefabs", "bonus_bonus5_pillar_dst.xml");
                if (!File.Exists(bonusPillarPrefabPath))
                {
                    string vanillaBonusPillarPrefabPath = Path.Combine(hwAssetPath, "prefabs", "bonus_bonus5_pillar_dst.xml");
                    XDocument pillarPrefabDoc = XDocument.Parse(File.ReadAllText(vanillaBonusPillarPrefabPath));
                    XElement pillarScriptNodeBase = pillarPrefabDoc.Root.Element("dictionary").Elements().ToArray()[4].Element("array");
                    Dictionary<string, XElement> idToNode = new Dictionary<string, XElement>();
                    foreach (XElement scriptNode in pillarScriptNodeBase.Elements().ToArray())
                    {
                        string scriptNodeId = scriptNode.Element("int").Value;
                        idToNode[scriptNodeId] = scriptNode;
                    }
                    int secretNodeCounter = 200000;
                    AddHammerNodesToBreakableWall(ref secretNodeCounter, idToNode, "86302", pillarScriptNodeBase);
                    Directory.CreateDirectory(Path.Combine(campaignAssetPath, "prefabs"));
                    File.WriteAllText(bonusPillarPrefabPath, pillarPrefabDoc.ToString());
                }

                //Remove layer node for vgt_plant
                string plantItemPath = Path.Combine(apAssetsPath, "common", "items", "vgt_plant.xml");
                if (!File.Exists(plantItemPath))
                {
                    XDocument plantItemDoc = XDocument.Parse(File.ReadAllText(Path.Combine(hwAssetPath, "items", "vgt_plant.xml")));
                    plantItemDoc.Root.Element("behavior").Element("dictionary").Element("entry").Remove();
                    File.WriteAllText(plantItemPath, plantItemDoc.ToString());
                }

                //Modify desert_quest_lever_solved prefab
                string leverQuestItemPath = Path.Combine(campaignAssetPath, "prefabs", "desert_quest_lever_solved.xml");
                if (!File.Exists(leverQuestItemPath))
                {
                    XDocument leverQuestItemDoc = XDocument.Parse(File.ReadAllText(Path.Combine(templeMapPath, "prefabs", "desert_quest_lever_solved.xml")));
                    XElement leverQuestScriptNodeBase = leverQuestItemDoc.Root.Element("dictionary").Elements().ToArray()[4].Element("array");
                    foreach (XElement scriptNode in leverQuestScriptNodeBase.Elements())
                    {
                        string scriptNodeId = scriptNode.Element("int").Value;
                        switch (scriptNodeId)
                        {
                            case "154134": //LevelLoaded EventTrigger
                                NodeHelper.SetConnectionNodes(scriptNode, 154137);
                                break;
                            case "154137": //CheckFlag node for if the quest was solved
                                XElement[] dictNodes = scriptNode.Element("dictionary").Elements().ToArray();
                                XElement staticNode = new XElement(dictNodes[1].Element("int-arr"));
                                staticNode.Value = "154133";
                                dictNodes[2].Add(staticNode);
                                break;
                        }
                    }
                    File.WriteAllText(leverQuestItemPath, leverQuestItemDoc.ToString());
                }

                //Modify desert_vendor prefabs
                string[] otherDesertVendorNames = new[]
                {
                    "combo", "defense", "misc"
                };
                string offenseVendorPath = Path.Combine(campaignAssetPath, "prefabs", "desert_vendor_offense.xml");
                XDocument offenseVendorDoc = null;
                int vendorEventId = 200000;
                if (!File.Exists(offenseVendorPath))
                {
                    //Add event trigger node so that we can remotely upgrade the shop
                    offenseVendorDoc = XDocument.Parse(File.ReadAllText(Path.Combine(templeMapPath, "prefabs", "desert_vendor_offense.xml")));
                    XElement scriptNode = offenseVendorDoc.Root.Element("dictionary").Elements("dictionary").First(node => node.HasAttributes && node.Attribute("name") != null && node.Attribute("name").Value == "scripting").Element("array");
                    scriptNode.Add(NodeHelper.CreateGlobalEventTriggerNode(vendorEventId, -1, new Vector2(-9.5f, -2), "vendor_offense_upgrade", new[] { 104445 }));
                    offenseVendorDoc.Save(offenseVendorPath);
                }
                if (offenseVendorDoc == null)
                {
                    offenseVendorDoc = XDocument.Parse(File.ReadAllText(offenseVendorPath));
                }
                foreach (string vendor in otherDesertVendorNames)
                {
                    string vendorPath = Path.Combine(campaignAssetPath, "prefabs", $"desert_vendor_{vendor}.xml");
                    if (!File.Exists(vendorPath))
                    {
                        XDocument newDoc = new XDocument(offenseVendorDoc);
                        //Replace offense doodads with new vendor doodads
                        XElement doodadsNode = newDoc.Root.Element("dictionary").Elements("dictionary").First(node => node.HasAttributes && node.Attribute("name") != null && node.Attribute("name").Value == "doodads").Element("array");
                        foreach (XElement doodadNode in doodadsNode.Elements())
                        {
                            XElement nameNode = doodadNode.Element("string");
                            if (!nameNode.Value.Contains("offense")) continue;
                            nameNode.Value = nameNode.Value.Replace("offense", vendor);
                        }
                        //Replace offense shop nodes with new vendor values
                        XElement scriptNodes = newDoc.Root.Element("dictionary").Elements("dictionary").First(node => node.HasAttributes && node.Attribute("name") != null && node.Attribute("name").Value == "scripting").Element("array");
                        string catString = vendor == "defense" ? "def" : vendor;
                        foreach (XElement scriptNode in scriptNodes.Elements())
                        {
                            string nodeId = scriptNode.Element("int").Value;
                            string nodeType = scriptNode.Element("string").Value;
                            if (nodeType == "ShopArea")
                            {
                                //Replace categories with new ones
                                XElement shopCategoryNode = scriptNode.Element("dictionary").Element("string");
                                shopCategoryNode.Value = shopCategoryNode.Value.Replace("off", catString);
                            }
                            else if (nodeId == vendorEventId.ToString())
                            {
                                XElement[] stringNodes = scriptNode.Elements("string").ToArray();
                                stringNodes[1].Value = stringNodes[1].Value.Replace("offense", vendor);
                            }
                        }
                        newDoc.Save(vendorPath);
                    }
                }
            }
        }

        private static void AddHammerNodesToBreakableWall(ref int modNodeCounter, Dictionary<string, XElement> idToNode, string objectEventTriggerNodeId, XElement scriptNodesRoot)
        {
            int checkFlagNodeId = modNodeCounter++;
            int playSoundNodeId = modNodeCounter++;

            //Modify wall node object event trigger to run infinitely and move connections to a check global flag node
            XElement wallNode = idToNode[objectEventTriggerNodeId];
            wallNode.Elements("int").ToArray()[1].Value = (-1).ToString();
            XElement[] wallConnectionNodes = wallNode.Elements("int-arr").ToArray();
            string wallConnectionNodeIdString = wallConnectionNodes[0].Value;
            List<int> wallConnectionNodeIds = new List<int>();
            foreach (string wallId in wallConnectionNodeIdString.Split(' '))
            {
                wallConnectionNodeIds.Add(int.Parse(wallId));
            }
            wallConnectionNodes[0].Value = checkFlagNodeId.ToString();
            wallConnectionNodes[1].Value = "0";

            //Find the sound node position and make the destroy node only run once
            Vector2 soundNodePos = new Vector2();
            for (int c = wallConnectionNodeIds.Count - 1; c >= 0; c++)
            {
                string counterConnectionString = idToNode[wallConnectionNodeIds[c].ToString()].Element("dictionary").Element("dictionary").Element("int-arr").Value;

                XElement counterConnectNode = idToNode[counterConnectionString];
                if (counterConnectNode.Element("string").Value == "DestroyObject")
                {
                    counterConnectNode.Elements("int").ToArray()[1].Value = "1";
                    string[] destroyConnectNodeIds = counterConnectNode.Element("int-arr").Value.Split(' ');
                    foreach (string destroyConnectNodeId in destroyConnectNodeIds)
                    {
                        XElement destroyConnectNode = idToNode[destroyConnectNodeId];
                        if (destroyConnectNode.Element("string").Value == "PlaySound" && (destroyConnectNode.Element("dictionary").Element("string").Value == "sound/misc.xml:wall_break"
                            || destroyConnectNode.Element("dictionary").Element("string").Value == "sound/bonus.xml:bonus_wall_dst"))
                        {
                            soundNodePos = NodeHelper.PosFromString(destroyConnectNode.Element("vec2").Value);
                            break;
                        }
                    }
                    break;
                }
            }

            Vector2 checkGlobalFlagNodePos = NodeHelper.PosFromString(wallNode.Element("vec2").Value) + new Vector2(0, -2);

            scriptNodesRoot.Add(NodeHelper.CreateCheckGlobalFlagNode(checkFlagNodeId, checkGlobalFlagNodePos, "has_hammer", wallConnectionNodeIds.ToArray(), new int[] { playSoundNodeId }));
            scriptNodesRoot.Add(NodeHelper.CreatePlaySoundNode(playSoundNodeId, soundNodePos, "sound/bonus.xml:bonus_gate", false, true, 5));
        }
        private static void CountTreasure(Dictionary<string, XDocument> docs)
        {
            if (archipelagoData.GetOption(SlotDataKeys.treasureShuffle) == 0)
                return;
            treasureCounts = new Dictionary<string, int>();
            foreach (string treasure in treasureNames)
            {
                treasureCounts[treasure] = 0;
            }

            foreach (XDocument doc in docs.Values)
            {
                XElement itemsNodeRoot = doc.Root.Elements("dictionary").First(node => node.HasAttributes && node.Attribute("name") != null && node.Attribute("name").Value == "items");
                XElement scriptNodeRoot = doc.Root.Elements("dictionary").First(node => node.HasAttributes && node.Attribute("name") != null && node.Attribute("name").Value == "scripting");
                XElement[] itemGroups = itemsNodeRoot.Elements().ToArray();
                XElement[] scriptNodes = scriptNodeRoot.Element("array").Elements().ToArray();
                Dictionary<string, int> actors = new Dictionary<string, int>();
                foreach (XElement group in itemGroups)
                {
                    string groupName = group.Attribute("name").Value;
                    if (!treasureCounts.ContainsKey(groupName))
                        continue;
                    treasureCounts[groupName] += group.Elements().Count();
                }
            }
        }

        private static string GetKeyNameFromCode(int code)
        {
            switch (code)
            {
                case 0:
                    return "bronze";
                case 1:
                    return "silver";
                case 2:
                    return "gold";
                case 10:
                    return "bonus";
                default:
                    return null;
            }
        }

        private static string EditLevel(string levelPath, XDocument doc, ConnectionInfo connectionInfo)
        {
            string[] levelPathDirectories = levelPath.Split('\\');
            List<int> nodesToNuke = new List<int>();
            List<int> secretNodesToGuaranteeSpawn = new List<int>();
            Dictionary<string, Vector2> effectNodePositions = new Dictionary<string, Vector2>();
            XElement doodadsNode = doc.Root.Elements("dictionary").First(node => node.HasAttributes && node.Attribute("name") != null && node.Attribute("name").Value == "doodads");
            XElement doodadsNodeRoot = doodadsNode.Elements().First();
            List<XElement> doodadsToAdd = new List<XElement>();
            List<string> doodadsToNuke = new List<string>();
            Dictionary<string, bool> doodadsToChangeSync = new Dictionary<string, bool>();
            XElement actorNodeRoot = doc.Root.Elements("dictionary").First(node => node.HasAttributes && node.Attribute("name") != null && node.Attribute("name").Value == "actors");
            XElement itemNodeRoot = doc.Root.Elements("dictionary").First(node => node.HasAttributes && node.Attribute("name") != null && node.Attribute("name").Value == "items");
            XElement[] itemsNodes = itemNodeRoot.Elements().ToArray();
            XElement scriptNode = doc.Root.Elements("dictionary").First(node => node.HasAttributes && node.Attribute("name") != null && node.Attribute("name").Value == "scripting");
            XElement scriptNodeRoot = scriptNode.Elements().First();
            XElement[] scriptNodes = scriptNodeRoot.Elements().ToArray();
            XElement prefabsNode = doc.Root.Elements("dictionary").First(node => node.HasAttributes && node.Attribute("name") != null && node.Attribute("name").Value == "prefabs");
            XElement lightNodeRoot = doc.Root.Elements("dictionary").First(node => node.HasAttributes && node.Attribute("name") != null && node.Attribute("name").Value == "lighting").Elements().First();

            globalScriptNodesToTriggerOnLoad = new List<int>();
            globalScriptNodesToTriggerOnceOnLoad = new List<int>();

            Dictionary<string, XElement> idToNode = new Dictionary<string, XElement>(scriptNodes.Length);
            foreach (XElement node in scriptNodes)
            {
                idToNode[node.Element("int").Value] = node;
            }

            string levelFile = levelPathDirectories[levelPathDirectories.Length - 1];
            List<XElement> scriptNodesToAdd = new List<XElement>();
            ReplaceItemLocations(levelFile, doc, out scriptNodesToAdd, out List<int> nonHololinkedItemIds);

            if (archipelagoData.GetOption(SlotDataKeys.gateShuffle) > 0)
            {
                ReplaceGates(levelFile, itemNodeRoot);
            }
            int buttonsanity = archipelagoData.GetOption(SlotDataKeys.buttonsanity);

            string sign_hub_text = "To Portal Hub";
            int bossDefeatMessageDelay = 2000;

            string pumpsItemName = ArchipelagoManager.GetItemName(20 + APData.templeButtonItemStartID);

            List<int> buttonEffectNodeIds = new List<int>();

            int panLocation = -1;
            int modNodeStartId = 200000;
            switch (archipelagoData.mapType)
            {
                case ArchipelagoData.MapType.Castle:
                    switch (levelFile)
                    {
                        //Castle Hammerwatch
                        case "level_1.xml": //Prison 1
                            int[] p1BronzeKeyEntrance = { 424, 423 }; //Lower, Upper
                            int[] p1BronzeKeySouth = { 3291, 3160, 4234 }; //3 Entries
                            int[] p1BronzeKeySouth2 = { 4048, 3474, 3290, 2839 }; //4 Entries
                            int[] p1BronzeKeyNe = { 3916, 2512, 3775 }; //3 Entries
                            int[] p1BronzeKeyExit = { 3676, 1203 }; //2 Entries

                            globalScriptNodesToTriggerOnceOnLoad.Add(p1BronzeKeyEntrance[archipelagoData.GetRandomLocation("Prison 1 Bronze Key 1")]);
                            globalScriptNodesToTriggerOnceOnLoad.Add(p1BronzeKeySouth[archipelagoData.GetRandomLocation("Prison 1 Bronze Key 2")]);
                            globalScriptNodesToTriggerOnceOnLoad.Add(p1BronzeKeySouth2[archipelagoData.GetRandomLocation("Prison 1 Bronze Key 3")]);
                            globalScriptNodesToTriggerOnceOnLoad.Add(p1BronzeKeyNe[archipelagoData.GetRandomLocation("Prison 1 Bronze Key 4")]);
                            globalScriptNodesToTriggerOnceOnLoad.Add(p1BronzeKeyExit[archipelagoData.GetRandomLocation("Prison 1 Bronze Key 5")]);

                            nodesToNuke = new List<int> { 425, 3161, 3475, 3917, 1141, 3915 }; //Bronze Keys, Strange plank trigger

                            //Change thief dialogue to show goal
                            //Adventurer dialogue node 1: 1652 //The Bridge!
                            string newText = "";
                            string goalCheckText1 = "";
                            string goalCheckText2 = "";
                            switch (archipelagoData.GetOption(SlotDataKeys.goal)) //See if you can find a way out, we'll wait here!
                            {
                                case 0:
                                    newText = "We have the supplies to repair it, but we aint gonna until you slay all the bosses in the castle!";
                                    goalCheckText1 = "Hey! You still need to defeat all the bosses!";
                                    goalCheckText2 = "You aren't getting over here until you do!";
                                    //Edit thief dialogue because there aren't any strange planks in this mode
                                    NodeHelper.EditShowSpeechBubbleNode(idToNode["104633"], "Hey! Strange planks are not scattered around the castle, kill the bosses instead!");
                                    break;
                                case 1:
                                    newText = "See if you can find some materials to repair it, we'll wait here!";
                                    goalCheckText1 = "Hey! You still need some more planks so we can fix the bridge!";
                                    goalCheckText2 = $"{archipelagoData.plankHuntRequirement} should do the trick!";
                                    break;
                                case 2:
                                    goalCheckText1 = "Hey! You won't escape this way, try finding another way out!";
                                    goalCheckText2 = $"The last party was missing {archipelagoData.plankHuntRequirement} plank{(archipelagoData.plankHuntRequirement > 1 ? "s" : "")}, you might need them if you want to escape!";
                                    break;
                            }
                            if (newText != "")
                            {
                                NodeHelper.EditShowSpeechBubbleNode(idToNode["1653"], newText);
                            }
                            //Adventurer dialogue node 3: 1654 //Bye!
                            //Adventurer dialogue node 4: 1734 //You'll never make it!

                            //Goal completion nodes
                            int shapeNodeId = modNodeStartId++;
                            int areaTriggerNodeId = modNodeStartId++;
                            int checkGoalNodeId = modNodeStartId++;
                            int goalEventTriggerNodeId = modNodeStartId++;
                            scriptNodesToAdd.Add(NodeHelper.CreateRectangleShapeNode(shapeNodeId, new Vector2(-73, -2), 2.5f, 2, 1));
                            scriptNodesToAdd.Add(NodeHelper.CreateAreaTriggerNode(areaTriggerNodeId, -1, new Vector2(-73, 0), 0, 1, new int[] { shapeNodeId }, new[] { goalEventTriggerNodeId }));
                            scriptNodesToAdd.Add(NodeHelper.CreateGlobalEventTriggerNode(goalEventTriggerNodeId, -1, new Vector2(-73, 4), "ap_check_end_game", new int[] { modNodeStartId, modNodeStartId + 1 }, null, false));
                            scriptNodesToAdd.Add(NodeHelper.CreateToggleElementNode(modNodeStartId++, new Vector2(-71, 2), -1, 1, new int[] { goalEventTriggerNodeId }));
                            scriptNodesToAdd.Add(NodeHelper.CreateSpeechBubbleNode(modNodeStartId++, new Vector2(-75, 2), -1, "menus/speech/normal_speech.xml", goalCheckText1, new int[] { 1648 }, new Vector2(0.1f, -1.2f), 100, 0, 4000));
                            NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { modNodeStartId }, new int[] { 5000 });
                            scriptNodesToAdd.Add(NodeHelper.CreateSpeechBubbleNode(modNodeStartId++, new Vector2(-77, 2), -1, "menus/speech/normal_speech.xml", goalCheckText2, new int[] { 1648 }, new Vector2(0.8f, -0.85f), 100, 0, 4000));
                            NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { 1654 }, new int[] { 5000 });
                            int resetNodeId = modNodeStartId++;
                            scriptNodesToAdd.Add(NodeHelper.CreateToggleElementNode(resetNodeId, new Vector2(-71, -4), -1, 0, new int[] { goalEventTriggerNodeId }));
                            NodeHelper.AddConnectionNodes(idToNode["1734"], new int[] { resetNodeId }, new int[] { 5000 });

                            if (buttonsanity > 0)
                            {
                                //Boss rune item and rewire
                                NodeHelper.SetConnectionNodes(idToNode["965"], new[] { modNodeStartId });

                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, new Vector2(33, -49), true));

                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(38, -47), "a1l1_boss1", ref buttonEffectNodeIds, new[] { 1139 }));
                            }
                            //Add save at the beginning
                            foreach (XElement itemGroup in itemsNodes)
                            {
                                string groupName = itemGroup.Attribute("name").Value;
                                if (groupName == "items/trigger_button_save.xml")
                                {
                                    XElement saveNode = new XElement("array");
                                    XElement idNode = new XElement("int")
                                    {
                                        Value = modNodeStartId++.ToString()
                                    };
                                    XElement posNode = new XElement("vec2")
                                    {
                                        Value = "-13 -22.5"
                                    };
                                    saveNode.Add(idNode);
                                    saveNode.Add(posNode);
                                    itemGroup.Add(saveNode);
                                    break;
                                }
                            }

                            //Remove broken wall that leads to portal that skips the act
                            if (archipelagoData.GetOption(SlotDataKeys.shortcutTeleporter) == 0)
                            {
                                nodesToNuke.Add(426); //Level skip breakable wall node
                                foreach (XElement doodadNode in doodadsNodeRoot.Elements())
                                {
                                    if (doodadNode.Element("int").Value != "354") continue;
                                    doodadNode.Element("string").Value = "doodads/theme_a/a_h_8.xml"; //type
                                    doodadNode.Element("bool").Value = "False"; //need-sync
                                }
                            }
                            else
                            {
                                doodadsToAdd.Add(NodeHelper.CreateDoodadNode(modNodeStartId++, "doodads/generic/exit_teleport_exit.xml", new Vector2(-41f, -48.75f), false));
                                //Level start node
                                int destroyNodeId = modNodeStartId++;
                                scriptNodesToAdd.Add(NodeHelper.CreateLevelStartNode(modNodeStartId++, new Vector2(-41.5f, -49.25f), 20));
                                scriptNodesToAdd.AddRange(NodeHelper.CreateAreaTriggerNodes(ref modNodeStartId, new Vector2(-41.5f, -49.25f), new List<int> { destroyNodeId })); //Unlock node
                                scriptNodesToAdd.Add(NodeHelper.CreateDestroyObjectNode(destroyNodeId, new Vector2(-50f, -50f), 1, new int[] { 66, 67 }));
                            }
                            //Hub portal
                            int portal1TriggerShapeNodeId = modNodeStartId++;
                            scriptNodesToAdd.Add(NodeHelper.CreateRectangleShapeNode(portal1TriggerShapeNodeId, new Vector2(-65.5f, -2), 5, 5, 15));
                            List<XElement> p1TpDoodads = CreateHubPortal(ref modNodeStartId, new Vector2(-65.5f, -3f), -1, "ap_hub", 0, -1, false, sign_hub_text, out List<XElement> p1TpScriptNodes, "portal_a1", portal1TriggerShapeNodeId);
                            doodadsToAdd.AddRange(p1TpDoodads);
                            scriptNodesToAdd.AddRange(p1TpScriptNodes);

                            scriptNodesToAdd.Add(NodeHelper.CreateLevelStartNode(modNodeStartId++, new Vector2(-65.5f, -1.75f), 5));
                            break;
                        case "level_2.xml": //Prison 2
                            int[] p2BronzeKey1 = { 1662, 2679, 3071, 3070, 5419 };
                            int[] p2BronzeKey2 = { 6060, 6059, 5845 };
                            int[] p2BronzeKey3 = { 4847, 5844, 6058 };
                            int[] p2BronzeKey4 = { 6344, 6208, 6651 };
                            int[] p2SilverKey = { 1839, 1661, 1500 };
                            int[] p2GoldKey1 = { 2267, 2268, 2730, 416 };
                            int[] p2GoldKey2 = { 3351, 3349, 3655 };

                            globalScriptNodesToTriggerOnceOnLoad.Add(p2BronzeKey1[archipelagoData.GetRandomLocation("Prison 2 Bronze Key 1")]);
                            globalScriptNodesToTriggerOnceOnLoad.Add(p2BronzeKey2[archipelagoData.GetRandomLocation("Prison 2 Bronze Key 2")]);
                            globalScriptNodesToTriggerOnceOnLoad.Add(p2BronzeKey3[archipelagoData.GetRandomLocation("Prison 2 Bronze Key 3")]);
                            globalScriptNodesToTriggerOnceOnLoad.Add(p2BronzeKey4[archipelagoData.GetRandomLocation("Prison 2 Bronze Key 4")]);
                            globalScriptNodesToTriggerOnceOnLoad.Add(p2SilverKey[archipelagoData.GetRandomLocation("Prison 2 Silver Key 1")]);
                            globalScriptNodesToTriggerOnceOnLoad.Add(p2GoldKey1[archipelagoData.GetRandomLocation("Prison 2 Gold Key 1")]);
                            globalScriptNodesToTriggerOnceOnLoad.Add(p2GoldKey2[archipelagoData.GetRandomLocation("Prison 2 Gold Key 2")]);

                            nodesToNuke = new List<int> { 2873, 6211, 6210, 6209, 1501, 2269, 3350, 3653 }; //Bronze Keys, Silver Key, Gold Keys, Strange plank trigger

                            if (buttonsanity > 0)
                            {
                                //Boss rune item and rewire
                                NodeHelper.SetConnectionNodes(idToNode["4187"], new[] { modNodeStartId });
                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, new Vector2(-6, -15), true));
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(38, -47), "a1l2_boss1", ref buttonEffectNodeIds, new[] { 4174 }));

                                //SE rune sequence
                                int p2RuneEventTriggerId = modNodeStartId++;
                                scriptNodesToAdd.Add(NodeHelper.CreateGlobalEventTriggerNode(p2RuneEventTriggerId, -1, new Vector2(22, 16), "p2_rune_sequence", null));

                                NodeHelper.SetConnectionNodes(idToNode["5414"], new[] { p2RuneEventTriggerId });

                                string seRuneItemName = ArchipelagoManager.GetItemName(28 + APData.castleButtonItemStartID);
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(53, -14), seRuneItemName, ref buttonEffectNodeIds, new[] { 5842 }, new[] { 1000 }));

                                //NW spike puzzle level load trigger, switch to activate the children nodes instead of the AreaTrigger, that'll spawn items!
                                NodeHelper.SetConnectionNodes(idToNode["245"], new[] { 1066, 1492, 1491, 1068, 935, 940, 197, 938 });
                                //Hack into the puzzle nodes to only toggle the spikes if you have the items
                                //East buttons
                                Dictionary<int, Dictionary<string, string[]>> linkNodeFunctionIds = new Dictionary<int, Dictionary<string, string[]>>()
                                {
                                    { 16, new Dictionary<string, string[]>() //East
                                    {
                                        { "1249", new[]{ "882", "1252" } },
                                        { "1250", new[]{ "927", "1254" } },
                                        { "1251", new[]{ "1017", "1256" } },
                                    }},
                                    { 17, new Dictionary<string, string[]>() //Bottom
                                    {
                                        { "1494", new[]{ "1074", "1493" } },
                                        { "1491", new[]{ "1490", "1489" } },
                                        { "1488", new[]{ "1061", "1486" } },
                                    }},
                                    { 18, new Dictionary<string, string[]>() //Top
                                    {
                                        { "195", new[]{ "961", "1012" } },
                                        { "197", new[]{ "933", "908" } },
                                        { "196", new[]{ "897", "877" } },
                                    }},
                                };
                                foreach (var buttonGroup in linkNodeFunctionIds)
                                {
                                    string groupItemName = ArchipelagoManager.GetItemName(buttonGroup.Key + APData.castleButtonItemStartID);
                                    foreach (var buttonNodes in buttonGroup.Value)
                                    {
                                        Vector2 flagNodePos = NodeHelper.PosFromString(idToNode[buttonNodes.Key].Element("vec2").Value) + new Vector2(0, 2);
                                        int flagNodeId = modNodeStartId++;
                                        scriptNodesToAdd.Add(NodeHelper.CreateCheckGlobalFlagNode(flagNodeId, flagNodePos, groupItemName, new[] { int.Parse(buttonNodes.Key) }, null));
                                        for (int n = 0; n < buttonNodes.Value.Length; n++)
                                        {
                                            //Modify the connection nodes to connect to the CheckGlobalFlag node instead of the ScriptLink node
                                            XElement connectionNode = idToNode[buttonNodes.Value[n]].Element("int-arr");
                                            connectionNode.Value = string.Join(" ", connectionNode.Value.Split(' ').Where(str => str != buttonNodes.Key)) + $" {flagNodeId}";
                                        }
                                    }
                                }
                            }

                            nodesToNuke.AddRange(new List<int> { 629, 628, 625, 626 }); //Rune puzzle RectangleShapes

                            scriptNodesToAdd.AddRange(CreateRandomizedRunePuzzle(levelFile, doodadsNodeRoot, ref buttonEffectNodeIds, new Vector2(-40.5f, -36.5f), ref modNodeStartId, 26,
                                new[] { 570, 569, 568, 567 }, 3354, 630, 300));
                            break;
                        case "level_3.xml": //Prison 3
                            int[] p3BronzeKey1 = { 381, 422, 783, 2606, 780 };
                            int[] p3BronzeKey2 = { 3286, 1827, 2607, 2888 };
                            int[] p3BronzeKey3 = { 6983, 6526, 6525, 7434 };
                            int[] p3SilverKey = { 1826, 191, 192, 1597, 1598 };
                            int[] p3GoldKey = { 5217, 2224, 2448, 2223, 2447 };

                            globalScriptNodesToTriggerOnceOnLoad.Add(p3BronzeKey1[archipelagoData.GetRandomLocation("Prison 3 Bronze Key 1")]);
                            globalScriptNodesToTriggerOnceOnLoad.Add(p3BronzeKey2[archipelagoData.GetRandomLocation("Prison 3 Bronze Key 2")]);
                            globalScriptNodesToTriggerOnceOnLoad.Add(p3BronzeKey3[archipelagoData.GetRandomLocation("Prison 3 Bronze Key 3")]);
                            globalScriptNodesToTriggerOnceOnLoad.Add(p3SilverKey[archipelagoData.GetRandomLocation("Prison 3 Silver Key 1")]);
                            globalScriptNodesToTriggerOnceOnLoad.Add(p3GoldKey[archipelagoData.GetRandomLocation("Prison 3 Gold Key 1")]);

                            nodesToNuke = new List<int> { 640, 3287, 7304, 380, 5218, 6689, 1043, 1045, 2062 }; //Bronze Keys, Silver Key, Gold Key, Strange plank trigger, bonus close nodes, C2 bridge destroy

                            scriptNodesToAdd.Add(NodeHelper.CreateLevelStartNode(modNodeStartId++, new Vector2(6, 40.5f), 100)); //Offset +0,+2 from RectangleShape
                            scriptNodesToAdd.Add(NodeHelper.CreateLevelStartNode(modNodeStartId++, new Vector2(41.5f, -35f), 88)); //Bonus return

                            int p3DestroyBonusCoverNodeId = modNodeStartId++;
                            scriptNodesToAdd.Add(NodeHelper.CreateDestroyObjectNode(p3DestroyBonusCoverNodeId, new Vector2(41.5f, -40f), 1, new int[] { 1135, 1134 })); //Destroy bonus doodads
                            scriptNodesToAdd.AddRange(NodeHelper.CreateAreaTriggerNodes(ref modNodeStartId, new Vector2(41.5f, -35f), new List<int> { 10005, 1204, 4245, 4246, 1215, 1210, p3DestroyBonusCoverNodeId })); //Bonus portal delete doodads trigger
                            scriptNodesToAdd.AddRange(NodeHelper.CreateAreaTriggerNodes(ref modNodeStartId, new Vector2(52f, -38f), new List<int> { 10005, 1204, 4245, 4246, 1215, 1210, 1042, p3DestroyBonusCoverNodeId })); //Bonus portal delete doodads trigger, bonus return
                            //4254 is the node that shows the secret announce and plays the sound, we might actually want to trigger it but idk

                            //Remove cover nodes if entering from the bonus return
                            NodeHelper.AddConnectionNodes(idToNode["1420"], new int[] { modNodeStartId - 1 });

                            //Make bonus torch nodes only trigger once
                            List<string> p3TorchIds = new List<string> { "1204", "4245", "4246", "1215", "1210" };
                            foreach (string torchId in p3TorchIds)
                            {
                                idToNode[torchId].Elements("int").ToArray()[1].Value = "1";
                            }
                            if (buttonsanity > 0)
                            {
                                //Add open bonus room nodes to the node that opens the side hallway
                                NodeHelper.AddConnectionNodes(idToNode["4249"], new int[] { 1204, 4245, 4246, 1215, 1210 });
                                idToNode["4249"].Element("dictionary").Element("int-arr").Value += " 1134 1135";

                                //Bonus puzzle buttons
                                string[] bonusPuzzleIds = new string[] { "1197", "1188", "4233", "4228", "4429", "1388", "1394", "1181", "1393" };
                                string bonusRuneItemIds = "";
                                foreach (string bonusPuzzleId in bonusPuzzleIds)
                                {
                                    string rectShapeNodeId = idToNode[bonusPuzzleId].Element("dictionary").Element("dictionary").Element("int-arr").Value;
                                    Vector2 itemPos = NodeHelper.PosFromString(idToNode[rectShapeNodeId].Element("vec2").Value) + new Vector2(0, 0.125f);
                                    bonusRuneItemIds += $"{modNodeStartId} ";
                                    scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, itemPos, true));
                                }

                                //Boss rune item and rewire
                                NodeHelper.SetConnectionNodes(idToNode["5784"], new[] { modNodeStartId });
                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, new Vector2(-3.5f, 52), true));
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(2, 41), "a1l3_boss1", ref buttonEffectNodeIds, new[] { 5794 }));

                                //Add the other boss rune CheckGlobalFlag nodes to the button EventTrigger
                                buttonEffectNodeIds.AddRange(new[] { 6297, 6299 });

                                //Bonus room sequence
                                int p3BonusRoomSeqTriggerId = modNodeStartId++;
                                scriptNodesToAdd.Add(NodeHelper.CreateGlobalEventTriggerNode(p3BonusRoomSeqTriggerId, -1, new Vector2(44, -18), "p3_seq_bonus", null));

                                NodeHelper.SetConnectionNodes(idToNode["4253"], p3BonusRoomSeqTriggerId);

                                string bonusRoomItemName = ArchipelagoManager.GetItemName(41 + APData.castleButtonItemStartID);
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(44, -22.5f), bonusRoomItemName, ref buttonEffectNodeIds, new[] { 4251 }, new[] { 200 }));

                                //Bonus rune sequence
                                int p3BonusSeqTriggerId = modNodeStartId++;
                                scriptNodesToAdd.Add(NodeHelper.CreateGlobalEventTriggerNode(p3BonusSeqTriggerId, -1, new Vector2(32, -43), "p3_rune_bonus", null));

                                XElement[] bonusWinConnectionNodes = idToNode["995"].Elements("int-arr").ToArray(); //Bonus win puzzle node
                                bonusWinConnectionNodes[0].Value = $"{bonusRuneItemIds}{p3BonusSeqTriggerId}";
                                bonusWinConnectionNodes[1].Value = string.Join(" ", new int[bonusPuzzleIds.Length + 1]);

                                string bonusPortalItemName = ArchipelagoManager.GetItemName(39 + APData.castleButtonItemStartID);
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(32, -43), bonusPortalItemName, ref buttonEffectNodeIds, new[] { 1203 }, new[] { 1000 }));
                            }

                            //Create return teleporter for shortcut return
                            if (archipelagoData.GetOption(SlotDataKeys.shortcutTeleporter) > 0)
                            {
                                //Remove existing teleport exit
                                foreach (XElement doodadNode in doodadsNodeRoot.Elements())
                                {
                                    if (doodadNode.Element("int").Value == "2002")
                                    {
                                        doodadNode.Remove();
                                    }
                                }
                                nodesToNuke.AddRange(new[] { 2058, 3294, 3299, 3301, 3302 }); //Return spawn node, boss button shapes
                                doodadsToNuke.AddRange(new[] { "3146", "3142", "3143", "3147", "3145", "3144" }); //Boss buttons, boss lock marker, boss sigil
                                //Create return portal + exit
                                doodadsToAdd.AddRange(CreateTeleporter(ref modNodeStartId, new Vector2(-38.5f, 6.5f), 81, "1", 20, -1, false, out List<XElement> shortcutTpScriptNodes));
                                scriptNodesToAdd.AddRange(shortcutTpScriptNodes);
                            }
                            break;
                        case "level_bonus_1.xml": //Bonus 1
                            nodesToNuke = new List<int> { 327 }; //SetGlobalFlag

                            int b1ReturnFlagNodeId = modNodeStartId++;
                            XElement n1ExitFlagNode = NodeHelper.CreateSetGlobalFlagNode(b1ReturnFlagNodeId, new Vector2(-19, 30.5f), 1, "a1bonus_l3return", true);
                            scriptNodesToAdd.Add(n1ExitFlagNode);

                            Dictionary<string, List<string>> b1DeleteNodePanelDoodads = new Dictionary<string, List<string>>
                            {
                                { "589", new List<string>() { "382", "383" } },
                                { "694", new List<string>() { "629", "630" } },
                                { "693", new List<string>() { "631", "632" } },
                                { "960", new List<string>() { "877", "878", "879" } },
                            };
                            Dictionary<string, string> b1AreaTriggerDestroyNodes = new Dictionary<string, string>
                            {
                                { "587", "589" },
                                { "207", "694" },
                                { "692", "693" },
                                { "959" , "960" },
                            };
                            //Connect new SetGlobalFlag node from the exit
                            NodeHelper.AddConnectionNodes(idToNode["841"], new[] { b1ReturnFlagNodeId });
                            foreach (var b1NodePanelDoodads in b1DeleteNodePanelDoodads)
                            {
                                XElement deleteDoodadsNode = idToNode[b1NodePanelDoodads.Key].Element("dictionary").Element("int-arr");
                                deleteDoodadsNode.Value = string.Join(" ", deleteDoodadsNode.Value.Split(' ').Where(str => !b1NodePanelDoodads.Value.Contains(str)));
                            }
                            foreach (var b1AreaTriggerNodes in b1AreaTriggerDestroyNodes)
                            {
                                NodeHelper.AddConnectionNodes(idToNode[b1AreaTriggerNodes.Key], new[] { modNodeStartId });

                                List<string> panelDoodads = b1DeleteNodePanelDoodads[b1AreaTriggerNodes.Value];
                                XElement panelDeleteNode = NodeHelper.CreateDestroyObjectNode(modNodeStartId++, NodeHelper.PosFromString(idToNode[b1AreaTriggerNodes.Key].Element("vec2").Value) + new Vector2(0, 5f), 1, new int[0]);
                                panelDeleteNode.Element("dictionary").Element("int-arr").Value = string.Join(" ", panelDoodads);
                                scriptNodesToAdd.Add(panelDeleteNode);
                            }

                            doodadsToAdd.AddRange(CreateTeleporter(ref modNodeStartId, new Vector2(-27.5f, 33.5f), -1, "3", 88, -1, false, out List<XElement> n1TpScriptNodes));
                            scriptNodesToAdd.AddRange(n1TpScriptNodes);
                            break;
                        case "level_boss_1.xml":
                            doodadsToNuke = new List<string> { "778" }; //Entrance fence
                            nodesToNuke = new List<int> { 919 }; //Boss music

                            scriptNodesToAdd.AddRange(CreateDoorwayTransition(ref modNodeStartId, new Vector2(-24, 18.5f), "3", 100)); //Offset +1,+0 from fence

                            modNodeStartId += 10;
                            int b1CheckGlobalFlagNodeId = modNodeStartId++;
                            int b1LevelMusicNodeId = modNodeStartId++;
                            int b1BossMusicNodeId = modNodeStartId++;
                            int b1SetGlobalFlagNodeId = modNodeStartId++;
                            scriptNodesToAdd.Add(NodeHelper.CreateCheckGlobalFlagNode(b1CheckGlobalFlagNodeId, new Vector2(-3, 15), "killed_boss_1", new[] { b1LevelMusicNodeId }, new[] { b1BossMusicNodeId }));
                            globalScriptNodesToTriggerOnLoad.Add(b1CheckGlobalFlagNodeId);
                            scriptNodesToAdd.Add(NodeHelper.CreatePlayMusicNode(b1LevelMusicNodeId, new Vector2(-5, 14), -1, "sound/music.xml:act1", true));
                            scriptNodesToAdd.Add(NodeHelper.CreatePlayMusicNode(b1BossMusicNodeId, new Vector2(-5, 15), -1, "sound/music.xml:boss_1", true));
                            scriptNodesToAdd.Add(NodeHelper.CreateSetGlobalFlagNode(b1SetGlobalFlagNodeId, new Vector2(9, -45), -1, "killed_boss_1", true));

                            int bossKillStatusId = modNodeStartId++;
                            int rightAreaTriggerId = modNodeStartId++;
                            //Unhook music node from the GlobalEventTrigger and replace with our CheckGlobalFlag node
                            NodeHelper.AddConnectionNodes(idToNode["200"], new[] { b1SetGlobalFlagNodeId, bossKillStatusId }, new[] { 0, bossDefeatMessageDelay });
                            if (buttonsanity > 0)
                            {
                                //Button AreaTrigger
                                int checkFlagNodeId = modNodeStartId++;

                                int leftButtonItemId = modNodeStartId;
                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, new Vector2(-7.5f, -9.5f), true));
                                int rightButtonItemId = modNodeStartId;
                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, new Vector2(1.5f, -9.5f), true));

                                //Set connections to ChangeDoodadState node and our custom stuff
                                NodeHelper.SetConnectionNodes(idToNode["498"], new[] { 499, checkFlagNodeId, leftButtonItemId, rightButtonItemId });

                                scriptNodesToAdd.Add(NodeHelper.CreateAreaTriggerNode(rightAreaTriggerId, -1, new Vector2(1.5f, -12), 1, 0, new int[] { 722 }, new int[] { checkFlagNodeId }));
                                string enableItemName = ArchipelagoManager.GetItemName(56 + APData.castleButtonItemStartID);
                                scriptNodesToAdd.Add(NodeHelper.CreateCheckGlobalFlagNode(checkFlagNodeId, new Vector2(-5.5f, -14), enableItemName, new int[] { 500 }, null));
                                globalScriptNodesToTriggerOnLoad.Add(checkFlagNodeId);
                                scriptNodesToAdd.Add(NodeHelper.CreateGlobalEventTriggerNode(modNodeStartId++, -1, new Vector2(-5.5f, -16), enableItemName, new int[] { 500 }));

                                //Button ScriptLink node
                                NodeHelper.SetConnectionNodes(idToNode["500"], new[] { 757, 501, 736, 716 }, new[] { 0, 0, 0, 7000 });

                                //Disable/Enable button node
                                idToNode["501"].Element("dictionary").Element("dictionary").Element("int-arr").Value += $" {rightAreaTriggerId}";
                                idToNode["718"].Element("dictionary").Element("dictionary").Element("int-arr").Value += $" {rightAreaTriggerId}";
                            }

                            scriptNodesToAdd.Add(NodeHelper.CreateLevelStartNode(modNodeStartId++, new Vector2(-3, -55.5f), 1)); //Offset +0,+2 from RectangleShape

                            doodadsToNuke.Add("8"); //End doodad cover
                            //Move other doodad cover down
                            foreach (XElement doodad in doodadsNodeRoot.Elements())
                            {
                                if (doodad.Element("int").Value == "7")
                                {
                                    doodad.Element("vec2").Value = "-11 -44.75";
                                    break;
                                }
                            }
                            break;
                        case "level_4.xml": //Armory 4
                            nodesToNuke = new List<int> { 3451 }; //Strange plank trigger

                            scriptNodesToAdd.AddRange(CreateDoorwayTransition(ref modNodeStartId, new Vector2(-46, 49.5f), "boss_1", 1)); //Offset +1,+0 from fence

                            scriptNodesToAdd.Add(NodeHelper.CreateLevelStartNode(modNodeStartId++, new Vector2(-40, 18.5f), 100)); //Offset +0,+2 from RectangleShape

                            doodadsToNuke.Add("6288"); //Entrance fence

                            if (buttonsanity > 0)
                            {
                                //Boss rune item and rewire
                                NodeHelper.SetConnectionNodes(idToNode["662"], modNodeStartId);
                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, new Vector2(-67, -51), true));
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(-62, -53), "a2l4_boss2", ref buttonEffectNodeIds, new[] { 670 }));

                                //Add the other boss rune CheckGlobalFlag nodes to the button EventTrigger
                                buttonEffectNodeIds.AddRange(new[] { 6347, 6349 });
                            }

                            nodesToNuke.AddRange(new List<int> { 5847, 5846, 5848, 5915 }); //Rune puzzle RectangleShapes

                            scriptNodesToAdd.AddRange(CreateRandomizedRunePuzzle(levelFile, doodadsNodeRoot, ref buttonEffectNodeIds, new Vector2(67.25f, 27f), ref modNodeStartId, 63,
                                new[] { 5772, 5773, 5774, 5775 }, 7245, 5850, 1000));

                            //Hub portal
                            int a2TriggerShapeNodeId = modNodeStartId++;
                            scriptNodesToAdd.Add(NodeHelper.CreateRectangleShapeNode(a2TriggerShapeNodeId, new Vector2(-40f, 50f), 5, 7, 15));
                            List<XElement> a1TpDoodads = CreateHubPortal(ref modNodeStartId, new Vector2(-36.5f, 49.5f), -1, "ap_hub", 0, -1, false, sign_hub_text, out List<XElement> a1TpScriptNodes, "portal_a2", a2TriggerShapeNodeId);
                            doodadsToAdd.AddRange(a1TpDoodads);
                            scriptNodesToAdd.AddRange(a1TpScriptNodes);
                            break;
                        case "level_5.xml": //Armory 5
                            nodesToNuke = new List<int> { 4819, 302 }; //Strange plank trigger, bonus tp close

                            //Make the bonus portal exit area trigger times be infinite
                            idToNode["295"].Elements("int").ToArray()[1].Value = "-1";
                            //Wire the bonus return area trigger node to open the wall
                            NodeHelper.AddConnectionNodes(idToNode["116"], new[] { 3122 });

                            if (buttonsanity > 0)
                            {
                                XElement[] connectionNodes = idToNode["991"].Elements("int-arr").ToArray();
                                connectionNodes[0].Value = modNodeStartId.ToString();
                                connectionNodes[1].Value = "0";

                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, new Vector2(20, -55), true));

                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(25, -57), "a2l5_boss2", ref buttonEffectNodeIds, new[] { 999 }));

                                //Change the blue spikes to activate only when getting the item
                                string blueSpikesName = ArchipelagoManager.GetItemName(APData.castleButtonItemStartID + 84);
                                idToNode["3125"].Element("dictionary").Element("string").Value = blueSpikesName;
                                buttonEffectNodeIds.Add(3125);
                            }

                            nodesToNuke.AddRange(new List<int> { 1840, 1847, 1819, 1826 }); //Rune puzzle RectangleShapes

                            scriptNodesToAdd.AddRange(CreateRandomizedRunePuzzle(levelFile, doodadsNodeRoot, ref buttonEffectNodeIds, new Vector2(64f, -68f), ref modNodeStartId, 78,
                                new[] { 1776, 1775, 1774, 1773 }, 1865, 1508, 300));

                            globalScriptNodesToTriggerOnLoad.Add(3123); //PlayMusic node

                            scriptNodesToAdd.Add(NodeHelper.CreateLevelStartNode(modNodeStartId++, new Vector2(-35.5f, -59.5f), 88)); //Bonus return
                            break;
                        case "level_bonus_2.xml": //Bonus 2
                            nodesToNuke = new List<int> { 19 }; //SetGlobalFlag

                            Dictionary<string, List<string>> b2DeleteNodePanelDoodads = new Dictionary<string, List<string>>
                            {
                                { "1025", new List<string>() { "163" } },
                                { "127", new List<string>() { "164", "165", "166", "167", "168" } },
                                { "599", new List<string>() { "909", "910" } },
                            };
                            Dictionary<string, string> b2AreaTriggerDestroyNodes = new Dictionary<string, string>
                            {
                                { "1026", "1025" },
                                { "311", "127" },
                                { "1024", "599" },
                            };

                            int b2ReturnFlagNodeId = modNodeStartId++;
                            scriptNodesToAdd.Add(NodeHelper.CreateSetGlobalFlagNode(b2ReturnFlagNodeId, new Vector2(-11, 2f), 1, "a2bonus_l5return", true));

                            NodeHelper.AddConnectionNodes(idToNode["126"], new[] { b2ReturnFlagNodeId });

                            foreach(string b2DeleteNodePanelId in b2DeleteNodePanelDoodads.Keys)
                            {
                                List<string> deleteDoodads = b2DeleteNodePanelDoodads[b2DeleteNodePanelId];
                                XElement deleteDoodadsNode = idToNode[b2DeleteNodePanelId].Element("dictionary").Element("int-arr");
                                deleteDoodadsNode.Value = string.Join(" ", deleteDoodadsNode.Value.Split(' ').Where(str => !deleteDoodads.Contains(str)));
                            }
                            foreach (string b2AreaTriggerDestroyNodeId in b2AreaTriggerDestroyNodes.Keys)
                            {
                                XElement node = idToNode[b2AreaTriggerDestroyNodeId];
                                NodeHelper.AddConnectionNodes(node, new int[] { modNodeStartId });

                                List<string> panelDoodads = b2DeleteNodePanelDoodads[b2AreaTriggerDestroyNodes[b2AreaTriggerDestroyNodeId]];
                                XElement panelDeleteNode = NodeHelper.CreateDestroyObjectNode(modNodeStartId++, NodeHelper.PosFromString(node.Element("vec2").Value) + new Vector2(0, 5f), 1, new int[0]);
                                panelDeleteNode.Element("dictionary").Element("int-arr").Value = string.Join(" ", panelDoodads);
                                scriptNodesToAdd.Add(panelDeleteNode);
                            }

                            XElement[] n2TpDoodads = CreateTeleporter(ref modNodeStartId, new Vector2(-26.5f, 18.5f), -1, "5", 88, -1, false, out List<XElement> n2TpScriptNodes);
                            doodadsToAdd.AddRange(n2TpDoodads);
                            scriptNodesToAdd.AddRange(n2TpScriptNodes);
                            break;
                        case "level_6.xml": //Armory 6
                            //Rewire the plank area trigger node
                            XElement[] intArrNodes = idToNode["3443"].Elements("int-arr").ToArray();
                            intArrNodes[0].Value = "1592";
                            intArrNodes[1].Value = "0";
                            //Make the level 6 entrance only spawn the torch once
                            idToNode["423"].Elements("int").ToArray()[1].Value = "1";

                            if (buttonsanity > 0)
                            {
                                //Boss rune item and rewire
                                NodeHelper.SetConnectionNodes(idToNode["2129"], modNodeStartId);
                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, new Vector2(50.5f, -57), true));
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(55.5f, -59), "a2l6_boss2", ref buttonEffectNodeIds, new[] { 2134 }));

                                //Knife sequence
                                int a3KnifeSeqTriggerId = modNodeStartId++;
                                scriptNodesToAdd.Add(NodeHelper.CreateGlobalEventTriggerNode(a3KnifeSeqTriggerId, -1, new Vector2(-0.5f, -72), "a3_seq_knife", null));

                                XElement[] knife1ConnectionNodes = idToNode["839"].Elements("int-arr").ToArray();
                                knife1ConnectionNodes[0].Value = a3KnifeSeqTriggerId.ToString();
                                knife1ConnectionNodes[1].Value = "0";

                                string knifeItemName = ArchipelagoManager.GetItemName(88 + APData.castleButtonItemStartID);
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(0, -78), knifeItemName, ref buttonEffectNodeIds, new[] { 1587, 846 }, new[] { 300, 300 }));

                                //Knife sequence 2
                                int a3KnifeSeq2TriggerId = modNodeStartId++;
                                scriptNodesToAdd.Add(NodeHelper.CreateGlobalEventTriggerNode(a3KnifeSeq2TriggerId, -1, new Vector2(-3f, -48), "a3_seq_knife_2", null));

                                NodeHelper.SetConnectionNodes(idToNode["1739"], a3KnifeSeq2TriggerId);

                                string knife2ItemName = ArchipelagoManager.GetItemName(92 + APData.castleButtonItemStartID);
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(-2, -44), knife2ItemName, ref buttonEffectNodeIds, new[] { 1740 }, new[] { 1000 }));

                                //Fix knife reward rooms doodads
                                doodadsToNuke.AddRange(new List<string> { "1005" });
                                doodadsToAdd.Add(NodeHelper.CreateDoodadNode(modNodeStartId++, "doodads/special/color_theme_b_48.xml", new Vector2(-17.5f, -59.5f), true));
                                doodadsToAdd.Add(NodeHelper.CreateDoodadNode(modNodeStartId++, "doodads/special/color_theme_b_48.xml", new Vector2(-17.5f, -57.0f), true));
                                doodadsToAdd.Add(NodeHelper.CreateDoodadNode(modNodeStartId++, "doodads/special/color_theme_b_48.xml", new Vector2(-17.5f, -54.5f), true));

                                idToNode["1587"].Element("dictionary").Element("int-arr").Value += $" {modNodeStartId - 3} {modNodeStartId - 2} {modNodeStartId - 1}";
                                idToNode["1080"].Element("dictionary").Element("int-arr").Value = $"998 999 997 1003 1004"; //1000 1001 1002

                                int enableNodeId = modNodeStartId++;
                                int destroyWallNodeId = modNodeStartId++;

                                NodeHelper.AddConnectionNodes(idToNode["1587"], new[] { enableNodeId, destroyWallNodeId }, new[] { 1, 0 });
                                NodeHelper.AddConnectionNodes(idToNode["1080"], new[] { enableNodeId, destroyWallNodeId }, new[] { 1, 0 });

                                scriptNodesToAdd.Add(NodeHelper.CreateToggleElementNode(enableNodeId, new Vector2(-21.5f, -48.5f), 1, 0, new int[] { destroyWallNodeId }));

                                XElement destroyNode = NodeHelper.CreateDestroyObjectNode(destroyWallNodeId, new Vector2(-21.5f, -50.5f), 1, new int[] { 1000, 1001, 1002 });
                                destroyNode.Element("bool").Value = "False";
                                NodeHelper.AddConnectionNodes(destroyNode, new int[] { 1081 });
                                scriptNodesToAdd.Add(destroyNode);

                                //Teleport pyramid item node
                                buttonEffectNodeIds.AddRange(new int[] { 2731, 3020, 3017, 3018 });
                                idToNode["2731"].Element("dictionary").Element("string").Value = ArchipelagoManager.GetItemName(APData.castleButtonItemStartID + 69); //NW
                                idToNode["3020"].Element("dictionary").Element("string").Value = ArchipelagoManager.GetItemName(APData.castleButtonItemStartID + 72); //NE
                                idToNode["3017"].Element("dictionary").Element("string").Value = ArchipelagoManager.GetItemName(APData.castleButtonItemStartID + 73); //SW
                                idToNode["3018"].Element("dictionary").Element("string").Value = ArchipelagoManager.GetItemName(APData.castleButtonItemStartID + 74); //SE
                            }
                            scriptNodesToAdd.AddRange(NodeHelper.CreateAreaTriggerNodes(ref modNodeStartId, new Vector2(-46f, -66.5f), new List<int> { 418 })); //Bonus portal delete doodads trigger
                            break;
                        case "level_boss_2.xml": //Boss 2
                            doodadsToNuke = new List<string> { "462" }; //Entrance fence

                            XElement[] b2DoorTransitionNodes = CreateDoorwayTransition(ref modNodeStartId, new Vector2(-16, 5.5f), "4", 100); //Offset +1,+0 from fence
                            scriptNodesToAdd.AddRange(b2DoorTransitionNodes);

                            int b2CheckGlobalFlagNodeId = modNodeStartId + 10;
                            int b2LevelMusicNodeId = modNodeStartId + 11;
                            int b2BossMusicNodeId = modNodeStartId + 12;
                            int b2SetGlobalFlagNodeId = modNodeStartId + 13;
                            XElement b2CheckBossKilledNode = NodeHelper.CreateCheckGlobalFlagNode(b2CheckGlobalFlagNodeId, new Vector2(-3, 15), "killed_boss_2", new[] { b2LevelMusicNodeId }, new[] { b2BossMusicNodeId });
                            scriptNodesToAdd.Add(b2CheckBossKilledNode);
                            globalScriptNodesToTriggerOnLoad.Add(b2CheckGlobalFlagNodeId);
                            XElement b2LevelMusicNode = NodeHelper.CreatePlayMusicNode(b2LevelMusicNodeId, new Vector2(-5, 14), -1, "sound/music.xml:act2", true);
                            scriptNodesToAdd.Add(b2LevelMusicNode);
                            XElement b2BossMusicNode = NodeHelper.CreatePlayMusicNode(b2BossMusicNodeId, new Vector2(-5, 15), -1, "sound/music.xml:boss_1", true);
                            scriptNodesToAdd.Add(b2BossMusicNode);
                            XElement b2SetFlagNode = NodeHelper.CreateSetGlobalFlagNode(b2SetGlobalFlagNodeId, new Vector2(9, -45), -1, "killed_boss_2", true);
                            scriptNodesToAdd.Add(b2SetFlagNode);

                            //Unhook music node from the GlobalEventTrigger and replace with our CheckGlobalFlag node
                            NodeHelper.AddConnectionNodes(idToNode["229"], new[] { b2SetGlobalFlagNodeId });

                            scriptNodesToAdd.Add(NodeHelper.CreateLevelStartNode(modNodeStartId + 14, new Vector2(1, -57.5f), 1)); //Offset +0,+2 from RectangleShape

                            //Move doodad cover down
                            foreach (XElement doodad in doodadsNodeRoot.Elements())
                            {
                                if (doodad.Element("int").Value == "50")
                                {
                                    doodad.Element("vec2").Value = "-6 -48.5";
                                    break;
                                }
                            }
                            break;
                        case "level_7.xml": //Archives 7
                            nodesToNuke = new List<int> { 4512 }; //Strange plank trigger

                            Vector2 r1ExitPos = new Vector2(-1, 10.5f);
                            XElement[] r1DoorTransitionNodes = CreateDoorwayTransition(ref modNodeStartId, r1ExitPos, "boss_2", 1); //Offset +1,+0 from fence
                            scriptNodesToAdd.AddRange(r1DoorTransitionNodes);

                            doodadsToNuke.Add("4307");

                            int l7DestroyId = modNodeStartId++;
                            scriptNodesToAdd.AddRange(NodeHelper.CreateAreaTriggerNodes(ref modNodeStartId, new Vector2(19f, 35f), new List<int> { l7DestroyId, 6430 })); //Right exit trigger, torch spawn
                            scriptNodesToAdd.Add(NodeHelper.CreateDestroyObjectNode(l7DestroyId, new Vector2(19f, 30f), 1, new int[] { 4459 })); //Destroy doodad

                            //Make the torch spawn only once
                            idToNode["6430"].Elements("int").ToArray()[1].Value = "1";

                            //Remove cover over left exit if both exits are opened
                            idToNode["6271"].Element("dictionary").Element("int-arr").Value += " 6246";

                            //Hub portal
                            int a3TriggerShapeNodeId = modNodeStartId++;
                            scriptNodesToAdd.Add(NodeHelper.CreateRectangleShapeNode(a3TriggerShapeNodeId, new Vector2(-1f, 14f), 6, 7, 15));
                            List<XElement> r1TpDoodads = CreateHubPortal(ref modNodeStartId, new Vector2(-4f, 11.5f), -1, "ap_hub", 0, -1, true, sign_hub_text, out List<XElement> r1TpScriptNodes, "portal_a3", a3TriggerShapeNodeId);
                            doodadsToAdd.AddRange(r1TpDoodads);
                            scriptNodesToAdd.AddRange(r1TpScriptNodes);
                            break;
                        case "level_8.xml": //Archives 8
                            nodesToNuke = new List<int> { 8181, 8610 }; //Strange plank triggers

                            int l8DestroyId = modNodeStartId++;
                            scriptNodesToAdd.AddRange(NodeHelper.CreateAreaTriggerNodes(ref modNodeStartId, new Vector2(-19f, 1.25f), new List<int> { l8DestroyId, 4188 })); //Right exit trigger, torch spawn
                            scriptNodesToAdd.Add(NodeHelper.CreateDestroyObjectNode(l8DestroyId, new Vector2(-19f, -5f), 1, new int[] { 4102, 4101, 4100, 4096, 4099 })); //Destroy doodads

                            //Make the torch spawn only once
                            idToNode["4188"].Elements("int").ToArray()[1].Value = "1";

                            if (buttonsanity > 0)
                            {
                                //Add connections to delete the cover at the right side of the south area if the other two entrances get opened
                                idToNode["7590"].Element("dictionary").Element("int-arr").Value += " 7675";
                                idToNode["3637"].Element("dictionary").Element("int-arr").Value += " 7675";

                                //NW boss rune item and rewire
                                NodeHelper.SetConnectionNodes(idToNode["862"], modNodeStartId);
                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, new Vector2(-22, -58), true));
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(-16, -55), "a3l8_boss3", ref buttonEffectNodeIds, new[] { 854 }));

                                //East boss rune item and rewire
                                NodeHelper.SetConnectionNodes(idToNode["4037"], modNodeStartId);
                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, new Vector2(21, 3), true));
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(26, 1), "a3l7_boss3", ref buttonEffectNodeIds, new[] { 4582 }));
                            }

                            //Bump up exit wall so it looks less bad
                            foreach (XElement doodad in doodadsNodeRoot.Elements())
                            {
                                if (doodad.Element("int").Value == "4094")
                                {
                                    doodad.Element("vec2").Value = "-23 -0.25";
                                }
                            }

                            nodesToNuke.AddRange(new List<int> { 6326, 6327, 6490, 6489 }); //Rune puzzle RectangleShapes

                            scriptNodesToAdd.AddRange(CreateRandomizedRunePuzzle(levelFile, doodadsNodeRoot, ref buttonEffectNodeIds, new Vector2(-69.5f, 53.5f), ref modNodeStartId, 115,
                                new[] { 6290, 6291, 6413, 6414 }, 6544, 6492, 1000));
                            break;
                        case "level_9.xml": //Archives 9
                            nodesToNuke = new List<int> { 12205, 5577 }; //Bonus tp close, bonus return check flag node

                            scriptNodesToAdd.Add(NodeHelper.CreateLevelStartNode(modNodeStartId++, new Vector2(-38, -44.5f), 100)); //Offset +0,+2 from RectangleShape

                            int l9DestroyId = modNodeStartId++;
                            scriptNodesToAdd.AddRange(NodeHelper.CreateAreaTriggerNodes(ref modNodeStartId, new Vector2(78f, -73.5f), new List<int> { 2825 })); //Bonus return trigger
                            scriptNodesToAdd.AddRange(NodeHelper.CreateAreaTriggerNodes(ref modNodeStartId, new Vector2(-38, -44.5f), new List<int> { l9DestroyId })); //Boss exit trigger
                            scriptNodesToAdd.Add(NodeHelper.CreateDestroyObjectNode(l9DestroyId, new Vector2(-38, -50f), 1, new int[] { 662, 1397, 1398, 4786, 4787 })); //Destroy doodads

                            string[][] simonSequenceAdvanceNodes = new string[][]
                            {
                                new string[]{ "13482", "13486", "13488", "13490", "13492", "13249" },
                                new string[]{ "13544", "13547", "13549", "13551", "13553", "13555" },
                                new string[]{ "13466", "13470", "13474", "13475", "13477", "13479" },
                                new string[]{ "13528", "13531", "13533", "13538", "13540", "13542" },
                                new string[]{ "13497", "13499", "13501", "13503", "13505", "13507" },
                                new string[]{ "13509", "13518", "13521", "13522", "13524", "13526" },
                            };
                            Vector2[] simonButtonPositions = new Vector2[] {
                                new Vector2(77, 47.5f),
                                new Vector2(80, 47.5f),
                                new Vector2(76, 49.5f),
                                new Vector2(81, 49.5f),
                                new Vector2(77, 51.5f),
                                new Vector2(80, 51.5f),
                            };
                            int[] simonItemSpawnIds = new int[6];
                            if (buttonsanity > 0)
                            {
                                for (int b = 0; b < simonItemSpawnIds.Length; b++)
                                {
                                    simonItemSpawnIds[b] = modNodeStartId;
                                    scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, simonButtonPositions[b], true));
                                }

                                //Removed end of secret passage cover doodads when entering from the top
                                NodeHelper.AddConnectionNodes(idToNode["4533"], new int[] { 4545 });
                                idToNode["4533"].Element("dictionary").Element("int-arr").Value += " 219 135 218";
                                NodeHelper.AddConnectionNodes(idToNode["4980"], new int[] { 5651 });
                                idToNode["4980"].Element("dictionary").Element("int-arr").Value += " 288 241 4945";
                                idToNode["9341"].Element("dictionary").Element("int-arr").Value += $" {modNodeStartId} {modNodeStartId + 1}";

                                doodadsToAdd.Add(NodeHelper.CreateDoodadNode(modNodeStartId++, "doodads/special/color_theme_c_128.xml", "-91.5 31.5", true));
                                doodadsToAdd.Add(NodeHelper.CreateDoodadNode(modNodeStartId++, "doodads/special/color_theme_c_128.xml", "-91.5 32.5", true));

                                foreach (XElement doodad in doodadsNodeRoot.Elements().ToArray())
                                {
                                    string nodeId = doodad.Element("int").Value;
                                    if (nodeId == "4945")
                                        doodad.Element("vec2").Value = "-88.5 0.5";
                                    else if (nodeId == "289")
                                        doodad.Remove();
                                }

                                //Fix torch nodes that could spawn multiple times
                                List<string> r3TorchIds = new List<string> { "4545", "5651" };
                                foreach (string torchId in r3TorchIds)
                                {
                                    idToNode[torchId].Elements("int").ToArray()[1].Value = "1";
                                }

                                //Boss rune item and rewire
                                NodeHelper.SetConnectionNodes(idToNode["12609"], modNodeStartId);
                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, new Vector2(65, 72), true));
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(70, 70), "a3l9_boss3", ref buttonEffectNodeIds, new[] { 12969 }));

                                //Add the other boss rune CheckGlobalFlag nodes to the button EventTrigger
                                buttonEffectNodeIds.AddRange(new[] { 4865, 4867 });

                                //Simon open room sequence
                                int r3SimonRoomSeqTriggerId = modNodeStartId++;
                                scriptNodesToAdd.Add(NodeHelper.CreateGlobalEventTriggerNode(r3SimonRoomSeqTriggerId, -1, new Vector2(43, 58), "r3_seq_simon_room", null));

                                NodeHelper.SetConnectionNodes(idToNode["12474"], r3SimonRoomSeqTriggerId);

                                string simonSeqItemName = ArchipelagoManager.GetItemName(142 + APData.castleButtonItemStartID);
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(41.5f, 64), simonSeqItemName, ref buttonEffectNodeIds, new[] { 12481 }, new[] { 300 }));

                                //Activate simon says button
                                XElement activeSimonSequenceButtonNode = idToNode["13325"];
                                int simonSaysButtonItem = modNodeStartId;
                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, new Vector2(78.5f, 54), true));
                                int resetButtonToggleNodeId = modNodeStartId++;
                                int buttonToggleNodeId = modNodeStartId++;
                                XElement[] activateSimonSaysConnectionNodes = activeSimonSequenceButtonNode.Elements("int-arr").ToArray();
                                string[] connectionStringIds = activateSimonSaysConnectionNodes[0].Value.Split(' ').Where((id) => id != "13323").ToArray();
                                int[] connectionIds = new int[connectionStringIds.Length];
                                for (int s = 0; s < connectionStringIds.Length - 2; s++)
                                {
                                    connectionIds[s] = int.Parse(connectionStringIds[s]);
                                }
                                int[] connectionDelays = new int[connectionIds.Length];
                                connectionDelays[0] = 10;
                                int checkFlagNodeId = modNodeStartId++;
                                scriptNodesToAdd.Add(NodeHelper.CreateScriptLinkNode(13325, true, -1, new Vector2(157.5f, 76), connectionIds, connectionDelays));
                                int activeSimonSequenceButtonId = modNodeStartId++;
                                activeSimonSequenceButtonNode.Element("int").Value = activeSimonSequenceButtonId.ToString();
                                activeSimonSequenceButtonNode.Element("vec2").Value = "153 73.5";
                                activateSimonSaysConnectionNodes[0].Value = $"13323 {checkFlagNodeId} {simonSaysButtonItem}";
                                activateSimonSaysConnectionNodes[1].Value = "0 0 0";
                                string activateSimonSaysPuzzleItemName = ArchipelagoManager.GetItemName(133 + APData.castleButtonItemStartID);
                                scriptNodesToAdd.Add(NodeHelper.CreateCheckGlobalFlagNode(checkFlagNodeId, new Vector2(155, 73.5f), activateSimonSaysPuzzleItemName, false, new int[] { 13325 }, false, null));
                                scriptNodesToAdd.Add(NodeHelper.CreateToggleElementNode(buttonToggleNodeId, new Vector2(153, 77.5f), -1, 2, new int[] { 13323 }));
                                NodeHelper.AddConnectionNodes(idToNode["13323"], new int[] { buttonToggleNodeId });
                                //Clone AreaTrigger node so that it pops the button back up
                                int resetSimonSequenceId = modNodeStartId++;
                                NodeHelper.AddConnectionNodes(idToNode["13564"], new int[] { buttonToggleNodeId });
                                //Add nodes to pop the button up if we receive the activate item
                                int simonActivateCheckFlagId = modNodeStartId++;
                                int simonActivateSetStateId = modNodeStartId++;
                                int simonActivatedToggleId = modNodeStartId++;
                                scriptNodesToAdd.Add(NodeHelper.CreateCheckGlobalFlagNode(simonActivateCheckFlagId, new Vector2(148, 78f), activateSimonSaysPuzzleItemName, false, new int[] { simonActivateSetStateId }, false, null));
                                scriptNodesToAdd.Add(NodeHelper.CreateChangeDoodadStateNode(simonActivateSetStateId, new Vector2(150, 78f), 1, "raised", new int[] { 12676 }));
                                NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { simonActivatedToggleId });
                                scriptNodesToAdd.Add(NodeHelper.CreateToggleElementNode(simonActivatedToggleId, new Vector2(150, 76f), -1, 0, new int[] { 13323 }));
                                globalScriptNodesToTriggerOnLoad.Add(simonActivateCheckFlagId);
                                buttonEffectNodeIds.Add(simonActivateCheckFlagId);

                                //Simon says complete node
                                NodeHelper.SetConnectionNodes(idToNode["13245"], new int[] { modNodeStartId, 13229 });
                                NodeHelper.AddConnectionNodes(idToNode["13245"], simonItemSpawnIds);

                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, new Vector2(78.5f, 50.5f), true));

                                string bonusPortalPassageItemName = ArchipelagoManager.GetItemName(136 + APData.castleButtonItemStartID);
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(51, 51), bonusPortalPassageItemName, ref buttonEffectNodeIds, new[] { 12616 }, new[] { 300 }));

                                //Simon says 1st sequence
                                int seqReward = modNodeStartId;
                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, new Vector2(), true));
                                idToNode["13261"].Element("dictionary").Element("dictionary").Element("int-arr").Value += $" {seqReward}";

                                //NE tp item top unhook
                                NodeHelper.SetConnectionNodes(idToNode["8143"], 8141);

                                for (int b = 0; b < simonItemSpawnIds.Length; b++)
                                {
                                    foreach (string advanceNode in simonSequenceAdvanceNodes[b])
                                    {
                                        idToNode[advanceNode].Element("dictionary").Element("dictionary").Element("int-arr").Value += $" {simonItemSpawnIds[b]}";
                                    }
                                }
                            }

                            nodesToNuke.AddRange(new List<int> { 4240, 4241, 8250, 8251 }); //Rune puzzle RectangleShapes

                            scriptNodesToAdd.AddRange(CreateRandomizedRunePuzzle(levelFile, doodadsNodeRoot, ref buttonEffectNodeIds, new Vector2(83.5f, -29.5f), ref modNodeStartId, 134,
                                new[] { 4146, 4147, 8211, 8212 }, 13060, 8253, 1000));
                            break;
                        case "level_bonus_3.xml": //Bonus 3
                            nodesToNuke = new List<int> { 3 }; //SetGlobalFlag

                            int b3ReturnId = modNodeStartId++;
                            scriptNodesToAdd.Add(NodeHelper.CreateSetGlobalFlagNode(b3ReturnId, new Vector2(-7, -22f), 1, "a3bonus_l9return", true));

                            //Level exit
                            NodeHelper.AddConnectionNodes(idToNode["137"], new int[] { b3ReturnId });
                            break;
                        case "level_boss_3.xml": //Boss 3
                            doodadsToNuke = new List<string> { "452" }; //Entrance fence

                            XElement[] b3DoorTransitionNodes = CreateDoorwayTransition(ref modNodeStartId, new Vector2(-5, 19.5f), "9", 100); //Offset +1,+0 from fence
                            scriptNodesToAdd.AddRange(b3DoorTransitionNodes);

                            int b3CheckGlobalFlagNodeId = modNodeStartId++;
                            int b3LevelMusicNodeId = modNodeStartId++;
                            int b3BossMusicNodeId = modNodeStartId++;
                            int b3SetGlobalFlagNodeId = modNodeStartId++;
                            XElement b3CheckBossKilledNode = NodeHelper.CreateCheckGlobalFlagNode(b3CheckGlobalFlagNodeId, new Vector2(-3, 15), "killed_boss_3", new[] { b3LevelMusicNodeId }, new[] { b3BossMusicNodeId });
                            scriptNodesToAdd.Add(b3CheckBossKilledNode);
                            globalScriptNodesToTriggerOnLoad.Add(b3CheckGlobalFlagNodeId);
                            XElement b3LevelMusicNode = NodeHelper.CreatePlayMusicNode(b3LevelMusicNodeId, new Vector2(-5, 14), -1, "sound/music.xml:act4", true);
                            scriptNodesToAdd.Add(b3LevelMusicNode);
                            XElement b3BossMusicNode = NodeHelper.CreatePlayMusicNode(b3BossMusicNodeId, new Vector2(-5, 15), -1, "sound/music.xml:boss_1", true);
                            scriptNodesToAdd.Add(b3BossMusicNode);
                            XElement b3SetFlagNode = NodeHelper.CreateSetGlobalFlagNode(b3SetGlobalFlagNodeId, new Vector2(9, -45), -1, "killed_boss_3", true);
                            scriptNodesToAdd.Add(b3SetFlagNode);

                            //Unhook music node from the GlobalEventTrigger and replace with our CheckGlobalFlag node
                            NodeHelper.AddConnectionNodes(idToNode["201"], new[] { b3SetGlobalFlagNodeId });

                            scriptNodesToAdd.Add(NodeHelper.CreateLevelStartNode(modNodeStartId++, new Vector2(1, -38.5f), 1)); //Offset +0,+2 from RectangleShape

                            //Move doodad cover down
                            foreach (XElement doodad in doodadsNodeRoot.Elements())
                            {
                                if (doodad.Element("int").Value == "51")
                                {
                                    doodad.Element("vec2").Value = "-5.5 -29.5";
                                    break;
                                }
                            }
                            break;
                        case "level_10.xml": //Chambers 10
                            nodesToNuke = new List<int> { 3586 }; //Strange plank trigger

                            int c1DoorwayId = modNodeStartId;
                            Vector2 c1ExitPos = new Vector2(72, -14.5f);
                            XElement[] c1DoorTransitionNodes = CreateDoorwayTransition(ref modNodeStartId, c1ExitPos, "boss_3", 1); //Offset +1,+0 from fence
                            scriptNodesToAdd.AddRange(c1DoorTransitionNodes);

                            doodadsToNuke.Add("4332");

                            //Hub portal
                            int a4TriggerShapeNodeId = modNodeStartId++;
                            scriptNodesToAdd.Add(NodeHelper.CreateRectangleShapeNode(a4TriggerShapeNodeId, new Vector2(66.5f, -11f), 6, 6, 15));
                            List<XElement> c1TpDoodads = CreateHubPortal(ref modNodeStartId, new Vector2(66.5f, -13.5f), -1, "ap_hub", 0, -1, false, sign_hub_text, out List<XElement> c1TpScriptNodes, "portal_a4", a4TriggerShapeNodeId);
                            doodadsToAdd.AddRange(c1TpDoodads);
                            scriptNodesToAdd.AddRange(c1TpScriptNodes);
                            break;
                        case "level_10_special.xml":
                            nodesToNuke = new List<int> { 291, 570, 571, 572 }; //Rune puzzle RectangleShapes

                            scriptNodesToAdd.AddRange(CreateRandomizedRunePuzzle(levelFile, doodadsNodeRoot, ref buttonEffectNodeIds, new Vector2(-79f, -9.25f), ref modNodeStartId, 152,
                                new[] { 187, 188, 519, 520 }, 55, 328, 1000));
                            break;
                        case "level_bonus_4.xml": //Bonus 4
                            nodesToNuke = new List<int> { 746 }; //Strange plank trigger

                            XElement[] n4TpDoodads = CreateTeleporter(ref modNodeStartId, new Vector2(-7.5f, 14.5f), -1, "11", 88, -1, false, out List<XElement> n4TpScriptNodes);
                            doodadsToAdd.AddRange(n4TpDoodads);
                            scriptNodesToAdd.AddRange(n4TpScriptNodes);
                            break;
                        case "level_11.xml": //Chambers 11
                            scriptNodesToAdd.Add(NodeHelper.CreateLevelStartNode(modNodeStartId++, new Vector2(-1, 1.5f), 1)); //Offset +0,+2 from RectangleShape
                            scriptNodesToAdd.Add(NodeHelper.CreateLevelStartNode(modNodeStartId++, new Vector2(-36, -73f), 88)); //Bonus return

                            int l11DestroyId = modNodeStartId++;
                            scriptNodesToAdd.AddRange(NodeHelper.CreateAreaTriggerNodes(ref modNodeStartId, new Vector2(-36, -73f), new List<int> { l11DestroyId, 2799, 2794 })); //Bonus portal trigger, torch nodes
                            scriptNodesToAdd.Add(NodeHelper.CreateDestroyObjectNode(l11DestroyId, new Vector2(-36, -78f), 1, new int[] { 2697, 2696, 2698, 2699 })); //Destroy doodads

                            int c2LeftBossRuneItemId = modNodeStartId;
                            scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, new Vector2(-51, -39), true));
                            int c2RightBossRuneItemId = modNodeStartId;
                            scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, new Vector2(74, -35), true));

                            //Add destroy nodes to the NW spike tp items if any are offworld items
                            List<int> c2SpikeTpDestroyNodeIds = new List<int>(4);
                            List<Tuple<int, int>> apIdToHWItemIds = new List<Tuple<int, int>>()
                            {
                                new Tuple<int, int>(1002, 1737),
                                new Tuple<int, int>(923, 2843),
                                new Tuple<int, int>(1018, 1864),
                                new Tuple<int, int>(1003, 3435),
                            };
                            foreach (Tuple<int, int> apIdtoHWItem in apIdToHWItemIds)
                            {
                                if (ArchipelagoManager.IsItemOurs(archipelagoData.GetItemFromLoc(apIdtoHWItem.Item1), false)) continue;
                                c2SpikeTpDestroyNodeIds.Add(apIdtoHWItem.Item2);
                            }
                            if (c2SpikeTpDestroyNodeIds.Count > 0)
                            {
                                int c2DestroyNodeId = modNodeStartId++;
                                scriptNodesToAdd.Add(NodeHelper.CreateDestroyObjectNode(c2DestroyNodeId, new Vector2(-83, -51), -1, c2SpikeTpDestroyNodeIds.ToArray()));
                                NodeHelper.AddConnectionNodes(idToNode["2854"], new int[] { c2DestroyNodeId });
                            }

                            List<string> torchIds = new List<string> { "2799", "2794" };
                            foreach (string torchId in torchIds) //Make bonus torch nodes only trigger once
                            {
                                idToNode[torchId].Elements("int").ToArray()[1].Value = "1";
                            }
                            if (buttonsanity > 0)
                            {
                                //Boss buttons
                                NodeHelper.SetConnectionNodes(idToNode["3931"], c2LeftBossRuneItemId);
                                NodeHelper.SetConnectionNodes(idToNode["6357"], c2RightBossRuneItemId);

                                //Add the other boss rune CheckGlobalFlag nodes to the button EventTrigger
                                buttonEffectNodeIds.Add(8115);

                                //Bonus puzzle buttons
                                string[] bonusPuzzleIds = new string[] { "4", "1", "13198", "54", "53", "52", "58", "55" };
                                string bonusRuneItemIds = "";
                                foreach (string bonusPuzzleId in bonusPuzzleIds)
                                {
                                    string rectShapeNodeId = idToNode[bonusPuzzleId].Element("dictionary").Element("dictionary").Element("int-arr").Value;
                                    Vector2 itemPos = NodeHelper.PosFromString(idToNode[rectShapeNodeId].Element("vec2").Value);
                                    bonusRuneItemIds += $"{modNodeStartId} ";
                                    scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, itemPos, true));
                                }

                                int c2BonusPuzzleTriggerId = modNodeStartId++;
                                scriptNodesToAdd.Add(NodeHelper.CreateGlobalEventTriggerNode(c2BonusPuzzleTriggerId, -1, new Vector2(-93.5f, -179), "c2_rune_bonus", null));

                                string portalOpenItemName = ArchipelagoManager.GetItemName(157 + APData.castleButtonItemStartID);
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(-32, -84), portalOpenItemName, ref buttonEffectNodeIds, new[] { 2791 }, new[] { 0 }));

                                XElement[] bonusWinConnectionNodes = idToNode["3"].Elements("int-arr").ToArray(); //Bonus win puzzle node
                                bonusWinConnectionNodes[0].Value = $"{bonusRuneItemIds}{c2BonusPuzzleTriggerId}";
                                bonusWinConnectionNodes[1].Value = string.Join(" ", new int[9]);

                                //Bonus room sequence
                                int c2BonusSeqTriggerId = modNodeStartId++;
                                scriptNodesToAdd.Add(NodeHelper.CreateGlobalEventTriggerNode(c2BonusSeqTriggerId, -1, new Vector2(-38, -95), "c2_seq_bonus", null));

                                NodeHelper.SetConnectionNodes(idToNode["510"], c2BonusSeqTriggerId);

                                string bonusRoomOpenItemName = ArchipelagoManager.GetItemName(161 + APData.castleButtonItemStartID);
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(-48, -69), bonusRoomOpenItemName, ref buttonEffectNodeIds, new[] { 3288 }, new[] { 1000 }));

                                //Boss door button runes
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(79, -33), "a4l10_boss4", ref buttonEffectNodeIds, new[] { 6371 }));
                                //Boss door button runes
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(-47.5f, -37), "a4l11_boss4", ref buttonEffectNodeIds, new[] { 3933 }));
                            }

                            nodesToNuke = new List<int> { 2851, 2850, 2847, 2848 }; //Rune puzzle RectangleShapes

                            scriptNodesToAdd.AddRange(CreateRandomizedRunePuzzle(levelFile, doodadsNodeRoot, ref buttonEffectNodeIds, new Vector2(-88, -59), ref modNodeStartId, 159,
                                new[] { 2839, 2840, 2841, 2842 }, 1704, 2935, 1000));

                            doodadsToAdd.AddRange(CreateTeleporterWithSign(ref modNodeStartId, new Vector2(-8f, -10.5f), -1, "ap_hub", 0, -1, false, true, sign_hub_text, out List<XElement> b4TpNodes));
                            scriptNodesToAdd.AddRange(b4TpNodes);

                            //Move walls a bit
                            foreach (XElement doodad in doodadsNodeRoot.Elements())
                            {
                                if (doodad.Element("int").Value == "3138")
                                {
                                    doodad.Element("vec2").Value = "-47 -67.25";
                                    break;
                                }
                                if (doodad.Element("int").Value == "3140")
                                {
                                    doodad.Element("vec2").Value = "-47 -64.625";
                                    break;
                                }
                            }
                            break;
                        case "level_12.xml": //Chambers 12
                            nodesToNuke = new List<int> { 921 }; //Strange plank trigger

                            if (buttonsanity > 0)
                            {
                                //Boss rune item and rewire
                                NodeHelper.SetConnectionNodes(idToNode["2541"], modNodeStartId);
                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, new Vector2(49, -53), true));
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(-54, -50), "a4l12_boss4", ref buttonEffectNodeIds, new[] { 2537 }));

                                //SW hall sequence
                                int c3SeqTriggerId = modNodeStartId++;
                                scriptNodesToAdd.Add(NodeHelper.CreateGlobalEventTriggerNode(c3SeqTriggerId, -1, new Vector2(-30, -52), "c3_rune", null));

                                NodeHelper.SetConnectionNodes(idToNode["159"], c3SeqTriggerId);

                                string swHallOpenItemName = ArchipelagoManager.GetItemName(171 + APData.castleButtonItemStartID);
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(-68.5f, -77), swHallOpenItemName, ref buttonEffectNodeIds, new[] { 7410 }, new[] { 1000 }));

                                //Blue wall button dialogue change
                                NetworkItem buttonItem = archipelagoData.GetGenItemFromPos(new Vector2(71, -74.5f), levelFile);
                                if ((buttonItem.Flags & ItemFlags.Trap) != ItemFlags.Trap)
                                {
                                    NodeHelper.EditShowSpeechBubbleNode(idToNode["104982"], "Guess you got lucky this time!");
                                }
                            }
                            break;
                        case "level_boss_4.xml": //Boss 4
                            int b4PortalId = modNodeStartId;
                            Vector2 b4PortalPos = new Vector2(-7.5f, 20.5f);
                            string b4PortalSignMessage = archipelagoData.GetOption(SlotDataKeys.exitRandomization) > 0 ? "Portal Back to Wherever You Came From" : "Portal Back to Floor 11";
                            List<XElement> b4TpDoodads = CreateToggleableTeleporterWithSign(ref modNodeStartId, b4PortalPos, -1, "11", 1, -1, true, -1, true, b4PortalSignMessage, out List<XElement> b4TpScriptNodes, out int portalOnNodeId, out int portalOffNodeId);
                            doodadsToAdd.AddRange(b4TpDoodads);
                            scriptNodesToAdd.AddRange(b4TpScriptNodes);

                            int toggleNodeId = modNodeStartId++;
                            scriptNodesToAdd.Add(NodeHelper.CreateToggleElementNode(toggleNodeId, b4PortalPos + new Vector2(0, -5), -1, 2, new int[] { b4PortalId + 3 }));

                            //If one of the doomspawns got shuffled to a different spawner we need to remove the razed doomspawn spawner node elsewise we won't be able to access the new spawner
                            if (archipelagoData.GetEnemyShuffleMode() > 0)
                            {
                                Tuple<string, string>[] doomspawnNodePairs = new Tuple<string, string>[]
                                {
                                    new Tuple<string, string>("205", "212"), //-11 -22
                                    new Tuple<string, string>("653", "659"), //-8 -18
                                    new Tuple<string, string>("651", "660"), //-2.5 -18
                                    new Tuple<string, string>("652", "661"), //1 -22
                                };
                                foreach (var pair in doomspawnNodePairs)
                                {
                                    string doomspawnSpawnerName = idToNode[pair.Item1].Elements("string").ToArray()[1].Value;
                                    if (doomspawnSpawnerName != "actors/spawners/doomspawn_1.xml")
                                    {
                                        idToNode[pair.Item2].Remove();
                                    }
                                }
                            }

                            //Boss killed trigger node, add connections to re-enable portal when boss is defeated
                            if (archipelagoData.goalType != ArchipelagoData.GoalType.FullCompletion)
                            {
                                NodeHelper.AddConnectionNodes(idToNode["22"], new[] { toggleNodeId, portalOnNodeId });
                            }
                            //Boss activate trigger node, add connections to disable portal
                            NodeHelper.AddConnectionNodes(idToNode["200"], new[] { toggleNodeId, portalOffNodeId });

                            int b4SetGlobalFlagNodeId = modNodeStartId + 25;
                            scriptNodesToAdd.Add(NodeHelper.CreateSetGlobalFlagNode(b4SetGlobalFlagNodeId, new Vector2(-11, -35), -1, "killed_boss_4", true));
                            //Unhook music node from the GlobalEventTrigger and replace with our CheckGlobalFlag node
                            NodeHelper.AddConnectionNodes(idToNode["23"], new[] { b4SetGlobalFlagNodeId });

                            string[] plankNodeIds = new string[] { "784", "786", "802", "801", "800", "799", "798", "797", "839", "1244", "1243", "1242" };

                            int plankNodeIdCounter = modNodeStartId + 30;

                            //Music node, make it so reentering will replay boss music
                            idToNode["1215"].Elements("int").ToArray()[1].Value = "-1";

                            if(archipelagoData.goalType != ArchipelagoData.GoalType.FullCompletion)
                            {
                                //If you have at least 1 plank and not playing the escape goal prevent the escape sequence from opening
                                idToNode["1136"].Element("bool").Value = "False"; //Deactivate the node
                                idToNode["37"].Element("bool").Value = "False"; //Deactivate the CameraShake node
                                //Countdown announce node
                                XElement[] parameters = idToNode["2"].Element("dictionary").Elements().ToArray();
                                parameters[0].Value = "Or is it?";
                                parameters[1].Value = "6000"; //Duration
                                NodeHelper.SetConnectionNodes(idToNode["2"], new int[] { 6 }, new int[] { 2000 }); //Remove sound node connection

                                XElement[] parameters2 = idToNode["6"].Element("dictionary").Elements().ToArray();
                                parameters2[0].Value = "The castle is NOT falling apart!";
                                parameters2[1].Value = "6000"; //Duration
                                parameters2[2].Value = "0"; //Title
                                NodeHelper.SetConnectionNodes(idToNode["6"], new int[] { 7 }, new int[] { 2000 }); //Remove sound node connection

                                XElement[] parameters3 = idToNode["7"].Element("dictionary").Elements().ToArray();
                                parameters3[0].Value = "Go do whatever you want I guess";
                                parameters3[1].Value = "4000"; //Duration
                                XElement[] connections = idToNode["7"].Elements("int-arr").ToArray();
                                connections[0].Remove();
                                connections[1].Remove();
                            }
                            else
                            {
                                //Else ensure the passage will open even if we don't have any planks
                                globalScriptNodesToTriggerOnceOnLoad.Add(1136);
                            }
                            //Plank nodes
                            for(int p = 0; p < plankNodeIds.Length; p++)
                            {
                                string plankNodeId = plankNodeIds[p];
                                XElement node = idToNode[plankNodeId];
                                XElement[] connectionNodes = node.Elements("int-arr").ToArray();
                                node.Element("bool").Value = "True"; //Make sure the node is enabled!
                                XElement triggerTimesNode = node.Elements("int").ToArray()[1];
                                string posString = node.Element("vec2").Value;

                                string connectionNodeValues = connectionNodes[0].Value;
                                int globalFlagNodeId = plankNodeIdCounter++;
                                int scriptNodeId = plankNodeIdCounter++;
                                connectionNodes[0].Value = globalFlagNodeId.ToString();
                                connectionNodes[1].Value = "0";
                                triggerTimesNode.Value = "-1";
                                Vector2 globalFlagNodePos = NodeHelper.PosFromString(posString) + new Vector2(1, 3);
                                string[] connectionNodeValuesSplits = connectionNodeValues.Split(' ');
                                int[] connectionNodeIds = new int[connectionNodeValuesSplits.Length];
                                for (int c = 0; c < connectionNodeIds.Length; c++)
                                {
                                    connectionNodeIds[c] = int.Parse(connectionNodeValuesSplits[c]);
                                }
                                XElement plankFlagNode = NodeHelper.CreateCheckGlobalFlagNode(globalFlagNodeId, globalFlagNodePos, $"l{p + 1}_plank", false, new int[] { scriptNodeId }, false, null);
                                XElement scriptLinkNode = NodeHelper.CreateScriptNodeBase(scriptNodeId, "ScriptLink", true, 1, globalFlagNodePos + new Vector2(2, 0));
                                NodeHelper.AddConnectionNodes(scriptLinkNode, connectionNodeIds);
                                scriptNodesToAdd.Add(plankFlagNode);
                                scriptNodesToAdd.Add(scriptLinkNode);
                            }

                            scriptNodesToAdd.Add(NodeHelper.CreateLevelStartNode(plankNodeIdCounter, new Vector2(30, -26.5f), 1)); //Offset +0,+2 from RectangleShape
                            break;
                        case "level_esc_4.xml": //Last escape room
                            //Prevent the level from collapsing if the player doesn't have the hammer
                            int e4CheckHammerFlagNodeId = modNodeStartId++;
                            int e4HammerTauntNodeBaseId = modNodeStartId++;
                            int e4StopCameraShakeNodeId = modNodeStartId++;
                            Vector2 e4TauntBasePos = new Vector2(-68.75f, -48.25f);
                            scriptNodesToAdd.Add(NodeHelper.CreateCheckGlobalFlagNode(e4CheckHammerFlagNodeId, e4TauntBasePos, "has_hammer", new int[] { 273 }, new int[] { e4HammerTauntNodeBaseId, 75, e4StopCameraShakeNodeId }));
                            //75 is the stop sound id for the rumble
                            NodeHelper.SetConnectionNodes(idToNode["274"], new int[] { e4CheckHammerFlagNodeId, 86 });
                            scriptNodesToAdd.Add(NodeHelper.CreateCameraShakeNode(e4StopCameraShakeNodeId, true, -1, e4TauntBasePos + new Vector2(6, 2), 1, 0.25f, 0.25f, 25));
                            scriptNodesToAdd.Add(NodeHelper.CreateAnnounceTextNode(e4HammerTauntNodeBaseId, e4TauntBasePos + new Vector2(2, 2), "Hey wait a minute...", 5000, 1, true, -1));
                            NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { modNodeStartId }, new int[] { 5000 });
                            scriptNodesToAdd.Add(NodeHelper.CreateAnnounceTextNode(modNodeStartId++, e4TauntBasePos + new Vector2(0, 4), "Where's your hammer??", 11000, 0, true, -1));
                            NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { modNodeStartId }, new int[] { 3000 });
                            scriptNodesToAdd.Add(NodeHelper.CreateAnnounceTextNode(modNodeStartId++, e4TauntBasePos + new Vector2(2, 6), "You can't escape without it you know!", 8000, 1, true, -1));
                            NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { modNodeStartId }, new int[] { 3000 });
                            scriptNodesToAdd.Add(NodeHelper.CreateAnnounceTextNode(modNodeStartId++, e4TauntBasePos + new Vector2(4, 8), "Did you forget or something?", 2000, 2, true, -1));
                            NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { modNodeStartId }, new int[] { 5000 });
                            scriptNodesToAdd.Add(NodeHelper.CreateAnnounceTextNode(modNodeStartId++, e4TauntBasePos + new Vector2(0, 10), "What are you going to do now, huh?", 8000, 0, true, -1));
                            NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { modNodeStartId }, new int[] { 3000 });
                            scriptNodesToAdd.Add(NodeHelper.CreateAnnounceTextNode(modNodeStartId++, e4TauntBasePos + new Vector2(2, 12), "You're kinda stuck", 5000, 1, true, -1));
                            NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { modNodeStartId }, new int[] { 8000 });
                            scriptNodesToAdd.Add(NodeHelper.CreateAnnounceTextNode(modNodeStartId++, e4TauntBasePos + new Vector2(0, 14), "Well?", 2000, 0, true, -1));
                            NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { modNodeStartId }, new int[] { 3000 });
                            scriptNodesToAdd.Add(NodeHelper.CreateAnnounceTextNode(modNodeStartId++, e4TauntBasePos + new Vector2(0, 16), "You're going to have to quit out", 120000, 0, true, -1));
                            NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { modNodeStartId }, new int[] { 3000 });
                            scriptNodesToAdd.Add(NodeHelper.CreateAnnounceTextNode(modNodeStartId++, e4TauntBasePos + new Vector2(2, 18), "Fight the dragon WITH the hammer this time!!", 117000, 1, true, -1));
                            NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { modNodeStartId }, new int[] { 5000 });
                            scriptNodesToAdd.Add(NodeHelper.CreateAnnounceTextNode(modNodeStartId++, e4TauntBasePos + new Vector2(4, 20), "Go on, shoo", 3000, 2, true, -1));
                            //Disable the game from ending (avoids potential softlocks, it doesn't add anything to the randomizer anyway)
                            int e4EndTauntNodeBaseId = modNodeStartId++;
                            Vector2 e4EndTauntBasePos = new Vector2(-59.5f, -49f);
                            NodeHelper.SetConnectionNodes(idToNode["269"], new int[] { e4EndTauntNodeBaseId }, new int[] { 1000 });
                            scriptNodesToAdd.Add(NodeHelper.CreateAnnounceTextNode(e4EndTauntNodeBaseId, e4EndTauntBasePos + new Vector2(0, 2), "Cmon we don't have all day!", 5000, 1, true, -1));
                            NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { modNodeStartId }, new int[] { 5000 });
                            scriptNodesToAdd.Add(NodeHelper.CreateAnnounceTextNode(modNodeStartId++, e4EndTauntBasePos + new Vector2(0, 4), "The castle is falling apart!", 5000, 1, true, -1));
                            NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { modNodeStartId }, new int[] { 7000 });
                            scriptNodesToAdd.Add(NodeHelper.CreateAnnounceTextNode(modNodeStartId++, e4EndTauntBasePos + new Vector2(0, 6), "...", 3000, 0, true, -1));
                            NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { modNodeStartId }, new int[] { 3000 });
                            scriptNodesToAdd.Add(NodeHelper.CreateAnnounceTextNode(modNodeStartId++, e4EndTauntBasePos + new Vector2(0, 8), "Fine, lollygag all you want", 10000, 0, true, -1));
                            NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { modNodeStartId }, new int[] { 5000 });
                            scriptNodesToAdd.Add(NodeHelper.CreateAnnounceTextNode(modNodeStartId++, e4EndTauntBasePos + new Vector2(0, 10), "The exit is right there just go!!", 5000, 1, true, -1));
                            NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { modNodeStartId }, new int[] { 5000 });
                            scriptNodesToAdd.Add(NodeHelper.CreateAnnounceTextNode(modNodeStartId++, e4EndTauntBasePos + new Vector2(0, 12), "Did you forget how to escape?", 120000, 0, true, -1));
                            NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { modNodeStartId }, new int[] { 3000 });
                            scriptNodesToAdd.Add(NodeHelper.CreateAnnounceTextNode(modNodeStartId++, e4EndTauntBasePos + new Vector2(0, 14), "Break the wall at the end!", 117000, 1, true, -1));
                            NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { modNodeStartId }, new int[] { 5000 });
                            scriptNodesToAdd.Add(NodeHelper.CreateAnnounceTextNode(modNodeStartId++, e4EndTauntBasePos + new Vector2(0, 16), "Go on, shoo", 3000, 2, true, -1));
                            break;
                    }
                    break;
                case ArchipelagoData.MapType.Temple:
                    panLocation = archipelagoData.GetRandomLocation("Frying Pan");
                    switch (levelFile)
                    {
                        case "level_hub.xml":
                            effectNodePositions.Add("156310", new Vector2(-34.25f, 0.25f));

                            //Replace hub quest npc with one that says the goal
                            int npcDoodadId = modNodeStartId++;
                            Vector2 npcPos = new Vector2(-8.25f, -2.375f);
                            doodadsToAdd.Add(NodeHelper.CreateDoodadNode(npcDoodadId, "doodads/special/npc_wizard_old_v2.xml", npcPos, false));
                            scriptNodesToAdd.Add(NodeHelper.CreateObjectEventTriggerNode(modNodeStartId++, npcPos + new Vector2(-2f, -2), -1, "Hit", new int[] { npcDoodadId }, false, new int[] { modNodeStartId, }));
                            scriptNodesToAdd.Add(NodeHelper.CreateCheckGlobalFlagNode(modNodeStartId++, npcPos + new Vector2(0, -2), "goal", false, new[] { modNodeStartId + 1, modNodeStartId + 2, modNodeStartId + 4 }, false, new[] { modNodeStartId }));
                            scriptNodesToAdd.Add(NodeHelper.CreateSpeechBubbleNode(modNodeStartId++, npcPos + new Vector2(2f, 2), -1, "menus/speech/normal_speech.xml", GetGoalText(archipelagoData),
                                new int[] { npcDoodadId }, new Vector2(0, -1.15f), 90, 100, 0));
                            scriptNodesToAdd.Add(NodeHelper.CreateSpeechBubbleNode(modNodeStartId++, npcPos + new Vector2(2f, 0), -1, "menus/speech/normal_speech.xml",
                                "Congratulations on completing your goal! Talk to me again to end the game.", new int[] { npcDoodadId }, new Vector2(0, -1.15f), 90, 100, 0));
                            scriptNodesToAdd.Add(NodeHelper.CreateScriptLinkNode(modNodeStartId++, true, -1, npcPos + new Vector2(2, -2), new[] { modNodeStartId }, new[] { 5000 }));
                            scriptNodesToAdd.Add(NodeHelper.CreateToggleElementNode(modNodeStartId++, npcPos + new Vector2(4, -2), -1, 0, new[] { modNodeStartId }));
                            XElement endGameNode = NodeHelper.CreateGlobalEventTriggerNode(modNodeStartId++, -1, npcPos + new Vector2(2, -4), "ap_check_end_game");
                            endGameNode.Element("bool").Value = "False";

                            scriptNodesToAdd.Add(endGameNode);

                            //Bump PoF reward spawn node
                            idToNode["155661"].Element("vec2").Value = "24 -1.5";

                            if (buttonsanity > 0)
                            {
                                //PoF pyramid raise node
                                idToNode["154103"].Remove();

                                //PoF pyramid flag script link node
                                NodeHelper.AddConnectionNodes(idToNode["153543"], new[] { modNodeStartId });

                                scriptNodesToAdd.Add(NodeHelper.CreateCheckGlobalFlagNode(modNodeStartId++, new Vector2(7.5f, 7), "puzzle_bonus_solved_hub", false, new int[] { 153503 }, false, null));
                                scriptNodesToAdd.Add(NodeHelper.CreateGlobalEventTriggerNode(modNodeStartId++, -1, new Vector2(-2, 3), "pyramid_update", new int[] { 153543 }));
                            }

                            // Pan item nodes
                            //int panItems = archipelagoData.GetOption("powerup_vendor_locations");
                            //int[] panSpawnObjectNodeIds = new int[panItems];
                            //int[] panItemCounterNodeIds = new int[panItems];
                            //int[] panCheckGlobalFlagNodeIds = new int[panItems];
                            //for (int p = 0; p < panItems; p++)
                            //{
                            //    panSpawnObjectNodeIds[p] = 9000 + p;
                            //    XElement panItemSpawnNode = NodeHelper.CreateSpawnObjectNode(panSpawnObjectNodeIds[p], "", new Vector2(-27.25f, -10.75f));
                            //    panItemCounterNodeIds[p] = 9100 + p;
                            //    XElement counterNode = NodeHelper.CreateScriptNodeBase(panItemCounterNodeIds[p], "Counter", true, 1, new Vector2(-28, -20 - p));
                            //    XElement counterParamsNode = CreateDictionaryNode(new XElement[]
                            //    {
                            //        CreateXNode("count", p + 1),
                            //        CreateDictionaryNode("execute", new XElement[]
                            //        {
                            //            CreateXNode("static", new int[]{ 9200 + p, 9201 + p })
                            //        })
                            //    });
                            //    counterNode.Add(counterParamsNode);
                            //    panCheckGlobalFlagNodeIds[p] = 9300 + p;
                            //    XElement globalFlagNode = NodeHelper.CreateCheckGlobalFlagNode(panCheckGlobalFlagNodeIds[p], new Vector2(-26, -20 - p), $"pan_item_task_{p + 1}", 
                            //        new [] { panSpawnObjectNodeIds[p] }, new int[0], p == 0, 1);
                            //    XElement toggleNode = NodeHelper.CreateToggleElementNode(9200 + p, new Vector2(-27, -20 - p), -1, 2, new int[] { panCheckGlobalFlagNodeIds[p] });

                            //    scriptNodeRoot.Add(panItemSpawnNode);
                            //    scriptNodeRoot.Add(counterNode);
                            //    scriptNodeRoot.Add(toggleNode);
                            //    scriptNodeRoot.Add(globalFlagNode);
                            //}

                            //XElement panEventTriggerNode = NodeHelper.CreateGlobalEventTriggerNode(8000, new Vector2(-21, -27), "pan_item_task", panCheckGlobalFlagNodeIds);
                            //int[] eventTriggerDynamicIds = new int[panItems + 1];
                            //panSpawnObjectNodeIds.CopyTo(eventTriggerDynamicIds, 0);
                            //XElement panObjectEventTriggerNode = NodeHelper.CreateScriptNodeBase(8001, "ObjectEventTrigger", true, -1, new Vector2(-30, -20));
                            //XElement objEventTriggerParamsNode = CreateDictionaryNode(new XElement[]
                            //{
                            //    CreateXNode("event", "Destroyed"),
                            //    CreateDictionaryNode("object", new []
                            //    {
                            //        CreateXNode("dynamic", eventTriggerDynamicIds)
                            //    })
                            //});
                            //panObjectEventTriggerNode.Add(objEventTriggerParamsNode);
                            //AddConnectionNodes(panObjectEventTriggerNode, panItemCounterNodeIds);

                            //scriptNodeRoot.Add(panEventTriggerNode);
                            //scriptNodeRoot.Add(panObjectEventTriggerNode);

                            //nodesToGuaranteeSpawn.AddRange(panCheckGlobalFlagNodeIds);

                            int[] t3EntranceNodes = { 137205, 137206, 137211 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(t3EntranceNodes[archipelagoData.GetRandomLocation("Temple 3 Entrance")]);

                            nodesToNuke = new List<int> { 155679, 137209 }; //PoF close event trigger, T3 entrance random node

                            //Randomize PoF switch
                            Vector2 pofSwitchPos = new Vector2(-40, -6.5f);
                            int counter = 700000;
                            scriptNodesToAdd.AddRange(CreateRandomizedPoFPuzzle(levelFile, pofSwitchPos, ref counter, out XElement[] pofDoodads));
                            doodadsToAdd.AddRange(pofDoodads);

                            //Hub portal
                            List<XElement> hubTpDoodads = CreateTeleporterWithSign(ref modNodeStartId, new Vector2(0.5f, -7f), 13, "ap_hub", 0, -1, false, false, sign_hub_text, out List<XElement> hubTpScriptNodes);
                            doodadsToAdd.AddRange(hubTpDoodads);
                            scriptNodesToAdd.AddRange(hubTpScriptNodes);
                            break;
                        case "level_library.xml":
                            //Change books to show hints?
                            scriptNodesToAdd.Add(NodeHelper.CreateLevelStartNode(modNodeStartId, new Vector2(34f, -13f), 5)); //Cave return
                            break;
                        case "level_cave_1.xml":
                            if (archipelagoData.GetRandomLocation("Cave 3 Squire") == 1)
                            {
                                globalScriptNodesToTriggerOnceOnLoad.Add(112648);
                            }

                            int[] c3PanNodes = { 135491, 135495, 135494 };

                            if (panLocation >= 0 && panLocation < 3)
                            {
                                globalScriptNodesToTriggerOnceOnLoad.Add(c3PanNodes[panLocation]);
                            }

                            //Change npc dialogue of the guard at the entrance
                            NetworkItem c1Guarditem = archipelagoData.GetItemFromPos(new Vector2(-74.375f, 27.375f), "level_cave_1.xml");
                            string itemName = ArchipelagoManager.GetItemName(c1Guarditem);
                            if(itemName != null)
                            {
                                string message = ArchipelagoMessageManager.GetLanguageString("d.cave1.npc.guardtrapped1a", new string[0]);
                                string[] sentences = message.Split('.');
                                message = message.Remove(message.Length - sentences[sentences.Length - 1].Length + 1);
                                string customMessage = "";
                                if (ArchipelagoManager.connectionInfo.IsPlayerPlayingSameGame(c1Guarditem.Player))
                                {
                                    if (itemName.Contains("Chest") || itemName.Contains("Trap"))
                                        customMessage = "Maybe you can use something from this chest?";
                                    else if (itemName.Contains("Ankh"))
                                        customMessage = "Maybe you could use this weird ankh thing?";
                                    else if (itemName == "Rune Key")
                                        customMessage = "Some dude ran past here and dropped this thing. It looks important so you should take it!";
                                    else if (itemName.Contains("Diamond"))
                                        customMessage = "This diamond will probably be useful!";
                                    else if (itemName.Contains("Upgrade") || itemName == "Serious Health" || APData.IsItemButton(c1Guarditem.Item))
                                        customMessage = "Maybe you could use this odd glowy thing?";
                                    else if (itemName.Contains("Potion"))
                                        customMessage = "Maybe you could use this strange potion?";
                                    else if (itemName.Contains("Pumps Lever") || itemName.Contains("Pickaxe") || itemName.Contains("Frying Pan"))
                                        customMessage = $"Take this {itemName.ToLowerInvariant()}, it looks important!";
                                    else
                                        customMessage = $"Maybe you could use this {itemName.ToLowerInvariant()}?";
                                }
                                else
                                {
                                    //Someone else's item
                                    if (c1Guarditem.Flags.HasFlag(ItemFlags.NeverExclude) || c1Guarditem.Flags.HasFlag(ItemFlags.Advancement))
                                    {
                                        customMessage = "Maybe you could use whatever this thing is?";
                                    }
                                    else
                                    {
                                        customMessage = "Maybe you could use whatever this weird thing is...?";
                                    }
                                    if (c1Guarditem.Flags.HasFlag(ItemFlags.Trap))
                                    {
                                        customMessage += " It looks kinda suspicious though.";
                                    }
                                }
                                message += customMessage;
                                NodeHelper.EditShowSpeechBubbleNode(idToNode["137595"], message);
                            }

                            //Hiding guard
                            string hidingGuardItemName = archipelagoData.GetItemNameFromPos("level_cave_1.xml", new Vector2(23, 9.25f));
                            if (hidingGuardItemName != "Rune Key")
                            {
                                string message = "Ah... Someone took the rune key needed to activate the teleporter.\n\n";
                                message += "I managed to kill him when he came back, but I think he hid it somewhere";
                                if (archipelagoData.GetOption(SlotDataKeys.portalAccessibility) > 0)
                                {
                                    message += " around this level.";
                                }
                                else
                                {
                                    message += ".";
                                }
                                message += " I wasn't sure if it was safe to try to find it, so I hid here instead.";
                                NodeHelper.EditShowSpeechBubbleNode(idToNode["137737"], message);
                            }
                            if (buttonsanity > 0)
                            {
                                //CheckGlobalFlag node for the pumps
                                idToNode["111152"].Element("dictionary").Element("string").Value = pumpsItemName;

                                int exitHideNodeId = modNodeStartId++;
                                int seBankHideNodeId = modNodeStartId++;

                                idToNode["111152"].Element("dictionary").Elements("dictionary").ToArray()[1].Add(NodeHelper.CreateXNode("static", new[] { exitHideNodeId, seBankHideNodeId }));

                                XElement c1PumpsEventTriggerNode = NodeHelper.CreateGlobalEventTriggerNode(modNodeStartId++, 1, new Vector2(-50, 13), pumpsItemName, new[] { 111155 });
                                scriptNodesToAdd.Add(c1PumpsEventTriggerNode);

                                nodesToNuke.AddRange(new[] { 154103 }); //puzzle_bonus_solved GlobalEventTrigger

                                //Switch the hide object node ids
                                idToNode["111849"].Element("int").Value = exitHideNodeId.ToString();
                                idToNode["111543"].Element("int").Value = seBankHideNodeId.ToString();
                            }

                            nodesToNuke = new List<int> { 112649, 135490, 135496 }; //Squire powerup rando, pan placer rando, pan trigger node

                            //Randomize PoF switch
                            Vector2 c1PofSwitchPos = new Vector2(-99.5f, -10.5f);
                            int c1Counter = 700000;
                            scriptNodesToAdd.AddRange(CreateRandomizedPoFPuzzle(levelFile, c1PofSwitchPos, ref c1Counter, out XElement[] c1PofDoodads));
                            doodadsToAdd.AddRange(c1PofDoodads);
                            break;
                        case "level_cave_2.xml":
                            int[] c2PortalNodes = { 113663, 113662, 113721 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(c2PortalNodes[archipelagoData.GetRandomLocation("Cave 2 Portal")]);
                            int[] c2KeystoneNodes = { 111541, 111540, 111543, 111544, 111539 };
                            int[] c2PanNodes = { 135491, 135494, 135495 };

                            globalScriptNodesToTriggerOnceOnLoad.Add(c2KeystoneNodes[archipelagoData.GetRandomLocation("Cave 2 Keystone")]);
                            if (panLocation >= 3 && panLocation < 6)
                            {
                                globalScriptNodesToTriggerOnceOnLoad.Add(c2PanNodes[panLocation - 3]);
                            }

                            nodesToNuke = new List<int> { 111550, 135490, 113664, 135496 }; //Keystone, Pan, Portal, Pan trigger

                            //Add delay to executing the portal event trigger node
                            idToNode["109352"].Elements("int-arr").ToArray()[1].Value = "0 0 0 10 0 0 0";

                            //Fix the double bridge button from being up half a unit
                            idToNode["110670"].Element("vec2").Value = "9.5 14";

                            //Change pumps event_trigger to be the buttonsanity item
                            if (buttonsanity > 0)
                            {
                                int pumpsTriggerNodeId = 111290;
                                //Pumps EventTrigger, change flag to be our item and rewire to not toggle the lever doodad
                                idToNode["154145"].Elements("string").ToArray()[1].Value = pumpsItemName; //Activate Water Pumps
                                NodeHelper.SetConnectionNodes(idToNode["154145"], pumpsTriggerNodeId);

                                //ChangeDoodadState node for the lever
                                NodeHelper.SetConnectionNodes(idToNode["110213"], new[] { 110211, 153202 });

                                int pumpsItemFlagId = modNodeStartId++;
                                buttonEffectNodeIds.Add(pumpsItemFlagId);
                                XElement checkPumpsItemNode = NodeHelper.CreateCheckGlobalFlagNode(pumpsItemFlagId, new Vector2(9, -41.5f), pumpsItemName, false, new[] { pumpsTriggerNodeId }, false, null);
                                scriptNodesToAdd.Add(checkPumpsItemNode);
                                globalScriptNodesToTriggerOnLoad.Add(pumpsItemFlagId);

                                //Make the activate pumps effect scriptLink node only run once
                                idToNode[pumpsTriggerNodeId.ToString()].Elements("int").ToArray()[1].Value = "1";

                                //Add in AreaTrigger for the lever if you have the pumps lever
                                int leverRectangleShapeId = modNodeStartId++;
                                int checkNodeId = modNodeStartId++;
                                int soundNodeId = modNodeStartId++;
                                Vector2 leverPos = new Vector2(-2, -42);
                                scriptNodesToAdd.Add(NodeHelper.CreateRectangleShapeNode(leverRectangleShapeId, leverPos, 1, 1, 1));
                                scriptNodesToAdd.Add(NodeHelper.CreateAreaTriggerNode(modNodeStartId++, -1, leverPos + new Vector2(0, -2.5f), 0, 1, new[] { leverRectangleShapeId }, new[] { checkNodeId }));
                                scriptNodesToAdd.Add(NodeHelper.CreateCheckGlobalFlagNode(checkNodeId, leverPos + new Vector2(2, -2.5f), "quest_pumps_solved", false, new[] { modNodeStartId }, false, new[] { soundNodeId }));
                                scriptNodesToAdd.Add(NodeHelper.CreatePlaySoundNode(soundNodeId, leverPos, "sound/misc.xml:info_nokey", false, true, 5));
                                scriptNodesToAdd.Add(NodeHelper.CreateScriptLinkNode(modNodeStartId++, true, 1, new Vector2(0, -42), new[] { 110213, modNodeStartId }));

                                Vector2 leverItemPos = leverPos + new Vector2(0, 1);
                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, leverItemPos, true));

                                nodesToNuke.AddRange(new[] { 153202, 154103 }); //quest_lever_icon_complete GlobalFlag, raise pyramid GlobalEventTrigger
                            }

                            //Randomize PoF switch
                            Vector2 c2PofSwitchPos = new Vector2(22.5f, 13.5f);
                            int c2Counter = 700000;
                            scriptNodesToAdd.AddRange(CreateRandomizedPoFPuzzle(levelFile, c2PofSwitchPos, ref c2Counter, out XElement[] c2PofDoodads));
                            doodadsToAdd.AddRange(c2PofDoodads);
                            break;
                        case "level_cave_3.xml":
                            int[] c1PortalNodes = { 117695, 117696, 121089 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(c1PortalNodes[archipelagoData.GetRandomLocation("Cave 1 Portal")]);
                            int[] c1KeystoneNodes = { 117646, 117644, 117645 };
                            int[] c1PanNodes = { 136200, 136198, 136199 };
                            int[] c1Exit = { 122536, 122537 }; //Open bottom, top
                            globalScriptNodesToTriggerOnceOnLoad.Add(c1Exit[archipelagoData.GetRandomLocation("Cave 1 Exit")]);

                            globalScriptNodesToTriggerOnceOnLoad.Add(c1KeystoneNodes[archipelagoData.GetRandomLocation("Cave 1 Keystone")]);
                            if (panLocation >= 6)
                            {
                                globalScriptNodesToTriggerOnceOnLoad.Add(c1PanNodes[panLocation - 6]);
                            }

                            nodesToNuke = new List<int> { 117647, 136205, 117697, 136201, 122551, 124202 }; //Keystone, Pan, Portal, Pan trigger, Exit, Puzzle spawn node
                            if (exitRando)
                            {
                                if (exitSwaps.ContainsKey("boss_2|0"))
                                    nodesToNuke.Add(156483); //Storage room node
                                //Move Krilith boss return node up so you don't spawn in the wall
                                idToNode["141426"].Element("vec2").Value = "-3 33.5";
                                //Remove disabling the main exit node
                                idToNode["156394"].Element("dictionary").Element("dictionary").Element("int-arr").Value = "156380 156381";
                            }

                            //Add delay to executing the portal event trigger node
                            idToNode["115377"].Elements("int-arr").ToArray()[1].Value = "0 0 10 0 0 0 0 0 0 0 0 0 10 10 0 0 0";

                            //Fix the random fly trap spawns running infinitely
                            idToNode["157682"].Elements("int").ToArray()[1].Value = "1";

                            //Exit returns
                            string[] c1ReturnNodes = { "122797", "122800" };
                            //Enable start node in case we come in from the back
                            idToNode[c1ReturnNodes[archipelagoData.GetRandomLocation("Cave 1 Exit")]].Element("bool").Value = "True";

                            if (buttonsanity > 0)
                            {
                                //CheckGlobalFlag node for the pumps
                                idToNode["141518"].Element("dictionary").Element("string").Value = pumpsItemName;

                                XElement c3PumpsEventTriggerNode = NodeHelper.CreateGlobalEventTriggerNode(modNodeStartId++, 1, new Vector2(-27, -7), pumpsItemName, new[] { 116081 });
                                scriptNodesToAdd.Add(c3PumpsEventTriggerNode);

                                nodesToNuke.Add(155718); //puzzle_bonus_solved GlobalEventTrigger
                            }

                            //The last puzzle in this level is spawned via script, we disabled it so we need to manually create the puzzle
                            XElement puzzleNodeGroup = prefabsNode.Elements().First(node => node.HasAttributes && node.Attribute("name") != null &&
                                node.Attribute("name").Value == "prefabs/puzzle_random.xml");
                            XElement puzzleNode = new XElement("vec2");
                            puzzleNode.Value = "84 11";
                            puzzleNodeGroup.Add(puzzleNode);

                            //Randomize PoF switch
                            Vector2 c3PofSwitchPos = new Vector2(14.5f, 2f);
                            int c3Counter = 700000;
                            scriptNodesToAdd.AddRange(CreateRandomizedPoFPuzzle(levelFile, c3PofSwitchPos, ref c3Counter, out XElement[] c3PofDoodads));
                            doodadsToAdd.AddRange(c3PofDoodads);
                            break;
                        case "level_boss_1.xml":
                            scriptNodesToAdd.Add(NodeHelper.CreateLevelStartNode(modNodeStartId++, new Vector2(-39, 10.25f), 5)); //Offset +0,+2 from RectangleShape

                            int b1TpId = modNodeStartId;
                            Vector2 tpPos = new Vector2(-23f, 2.5f);
                            XElement[] b1TpDoodads = CreateToggleableTeleporter(ref modNodeStartId, new Vector2(-23f, 2.5f), -1, "boss_1", 5, -1, out List<XElement> b1TpScriptNodes, out int portalOnNodeId, out int portalOffNodeId);
                            doodadsToAdd.AddRange(b1TpDoodads);
                            scriptNodesToAdd.AddRange(b1TpScriptNodes);

                            int b1ToggleOnNodeId = modNodeStartId++;
                            int b1ToggleOffNodeId = modNodeStartId++;
                            scriptNodesToAdd.Add(NodeHelper.CreateToggleElementNode(b1ToggleOnNodeId, tpPos + new Vector2(2f, -3.5f), 1, 0, new[] { b1TpId + 3 }));
                            scriptNodesToAdd.Add(NodeHelper.CreateToggleElementNode(b1ToggleOffNodeId, tpPos + new Vector2(2f, -2.5f), 1, 1, new[] { b1TpId + 3 }));

                            //Turn the portal off until the boss is defeated
                            globalScriptNodesToTriggerOnceOnLoad.AddRange(new[] { b1ToggleOffNodeId, portalOffNodeId });

                            //Add a SetGlobalFlag node that sets the boss killed status
                            int b1FlagId = modNodeStartId + 20;
                            XElement b1SetFlagNode = NodeHelper.CreateSetGlobalFlagNode(b1FlagId, new Vector2(-7, -7), -1, "killed_boss_1", true);
                            scriptNodesToAdd.Add(b1SetFlagNode);

                            //ObjectEventNode that handles if you killed the last sand shark
                            NodeHelper.AddConnectionNodes(idToNode["136211"], new[] { b1FlagId, b1ToggleOnNodeId, portalOnNodeId });

                            int[] passageEntNodes = { 113652, 113653 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(passageEntNodes[archipelagoData.GetRandomLocation("Passage Entrance")]);

                            nodesToNuke = new List<int> { 113657 }; //Passage transition
                            break;
                        case "level_passage.xml":
                            int start = archipelagoData.GetRandomLocation("Passage Entrance");
                            int[] pEntExitNodes = { 121412, 121415 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(pEntExitNodes[start]);
                            int middle = archipelagoData.GetRandomLocation("Passage Middle");
                            int[] pEnt1Nodes = { 120795, 120796, 120797, 120798, 120799 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(pEnt1Nodes[middle]);
                            int[] pEnt2Nodes = { 121387, 121383, 121384, 121385, 121386 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(pEnt2Nodes[middle]);
                            int end = archipelagoData.GetRandomLocation("Passage End");
                            int[] pMid1Nodes = { 156098, 156100, 156099 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(pMid1Nodes[end]);
                            int[] pMid2Nodes = { 156122, 156124, 156123 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(pMid2Nodes[end]);
                            int[] pMid3Nodes = { 156144, 156146, 156145 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(pMid3Nodes[end]);
                            int[] pMid4Nodes = { 156154, 156156, 156155 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(pMid4Nodes[end]);
                            int[] pMid5Nodes = { 156164, 156166, 156165 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(pMid5Nodes[end]);
                            int[] pMidExitNodes = { 156105, 156128, 156150, 156160, 156170 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(pMidExitNodes[middle]);

                            nodesToNuke = new List<int> { 120800, 121392, 156107, 156127, 156149, 156159, 156169 };
                            //Entrance 1 through 2, Middle 1 through 5

                            //Exit returns
                            string[] pEntReturnNodes = { "120773", "120822" };
                            string[] pMidReturnNodes = { "120851", "120852", "120853", "120854", "120855" };
                            string[] pEndReturnNodes = { "120857", "120858", "120861" };
                            string[] enableNodeIds = { pEntReturnNodes[start], pMidReturnNodes[middle], pEndReturnNodes[end] };
                            //Enable start nodes in case we come in from the back
                            foreach (string enableId in enableNodeIds)
                            {
                                idToNode[enableId].Element("bool").Value = "True";
                            }

                            break;
                        case "level_temple_1.xml":
                            //Create pumps lever item
                            Vector2 telarianItemPos = new Vector2(36, -35.25f);
                            int pumpsItemNodeId = modNodeStartId;
                            string telarianItemXmlName = CreateItemScriptNodes(levelFile, pumpsItemNodeId, telarianItemPos, ref modNodeStartId, out List<XElement> pumpsLeverScriptNodesToAdd, true, out int holoEffectNodeId);
                            XElement pumpsItemNode = NodeHelper.CreateSpawnObjectNode(pumpsItemNodeId, telarianItemXmlName, telarianItemPos);
                            if (holoEffectNodeId != -1)
                                NodeHelper.AddConnectionNodes(pumpsItemNode, new int[] { holoEffectNodeId });
                            pumpsLeverScriptNodesToAdd.Add(pumpsItemNode);
                            scriptNodesToAdd.AddRange(pumpsLeverScriptNodesToAdd);
                            globalScriptNodesToTriggerOnceOnLoad.Add(pumpsItemNodeId);

                            effectNodePositions.Add("156397", new Vector2(37.75f, -30f));
                            effectNodePositions.Add("156276", new Vector2(81.5f, -0.5f));
                            effectNodePositions.Add("156280", new Vector2(81.5f, -0.5f));
                            effectNodePositions.Add("156281", new Vector2(81.5f, -0.5f));
                            effectNodePositions.Add("156282", new Vector2(81.5f, -0.5f));

                            int[] t1PortalNodes = { 131895, 131894, 130029 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(t1PortalNodes[archipelagoData.GetRandomLocation("Temple 1 Portal")]);
                            int[] t1KeystoneNodes = { 126436, 126437, 132118, 126435 };
                            int t1Keystone = archipelagoData.GetRandomLocation("Temple 1 Keystone");
                            globalScriptNodesToTriggerOnceOnLoad.Add(t1KeystoneNodes[t1Keystone]);
                            if (t1Keystone == 2)
                            {
                                //If this will be placed on the right ledge where the item already exists remove the spawn node, we're just going to be modifying the freestanding item anyways
                                nodesToNuke.Add(132118);
                            }
                            int[] sSilverKeyNodes = { 126145, 126146, 126144 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(sSilverKeyNodes[archipelagoData.GetRandomLocation("Temple 1 South Silver Key")]);
                            int[] nSilverKeyNodes = { 132226, 132227 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(nSilverKeyNodes[archipelagoData.GetRandomLocation("Temple 1 North Silver Key")]);
                            int[] iceTurretSilverKeyNodes = { 132229, 132228 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(iceTurretSilverKeyNodes[archipelagoData.GetRandomLocation("Temple 1 Ice Turret Silver Key")]);
                            int[] funkySilverKeyNodes = { 127922, 127923 };
                            int funkySilverKey = archipelagoData.GetRandomLocation("Temple 1 Funky Silver Key");
                            globalScriptNodesToTriggerOnceOnLoad.Add(funkySilverKeyNodes[funkySilverKey]);
                            if (funkySilverKey == 0)
                            {
                                int[] funkyOreNodes = { 127926, 127927 };
                                globalScriptNodesToTriggerOnceOnLoad.Add(funkyOreNodes[archipelagoData.GetRandomLocation("Temple 1 Funky Ore")]);
                            }

                            int[] goldKeyNodes = { 132223, 132221 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(goldKeyNodes[archipelagoData.GetRandomLocation("Temple 1 Gold Key")]);
                            int[] eOreNodes = { 141934, 141935 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(eOreNodes[archipelagoData.GetRandomLocation("Temple 1 East Ore")]);
                            int[] t1MirrorNodes = { 132218, 132216, 132217 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(t1MirrorNodes[archipelagoData.GetRandomLocation("Temple 1 Mirror")]);
                            if (archipelagoData.GetRandomLocation("Temple 1 Hidden Room Random Node") == 1)
                                globalScriptNodesToTriggerOnceOnLoad.Add(129835);
                            int[] t1PuzzleNodes = { 134196, 134210 }; //Deactivate right, left
                            int t1PuzzleNode = t1PuzzleNodes[archipelagoData.GetRandomLocation("Temple 1 Puzzle Spawn")];

                            //Add delay to the deactivation node, as there's funky stuff with timing and activating
                            idToNode[t1PuzzleNode.ToString()].Elements("int-arr").ToArray()[1].Value = "1 1";
                            //Replace hallway prefab in SpawnObject node, hallway spawn node in west puzzle
                            XElement[] t1Strs = idToNode["129831"].Elements("string").ToArray();
                            t1Strs[1].Value = $"prefabs/theme_g/parts/alley_med_v_{archipelagoData.GetRandomLocation("Temple 1 Hidden Room") + 1}.xml";
                            //Bottom levelLoaded GlobalEventTrigger, we need to add this manually else the walls won't be diabled correctly
                            NodeHelper.AddConnectionNodes(idToNode["112617"], new[] { t1PuzzleNode }, new[] { 100 });

                            int[] t1ExitNodes = { 133955, 133956 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(t1ExitNodes[archipelagoData.GetRandomLocation("Temple 2 Entrance")]);

                            nodesToNuke.AddRange(new List<int> { 126439, 131896, 141936, 132219/*SE mirror*/, 127928, 129840, 134207, 154194, 133957 });
                            //Keystone, portal, east ore, se mirror, funky silver key ore, hidden room, puzzle, Lever trigger, exit
                            nodesToNuke.AddRange(new[] { 126147, 127924, 132231, 132232 }); //Silver keys: left key, funky key node, ice turret key, north silver key
                            nodesToNuke.AddRange(new[] { 132224 }); //Gold keys: ice turret key

                            //Removes the DangerArea that has the stairs buff at the beginning of the level, useless unless you're a wizard :P
                            nodesToNuke.Add(141465);

                            if (archipelagoData.GetOption(SlotDataKeys.noSunbeamDamage) > 0)
                                nodesToNuke.AddRange(new[] { 132507, 132505, 132514, 132517, 4279, 127124, 127127, 134527, 132520, 132526, 132523 }); //Beam shape nodes

                            //Create NPC button push nodes and hook up events to their effects
                            int guardSpawnNodeId = 0;
                            int telarianSpawnNodeId = 0;
                            if (buttonsanity > 0)
                            {
                                guardSpawnNodeId = modNodeStartId;
                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, new Vector2(-10.375f, 25.5f), true));
                                telarianSpawnNodeId = modNodeStartId;
                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, new Vector2(36.5f, -36.5f), true));

                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(-11, 22), "Open TF1 Trapped Guard Gate", ref buttonEffectNodeIds, new[] { 153519 }));
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(33.5f, -34.5f), "Open TF1 Telarian Gate", ref buttonEffectNodeIds, new[] { 153459 }));

                                nodesToNuke.Add(154103); //puzzle_bonus_solved GlobalEventTrigger
                            }
                            //Change npc dialogues
                            //Guard 2nd dialogue
                            string t1Guard2ndMessage = ArchipelagoMessageManager.GetLanguageString("d.temple1.npc.guardmirrorb", new string[0]);
                            string[] t1Guard2ndSentences = t1Guard2ndMessage.Split('.');
                            t1Guard2ndMessage = t1Guard2ndMessage.Remove(t1Guard2ndMessage.Length - t1Guard2ndSentences[t1Guard2ndSentences.Length - 2].Length);
                            t1Guard2ndMessage += "They're scattered around everywhere.";
                            NodeHelper.EditShowSpeechBubbleNode(idToNode["142439"], t1Guard2ndMessage);
                            //Guard 3nd dialogue
                            if (archipelagoData.GetOption(SlotDataKeys.noSunbeamDamage) > 0)
                            {
                                string message = ArchipelagoMessageManager.GetLanguageString("d.temple1.npc.guardmirrorc", new string[0]);
                                string[] sentences = message.Split('.');
                                message = message.Remove(message.Length - sentences[sentences.Length - 1].Length + 1);
                                message += "Don't worry about the light beams, they may look intense but they're actually quite pleasant!";
                                NodeHelper.EditShowSpeechBubbleNode(idToNode["142398"], message);
                            }
                            //Telarian dialouge
                            string t1TelarianMessage = ArchipelagoMessageManager.GetLanguageString("d.temple1.npc.telariana", new string[0]);
                            string[] t1TelarianSentences = t1TelarianMessage.Split('.');
                            t1TelarianMessage = t1TelarianMessage.Remove(t1TelarianMessage.Length - t1TelarianSentences[t1TelarianSentences.Length - 2].Length);
                            switch (telarianItemXmlName)
                            {
                                case "items/tool_lever.xml":
                                    t1TelarianMessage += t1TelarianSentences[t1TelarianMessage.Length - 2]; //Keep the dialogue as normal
                                    break;
                                case "items/tool_lever_fragment.xml":
                                    t1TelarianMessage += "However, when I got here all I found was that piece of it! If you want to lower the water, find the rest of them and bring the whole lever to the pump station.";
                                    break;
                                case APData.checkItemXmlName:
                                    t1TelarianMessage += $"However, when I got here it was missing. I think someone stole it. If you want to lower the water, find it and bring the lever to the pump station.";
                                    break;
                                default:
                                    string extraString = "";
                                    if (telarianItemXmlName.Contains("archipelago"))
                                        extraString = " weird thing";
                                    t1TelarianMessage += $"However, when I got here it was missing. That{extraString} was in its place. If you want to lower the water, find it and bring the lever to the pump station.";
                                    break;
                            }
                            NodeHelper.EditShowSpeechBubbleNode(idToNode["153410"], t1TelarianMessage);

                            //Telarian hit node, make it spawn his item
                            NodeHelper.AddConnectionNodes(idToNode["153407"], new[] { pumpsItemNodeId });

                            if (buttonsanity > 0)
                            {
                                //Trapped Guard button sound node
                                NodeHelper.SetConnectionNodes(idToNode["153521"], new[] { guardSpawnNodeId });
                                //Telarian button sound node
                                NodeHelper.SetConnectionNodes(idToNode["153461"], new[] { telarianSpawnNodeId });
                            }

                            //Randomize PoF switch
                            Vector2 t1PofSwitchPos = new Vector2(65.5f, -24.5f);
                            int t1Counter = 700000;
                            scriptNodesToAdd.AddRange(CreateRandomizedPoFPuzzle(levelFile, t1PofSwitchPos, ref t1Counter, out XElement[] t1PofDoodads));
                            doodadsToAdd.AddRange(t1PofDoodads);
                            break;
                        case "level_boss_2.xml":
                            effectNodePositions.Add("156146", new Vector2(-10f, -14.5f));
                            effectNodePositions.Add("156149", new Vector2(15f, 7.5f));

                            int b2FlagId = modNodeStartId++;
                            XElement b2SetFlagNode = NodeHelper.CreateSetGlobalFlagNode(b2FlagId, new Vector2(-13.5f, -25), -1, "killed_boss_2", true);
                            scriptNodesToAdd.Add(b2SetFlagNode);

                            //ObjectEventNode that triggers the killed_boss_2 flag
                            NodeHelper.AddConnectionNodes(idToNode["158133"], new int[] { b2FlagId });
                            break;
                        case "level_temple_2.xml":
                            int[] t2PortalNodes = { 3322, 137953, 137951, 137952 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(t2PortalNodes[archipelagoData.GetRandomLocation("Temple 2 Portal")]);
                            int[] t2KeystoneNodes = { 145350, 145355, 145353, 145352, 145351, 145349, 145354 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(t2KeystoneNodes[archipelagoData.GetRandomLocation("Temple 2 Keystone")]);
                            int[] t2Puzzle1Nodes = { 136507, 136503 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(t2Puzzle1Nodes[archipelagoData.GetRandomLocation("Temple 2 Puzzle Spawn EW")]);
                            int[] t2Puzzle2Nodes = { 136505, 136488 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(t2Puzzle2Nodes[archipelagoData.GetRandomLocation("Temple 2 Puzzle Spawn NS")]);

                            int t2LightBridgesItemId = modNodeStartId++;
                            scriptNodesToAdd.Add(NodeHelper.CreateGlobalEventTriggerNode(t2LightBridgesItemId, 1, new Vector2(22, 16), "t2_runes", null));
                            int t2PortalSequenceItemId = modNodeStartId++;
                            scriptNodesToAdd.Add(NodeHelper.CreateGlobalEventTriggerNode(t2PortalSequenceItemId, 1, new Vector2(-17, -5), "t2_portal", null));

                            //Hallway spawn node in west puzzle
                            XElement[] t2Strs = idToNode["136606"].Elements("string").ToArray();
                            t2Strs[1].Value = $"prefabs/theme_g/parts/alley_med_v_{archipelagoData.GetRandomLocation("Temple 2 Hidden Room") + 1}.xml";

                            //Put a delay on the ScriptLink node that deletes the dummy objects on the ice blocks so they'll delete properly
                            idToNode["156429"].Elements("int-arr").ToArray()[1].Value = "100";

                            if (buttonsanity > 0)
                            {
                                //Light bridges effect sound node and portal sequence effect node, set trigger times to 1
                                idToNode["141393"].Elements("int").ToArray()[1].Value = "1";
                                idToNode["143589"].Elements("int").ToArray()[1].Value = "1";
                                //Light bridges sequence scriptLink node
                                NodeHelper.SetConnectionNodes(idToNode["141391"], t2LightBridgesItemId);
                                //Portal sequence scriptLink node
                                NodeHelper.SetConnectionNodes(idToNode["143587"], t2PortalSequenceItemId);

                                nodesToNuke.Add(154103); //puzzle_bonus_solved GlobalEventTrigger
                            }

                            int[] t2JonesNodes = { 144534, 144529 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(t2JonesNodes[archipelagoData.GetRandomLocation("Temple 2 Jones Reward")]);
                            if (archipelagoData.GetRandomLocation("Temple 2 Jones Reward") == 0)
                            {
                                int[] t2GoldKeyNodes = { 144531, 144532, 144530 };
                                globalScriptNodesToTriggerOnceOnLoad.Add(t2GoldKeyNodes[archipelagoData.GetRandomLocation("Temple 2 Gold Key")]);
                            }
                            int[] t2SilverKey1Nodes = { 144456, 144457, 144462, 144459, 144460 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(t2SilverKey1Nodes[archipelagoData.GetRandomLocation("Temple 2 Silver Key 1")]);
                            int[] t2SilverKey2Nodes = { 144466, 144467, 144465, 144464 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(t2SilverKey2Nodes[archipelagoData.GetRandomLocation("Temple 2 Silver Key 2")]);

                            int pickaxeSpawnIndex = archipelagoData.GetRandomLocation("Pickaxe");
                            int[] pickaxeNodes = { 150186, 150192 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(pickaxeNodes[pickaxeSpawnIndex]);
                            //Pickaxe ice block effect nodes
                            if (pickaxeSpawnIndex == 0)
                            {
                                Vector2 leftPickaxePos = new Vector2(-65, -17);
                                effectNodePositions.Add("156283", leftPickaxePos);
                                int spawnNodeId = modNodeStartId; //898249;
                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, leftPickaxePos, true));
                                scriptNodesToAdd.Add(NodeHelper.CreateCheckGlobalFlagNode(modNodeStartId++, leftPickaxePos, "boss_krilith_dead", false, new[] { spawnNodeId }, false, null));
                                globalScriptNodesToTriggerOnLoad.Add(modNodeStartId - 1);
                            }
                            else
                            {
                                Vector2 rightPickaxePos = new Vector2(3, 34);
                                effectNodePositions.Add("156282", rightPickaxePos);
                                int spawnNodeId = modNodeStartId; //898249;
                                scriptNodesToAdd.AddRange(CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, rightPickaxePos, true));
                                scriptNodesToAdd.Add(NodeHelper.CreateCheckGlobalFlagNode(modNodeStartId++, rightPickaxePos, "boss_krilith_dead", false, new[] { spawnNodeId }, false, null));
                                globalScriptNodesToTriggerOnLoad.Add(modNodeStartId - 1);
                            }

                            int[] t2StartAreaEnableNodes = { 2189, 2585 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(t2StartAreaEnableNodes[archipelagoData.GetRandomLocation("Temple 2 Entrance")]);

                            nodesToNuke = new List<int> { 138196, 145356, 136508, 136509, 144533, 144535, 144463, 144469, 150198 };
                            //Portal, keystone, puzzle spawn 1, puzzle spawn 2, Jones reward, gold key, silver key 1, silver key 2

                            if (archipelagoData.GetOption(SlotDataKeys.noSunbeamDamage) > 0)
                                nodesToNuke.AddRange(new[] { 154509, 154512, 154515, 154506, 154488, 154503, 154494, 154497, 154491, 154500 }); //Beam shape nodes
                            //Last two are below the bottom of the map

                            //Special handling for buttonsanity and button sets
                            if (buttonsanity > 0)
                            {
                                //Light bridges
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(21, 14), "Activate TF2 Light Bridges", ref buttonEffectNodeIds, new[] { 148445 }, new[] { 1000 }));
                                //Portal
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(-21.5f, 2), "Open TF2 Portal Room Shortcut", ref buttonEffectNodeIds, new[] { 143591, 143581 }, new[] { 300, 300 }));
                            }

                            //Randomize PoF switch
                            Vector2 t2PofSwitchPos = new Vector2(-17.5f, 54.5f);
                            int t2Counter = 700000;
                            scriptNodesToAdd.AddRange(CreateRandomizedPoFPuzzle(levelFile, t2PofSwitchPos, ref t2Counter, out XElement[] t2PofDoodads));
                            doodadsToAdd.AddRange(t2PofDoodads);
                            break;
                        case "level_temple_3.xml":
                            List<int> blockItemNodes = new List<int>(5);

                            int[] t3Block1Nodes = { 143522, 143523 };
                            blockItemNodes.Add(t3Block1Nodes[archipelagoData.GetRandomLocation("Temple 3 S Beam 1")]);
                            int[] t3Block2Nodes = { 143518, 143510 };
                            blockItemNodes.Add(t3Block2Nodes[archipelagoData.GetRandomLocation("Temple 3 S Beam 2")]);
                            int[] t3Block3Nodes = { 143519, 143520 };
                            blockItemNodes.Add(t3Block3Nodes[archipelagoData.GetRandomLocation("Temple 3 S Beam 3")]);
                            int[] t3Block4Nodes = { 143528, 143529 };
                            blockItemNodes.Add(t3Block4Nodes[archipelagoData.GetRandomLocation("Temple 3 S Beam 4")]);
                            int[] t3Block5Nodes = { 143525, 143526 };
                            blockItemNodes.Add(t3Block5Nodes[archipelagoData.GetRandomLocation("Temple 3 S Beam 5")]);

                            nodesToNuke = new List<int> { 143524, 143517, 143521, 143530, 143527 };
                            //S Beam 1 though 5

                            int t3CombinationItemId = modNodeStartId++;
                            scriptNodesToAdd.Add(NodeHelper.CreateGlobalEventTriggerNode(t3CombinationItemId, 1, new Vector2(-10.5f, -51), "t3_levers", null));
                            List<string> destroyObjNodes = new List<string> { "137983", "137985", "137987", "137989", "137991" };
                            //DestroyObject node that would break a block with an item underneath
                            for (int d = 0; d < destroyObjNodes.Count; d++)
                            {
                                NodeHelper.AddConnectionNodes(idToNode[destroyObjNodes[d]], new[] { blockItemNodes[d] });
                            }
                            if (buttonsanity > 0)
                            {
                                //Redirect sequence completed announce node
                                NodeHelper.SetConnectionNodes(idToNode["148447"], t3CombinationItemId);
                                //Add in node to trigger opening the puzzle
                                string chutesItemName = ArchipelagoManager.GetItemName(APData.templeButtonItemStartID + 51);
                                scriptNodesToAdd.AddRange(CreateButtonEffectTriggerNodes(ref modNodeStartId, new Vector2(-13, -46), chutesItemName, ref buttonEffectNodeIds, new[] { 148445 }, new[] { 1000 }));

                                //Lever nodes
                                string[] leverAreaTriggerNodeIds = new string[]
                                {
                                    "149241",
                                    "149242",
                                    "149243",
                                    "148142",
                                };
                                int[] leverItemNodeIds = new int[4];
                                for (int s = 0; s < leverAreaTriggerNodeIds.Length; s++)
                                {
                                    string leverAreaTriggerNodeId = leverAreaTriggerNodeIds[s];
                                    string leverAreaNodeId = idToNode[leverAreaTriggerNodeId].Element("dictionary").Element("dictionary").Element("int-arr").Value;
                                    leverItemNodeIds[s] = modNodeStartId;
                                    Vector2 pos = NodeHelper.PosFromString(idToNode[leverAreaNodeId].Element("vec2").Value) + new Vector2(0, 1);
                                    XElement[] leverItemNodes = CreateSpawnItemScriptNodes(levelFile, ref modNodeStartId, pos, true);
                                    scriptNodesToAdd.AddRange(leverItemNodes);
                                    NodeHelper.AddConnectionNodes(idToNode[leverAreaTriggerNodeId], new int[] { leverItemNodeIds[s] });
                                }
                                NodeHelper.AddConnectionNodes(idToNode["148447"], leverItemNodeIds);
                            }

                            if (archipelagoData.GetOption(SlotDataKeys.noSunbeamDamage) > 0)
                            {
                                nodesToNuke.AddRange(new[] { 154587, 154590, 154503, 154554, 154557, 154596, 154599, 154602, 154605, 154548, 154551, 154563, 154560, 154566, 154500, 154491 });
                                //Beam shape nodes, last two are below the bottom of the map
                                nodesToNuke.AddRange(new[] { 154506, 154509, 154512, 154515, 154518, 154521, 154524, 154527, 154530, 154533, 154536, 154539, 154542 }); //Beam shape nodes
                            }

                            int[] t3StartAreaEnableNodes = { 143331, 143317, 143285 };
                            globalScriptNodesToTriggerOnceOnLoad.Add(t3StartAreaEnableNodes[archipelagoData.GetRandomLocation("Temple 3 Entrance")]);
                            break;
                        case "level_bonus_5.xml":
                            nodesToNuke = new List<int> { 88400 }; //SetGlobalFlag

                            int pofCompleteFlagNode = modNodeStartId++;
                            XElement n5ExitFlagNode = NodeHelper.CreateSetGlobalFlagNode(pofCompleteFlagNode, new Vector2(-2.5f, 104f), 1, "quest_pyramid_solved", true);
                            scriptNodesToAdd.Add(n5ExitFlagNode);

                            List<string> exitNodeIds = new List<string>() { "87603", "87620", "87610", "87615" };
                            Dictionary<string, List<string>> bonusDeleteNodePanelDoodads = new Dictionary<string, List<string>>
                            {
                                { "86367", new List<string>() { "86335", "86383", "86405", "86406", "86407", "86408", "86409", "86410", "86411", "86412", "86413", "86414", "86415" } },
                                { "86448", new List<string>() { "86447", "86446", "86445", "86444", "86443" } },
                                { "87672", new List<string>() { "88316", "88305", "88306", "88307" } },
                                { "87634", new List<string>() { "87635", "87636" } },
                                { "87990", new List<string>() { "87578", "87579", "87580" } },
                            };
                            Dictionary<string, string> areaTriggerDestroyNodes = new Dictionary<string, string>
                            {
                                { "86381", "86367" },
                                { "86441", "86448" },
                                { "87670", "87672" },
                                { "87632" , "87634" },
                                { "87991" , "87990" },
                            };
                            foreach(string exitNodeId in exitNodeIds)
                            {
                                NodeHelper.AddConnectionNodes(idToNode[exitNodeId], new[] { pofCompleteFlagNode });
                            }
                            foreach(var bonusDeleteNodePanel in bonusDeleteNodePanelDoodads)
                            {
                                List<string> deleteDoodads = bonusDeleteNodePanel.Value;
                                XElement deleteDoodadsNode = idToNode[bonusDeleteNodePanel.Key].Element("dictionary").Element("int-arr");
                                deleteDoodadsNode.Value = string.Join(" ", deleteDoodadsNode.Value.Split(' ').Where(str => !deleteDoodads.Contains(str)));
                            }
                            foreach (var areaTriggerDestroyNode in areaTriggerDestroyNodes)
                            {
                                string deleteNodeId = areaTriggerDestroyNode.Value;
                                NodeHelper.AddConnectionNodes(idToNode[areaTriggerDestroyNode.Key], new[] { modNodeStartId });

                                List<string> panelDoodads = bonusDeleteNodePanelDoodads[deleteNodeId];
                                XElement panelDeleteNode = NodeHelper.CreateDestroyObjectNode(modNodeStartId++, NodeHelper.GetNodePos(idToNode[areaTriggerDestroyNode.Key]) + new Vector2(0, 5f), 1, new int[0]);
                                panelDeleteNode.Element("dictionary").Element("int-arr").Value = string.Join(" ", panelDoodads);
                                scriptNodesToAdd.Add(panelDeleteNode);
                            }

                            List<string> intermediateExitNodeIds = new List<string>() { "87662", "87660", "86426", "86429", "86327", "86323" };
                            //Top area (left, middle, exit), middle area (exit, top left, top right, bottom right)
                            List<string> intraLevelExitIds = new List<string>() { "87980", "87978" };
                            //Top exit, middle exit

                            XElement[] n5TpDoodads = CreateTeleporter(ref modNodeStartId, new Vector2(23.5f, 6.5f), -1, "hub", 111, -1, false, out List<XElement> n5TpScriptNodes);
                            doodadsToAdd.AddRange(n5TpDoodads);
                            scriptNodesToAdd.AddRange(n5TpScriptNodes);
                            break;
                        case "level_boss_3.xml":
                            int b3FlagId = modNodeStartId;
                            XElement b3SetFlagNode = NodeHelper.CreateSetGlobalFlagNode(b3FlagId, new Vector2(25, -17), -1, "killed_boss_3", true);
                            scriptNodesToAdd.Add(b3SetFlagNode);

                            //Connect the killed boss SetGlobalFlag node to run after the event is triggered. Also trigger the nodes to fix the bridges to the other platforms in case the player kills the boss too fast
                            NodeHelper.AddConnectionNodes(idToNode["158133"], new int[] { b3FlagId, 154469, 154743, 156550 });
                            break;
                    }
                    break;
            }
            if (archipelagoData.GetOption(SlotDataKeys.shopShuffle) > 0)
            {
                ShuffleShops(levelFile, doc, out List<XElement> shopDoodadsToAdd, out List<XElement> shopDoodadsToNuke);
                doodadsToAdd.AddRange(shopDoodadsToAdd);
                foreach (XElement doodad in shopDoodadsToNuke)
                {
                    doodadsToNuke.Add(doodad.Element("int").Value);
                }
            }
            scriptNodesToAdd.AddRange(ReplacePrefabs(levelFile, doc, ref modNodeStartId, out secretNodesToGuaranteeSpawn, out List<XElement> prefabDoodads, ref buttonEffectNodeIds));
            globalScriptNodesToTriggerOnceOnLoad.AddRange(secretNodesToGuaranteeSpawn);
            doodadsToAdd.AddRange(prefabDoodads);

            List<XElement> enemyLootSpawnNodes = EditActors(levelFile, actorNodeRoot);
            scriptNodesToAdd.AddRange(enemyLootSpawnNodes);

            EditScriptNodes(levelFile, scriptNodeRoot, nodesToNuke, buttonEffectNodeIds, effectNodePositions, nonHololinkedItemIds, idToNode, ref modNodeStartId);

            EditDoodads(levelFile, doodadsNodeRoot, actorNodeRoot);

            foreach (XElement node in scriptNodesToAdd)
            {
                scriptNodeRoot.Add(node);
            }

            foreach (XElement doodad in doodadsToAdd)
            {
                doodadsNodeRoot.Add(doodad);
            }
            foreach (XElement doodadNode in doodadsNodeRoot.Elements().ToArray())
            {
                string doodadId = doodadNode.Element("int").Value;
                if (doodadsToNuke.Contains(doodadId))
                {
                    doodadNode.Remove();
                    continue;
                }
                if (doodadsToChangeSync.ContainsKey(doodadId))
                {
                    doodadNode.Element("bool").Value = doodadsToChangeSync[doodadId] ? "True" : "False"; //need-sync
                }
            }

            CheckDuplicateNodeIds(levelFile, doodadsNodeRoot, scriptNodeRoot, lightNodeRoot, actorNodeRoot, itemNodeRoot);

            return doc.ToString();
        }
        private static void CheckDuplicateNodeIds(string levelFile, XElement doodadsNodeRoot, XElement scriptNodeRoot, XElement lightNodeRoot, XElement actorNodeRoot, XElement itemNodeRoot)
        {
            HashSet<string> nodeIds = new HashSet<string>();
            foreach (XElement node in doodadsNodeRoot.Elements())
            {
                string id = node.Element("int").Value;
                if (nodeIds.Contains(id))
                {
                    string pos = node.Element("vec2").Value;
                    Logging.Log($"{levelFile}: Found duplicate id {id} in doodad! Position {pos}");
                    continue;
                }
                nodeIds.Add(id);
            }
            foreach (XElement node in scriptNodeRoot.Elements())
            {
                string id = node.Element("int").Value;
                if (nodeIds.Contains(id))
                {
                    string pos = node.Element("vec2").Value;
                    Logging.Log($"{levelFile}: Found duplicate id {id} in script! Position {pos}");
                    continue;
                }
                nodeIds.Add(id);
            }
            foreach (XElement node in lightNodeRoot.Elements())
            {
                string id = node.Element("int").Value;
                if (nodeIds.Contains(id))
                {
                    string pos = node.Element("vec2").Value;
                    Logging.Log($"{levelFile}: Found duplicate id {id} in light! Position {pos}");
                    continue;
                }
                nodeIds.Add(id);
            }
            foreach (XElement actorGroup in actorNodeRoot.Elements())
            {
                foreach (XElement actorNode in actorGroup.Elements())
                {
                    string id = actorNode.Element("int").Value;
                    if (nodeIds.Contains(id))
                    {
                        string pos = actorNode.Element("vec2").Value;
                        Logging.Log($"{levelFile}: Found duplicate id {id} in actor! Position {pos}");
                        continue;
                    }
                    nodeIds.Add(id);
                }
            }
            foreach (XElement itemGroup in itemNodeRoot.Elements())
            {
                foreach (XElement itemNode in itemGroup.Elements())
                {
                    string id = itemNode.Element("int").Value;
                    if (nodeIds.Contains(id))
                    {
                        string pos = itemNode.Element("vec2").Value;
                        Logging.Log($"{levelFile}: Found duplicate id {id} in item! Position {pos}");
                        continue;
                    }
                    nodeIds.Add(id);
                }
            }
        }
        private static void ReplaceItemLocations(string levelFile, XDocument doc, out List<XElement> scriptNodesToAdd, out List<int> nonHololinkedItemIds)
        {
            scriptNodesToAdd = new List<XElement>();
            nonHololinkedItemIds = new List<int>();
            //Build items dictionary
            Dictionary<string, List<XElement>> itemsDict = new Dictionary<string, List<XElement>>();
            //Add items in items root element to itemsDict
            XElement itemsNode = doc.Root.Elements("dictionary").First(node => node.HasAttributes && node.Attribute("name") != null && node.Attribute("name").Value == "items");
            XElement[] itemsNodes = itemsNode.Elements().ToArray();
            int bombTrapCounter = 350000;
            foreach (XElement itemGroup in itemsNodes)
            {
                string itemGroupXmlName = itemGroup.Attribute("name").Value;
                //If this is not an item that we would randomize, ignore it
                if (!ArchipelagoManager.IsRandomized(itemGroupXmlName)) continue;
                if (itemGroupXmlName == "items/tool_pickaxe.xml") //We're placing pickaxe items a different way
                {
                    itemGroup.Remove();
                    continue;
                }
                XElement[] items = itemGroup.Elements().ToArray();
                foreach (XElement item in items)
                {
                    Vector2 pos = NodeHelper.PosFromString(item.Element("vec2").Value);
                    int itemId = int.Parse(item.Element("int").Value);
                    string xmlName = CreateItemScriptNodes(levelFile, itemId, pos, ref bombTrapCounter, out List<XElement> newNodes, false, out int holoNodeId);
                    if (xmlName == null)
                    {
                        if (!itemsDict.ContainsKey(itemGroupXmlName))
                            itemsDict[itemGroupXmlName] = new List<XElement>();
                        itemsDict[itemGroupXmlName].Add(item);
                        continue;
                    }
                    if (APData.IsPositionLinked(archipelagoData.mapType, levelFile, pos) && holoNodeId == -1)
                    {
                        nonHololinkedItemIds.Add(itemId);
                    }
                    scriptNodesToAdd.AddRange(newNodes);
                    if (!itemsDict.ContainsKey(xmlName))
                        itemsDict[xmlName] = new List<XElement>();
                    itemsDict[xmlName].Add(item);
                }
                itemGroup.Remove();
            }
            //Add randomized prefabs in prefabs root element to itemsDict
            XElement prefabsNode = doc.Root.Elements("dictionary").First(node => node.HasAttributes && node.Attribute("name") != null && node.Attribute("name").Value == "prefabs");
            int prefabId = 300000;
            foreach (XElement prefabGroup in prefabsNode.Elements())
            {
                string prefabGroupXmlName = prefabGroup.Attribute("name").Value;
                //If this is not an item that we would randomize, ignore it
                if (!ArchipelagoManager.IsRandomized(prefabGroupXmlName)) continue;
                IEnumerable<XElement> prefabPositions = prefabGroup.Elements("vec2").ToList();
                foreach (XElement prefabPos in prefabPositions)
                {
                    XElement itemElement = new XElement("array");
                    XElement idElement = new XElement("int")
                    {
                        Value = (prefabId++).ToString()
                    };
                    itemElement.Add(idElement);
                    itemElement.Add(prefabPos);
                    Vector2 pos = NodeHelper.PosFromString(prefabPos.Value);
                    string xmlName = CreateItemScriptNodes(levelFile, int.Parse(idElement.Value), pos, ref bombTrapCounter, out List<XElement> newNodes, false, out _);
                    if (xmlName == null) continue; //Leave this position alone in the group
                    scriptNodesToAdd.AddRange(newNodes);
                    prefabPos.Remove();
                    if (!itemsDict.ContainsKey(xmlName))
                        itemsDict[xmlName] = new List<XElement>();
                    itemsDict[xmlName].Add(itemElement);
                }
            }

            string randomUpgradeName = "prefabs/upgrade_random.xml";
            if (itemsDict.ContainsKey(randomUpgradeName))
            {
                Console.WriteLine("The item pool contains random upgrades somehow???");
            }
            itemsDict.Remove("prefabs/upgrade_random.xml"); //These don't exist anymore, they're converted to stat upgrades server-side

            if (archipelagoData.GetOption(SlotDataKeys.treasureShuffle) > 0)
            {
                foreach (string xmlName in treasureNames)
                {
                    if (itemsDict.ContainsKey(xmlName)) continue;
                    itemsDict[xmlName] = new List<XElement>();
                }
                foreach (XElement itemGroup in itemsNodes)
                {
                    string itemGroupXmlName = itemGroup.Attribute("name").Value;
                    if (!treasureNames.Contains(itemGroupXmlName))
                        continue;
                    foreach (XElement treasure in itemGroup.Elements())
                    {
                        string newGroupName = MiscHelper.GetDictWeighted(treasureCounts, random);
                        if (--treasureCounts[newGroupName] <= 0)
                            treasureCounts.Remove(newGroupName);
                        itemsDict[newGroupName].Add(treasure);
                    }
                    itemGroup.Remove();
                }
            }

            //Add all items from itemsDict to items root element
            foreach (string xmlName in itemsDict.Keys)
            {
                XElement itemsElement = new XElement("array");
                itemsElement.SetAttributeValue("name", xmlName);
                foreach (XElement itemPos in itemsDict[xmlName])
                {
                    itemsElement.Add(itemPos);
                }
                itemsNode.Add(itemsElement);
            }
        }
        private static void ReplaceGates(string levelFile, XElement itemsNode)
        {
            //If this is an escape level ignore it
            if (levelFile.Contains("esc")) return;
            //Build gate dictionary
            Dictionary<string, List<XElement>> gateDict = new Dictionary<string, List<XElement>>();
            string levelIndex = APData.GetLevelIdFromFileName(levelFile, archipelagoData);
            //Add items in items root element to itemsDict
            XElement[] itemsNodes = itemsNode.Elements().ToArray();
            foreach (XElement itemGroup in itemsNodes)
            {
                string itemGroupXmlName = itemGroup.Attribute("name").Value;
                //Is the item a segment of a door that isn't a bonus door
                if (!itemGroupXmlName.Contains("door") || itemGroupXmlName.Contains("bonus")) continue;
                XElement[] gates = itemGroup.Elements().ToArray();
                foreach (XElement item in gates)
                {
                    int itemId = int.Parse(item.Element("int").Value);
                    int doorId = APData.GetGateId(levelFile, itemId, archipelagoData);
                    string doorType = GetGateType($"{levelIndex}|{doorId}");
                    string newGateName = itemGroupXmlName.Replace("bronze", doorType);
                    //This one bronze gate piece is named different from the others >:|
                    if (itemGroupXmlName == "items/door_a_bronze_v2.xml" && doorType != "bronze")
                    {
                        newGateName = $"items/door_a_{doorType}_v_v2.xml";
                    }
                    else if (itemGroupXmlName.EndsWith("_v_v2.xml") && doorType == "bronze")
                    {
                        newGateName = "items/door_a_bronze_v2.xml";
                    }
                    else
                    {
                        newGateName = newGateName.Replace("silver", doorType);
                        newGateName = newGateName.Replace("gold", doorType);
                    }
                    if (!gateDict.ContainsKey(newGateName))
                        gateDict[newGateName] = new List<XElement>();
                    gateDict[newGateName].Add(item);
                }
                itemGroup.Remove();
            }
            //Add all items from gateDict to items root element
            foreach (string xmlName in gateDict.Keys)
            {
                XElement itemsElement = new XElement("array");
                itemsElement.SetAttributeValue("name", xmlName);
                foreach (XElement itemPos in gateDict[xmlName])
                {
                    itemsElement.Add(itemPos);
                }
                itemsNode.Add(itemsElement);
            }
        }
        private static List<XElement> ReplacePrefabs(string levelFile, XDocument doc, ref int nodeIdCounter, out List<int> scriptNodeIdsToSpawn, out List<XElement> doodadsToAdd, ref List<int> buttonEventNodes)
        {
            string levelPrefix = APData.GetLevelPrefix(levelFile, archipelagoData);
            XElement prefabsNode = doc.Root.Elements("dictionary").First(node => node.HasAttributes && node.Attribute("name") != null && node.Attribute("name").Value == "prefabs");
            List<XElement> scriptNodesToAdd = new List<XElement>();
            scriptNodeIdsToSpawn = new List<int>();
            doodadsToAdd = new List<XElement>();
            //Remove PoF switch prefabs and temple hub quest npc, we create them ourselves now
            List<string> deletePrefabs = new List<string> { "prefabs/puzzle_bonus_lock.xml", "prefabs/desert_quest_main.xml" };
            XElement[] prefabs = prefabsNode.Elements().ToArray();
            foreach (XElement prefabGroup in prefabs)
            {
                string prefabGroupXmlName = prefabGroup.Attribute("name").Value;
                if (!deletePrefabs.Contains(prefabGroupXmlName)) continue;
                prefabGroup.Remove();
            }
            int randomPrefabIndex = 1;
            //Secret randomization
            if (archipelagoData.GetOption(SlotDataKeys.randomizeSecrets) == 1)
            {
                XElement secretRoom = new XElement("array");
                secretRoom.SetAttributeValue("name", "prefabs/theme_e/parts/secret_random_1_blank.xml");
                XElement secretDud = new XElement("array");
                secretDud.SetAttributeValue("name", "prefabs/theme_e/parts/secret_random_2.xml");
                foreach (XElement prefabGroup in prefabsNode.Elements())
                {
                    XAttribute groupName = prefabGroup.Attribute("name");
                    string prefabGroupXmlName = groupName.Value;
                    switch (prefabGroupXmlName)
                    {
                        case "prefabs/theme_e/room_secret_random.xml":
                            XElement[] positions = prefabGroup.Elements().ToArray();
                            foreach (XElement pos in positions)
                            {
                                if (archipelagoData.GetRandomLocation($"{levelPrefix} Secret {randomPrefabIndex++}") == 0)
                                {
                                    pos.Remove();
                                    secretDud.Add(pos);
                                }
                                else
                                {
                                    Vector2 posVec = NodeHelper.PosFromString(pos.Value);
                                    posVec += new Vector2(.5f, -5f); //Item offset is (.5, -5) btw
                                    int nodeId = nodeIdCounter++;
                                    string xmlName = CreateItemScriptNodes(levelFile, nodeId, posVec, ref nodeIdCounter, out List<XElement> prefabItemNodes, true, out int holoEffectNodeId);
                                    if (xmlName == null)
                                        continue;
                                    scriptNodesToAdd.AddRange(prefabItemNodes);
                                    scriptNodesToAdd.Add(NodeHelper.CreateSpawnObjectNode(nodeId, xmlName, posVec));
                                    if (holoEffectNodeId != -1)
                                        NodeHelper.AddConnectionNodes(scriptNodesToAdd[scriptNodesToAdd.Count - 1], new int[] { holoEffectNodeId });
                                    pos.Remove();
                                    secretRoom.Add(pos);
                                    scriptNodeIdsToSpawn.Add(nodeId);
                                }
                            }
                            break;
                    }
                }

                prefabsNode.Add(secretRoom);
                prefabsNode.Add(secretDud);
            }
            //Puzzle randomization
            if (archipelagoData.GetOption(SlotDataKeys.randomizePegPuzzles) == 1)
            {
                int puzzleCounter = 1;
                int puzzleItemCounter = 800000;
                foreach (XElement prefabGroup in prefabsNode.Elements().ToArray())
                {
                    string prefabGroupXmlName = prefabGroup.Attribute("name").Value;
                    switch (prefabGroupXmlName)
                    {
                        case "prefabs/puzzle_random.xml":
                            XElement[] positions = prefabGroup.Elements().ToArray();
                            foreach (XElement pos in positions)
                            {
                                string puzzleSlotName = $"{levelPrefix} Puzzle {puzzleCounter++}";
                                int puzzlePegs = archipelagoData.GetRandomLocation(puzzleSlotName);
                                if (puzzlePegs >= 0)
                                {
                                    Vector2 basePosVec = NodeHelper.PosFromString(pos.Value);
                                    List<XElement> puzzleNodes = CreateRandomizedPegPuzzle(levelFile, basePosVec, ref puzzleItemCounter, puzzleSlotName,
                                        out List<XElement> doodads, ref buttonEventNodes);
                                    scriptNodesToAdd.AddRange(puzzleNodes);
                                    doodadsToAdd.AddRange(doodads);
                                    pos.Remove();
                                }
                            }
                            break;
                    }
                }
            }
            //Handle if the map would generate to close off some checks
            if (archipelagoData.mapType == ArchipelagoData.MapType.Temple && (levelPrefix == "Cave 2" || levelPrefix == "Cave 1" || levelPrefix == "Passage"))
            {
                XElement generatedGroup = new XElement("array");
                foreach (XElement prefabGroup in prefabsNode.Elements().ToArray())
                {
                    XAttribute groupName = prefabGroup.Attribute("name");
                    string prefabGroupXmlName = groupName.Value;
                    if (prefabGroupXmlName == "prefabs/theme_e/room_alley_med_v.xml")
                    {
                        int switchIndex = archipelagoData.GetRandomLocation($"{levelPrefix} Hidden Room");
                        string resultPrefab = $"prefabs/theme_e/parts/alley_med_v_{switchIndex + 1}.xml";
                        int posIndex = 0;
                        switch (levelPrefix)
                        {
                            case "Cave 2":
                                posIndex = 1;
                                break;
                            case "Cave 1":
                            case "Passage":
                                posIndex = 0;
                                break;
                        }
                        XElement posElement = prefabGroup.Elements().ToArray()[posIndex];
                        generatedGroup.SetAttributeValue("name", resultPrefab);
                        posElement.Remove();
                        generatedGroup.Add(posElement);
                        if (levelPrefix == "Cave 1")
                        {
                            int shortcutIndex = archipelagoData.GetRandomLocation("Cave 1 East Shortcut Hall");
                            string shortcutResultPrefab = $"prefabs/theme_e/parts/alley_med_v_{shortcutIndex + 1}.xml";
                            int shortcutPosIndex = 2; //Technically index 3 but we removed the first one
                            XElement generatedShortcutGroup;
                            if (switchIndex == shortcutIndex)
                            {
                                generatedShortcutGroup = generatedGroup;
                            }
                            else
                            {
                                generatedShortcutGroup = new XElement("array");
                                generatedShortcutGroup.SetAttributeValue("name", shortcutResultPrefab);
                            }
                            XElement shortcutPosElement = prefabGroup.Elements().ToArray()[shortcutPosIndex];
                            shortcutPosElement.Remove();
                            generatedShortcutGroup.Add(shortcutPosElement);
                            prefabsNode.Add(generatedShortcutGroup);
                        }
                        break;
                    }
                }
                prefabsNode.Add(generatedGroup);
            }
            //Shop shuffle
            if (archipelagoData.mapType == ArchipelagoData.MapType.Temple && archipelagoData.GetOption(SlotDataKeys.shopShuffle) > 0)
            {
                foreach (XElement prefabGroup in prefabsNode.Elements())
                {
                    XAttribute groupName = prefabGroup.Attribute("name");
                    string prefabGroupXmlName = groupName.Value;
                    if (prefabGroupXmlName.StartsWith("prefabs/desert_vendor_"))
                    {
                        string type = prefabGroupXmlName.Remove(0, "prefabs/desert_vendor_".Length);
                        type = type.Remove(type.Length - 4);
                        if (type == "gambler") continue;
                        if (type == "misc")
                            type = "vitality";
                        type = type[0].ToString().ToUpper() + type.Remove(0, 1);
                        string shuffledType = APData.GetShopPrefabSuffixFromAPId(archipelagoData.GetShopLocation($"{type} Shop"));
                        groupName.Value = $"prefabs/desert_vendor_{shuffledType}.xml";
                    }
                }
            }
            return scriptNodesToAdd;
        }
        private static void ShuffleShops(string levelFile, XDocument doc, out List<XElement> doodadsToAdd, out List<XElement> doodadsToNuke)
        {
            string levelPrefix = APData.GetLevelPrefix(levelFile, archipelagoData);
            XElement doodadsNode = doc.Root.Elements("dictionary").First(node => node.HasAttributes && node.Attribute("name") != null && node.Attribute("name").Value == "doodads").Element("array");
            doodadsToAdd = new List<XElement>();
            doodadsToNuke = new List<XElement>();
            if (levelFile == "level_esc_2.xml") return; //WHY is there a shop vendor in this level you'll never be able to see it!!!
            //Edit the shop sign doodads to match the randomized shops
            switch (archipelagoData.mapType)
            {
                case ArchipelagoData.MapType.Castle:
                    Dictionary<string, Tuple<XElement, XElement>> shopBubbles = new Dictionary<string, Tuple<XElement, XElement>>();
                    foreach (XElement doodad in doodadsNode.Elements())
                    {
                        XElement nameNode = doodad.Element("string");
                        string doodadName = nameNode.Value;
                        if (doodadName.Contains("_floorsign_"))
                        {
                            string pos = doodad.Element("vec2").Value;
                            if (!APData.shopSignVendors.ContainsKey(levelFile) || !APData.shopSignVendors[levelFile].ContainsKey(pos)) continue;
                            string vendor = APData.shopSignVendors[levelFile][pos];
                            string[] splits = doodadName.Split('_');
                            string shuffleType = APData.GetShortShopTypeFromAPId(archipelagoData.GetShopLocation(vendor));
                            if (shuffleType == "power")
                                shuffleType = "boost";
                            nameNode.Value = $"{splits[0]}_{splits[1]}_{splits[2]}_{shuffleType}_{splits[4]}";
                        }
                        else if (doodadName.StartsWith("doodads/special/vendor_speech_")) //Dialogue doodads
                        {
                            string pos = doodad.Element("vec2").Value;
                            if (!shopBubbles.ContainsKey(pos))
                            {
                                shopBubbles.Add(pos, new Tuple<XElement, XElement>(null, null));
                            }
                            string type = doodadName.Remove(0, "doodads/special/vendor_speech_".Length).Replace(".xml", "");
                            if (type.StartsWith("level"))
                            {
                                shopBubbles[pos] = new Tuple<XElement, XElement>(shopBubbles[pos].Item1, nameNode);
                            }
                            else
                            {
                                shopBubbles[pos] = new Tuple<XElement, XElement>(nameNode, shopBubbles[pos].Item2);
                            }
                        }
                        else if (doodadName.StartsWith("doodads/special/vendor_")) //Vendor doodad
                        {
                            string type = doodadName.Remove(0, "doodads/special/vendor_".Length).Replace(".xml", "");

                            string nodeId = doodad.Element("int").Value;
                            string prefix = levelPrefix;
                            if (prefix == "Chambers 11" && (nodeId == "1025" || nodeId == "1036")) //Chambers 11 secret shop ids
                            {
                                prefix += " Secret";
                            }
                            string vendor = $"{prefix} {APData.GetShopTypeName(type)} Shop";
                            string shuffledShop = APData.GetShopPrefabSuffixFromAPId(archipelagoData.GetShopLocation(vendor));
                            nameNode.Value = $"doodads/special/vendor_{shuffledShop}.xml";
                        }
                    }
                    foreach (string pos in shopBubbles.Keys.ToArray())
                    {
                        XElement typeNode = shopBubbles[pos].Item1;
                        XElement levelNode = shopBubbles[pos].Item2;
                        string type = typeNode.Value.Remove(0, "doodads/special/vendor_speech_".Length).Replace(".xml", "");

                        //Handling for the additional secret shops on floor 11
                        string prefix = levelPrefix;
                        if (prefix == "Chambers 11")
                        {
                            string[] posSplits = pos.Split(' ');
                            float posY = float.Parse(posSplits[1], System.Globalization.CultureInfo.InvariantCulture);
                            if (posY < -100)
                            {
                                prefix += " Secret";
                            }
                        }

                        string vendor = $"{prefix} {APData.GetShopTypeName(type)} Shop";
                        int shopId = archipelagoData.GetShopLocation(vendor);
                        string shuffledShop = APData.GetShopPrefabSuffixFromAPId(shopId);
                        string shuffledType = shuffledShop;
                        typeNode.Value = $"doodads/special/vendor_speech_{shuffledType}.xml";

                        if (shuffledType == "power")
                        {
                            //If the new shop is a Powerup vendor, we need to destroy the level doodad
                            if (levelNode != null)
                            {
                                doodadsToNuke.Add(levelNode.Parent);
                            }
                        }
                        else
                        {
                            int shuffledLevel = APData.GetShopLevelFromAPId(shopId);
                            if (levelNode == null)
                            {
                                levelNode = NodeHelper.CreateDoodadNode(550000 + doodadsToAdd.Count, "", pos, false).Element("string");
                                doodadsToAdd.Add(levelNode.Parent);
                            }
                            levelNode.Value = $"doodads/special/vendor_speech_level{shuffledLevel}.xml";
                        }
                    }
                    break;
                case ArchipelagoData.MapType.Temple:
                    break;
            }
        }
        private static List<XElement> EditActors(string levelFile, XElement actorNodeRoot)
        {
            XElement[] actorNodes = actorNodeRoot.Elements().ToArray();

            //Create loot spawn nodes
            List<XElement> lootScriptNodes = new List<XElement>();
            if (archipelagoData.GetOption(SlotDataKeys.randomizeEnemyLoot) == 0) return lootScriptNodes;
            int lootNodeIdCounter = 400000;
            foreach (XElement actorGroup in actorNodes)
            {
                string actorName = actorGroup.Attribute("name").Value;
                if (!actorName.Contains("tower") && !actorName.Contains("boss_worm")) continue; //We get items from minibosses and bosses during the game now

                bool doubleLocations = actorName.EndsWith("mb.xml");
                bool bumpLocation = actorName.StartsWith("actors/tower_tracking") || actorName.StartsWith("actors/tower_nova") || actorName.StartsWith("actors/boss_");
                foreach (XElement actor in actorGroup.Elements())
                {
                    XElement posElement = actor.Element("vec2");
                    Vector2 basePos = NodeHelper.PosFromString(posElement.Value);
                    int nodeId = int.Parse(actor.Element("int").Value);
                    List<Vector2> positions = new List<Vector2>(doubleLocations ? 2 : 1);
                    if (doubleLocations)
                    {
                        positions.Add(basePos - new Vector2(.25f, .25f));
                        positions.Add(basePos + new Vector2(.25f, .25f));
                    }
                    else if (bumpLocation)
                    {
                        Vector2 bumpPosition = basePos + new Vector2(0, 1);
                        if (archipelagoData.mapType == ArchipelagoData.MapType.Temple && APData.templeTowerBumpInvertLocations.TryGetValue(levelFile, out Vector2[] invertLocations))
                        {
                            if(invertLocations.Contains(basePos))
                            {
                                bumpPosition = basePos + new Vector2(0, -1);
                            }
                        }
                        positions.Add(bumpPosition);
                    }
                    else
                    {
                        positions.Add(basePos);
                    }
                    List<int> scriptNodeIds = new List<int>(2);
                    for (int p = 0; p < positions.Count; p++)
                    {
                        int lootSpawnNodeId = lootNodeIdCounter;
                        string xmlItem = CreateItemScriptNodes(levelFile, lootSpawnNodeId, positions[p], ref lootNodeIdCounter, out List<XElement> scriptNodes, true, out _);
                        if (xmlItem == null) break;
                        //If an item was rolled to be here, add script nodes for it
                        XElement spawnObjNode = NodeHelper.CreateSpawnObjectNode(lootSpawnNodeId, xmlItem, positions[p]);
                        scriptNodeIds.Add(lootSpawnNodeId);
                        lootScriptNodes.Add(spawnObjNode);

                        //If the item is a holo item add the play effect node as a connection
                        if (xmlItem.StartsWith(APData.apHoloPrefix))
                            scriptNodeIds.Add(lootNodeIdCounter - 1);
                        lootScriptNodes.AddRange(scriptNodes);
                    }

                    if (scriptNodeIds.Count > 0)
                    {
                        XElement onDestroyedNode = NodeHelper.CreateObjectEventTriggerNode(lootNodeIdCounter++, basePos + new Vector2(0, -2), 1, "Destroyed", new[] { nodeId }, false, scriptNodeIds.ToArray());
                        lootScriptNodes.Add(onDestroyedNode);
                    }
                }
            }
            return lootScriptNodes;
        }
        private static void EditDoodads(string levelFile, XElement doodadsRootNode, XElement actorRootNode)
        {
            int shuffleMode = archipelagoData.GetEnemyShuffleMode();
            if (shuffleMode > 0 && !levelFile.Contains("esc"))
            {
                //Find all spawners
                Dictionary<string, string> spawnerDoodadPositions = new Dictionary<string, string>();
                foreach (XElement actorGroup in actorRootNode.Elements())
                {
                    string fullGroupName = actorGroup.Attribute("name").Value;
                    if (APData.excludedActors.Contains(fullGroupName) || fullGroupName.StartsWith("actors/boss")) continue;
                    if (fullGroupName.StartsWith("actors/spawners/"))
                    {
                        foreach (XElement spawner in actorGroup.Elements())
                        {
                            if (!spawnerDoodads.TryGetValue(fullGroupName, out string value))
                                continue;
                            if (value == "doodads/special/marker_grave.xml" && archipelagoData.mapType == ArchipelagoData.MapType.Temple)
                                value = "doodads/special/marker_grave_g.xml";
                            string spawnerPos = spawner.Element("vec2").Value;
                            spawnerDoodadPositions.Add(spawnerPos, value);
                        }
                    }
                }
                //Modify spawner doodads or remove if the spawner is different and doesn't have one
                XElement[] allDoodads = doodadsRootNode.Elements().ToArray();
                foreach (XElement doodad in allDoodads)
                {
                    string name = doodad.Element("string").Value;
                    if (!name.StartsWith("doodads/special/marker_grave") && name != "doodads/special/marker_maggot_1.xml") continue;
                    string pos = doodad.Element("vec2").Value;
                    if (spawnerDoodadPositionCorrections.TryGetValue(levelFile, out Dictionary<string, string> corrections) &&
                        corrections.TryGetValue(pos, out string correctedPos))
                        pos = correctedPos;
                    if (!spawnerDoodadPositions.TryGetValue(pos, out string doodadType))
                    {
                        doodad.Remove();
                        continue;
                    }
                    doodad.Element("string").Value = doodadType;
                }
                //Create doodads if the spawner needs one
                int doodadCounter = 0;
                foreach (var pair in spawnerDoodadPositions)
                {
                    if (pair.Value == "") continue;
                    XElement doodad = NodeHelper.CreateDoodadNode(1313131 + doodadCounter++, pair.Value, pair.Key, false);
                    doodadsRootNode.Add(doodad);
                }
            }
            if (archipelagoData.mapType == ArchipelagoData.MapType.Castle && archipelagoData.GetOption(SlotDataKeys.exitRandomization) > 0)
            {
                //Set floorsigns
                XElement[] allDoodads = doodadsRootNode.Elements().ToArray();
                foreach (XElement doodad in allDoodads)
                {
                    string name = doodad.Element("string").Value;
                    if (!name.Contains("floorsign") || (!name.EndsWith("up.xml") && !name.EndsWith("dn.xml"))) continue;
                    string pos = doodad.Element("vec2").Value;
                    string exitCode = floorSignExits[levelFile][pos];
                    if (!exitSwaps.TryGetValue(exitCode, out string newCode))
                    {
                        continue;
                    }
                    if (!exitCodeToActChar.TryGetValue(newCode, out char actChar))
                    {
                        continue;
                    }
                    string signNameEnd = name.Substring(name.Length - 6);
                    string[] splits = newCode.Split('|');
                    if (splits[0].Contains("bonus")) //Bonus levels don't have signs!
                    {
                        doodad.Remove();
                        break;
                    }
                    string signType = "";
                    if (int.TryParse(splits[0], out int levelNum))
                    {
                        signType = levelNum.ToString();
                    }
                    else
                    {
                        switch (splits[0])
                        {
                            case "esc_1":
                                signType = "10";
                                break;
                            case "esc_2":
                                signType = "7";
                                break;
                            case "esc_3":
                                signType = "4";
                                break;
                            case "esc_4":
                                signType = "1";
                                break;
                            case "10b":
                                signType = "question";
                                break;
                            default:
                                if (splits[0].Contains("boss"))
                                {
                                    signType = "boss";
                                }
                                break;
                        }
                    }
                    string newName = $"doodads/theme_{actChar}/{actChar}_floorsign_{signType}_{signNameEnd}";
                    doodad.Element("string").Value = newName;
                }
            }
        }

        private static void EditScriptNodes(string levelFile, XElement scriptNodesRoot, List<int> nodesToDisable, List<int> buttonEffectNodes, Dictionary<string, Vector2> effectNodePositions,
            List<int> nonHololinkedItemIds, Dictionary<string, XElement> idToNode, ref int modNodeCounter)
        {
            string levelPrefix = APData.GetLevelPrefix(levelFile, archipelagoData);
            XElement[] scriptNodes = scriptNodesRoot.Elements().ToArray();
            List<XElement> actorSpawnObjectNodes = new List<XElement>();
            int bombTrapCounter = 370000;
            int doodadSpawnCounter = 500000;
            int enemyLootNodeCounter = 450000;
            foreach (XElement node in scriptNodes)
            {
                string nodeType = node.Element("string").Value;
                string nodeId = node.Element("int").Value;
                if (nodesToDisable.Contains(int.Parse(nodeId)))
                {
                    node.Remove();
                    continue;
                }
                switch (nodeType)
                {
                    case "GlobalEventTrigger":
                        //Temple shop shuffle, change which shop gets the starting ore
                        if (archipelagoData.GetOption(SlotDataKeys.shopShuffle) == 0) break;
                        XElement eventValue = node.Elements("string").ToArray()[1];
                        if (!eventValue.Value.StartsWith("vendor_")) break;
                        switch (eventValue.Value)
                        {
                            case "vendor_misc_upgrade":
                                int shopUpgrade2Id = archipelagoData.GetShopLocation("Vitality Shop");
                                string shopUpgrade2 = APData.GetShopPrefabSuffixFromAPId(shopUpgrade2Id);
                                eventValue.Value = $"vendor_{shopUpgrade2}_upgrade";
                                break;
                            case "vendor_combo_upgrade":
                                int shopUpgradeId = archipelagoData.GetShopLocation("Combo Shop");
                                string shopUpgrade = APData.GetShopPrefabSuffixFromAPId(shopUpgradeId);
                                eventValue.Value = $"vendor_{shopUpgrade}_upgrade";
                                break;
                        }
                        break;
                    case "SpawnObject": //Replace what the node would normally spawn with an archipelago item if it's randomized
                        XElement spawnObjElement = node.Elements("string").ToArray()[1];
                        if (ArchipelagoManager.IsRandomized(spawnObjElement.Value))
                        {
                            Vector2 pos = NodeHelper.PosFromString(node.Element("vec2").Value);
                            string xmlName = CreateItemScriptNodes(levelFile, int.Parse(nodeId), pos, ref bombTrapCounter, out List<XElement> extraNodes, true, out int holoEffectNodeId);
                            if (xmlName == null)
                                continue;
                            scriptNodesRoot.Add(extraNodes);
                            spawnObjElement.Value = xmlName;
                            XElement[] ints = node.Elements("int").ToArray();
                            ints[1].Value = "1";
                            if (holoEffectNodeId != -1)
                                NodeHelper.AddConnectionNodes(node, new int[] { holoEffectNodeId });
                        }
                        if (archipelagoData.GetOption(SlotDataKeys.shopShuffle) > 0 && spawnObjElement.Value.StartsWith("doodads/special/vendor_speech_"))
                        {
                            string speechType = spawnObjElement.Value.Remove(0, "doodads/special/vendor_speech_".Length);
                            speechType = speechType.Remove(speechType.Length - 4);
                            int shuffleAPId;
                            if (speechType == "power")
                            {
                                shuffleAPId = archipelagoData.GetShopLocation($"{levelPrefix} Powerup Shop");
                            }
                            else
                            {
                                shuffleAPId = archipelagoData.GetShopLocation($"{levelPrefix} Combo Shop");
                            }
                            string spawnShopType = APData.GetShopPrefabSuffixFromAPId(shuffleAPId);
                            int spawnShopLevel = APData.GetShopLevelFromAPId(shuffleAPId);
                            if (speechType.StartsWith("level") && spawnShopType != "power")
                            {
                                spawnObjElement.Value = $"doodads/special/vendor_speech_level{spawnShopLevel}.xml";
                            }
                            else
                            {
                                spawnObjElement.Value = $"doodads/special/vendor_speech_{spawnShopType}.xml";
                            }
                            if (speechType == "power" && spawnShopType != "power")
                            {
                                Vector2 pos = NodeHelper.PosFromString(node.Element("vec2").Value);
                                int newId = doodadSpawnCounter++;
                                XElement spawnLevelSpeechNode = NodeHelper.CreateSpawnObjectNode(newId, $"doodads/special/vendor_speech_level{spawnShopLevel}.xml", pos);
                                scriptNodesRoot.Add(spawnLevelSpeechNode);
                                NodeHelper.AddConnectionNodes(node, new[] { newId });
                            }
                            if (speechType == "level2" && spawnShopType == "power")
                            {
                                node.Remove();
                            }
                        }
                        if (spawnObjElement.Value.StartsWith("actors/"))
                            actorSpawnObjectNodes.Add(node);
                        break;
                    case "PlayAttachedEffect":
                        if (effectNodePositions.ContainsKey(nodeId))
                        {
                            string xmlName = archipelagoData.GetItemXmlNameFromPos(levelFile, effectNodePositions[nodeId]);
                            if (xmlName == null)
                                continue;
                            if (xmlName.StartsWith(APData.apHoloPrefix))
                            {
                                string holoXmlName = xmlName;
                                xmlName = archipelagoData.GetItemXmlNameFromPos(levelFile, effectNodePositions[nodeId], true);
                                Vector2 pos = NodeHelper.PosFromString(node.Element("vec2").Value) + new Vector2(0, 2);
                                XElement parametersNode = node.Element("dictionary");
                                XElement objectsNode = parametersNode.Element("dictionary").Element("int-arr");
                                int[] attachObjects = MiscHelper.SplitIntsFromString(objectsNode.Value);
                                bool objectsDynamic = objectsNode.Attribute("name").Value == "dynamic";
                                int yOffset = int.Parse(parametersNode.Element("float").Value);
                                scriptNodesRoot.Add(NodeHelper.CreatePlayAttachedEffectNode(modNodeCounter++, pos, holoXmlName, true, attachObjects, objectsDynamic, 900, yOffset));
                                NodeHelper.AddConnectionNodes(node, new int[] { modNodeCounter - 1 });
                            }
                            node.Element("dictionary").Element("string").Value = xmlName + ":";
                        }
                        break;
                    case "DestroyObject":
                        if (nonHololinkedItemIds.Count == 0) break;
                        XElement paramsNode = node.Element("dictionary");
                        XElement[] paramsChildren = paramsNode.Elements().ToArray();
                        if (paramsChildren.Length == 0) break;
                        XElement linksNode = paramsChildren[0];
                        string[] ids = linksNode.Value.Split(' ');
                        foreach (string id in ids)
                        {
                            int curId = int.Parse(id);
                            if (!nonHololinkedItemIds.Contains(curId)) continue;
                            nonHololinkedItemIds.Remove(curId); //Might be a problem if two Destroy nodes point to the same item, but I think this is fine it makes the code faster
                            //Convert to HideObject
                            linksNode.Remove();
                            node.Element("string").Value = "HideObject";
                            paramsNode.Add(NodeHelper.CreateXNode("state", 1));
                            paramsNode.Add(NodeHelper.CreateDictionaryNode("element", new[] { linksNode }));
                        }
                        break;
                    case "ShopArea":
                        //Shop shuffle
                        if (archipelagoData.GetOption(SlotDataKeys.shopShuffle) == 0) break;
                        XElement shopParamsNode = node.Element("dictionary");
                        XElement categoriesNode = shopParamsNode.Element("string");
                        string[] cats = categoriesNode.Value.Split(' ');
                        string highestCat = cats[cats.Length - 1];
                        string shopType = highestCat == "power" ? highestCat : highestCat.Remove(highestCat.Length - 1);
                        string prefix = levelPrefix;
                        if (prefix == "Chambers 11" && (nodeId == "1019" || nodeId == "1052"))
                            prefix += " Secret";
                        int shuffledShopId = archipelagoData.GetShopLocation($"{prefix} {APData.GetShopTypeName(shopType)} Shop");
                        string shuffledType = APData.GetShortShopTypeFromAPId(shuffledShopId);
                        int shuffledLevel = APData.GetShopLevelFromAPId(shuffledShopId);
                        if (shuffledType == "power")
                        {
                            categoriesNode.Value = "power";
                        }
                        else
                        {
                            categoriesNode.Value = "";
                            for (int l = 1; l <= shuffledLevel; l++)
                            {
                                categoriesNode.Value += $"{shuffledType}{l} ";
                            }
                            categoriesNode.Value = categoriesNode.Value.Remove(categoriesNode.Value.Length - 1);
                        }
                        break;
                    case "LevelExitArea":
                        if (!exitRando) break;
                        XElement exitParamsNode = node.Element("dictionary");
                        XElement levelNode = exitParamsNode.Element("string");
                        XElement startIdNode = exitParamsNode.Element("int");
                        string levelCode = $"{levelNode.Value}|{startIdNode.Value}";
                        if (levelFile.EndsWith("bonus_5.xml") && levelCode == "hub|111") //Custom stuff for the one-way back to the hub
                        {
                            levelCode += "*";
                        }
                        if (!exitSwaps.TryGetValue(levelCode, out string newCode))
                        {
                            continue;
                        }
                        if (newCode.EndsWith("*"))
                        {
                            newCode = newCode.TrimEnd('*');
                        }
                        string[] levelSplits = newCode.Split('|');
                        levelNode.Value = levelSplits[0];
                        startIdNode.Value = levelSplits[1];
                        //Temple Krilith boss arena custom stuff
                        if (archipelagoData.mapType == ArchipelagoData.MapType.Temple && newCode == "boss_2|0")
                        {
                            Vector2 nodePos = NodeHelper.PosFromString(node.Element("vec2").Value);
                            int thisNodeId = int.Parse(nodeId);
                            int bossTestNodeId = modNodeCounter++;
                            int bossDiableId = modNodeCounter++;
                            int storageEnableId = modNodeCounter++;
                            int storageId = modNodeCounter++;
                            scriptNodesRoot.Add(NodeHelper.CreateCheckGlobalFlagNode(bossTestNodeId, nodePos - new Vector2(1.5f, 6), "boss_krilith_dead", new int[] { bossDiableId, storageEnableId }, null));
                            scriptNodesRoot.Add(NodeHelper.CreateToggleElementNode(bossDiableId, nodePos + new Vector2(0, -3), -1, 1, new int[] { thisNodeId }));
                            scriptNodesRoot.Add(NodeHelper.CreateToggleElementNode(storageEnableId, nodePos + new Vector2(3, -3), -1, 0, new int[] { storageId }));
                            XElement storageExitNode = NodeHelper.CreateLevelExitNode(storageId, false, nodePos + new Vector2(3, 0), "boss_2_special", 0, null);
                            XElement newNodeShapeDict = storageExitNode.Element("dictionary").Element("dictionary");
                            newNodeShapeDict.LastNode.Remove();
                            newNodeShapeDict.Add(new XElement(exitParamsNode.Element("dictionary").Element("int-arr")));
                            scriptNodesRoot.Add(storageExitNode);
                            globalScriptNodesToTriggerOnLoad.Add(bossTestNodeId);
                        }
                        break;
                }
            }
            //Enemy loot rando
            if (archipelagoData.GetOption(SlotDataKeys.randomizeEnemyLoot) > 0)
            {
                foreach (XElement node in actorSpawnObjectNodes)
                {
                    XElement spawnObjElement = node.Elements("string").ToArray()[1];
                    if (!spawnObjElement.Value.Contains("tower") && !spawnObjElement.Value.Contains("boss_worm")) continue; //We get items from minibosses and bosses during the game now

                    string nodeId = node.Element("int").Value;
                    Vector2 basePos = NodeHelper.PosFromString(node.Element("vec2").Value);
                    bool needsOffset = spawnObjElement.Value.StartsWith("actors/tower_nova") || spawnObjElement.Value.StartsWith("actors/tower_tracking");
                    if (needsOffset)
                        basePos += new Vector2(0, 1);
                    int scriptId = int.Parse(nodeId);
                    bool doubleLocations = spawnObjElement.Value.EndsWith("mb.xml");
                    List<Vector2> positions = new List<Vector2>(doubleLocations ? 2 : 1);
                    if (spawnObjElement.Value == "actors/boss_worm/boss_worm.xml")
                    {
                        doubleLocations = true;
                        positions.Add(new Vector2(-10.25f, -20.25f));
                        positions.Add(new Vector2(-9.75f, -19.75f));
                        positions.Add(new Vector2(-8.25f, -20.25f));
                        positions.Add(new Vector2(-7.75f, -19.75f));
                        positions.Add(new Vector2(-6.25f, -20.25f));
                        positions.Add(new Vector2(-5.75f, -19.75f));
                        positions.Add(new Vector2(-4.25f, -20.25f));
                        positions.Add(new Vector2(-3.75f, -19.75f));
                    }
                    else if (doubleLocations)
                    {
                        positions.Add(basePos - new Vector2(.25f, .25f));
                        positions.Add(basePos + new Vector2(.25f, .25f));
                    }
                    else
                    {
                        //The Dune Sharks key is now spawned dynamically
                        positions.Add(basePos);
                    }
                    List<int> scriptNodeIds = new List<int>(2);
                    for (int p = 0; p < positions.Count; p++)
                    {
                        int spawnItemNodeId = bombTrapCounter;
                        XElement[] spawnNodes = CreateSpawnItemScriptNodes(levelFile, ref bombTrapCounter, positions[p], false);
                        if (spawnNodes.Length == 0) continue;
                        scriptNodeIds.Add(spawnItemNodeId);
                        scriptNodesRoot.Add(spawnNodes);
                    }

                    if (scriptNodeIds.Count > 0 && spawnObjElement.Value != "actors/boss_worm/boss_worm.xml")
                    {
                        XElement onDestroyedNode = NodeHelper.CreateObjectEventTriggerNode(enemyLootNodeCounter++, basePos + new Vector2(0, -2), 1, "Destroyed", new[] { scriptId }, true, scriptNodeIds.ToArray());
                        scriptNodesRoot.Add(onDestroyedNode);
                    }
                }
            }

            //Buttonsanity
            int buttonsanity = archipelagoData.GetOption(SlotDataKeys.buttonsanity);
            if (buttonsanity > 0)
            {
                Dictionary<string, Dictionary<string, APData.ButtonData>> buttonNodeDict;
                int buttonEffectOffset;
                switch (archipelagoData.mapType)
                {
                    case ArchipelagoData.MapType.Castle:
                        buttonNodeDict = APData.castleButtonNodes;
                        buttonEffectOffset = APData.castleButtonItemStartID;
                        break;
                    case ArchipelagoData.MapType.Temple:
                        buttonNodeDict = APData.templeButtonNodes;
                        buttonEffectOffset = APData.templeButtonItemStartID;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                int buttonsanityCounter = 600000;
                if (buttonNodeDict.TryGetValue(levelFile, out var levelButtonNodes))
                {
                    foreach (string nodeId in levelButtonNodes.Keys)
                    {
                        var data = levelButtonNodes[nodeId];
                        XElement areaTriggerNode = idToNode[nodeId];

                        //Get item spawn position
                        Vector2 itemSpawnPos = new Vector2();
                        string[] shapeNodeIds = areaTriggerNode.Element("dictionary").Element("dictionary").Element("int-arr").Value.Split(' ');
                        for (int s = 0; s < shapeNodeIds.Length; s++)
                        {
                            itemSpawnPos += NodeHelper.PosFromString(idToNode[shapeNodeIds[s]].Element("vec2").Value);
                        }
                        itemSpawnPos /= shapeNodeIds.Length;
                        switch (data.buttonType)
                        {
                            case APData.ButtonType.Wall:
                                itemSpawnPos += new Vector2(0, 0.5f);
                                break;
                            case APData.ButtonType.Lever:
                                itemSpawnPos += new Vector2(0, 1f);
                                break;
                        }
                        if (data.posOverride != Vector2.Zero)
                        {
                            itemSpawnPos = data.posOverride;
                        }

                        //Get what ap item is found at this button location and create SpawnObject
                        NetworkItem item = archipelagoData.GetItemFromPos(itemSpawnPos, levelFile);
                        if (item.Item == -1)
                        {
                            continue;
                        }
                        int spawnObjNodeId = buttonsanityCounter;
                        XElement[] buttonSpawnNodes = CreateSpawnItemScriptNodes(levelFile, ref buttonsanityCounter, itemSpawnPos, true);
                        foreach (XElement extraNode in buttonSpawnNodes)
                        {
                            scriptNodesRoot.Add(extraNode);
                        }

                        //Remove nodes that pertain to button effects and add item spawn node
                        XElement[] connectionNodes = areaTriggerNode.Elements("int-arr").ToArray();
                        string[] connectionNodeIds = connectionNodes[0].Value.Split(' ');
                        string[] connectionDelaysIds = connectionNodes[1].Value.Split(' ');
                        string connectionNodeIdString = "";
                        string connectionNodeDelayString = "";
                        for (int c = 0; c < connectionNodeIds.Length; c++)
                        {
                            //Don't keep nodes that pertain to button effects
                            if (!data.buttonEffectNodes.Contains(connectionNodeIds[c]))
                            {
                                connectionNodeIdString += $"{connectionNodeIds[c]} ";
                                connectionNodeDelayString += $"{connectionDelaysIds[c]} ";
                            }
                        }

                        connectionNodeIdString += spawnObjNodeId.ToString();
                        connectionNodeDelayString += data.buttonType == APData.ButtonType.Panel ? "0" : "300"; //Delay for spawning an item, panels have no delay
                        connectionNodes[0].Value = connectionNodeIdString;
                        connectionNodes[1].Value = connectionNodeDelayString;

                        int[] buttonEffectNodeIdArray = new int[data.buttonEffectNodes.Count];
                        for (int b = 0; b < buttonEffectNodeIdArray.Length; b++)
                        {
                            buttonEffectNodeIdArray[b] = int.Parse(data.buttonEffectNodes[b]);
                            //Set the effect nodes to only trigger once
                            XElement[] intNodes = idToNode[data.buttonEffectNodes[b]].Elements("int").ToArray();
                            intNodes[1].Value = "1";
                        }

                        if (buttonEffectNodeIdArray.Length > 0)
                        {
                            //Create trigger and flag nodes
                            string buttonEffectName = ArchipelagoManager.GetItemName(buttonEffectOffset + data.itemId);
                            //If the button is part of a set, we don't create the trigger nodes for each one. We currently create them in EditLevel on a case-by-case basis
                            if (buttonEffectName == null || buttonEffectName.Contains("Progress"))
                                continue;
                            Vector2 basePos = NodeHelper.PosFromString(areaTriggerNode.Element("vec2").Value);
                            XElement[] locEventNodes = CreateButtonEffectTriggerNodes(ref buttonsanityCounter, basePos, buttonEffectName, ref buttonEffectNodes, buttonEffectNodeIdArray);
                            scriptNodesRoot.Add(locEventNodes[0]);
                            scriptNodesRoot.Add(locEventNodes[1]);
                        }

                        //Unhook nodes if we need to
                        if (data.unhookNode != null)
                        {
                            XElement[] unhookConnectionNodes = idToNode[data.unhookNode].Elements("int-arr").ToArray();
                            unhookConnectionNodes[0].Remove();
                            unhookConnectionNodes[1].Remove();
                        }
                    }
                }

                //Create refresh button events node
                if (buttonEffectNodes.Count > 0)
                    scriptNodesRoot.Add(NodeHelper.CreateGlobalEventTriggerNode(buttonsanityCounter++, -1, new Vector2(), ArchipelagoManager.BUTTON_EVENT_NAME, buttonEffectNodes.ToArray()));
            }

            //Breakable walls
            int hammerFragments = archipelagoData.GetOption(SlotDataKeys.hammerFragments);
            if (hammerFragments > 0)
            {
                if (APData.breakableWallNodes[archipelagoData.mapType].TryGetValue(levelFile, out List<string> breakableWallNodeIds))
                {
                    foreach (string wallDataNodeId in breakableWallNodeIds)
                    {
                        AddHammerNodesToBreakableWall(ref modNodeCounter, idToNode, wallDataNodeId, scriptNodesRoot);
                    }
                }
            }

            if (globalScriptNodesToTriggerOnLoad.Count > 0)
            {
                int[] connectNodeDelays = Enumerable.Repeat(5, globalScriptNodesToTriggerOnLoad.Count + 1).ToArray();
                XElement singleGlobalEventTrigger = NodeHelper.CreateGlobalEventTriggerNode(modNodeCounter++, -1, new Vector2(0, 10), "LevelLoaded", globalScriptNodesToTriggerOnLoad.ToArray(), connectNodeDelays);
                scriptNodesRoot.Add(singleGlobalEventTrigger);
            }
            if (globalScriptNodesToTriggerOnceOnLoad.Count > 0)
            {
                int[] connectNodeDelays = Enumerable.Repeat(5, globalScriptNodesToTriggerOnceOnLoad.Count + 1).ToArray();
                XElement singleGlobalEventTrigger = NodeHelper.CreateGlobalEventTriggerNode(modNodeCounter++, 1, new Vector2(0, -10), "LevelLoaded", globalScriptNodesToTriggerOnceOnLoad.ToArray(), connectNodeDelays);
                scriptNodesRoot.Add(singleGlobalEventTrigger);
            }
        }

        private static string CreateItemScriptNodes(string levelFile, int itemId, Vector2 pos, ref int extraNodeCounter, out List<XElement> scriptNodesToAdd, bool isScriptItem, out int holoEffectNodeId)
        {
            scriptNodesToAdd = new List<XElement>();
            if (itemId == extraNodeCounter) extraNodeCounter++;
            NetworkItem item = archipelagoData.GetGenItemFromPos(pos, levelFile);
            string xmlName = item.Item == -1 ? null : ArchipelagoManager.GetItemXmlName(item);
            holoEffectNodeId = -1;
            switch (xmlName)
            {
                case null:
                    return null;
                case APData.bombTrapXmlName:
                    XElement[] bombTrapNodes = NodeHelper.CreateDestroySpawnNodes(extraNodeCounter, "prefabs/bomb_roof_trap.xml", pos, itemId, isScriptItem);
                    scriptNodesToAdd.AddRange(bombTrapNodes);
                    extraNodeCounter += 2;
                    break;
                case APData.SPEECH_TRAP_XML_NAME:
                    XElement[] speechTrapNodes = NodeHelper.CreateDestroyTriggerNodes(extraNodeCounter, APData.SPEECH_TRAP_EVENT_NAME, pos, itemId, isScriptItem);
                    scriptNodesToAdd.AddRange(speechTrapNodes);
                    extraNodeCounter += 2;
                    break;
                default:
                    if (!APData.IsItemXmlNameCorporeal(xmlName))
                    {
                        string buttonName = ArchipelagoManager.GetItemName(item);
                        XElement[] buttonTriggerNodes = new XElement[3];
                        buttonTriggerNodes[0] = NodeHelper.CreateObjectEventTriggerNode(extraNodeCounter++, pos + new Vector2(-2, -2), -1, "Destroyed", new int[] { itemId }, isScriptItem, new int[] { extraNodeCounter });
                        buttonTriggerNodes[1] = NodeHelper.CreateSetGlobalFlagNode(extraNodeCounter++, pos + new Vector2(0, -2), -1, buttonName, true);
                        NodeHelper.AddConnectionNodes(buttonTriggerNodes[1], new int[] { extraNodeCounter });
                        buttonTriggerNodes[2] = NodeHelper.CreateGlobalEventTriggerNode(extraNodeCounter++, -1, pos + new Vector2(2, -2), ArchipelagoManager.BUTTON_EVENT_NAME, null);
                        scriptNodesToAdd.AddRange(buttonTriggerNodes);
                    }
                    break;
            }
            //If this is someone else's item create nodes as a holo effect
            if (xmlName.StartsWith(APData.apHoloPrefix))
            {
                holoEffectNodeId = extraNodeCounter++;
                if (!isScriptItem)
                    globalScriptNodesToTriggerOnceOnLoad.Add(holoEffectNodeId);
                string holoXmlName = ArchipelagoManager.GetItemXmlName(item, false, true);
                scriptNodesToAdd.Add(NodeHelper.CreatePlayAttachedEffectNode(holoEffectNodeId, pos + new Vector2(0, 2), holoXmlName, true, new int[] { itemId }, isScriptItem));
            }
            return xmlName;
        }
        private static XElement[] CreateSpawnItemScriptNodes(string levelFile, ref int counterId, Vector2 pos, bool isScriptEffect)
        {
            return CreateSpawnItemScriptNodes(levelFile, counterId++, ref counterId, pos, isScriptEffect);
        }
        private static XElement[] CreateSpawnItemScriptNodes(string levelFile, int spawnItemId, ref int counterId, Vector2 pos, bool isScriptEffect)
        {
            string spawnItemXmlName = CreateItemScriptNodes(levelFile, spawnItemId, pos, ref counterId, out List<XElement> scriptNodes, true, out int holoEffectNodeId);
            if (spawnItemXmlName != null)
            {
                //Override if this is a button item to spawn as a check
                if (!APData.IsItemXmlNameCorporeal(spawnItemXmlName) && isScriptEffect)
                    spawnItemXmlName = APData.checkItemXmlName;
                scriptNodes.Add(NodeHelper.CreateSpawnObjectNode(spawnItemId, spawnItemXmlName, pos));
                if (holoEffectNodeId != -1)
                    NodeHelper.AddConnectionNodes(scriptNodes[scriptNodes.Count - 1], new int[] { holoEffectNodeId });
            }
            return scriptNodes.ToArray();
        }

        private static XElement[] CreateButtonEffectTriggerNodes(ref int idCounter, Vector2 pos, string eventTrigger, ref List<int> buttonEffectIds, int[] connections, int[] connectionDelays = null)
        {
            XElement[] nodes = new XElement[2];
            buttonEffectIds.Add(idCounter);
            globalScriptNodesToTriggerOnLoad.Add(idCounter);
            nodes[1] = NodeHelper.CreateCheckGlobalFlagNode(idCounter++, pos + new Vector2(-1, 0), eventTrigger, false, new int[] { idCounter }, false, null);
            nodes[0] = NodeHelper.CreateScriptLinkNode(idCounter++, true, 1, pos + new Vector2(1, 0), connections, connectionDelays);
            return nodes;
        }

        //doodadIds array is ordered { Rune A, Rune B, Rune C, Rune D }
        private static List<XElement> CreateRandomizedRunePuzzle(string levelFile, XElement doodadsNodeRoot, ref List<int> buttonEventNodes, Vector2 puzzlePos, ref int idCounter,
            int itemEffectAPId, int[] doodadIds, int winEffectId, int failSoundNodeId, int winEffectDelay)
        {
            int buttonsanity = archipelagoData.GetOption(SlotDataKeys.buttonsanity);
            Vector2[] buttonPositions = new Vector2[4];
            foreach (XElement doodad in doodadsNodeRoot.Elements())
            {
                int doodadId = int.Parse(doodad.Element("int").Value);
                for (int d = 0; d < doodadIds.Length; d++)
                {
                    if (doodadId != doodadIds[d]) continue;
                    buttonPositions[d] = NodeHelper.PosFromString(doodad.Element("vec2").Value) + new Vector2(0.5f, 0.5f);
                    break;
                }
            }

            List<XElement> scriptNodes = new List<XElement>();
            Vector2 basePos = puzzlePos + new Vector2(0, -100);
            List<int> numbers = new List<int>() { 0, 1, 2, 3 };
            List<int> combination = new List<int>();
            string combinationString = "";
            for (int c = 0; c < 4; c++)
            {
                int index = random.Next(numbers.Count);
                combination.Add(numbers[index]);
                combinationString += numbers[index];
                numbers.RemoveAt(index);
            }
            int resetSequenceNodeId = idCounter++;
            int buttonNodeId = resetSequenceNodeId + 25;
            int completeId = resetSequenceNodeId + 50;
            int failNodeId = resetSequenceNodeId + 60;
            int buttonSound = resetSequenceNodeId + 65;
            int buttonSpawnId = resetSequenceNodeId + 70;
            idCounter = resetSequenceNodeId + 75;
            //Sequence nodes
            int[] advance = new int[] { resetSequenceNodeId + 5, resetSequenceNodeId + 10, resetSequenceNodeId + 15, resetSequenceNodeId + 20 };
            int[] fail = new int[] { resetSequenceNodeId + 6, resetSequenceNodeId + 11, resetSequenceNodeId + 16, resetSequenceNodeId + 21, failNodeId }; //Technically fail 0 is useless but it makes the code cleaner
            int[] button = new int[] { buttonNodeId, buttonNodeId + 5, buttonNodeId + 10, buttonNodeId + 15 };
            int[] buttonSpawn = new int[] { buttonSpawnId + 1, buttonSpawnId + 2, buttonSpawnId + 3, buttonSpawnId + 4 };
            XElement resetSequenceNode = NodeHelper.CreateScriptLinkNode(resetSequenceNodeId, true, -1, basePos, new int[] { resetSequenceNodeId + 1, resetSequenceNodeId + 2, resetSequenceNodeId + 3 });
            XElement resetSequenceEnableNode = NodeHelper.CreateToggleElementNode(resetSequenceNodeId + 1, basePos + new Vector2(2, 0), -1, 0,
                new int[] { advance[0], fail[1], fail[2], fail[3], button[0], button[1], button[2], button[3] });
            XElement resetSequenceDisableNode = NodeHelper.CreateToggleElementNode(resetSequenceNodeId + 2, basePos + new Vector2(2, 1.5f), -1, 1, new int[] { fail[0], advance[1], advance[2], advance[3] });
            XElement resetDoodadNode = NodeHelper.CreateChangeDoodadStateNode(resetSequenceNodeId + 3, basePos + new Vector2(0, -10f), -1, "deactivate", doodadIds);
            scriptNodes.Add(resetSequenceNode);
            scriptNodes.Add(resetSequenceEnableNode);
            scriptNodes.Add(resetSequenceDisableNode);
            scriptNodes.Add(resetDoodadNode);
            for (int i = 0; i < 4; i++)
            {
                Vector2 seqBasePos = basePos + new Vector2(4 + 3.5f * i, 0);
                List<int> advanceConnections = new List<int>() { advance[i] + 2, fail[i] + 2 };
                if (i == 3)
                {
                    advanceConnections.Add(completeId);
                }
                if (buttonsanity == 3)
                {
                    advanceConnections.Add(buttonSpawn[combination.IndexOf(i)]);
                }
                XElement advanceNode = NodeHelper.CreateScriptLinkNode(advance[i], i == 0, -1, seqBasePos, advanceConnections.ToArray());
                XElement failNode = NodeHelper.CreateScriptLinkNode(fail[i], i != 0, -1, seqBasePos + new Vector2(1.5f, 0), new int[] { failNodeId });
                if (i < 3)
                {
                    XElement nextAdvanceEnableNode = NodeHelper.CreateToggleElementNode(advance[i] + 2, seqBasePos + new Vector2(-0.5f, 1.5f), -1, 0, new int[] { advance[i + 1] });
                    scriptNodes.Add(nextAdvanceEnableNode);
                }
                XElement curSeqDisableNode = NodeHelper.CreateToggleElementNode(fail[i] + 2, seqBasePos + new Vector2(0.5f, 1.5f), -1, 1, new int[] { advance[i], fail[i] });
                scriptNodes.Add(advanceNode);
                scriptNodes.Add(failNode);
                scriptNodes.Add(curSeqDisableNode);
            }
            //Button nodes
            for (int b = 0; b < 4; b++)
            {
                int xMod = b % 2;
                int yMod = b / 2;
                Vector2 buttonBasePos = basePos + new Vector2(5 * (2 - xMod), -10f + 4 * yMod);
                XElement buttonAreaTriggerNode = NodeHelper.CreateAreaTriggerNode(button[b], -1, buttonBasePos, 0, 1, new[] { button[b] + 1 },
                    new[] { buttonSound, button[b] + 2, button[b] + 3, advance[combination[b]], fail[combination[b]] });
                XElement rectangleShapeNode = NodeHelper.CreateRectangleShapeNode(button[b] + 1, buttonPositions[b], 1, 1, 15);
                if (b == 3)
                {
                    //Complete switch sequence item
                    if (buttonsanity > 0 && buttonsanity != 3)
                    {
                        Vector2 spawnPos = puzzlePos;
                        XElement[] completeSpawnNodes = CreateSpawnItemScriptNodes(levelFile, buttonSpawnId, ref idCounter, spawnPos, true);
                        NodeHelper.AddConnectionNodes(completeSpawnNodes[completeSpawnNodes.Length - 1], new[] { idCounter, idCounter + 1 });
                        scriptNodes.AddRange(completeSpawnNodes);
                        if (APData.IsItemXmlNameCorporeal(archipelagoData.GetItemXmlNameFromPos(levelFile, spawnPos)))
                        {
                            scriptNodes.Add(NodeHelper.CreatePlayEffectNode(idCounter++, spawnPos, "effects/particles.xml:flash", 100));
                            scriptNodes.Add(NodeHelper.CreatePlaySoundNode(idCounter++, spawnPos, "sound/misc.xml:spawn_tele", false, true, 10f));
                        }
                        else
                        {
                            idCounter += 2;
                        }
                    }
                }
                //Individual switch items
                if (buttonsanity == 3)
                {
                    scriptNodes.AddRange(CreateSpawnItemScriptNodes(levelFile, buttonSpawn[b], ref idCounter, buttonPositions[b], true));
                }
                XElement buttonLockNode = NodeHelper.CreateToggleElementNode(button[b] + 2, buttonBasePos + new Vector2(1.5f, -1), -1, 1, new[] { button[b] });
                XElement buttonStateNode = NodeHelper.CreateChangeDoodadStateNode(button[b] + 3, buttonBasePos + new Vector2(1.5f, 1), -1, "activate", new[] { doodadIds[b] });
                scriptNodes.Add(buttonAreaTriggerNode);
                scriptNodes.Add(rectangleShapeNode);
                scriptNodes.Add(buttonLockNode);
                scriptNodes.Add(buttonStateNode);
            }
            //Sound nodes
            XElement buttonSoundNode = NodeHelper.CreatePlaySoundNode(buttonSound, puzzlePos, "sound/misc.xml:button_magic", false, true, 5);
            scriptNodes.Add(buttonSoundNode);

            //Fail nodes
            XElement failNodeRoot = NodeHelper.CreateScriptLinkNode(failNodeId, true, -1, basePos + new Vector2(15, -15), new[] { failNodeId + 1, resetSequenceNodeId, failSoundNodeId, failNodeId + 4 }, new int[] { 0, 4000, 500, 750 });
            XElement failButtonLockNode = NodeHelper.CreateToggleElementNode(failNodeId + 1, basePos + new Vector2(7.5f, -15), -1, 1, button);
            scriptNodes.Add(failNodeRoot);
            scriptNodes.Add(failButtonLockNode);

            //Complete nodes
            Vector2 winBasePos = basePos + new Vector2(20, -5);
            List<int> winRootConnections = new List<int>() { buttonSpawnId, completeId + 2, completeId + 3 };
            List<int> winRootDelays = new List<int>() { 300, 0, 0 };
            if (buttonsanity > 0)
            {
                string itemEffectName = ArchipelagoManager.GetItemName(itemEffectAPId + APData.castleButtonItemStartID);
                int completeEffectId = completeId + 5;
                scriptNodes.AddRange(CreateButtonEffectTriggerNodes(ref completeEffectId, winBasePos + new Vector2(2, 0), itemEffectName, ref buttonEventNodes, new[] { winEffectId }));
            }
            else
            {
                winRootConnections.Add(winEffectId);
                winRootDelays.Add(winEffectDelay);
            }
            XElement winNodeRoot = NodeHelper.CreateScriptLinkNode(completeId, true, 1, winBasePos + new Vector2(-3, 0), winRootConnections.ToArray(), winRootDelays.ToArray());
            XElement announceNode = NodeHelper.CreateAnnounceTextNode(completeId + 2, winBasePos + new Vector2(4, -3), "ig.seq-compl", 2500, 0, true, -1);
            XElement winSoundNode = NodeHelper.CreatePlaySoundNode(completeId + 3, winBasePos + new Vector2(4, -1.5f), "sound/misc.xml:info_seqcomplete", false, false, 0);

            scriptNodes.Add(winNodeRoot);
            scriptNodes.Add(announceNode);
            scriptNodes.Add(winSoundNode);

            return scriptNodes;
        }
        private static List<XElement> CreateRandomizedPoFPuzzle(string levelFile, Vector2 puzzlePos, ref int idCounter, out XElement[] doodads)
        {
            int buttonsanity = archipelagoData.GetOption(SlotDataKeys.buttonsanity);
            int centerDoodad = 750000;
            int[] outerButtonIds = new int[4];
            doodads = new XElement[5];
            doodads[0] = NodeHelper.CreateDoodadNode(centerDoodad, "doodads/generic/deco_elevated_pyramid.xml", puzzlePos, true);
            for (int d = 0; d < 4; d++)
            {
                int xMod = d % 2;
                int yMod = d / 2;
                outerButtonIds[d] = centerDoodad + 1 + d;
                doodads[1 + d] = NodeHelper.CreateDoodadNode(outerButtonIds[d], $"doodads/special/trigger_button_rune_{(char)('a' + d)}.xml", puzzlePos + new Vector2(1 + -3 * xMod, -2 + 3 * yMod), true);
            }

            List<XElement> scriptNodes = new List<XElement>();
            Vector2 basePos = puzzlePos + new Vector2(0, -100);
            Vector2 puzzleButtonPos = puzzlePos + new Vector2(-1.5f, -1.5f);
            List<int> numbers = new List<int>() { 0, 1, 2, 3 };
            List<int> combination = new List<int>();
            string combinationString = "";
            for (int c = 0; c < 4; c++)
            {
                int index = random.Next(numbers.Count);
                combination.Add(numbers[index]);
                combinationString += numbers[index];
                numbers.RemoveAt(index);
            }
            int resetSequenceNodeId = idCounter++;
            int buttonNodeId = resetSequenceNodeId + 25;
            int completeId = resetSequenceNodeId + 50;
            int failNodeId = resetSequenceNodeId + 60;
            int buttonSound = resetSequenceNodeId + 65;
            int physicsResetId = resetSequenceNodeId + 67;
            int buttonSpawnId = resetSequenceNodeId + 70;
            idCounter = resetSequenceNodeId + 75;
            //Sequence nodes
            int[] advance = new int[] { resetSequenceNodeId + 5, resetSequenceNodeId + 10, resetSequenceNodeId + 15, resetSequenceNodeId + 20 };
            int[] fail = new int[] { resetSequenceNodeId + 6, resetSequenceNodeId + 11, resetSequenceNodeId + 16, resetSequenceNodeId + 21, failNodeId }; //Technically fail 0 is useless but it makes the code cleaner
            int[] button = new int[] { buttonNodeId, buttonNodeId + 5, buttonNodeId + 10, buttonNodeId + 15 };
            int[] buttonSpawn = new int[] { buttonSpawnId + 1, buttonSpawnId + 2, buttonSpawnId + 3, buttonSpawnId + 4 };
            XElement resetSequenceNode = NodeHelper.CreateScriptLinkNode(resetSequenceNodeId, true, -1, basePos, new int[] { resetSequenceNodeId + 1, resetSequenceNodeId + 2, resetSequenceNodeId + 3 });
            XElement resetSequenceEnableNode = NodeHelper.CreateToggleElementNode(resetSequenceNodeId + 1, basePos + new Vector2(2, 0), -1, 0,
                new int[] { advance[0], fail[1], fail[2], fail[3], button[0], button[1], button[2], button[3] });
            XElement resetSequenceDisableNode = NodeHelper.CreateToggleElementNode(resetSequenceNodeId + 2, basePos + new Vector2(2, 1.5f), -1, 1, new int[] { fail[0], advance[1], advance[2], advance[3] });
            XElement resetDoodadNode = NodeHelper.CreateChangeDoodadStateNode(resetSequenceNodeId + 3, basePos + new Vector2(0, -10f), -1, "deactivate", outerButtonIds);
            XElement removePhysicsNode = NodeHelper.CreateTogglePhysicsNode(physicsResetId, true, 1, basePos + new Vector2(-3, 0), 1, new[] { centerDoodad });
            globalScriptNodesToTriggerOnceOnLoad.Add(physicsResetId);
            scriptNodes.Add(resetSequenceNode);
            scriptNodes.Add(resetSequenceEnableNode);
            scriptNodes.Add(resetSequenceDisableNode);
            scriptNodes.Add(resetDoodadNode);
            scriptNodes.Add(removePhysicsNode);
            for (int i = 0; i < 4; i++)
            {
                Vector2 seqBasePos = basePos + new Vector2(4 + 3.5f * i, 0);
                List<int> advanceConnections = new List<int>() { advance[i] + 2, fail[i] + 2 };
                if (i == 3)
                {
                    advanceConnections.Add(completeId);
                }
                if (buttonsanity == 3)
                {
                    advanceConnections.Add(buttonSpawn[combination.IndexOf(i)]);
                }
                XElement advanceNode = NodeHelper.CreateScriptLinkNode(advance[i], i == 0, -1, seqBasePos, advanceConnections.ToArray());
                XElement failNode = NodeHelper.CreateScriptLinkNode(fail[i], i != 0, -1, seqBasePos + new Vector2(1.5f, 0), new int[] { failNodeId });
                if (i < 3)
                {
                    XElement nextAdvanceEnableNode = NodeHelper.CreateToggleElementNode(advance[i] + 2, seqBasePos + new Vector2(-0.5f, 1.5f), -1, 0, new int[] { advance[i + 1] });
                    scriptNodes.Add(nextAdvanceEnableNode);
                }
                XElement curSeqDisableNode = NodeHelper.CreateToggleElementNode(fail[i] + 2, seqBasePos + new Vector2(0.5f, 1.5f), -1, 1, new int[] { advance[i], fail[i] });
                scriptNodes.Add(advanceNode);
                scriptNodes.Add(failNode);
                scriptNodes.Add(curSeqDisableNode);
            }
            //Button nodes
            for (int b = 0; b < 4; b++)
            {
                int xMod = b % 2;
                int yMod = b / 2;
                Vector2 buttonBasePos = basePos + new Vector2(5 * (2 - xMod), -10f + 4 * yMod);
                Vector2 buttonSwitchPos = puzzleButtonPos + new Vector2(3 * (1 - xMod), 3 * yMod);
                XElement buttonAreaTriggerNode = NodeHelper.CreateAreaTriggerNode(button[b], -1, buttonBasePos, 0, 1, new[] { button[b] + 1 },
                    new[] { buttonSound, button[b] + 2, button[b] + 3, advance[combination[b]], fail[combination[b]] });
                XElement rectangleShapeNode = NodeHelper.CreateRectangleShapeNode(button[b] + 1, buttonSwitchPos, 1, 1, 15);
                if (b == 3)
                {
                    //Complete switch sequence item
                    if (buttonsanity > 0 && buttonsanity != 3)
                    {
                        Vector2 spawnPos = puzzlePos + new Vector2(0, 1);
                        XElement[] completeSpawnNodes = CreateSpawnItemScriptNodes(levelFile, buttonSpawnId, ref idCounter, spawnPos, true);
                        NodeHelper.AddConnectionNodes(completeSpawnNodes[completeSpawnNodes.Length - 1], new[] { idCounter, idCounter + 1 });
                        scriptNodes.AddRange(completeSpawnNodes);
                        if (APData.IsItemXmlNameCorporeal(archipelagoData.GetItemXmlNameFromPos(levelFile, spawnPos)))
                        {
                            scriptNodes.Add(NodeHelper.CreatePlayEffectNode(idCounter++, spawnPos, "effects/particles.xml:flash", 100));
                            scriptNodes.Add(NodeHelper.CreatePlaySoundNode(idCounter++, spawnPos, "sound/misc.xml:spawn_tele", false, true, 10f));
                        }
                        else
                        {
                            idCounter += 2;
                        }
                    }
                }
                //Individual switch items
                if (buttonsanity == 3)
                {
                    scriptNodes.AddRange(CreateSpawnItemScriptNodes(levelFile, buttonSpawn[b], ref idCounter, buttonSwitchPos, true));
                }
                XElement buttonLockNode = NodeHelper.CreateToggleElementNode(button[b] + 2, buttonBasePos + new Vector2(1.5f, -1), -1, 1, new[] { button[b] });
                XElement buttonStateNode = NodeHelper.CreateChangeDoodadStateNode(button[b] + 3, buttonBasePos + new Vector2(1.5f, 1), -1, "activate", new[] { outerButtonIds[b] });
                scriptNodes.Add(buttonAreaTriggerNode);
                scriptNodes.Add(rectangleShapeNode);
                scriptNodes.Add(buttonLockNode);
                scriptNodes.Add(buttonStateNode);
            }
            //Sound nodes
            XElement buttonSoundNode = NodeHelper.CreatePlaySoundNode(buttonSound, puzzlePos, "sound/misc.xml:button_magic", false, true, 5);
            XElement elevateNode = NodeHelper.CreatePlaySoundNode(buttonSound + 1, puzzlePos, "sound/misc.xml:button_hatch_2", false, true, 5);
            scriptNodes.Add(buttonSoundNode);
            scriptNodes.Add(elevateNode);

            //Fail nodes
            XElement failNodeRoot = NodeHelper.CreateScriptLinkNode(failNodeId, true, -1, basePos + new Vector2(15, -15), new[] { failNodeId + 1, resetSequenceNodeId, failNodeId + 3, failNodeId + 4 }, new int[] { 0, 4000, 500, 750 });
            XElement failButtonLockNode = NodeHelper.CreateToggleElementNode(failNodeId + 1, basePos + new Vector2(7.5f, -15), -1, 1, button);
            XElement failSoundNode = NodeHelper.CreatePlaySoundNode(failNodeId + 3, puzzlePos, "sound/misc.xml:info_puzzle_fail", false, true, 5);
            XElement failEventNode = NodeHelper.CreateGlobalEventTriggerNode(failNodeId + 4, -1, puzzlePos + new Vector2(0, -5), "puzzle_bonus_fail", null);
            scriptNodes.Add(failNodeRoot);
            scriptNodes.Add(failButtonLockNode);
            scriptNodes.Add(failSoundNode);
            scriptNodes.Add(failEventNode);

            //Complete nodes
            Vector2 winBasePos = basePos + new Vector2(20, -5);
            List<int> winRootConnections = new List<int>() { buttonSpawnId };
            List<int> winRootDelays = new List<int>() { 1100 };
            winRootConnections.Add(completeId + 1);
            winRootDelays.Add(0);
            XElement winNodeRoot = NodeHelper.CreateScriptLinkNode(completeId, true, 1, winBasePos + new Vector2(-3, 0), winRootConnections.ToArray(), winRootDelays.ToArray());
            //Used as a convenient unhooking place for buttonsanity
            List<int> completeEffectNodeIds = new List<int> { buttonSound + 1, completeId + 5, completeId + 6, completeId + 7 };
            XElement winNodeRoot2 = NodeHelper.CreateScriptLinkNode(completeId + 1, true, -1, winBasePos + new Vector2(2, 0), new[] { completeId + 2, completeId + 3, completeId + 4 }, new[] { 0, 0, 1100 });
            XElement announceNode = NodeHelper.CreateAnnounceTextNode(completeId + 2, winBasePos + new Vector2(4, -3), "ig.seq-compl", 2500, 0, true, -1);
            XElement winSoundNode = NodeHelper.CreatePlaySoundNode(completeId + 3, winBasePos + new Vector2(4, -1.5f), "sound/misc.xml:info_seqcomplete", false, false, 0);
            XElement raisePyramidNode = NodeHelper.CreateChangeDoodadStateNode(completeId + 4, winBasePos + new Vector2(4, 0), -1, "activate", new[] { centerDoodad });
            NodeHelper.AddConnectionNodes(raisePyramidNode, completeEffectNodeIds.ToArray());
            XElement pyramidPhysicsNode = NodeHelper.CreateTogglePhysicsNode(completeId + 5, true, -1, winBasePos + new Vector2(6, -1.5f), 0, new[] { centerDoodad });
            XElement winEventNode = NodeHelper.CreateGlobalEventTriggerNode(completeId + 6, -1, winBasePos + new Vector2(6, 0), "puzzle_bonus_solved", null);
            //The announce text is different if we're playing buttonsanity as we might not actually be elevating the pyramid
            bool elevatePyramid = true;
            if (buttonsanity > 0 && buttonsanity < 3)
            {
                string itemName = archipelagoData.GetItemNameFromPos(levelFile, puzzlePos + new Vector2(0, 1));
                if (itemName != null && !itemName.Contains("Elevate PoF Pyramid"))
                {
                    elevatePyramid = false;
                }
            }
            if (elevatePyramid)
            {
                XElement pyramidAnnounceNode = NodeHelper.CreateAnnounceTextNode(completeId + 7, winBasePos + new Vector2(6, 1.5f), "d.event.pyramid.restored", 2500, 0, true, -1);
                scriptNodes.Add(winSoundNode);
                scriptNodes.Add(pyramidAnnounceNode);
            }

            scriptNodes.Add(winNodeRoot);
            scriptNodes.Add(winNodeRoot2);
            scriptNodes.Add(announceNode);
            scriptNodes.Add(raisePyramidNode);
            scriptNodes.Add(pyramidPhysicsNode);
            scriptNodes.Add(winEventNode);

            return scriptNodes;
        }
        private static List<XElement> CreateRandomizedPegPuzzle(string levelFile, Vector2 puzzlePos, ref int idCounter, string puzzleCode, out List<XElement> doodads,
            ref List<int> buttonEventNodes)
        {
            List<XElement> scriptNodes = new List<XElement>();
            doodads = new List<XElement>(29);

            List<int> numbers = new List<int>(25);
            int pegs = archipelagoData.GetRandomLocation(puzzleCode);
            List<int> popupNums = new List<int>(pegs);
            for (int n = 0; n < 25; n++)
            {
                numbers.Add(n);
            }
            for (int p = 0; p < pegs; p++)
            {
                int pegIndex = random.Next(numbers.Count);
                popupNums.Add(numbers[pegIndex]);
                numbers.RemoveAt(pegIndex);
            }

            int doodadStartId = idCounter + 50000;
            int doodadCounter = doodadStartId;
            doodads.Add(NodeHelper.CreateDoodadNode(doodadCounter++, "doodads/special/trigger_button_floor.xml", puzzlePos - new Vector2(0.5f, 0.5f), true));
            doodads.Add(NodeHelper.CreateDoodadNode(doodadCounter++, "doodads/generic/marker_spawn.xml", puzzlePos + new Vector2(5f, 0.5f), false));
            doodads.Add(NodeHelper.CreateDoodadNode(doodadCounter++, "doodads/generic/marker_spawn.xml", puzzlePos + new Vector2(0.5f, 5f), false));
            doodads.Add(NodeHelper.CreateDoodadNode(doodadCounter++, "doodads/generic/marker_spawn.xml", puzzlePos + new Vector2(-4f, 0.5f), false));
            doodads.Add(NodeHelper.CreateDoodadNode(doodadCounter++, "doodads/generic/marker_spawn.xml", puzzlePos + new Vector2(0.5f, -4f), false));

            int pegSoundNodeId = idCounter++;
            scriptNodes.Add(NodeHelper.CreatePlaySoundNode(pegSoundNodeId, puzzlePos, "sound/misc.xml:button_metal", false, true, 10));

            List<int> pegIds = new List<int>(24);
            List<int> pegSpawnIds = new List<int>(24);
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    if (x == 2 && y == 2) continue;
                    int pegId = 5 + 5 * y + x;
                    Vector2 pegPos = puzzlePos + new Vector2(-3 + 1.5f * x, -3 + 1.5f * y);
                    pegIds.Add(doodadCounter);
                    doodads.Add(NodeHelper.CreateDoodadNode(doodadCounter++, "doodads/special/puzzle_ball_default.xml", pegPos, true));
                    string pegType = popupNums.Contains(pegId - 5) ? "doodads/special/puzzle_ball_right.xml" : "doodads/special/puzzle_ball_wrong.xml";
                    pegSpawnIds.Add(idCounter);
                    scriptNodes.Add(NodeHelper.CreateSpawnObjectNode(idCounter++, pegType, pegPos));
                }
            }

            scriptNodes.Add(NodeHelper.CreateAreaTriggerNode(idCounter++, 1, puzzlePos + new Vector2(-2.5f, 6.5f), 0, 1, new[] { idCounter }, new[] { idCounter + 1, idCounter + 2, idCounter + 3 }, new[] { 0, 0, 300 }));
            scriptNodes.Add(NodeHelper.CreateRectangleShapeNode(idCounter++, puzzlePos, 1, 1, 15));
            scriptNodes.Add(NodeHelper.CreateChangeDoodadStateNode(idCounter++, puzzlePos + new Vector2(0, 6.5f), 1, "activate", new[] { doodadStartId }));
            scriptNodes.Add(NodeHelper.CreatePlaySoundNode(idCounter++, puzzlePos, "sound/misc.xml:button_hatch", false, true, 5));
            if (archipelagoData.GetOption(SlotDataKeys.buttonsanity) > 0)
            {
                scriptNodes.AddRange(CreateSpawnItemScriptNodes(levelFile, ref idCounter, puzzlePos, true));
                string puzzleItemName = APData.GetPuzzleItemNameFromPuzzleCode(puzzleCode, archipelagoData);
                scriptNodes.AddRange(CreateButtonEffectTriggerNodes(ref idCounter, puzzlePos + new Vector2(0, 5), puzzleItemName, ref buttonEventNodes, new[] { idCounter + 2 }));
            }
            scriptNodes.Add(NodeHelper.CreateScriptLinkNode(idCounter++, true, 1, puzzlePos + new Vector2(8, 0), new[] { idCounter, idCounter + 1, pegSoundNodeId }));
            scriptNodes.Add(NodeHelper.CreateDestroyObjectNode(idCounter++, puzzlePos + new Vector2(10, 0), 1, pegIds.ToArray()));
            NodeHelper.AddConnectionNodes(scriptNodes[scriptNodes.Count - 1], pegSpawnIds.ToArray(), new int[24]);
            XElement itemLinkNode = NodeHelper.CreateScriptLinkNode(idCounter++, true, -1, puzzlePos + new Vector2(8, 2));
            scriptNodes.Add(itemLinkNode);

            List<Vector2> prizePositions = new List<Vector2>();
            if (pegs >= 1)
            {
                prizePositions.Add(puzzlePos + new Vector2(0, 4.5f));
            }
            if (pegs >= 10)
            {
                prizePositions.Add(puzzlePos + new Vector2(-4.5f, 0));
            }
            if (pegs >= 14)
            {
                prizePositions.Add(puzzlePos + new Vector2(4.5f, 0));
            }
            if (pegs >= 18)
            {
                prizePositions.Add(puzzlePos + new Vector2(0, -4.5f));
            }

            List<int> puzzleNodeIds = new List<int>(prizePositions.Count);
            for (int p = 0; p < prizePositions.Count; p++)
            {
                Vector2 prizePos = prizePositions[p];
                puzzleNodeIds.Add(idCounter);
                scriptNodes.Add(NodeHelper.CreatePlaySoundNode(idCounter++, prizePos, "sound/misc.xml:spawn_tele", false, true, 5));
                puzzleNodeIds.Add(idCounter);
                scriptNodes.Add(NodeHelper.CreatePlayEffectNode(idCounter++, prizePos, "effects/particles.xml:flash"));
                int itemSpawnId = idCounter;
                string xmlName = CreateItemScriptNodes(levelFile, itemSpawnId, prizePos, ref idCounter, out List<XElement> extraNodes, true, out int holoEffectNodeId);
                if (xmlName == null) //The item is not randomized so it stays the same
                {
                    switch (p)
                    {
                        case 0:
                            xmlName = "items/powerup_potion2.xml";
                            break;
                        case 1:
                            xmlName = "items/powerup_1up.xml";
                            break;
                        case 2:
                            xmlName = "prefabs/upgrade_random.xml";
                            break;
                        case 3:
                            xmlName = "items/chest_purple.xml";
                            break;
                    }
                }
                puzzleNodeIds.Add(itemSpawnId);
                scriptNodes.Add(NodeHelper.CreateSpawnObjectNode(itemSpawnId, xmlName, prizePos));
                if (holoEffectNodeId != -1)
                    NodeHelper.AddConnectionNodes(scriptNodes[scriptNodes.Count - 1], new int[] { holoEffectNodeId });
                scriptNodes.AddRange(extraNodes);
            }
            NodeHelper.AddConnectionNodes(itemLinkNode, puzzleNodeIds.ToArray());

            return scriptNodes;
        }

        private static List<XElement> CreateHubPortal(ref int id, Vector2 tpPos, int startId, string dest, int destId, int tpPlaySoundId, bool signToRight, string signText,
            out List<XElement> scriptNodes, string portalUseSetFlag, int portalActiveShapeNodeId)
        {
            List<XElement> doodads = new List<XElement>(4);
            scriptNodes = new List<XElement>();

            doodads.AddRange(CreateToggleableTeleporterWithSign(ref id, tpPos, startId, dest, destId, tpPlaySoundId, true, 1, signToRight, signText, out List<XElement> b1Nodes, out int portalOnNodeId, out int portalOffNodeId));
            scriptNodes.AddRange(b1Nodes);

            int areaTriggerNodeId = id++;
            int setFlagNodeId = id++;
            scriptNodes.Add(NodeHelper.CreateAreaTriggerNode(areaTriggerNodeId, -1, tpPos + new Vector2(2, -2), 0, 1, new int[] { portalActiveShapeNodeId }, new int[] { setFlagNodeId, portalOnNodeId }));
            scriptNodes.Add(NodeHelper.CreateSetGlobalFlagNode(setFlagNodeId, tpPos + new Vector2(2, -4), 1, portalUseSetFlag, true));
            globalScriptNodesToTriggerOnceOnLoad.Add(portalOffNodeId);

            return doodads;
        }
        //Uses 10 ids
        private static List<XElement> CreateTeleporterWithSign(ref int id, Vector2 tpPos, int startId, string dest, int destId, int tpPlaySoundId, bool toggleable, bool signToRight, string signText,
            out List<XElement> scriptNodes)
        {
            List<XElement> doodads = new List<XElement>(4);
            scriptNodes = new List<XElement>();

            int setFlagNodeId = -1;
            doodads.AddRange(CreateTeleporter(ref id, tpPos, startId, dest, destId, tpPlaySoundId, toggleable, out List<XElement> b1Nodes, setFlagNodeId));
            scriptNodes.AddRange(b1Nodes);

            //Signpost
            Vector2 signPos = tpPos + new Vector2(signToRight ? 1 : -1, 0.5f);

            int doodadId = id + 50000;
            int objEventTriggerId = id++;
            int speechId = id++;
            doodads.Add(NodeHelper.CreateDoodadNode(doodadId, "doodads/generic/deco_signpost.xml", signPos, false));
            scriptNodes.Add(NodeHelper.CreateObjectEventTriggerNode(objEventTriggerId, signPos + new Vector2(0, 4), -1, "Hit", new int[] { doodadId }, false, new int[] { speechId }));
            scriptNodes.Add(NodeHelper.CreateSpeechBubbleNode(speechId, signPos + new Vector2(0, 2), -1, "menus/speech/sign_wood_speech.xml", signText, new[] { doodadId }, new Vector2(0.15f, -1.15f), 100, 0, 0));

            return doodads;
        }
        private static List<XElement> CreateToggleableTeleporterWithSign(ref int id, Vector2 tpPos, int startId, string dest, int destId, int tpPlaySoundId, bool toggleable, int triggerTimes, bool signToRight, string signText,
            out List<XElement> scriptNodes, out int portalOnNodeId, out int portalOffNodeId)
        {
            List<XElement> doodads = new List<XElement>(4);
            scriptNodes = new List<XElement>();

            int setFlagNodeId = -1;
            doodads.AddRange(CreateToggleableTeleporter(ref id, tpPos, startId, dest, destId, tpPlaySoundId, out List<XElement> b1Nodes, out portalOnNodeId, out portalOffNodeId, setFlagNodeId, triggerTimes));
            scriptNodes.AddRange(b1Nodes);

            //Signpost
            Vector2 signPos = tpPos + new Vector2(signToRight ? 1 : -1, 0.5f);

            int doodadId = id + 50000;
            int objEventTriggerId = id++;
            int speechId = id++;
            doodads.Add(NodeHelper.CreateDoodadNode(doodadId, "doodads/generic/deco_signpost.xml", signPos, false));
            scriptNodes.Add(NodeHelper.CreateObjectEventTriggerNode(objEventTriggerId, signPos + new Vector2(0, 4), -1, "Hit", new int[] { doodadId }, false, new int[] { speechId }));
            scriptNodes.Add(NodeHelper.CreateSpeechBubbleNode(speechId, signPos + new Vector2(0, 2), -1, "menus/speech/sign_wood_speech.xml", signText, new[] { doodadId }, new Vector2(0.15f, -1.15f), 100, 0, 0));

            return doodads;
        }

        //Uses 6 ids, plus 4 more potentially
        private static XElement[] CreateTeleporter(ref int id, Vector2 pos, int startId, string destLevel, int destId, int tpPlaySoundId, bool toggleable, out List<XElement> scriptNodes, int connectNodeId = -1)
        {
            if (exitRando)
            {
                string code = $"{destLevel}|{destId}";
                if (exitSwaps.TryGetValue(code, out string newCode))
                {
                    string[] destSplits = newCode.Split('|');
                    destLevel = destSplits[0];
                    if (destSplits[1].EndsWith("*"))
                        destSplits[1] = destSplits[1].Substring(0, destSplits[1].Length - 1);
                    destId = int.Parse(destSplits[1]);
                }
            }
            XElement[] doodadNodes = new XElement[startId >= 0 ? 3 : 2];
            int portalDoodadId = id++;
            doodadNodes[0] = NodeHelper.CreateDoodadNode(portalDoodadId, "doodads/generic/exit_teleport.xml", pos, toggleable);
            doodadNodes[1] = NodeHelper.CreateDoodadNode(id++, "doodads/generic/exit_teleport_stand.xml", pos, false);
            if (startId >= 0)
            {
                doodadNodes[2] = NodeHelper.CreateDoodadNode(id++, "doodads/generic/exit_teleport_exit.xml", pos + new Vector2(0.5f, 2), false);
            }

            scriptNodes = new List<XElement>(3);
            //RectangleShape
            int rectangleShapeId = id++;
            scriptNodes.Add(NodeHelper.CreateRectangleShapeNode(rectangleShapeId, pos, 1f, 1f, 15));

            //LevelExitArea
            List<int> levelExitConnections = new List<int>(2);
            int levelExitAreaNodeId = id++;
            scriptNodes.Add(NodeHelper.CreateLevelExitNode(levelExitAreaNodeId, true, pos + new Vector2(0, -3), destLevel, destId, new[] { rectangleShapeId }));
            if (tpPlaySoundId > -1)
                levelExitConnections.Add(tpPlaySoundId);
            if (connectNodeId != -1)
                levelExitConnections.Add(connectNodeId);
            if (levelExitConnections.Count > 0)
                NodeHelper.AddConnectionNodes(scriptNodes[1], levelExitConnections.ToArray());

            //LevelStart
            if (startId >= 0)
            {
                scriptNodes.Add(NodeHelper.CreateLevelStartNode(id++, pos + new Vector2(0, 1.5f), startId));
            }

            if (exitRando && archipelagoData.mapType == ArchipelagoData.MapType.Temple && destLevel == "boss_2") //Special Krilith arena stuff
            {
                Vector2 nodePos = NodeHelper.PosFromString(scriptNodes[1].Element("vec2").Value);
                int bossTestNodeId = id++;
                int bossDiableId = id++;
                int storageEnableId = id++;
                int storageId = id++;
                scriptNodes.Add(NodeHelper.CreateCheckGlobalFlagNode(bossTestNodeId, nodePos - new Vector2(1.5f, 6), "boss_krilith_dead",
                    new int[] { bossDiableId, storageEnableId }, null));
                scriptNodes.Add(NodeHelper.CreateToggleElementNode(bossDiableId, nodePos + new Vector2(0, -3), -1, 1, new int[] { levelExitAreaNodeId }));
                scriptNodes.Add(NodeHelper.CreateToggleElementNode(storageEnableId, nodePos + new Vector2(3, -3), -1, 0, new int[] { storageId }));
                scriptNodes.Add(NodeHelper.CreateLevelExitNode(storageId, true, nodePos + new Vector2(3, 0), "boss_2_special", 0, new[] { rectangleShapeId }));
                globalScriptNodesToTriggerOnLoad.Add(bossTestNodeId);
            }

            return doodadNodes;
        }
        private static XElement[] CreateToggleableTeleporter(ref int id, Vector2 pos, int startId, string destLevel, int destId, int tpPlaySoundId, out List<XElement> scriptNodes,
            out int turnOnNodeId, out int turnOffNodeId, int connectNodeId = -1, int triggerTimes = -1)
        {
            int teleporterNodeId = id;
            XElement[] doodadNodes = CreateTeleporter(ref id, pos, startId, destLevel, destId, tpPlaySoundId, true, out scriptNodes, connectNodeId);

            turnOnNodeId = id++;
            turnOffNodeId = id++;
            scriptNodes.Add(NodeHelper.CreateChangeDoodadStateNode(turnOnNodeId, pos + new Vector2(3, 2), triggerTimes, "open", new int[] { teleporterNodeId }));
            scriptNodes.Add(NodeHelper.CreateChangeDoodadStateNode(turnOffNodeId, pos + new Vector2(3, 0), triggerTimes, "closed", new int[] { teleporterNodeId }));

            return doodadNodes;
        }
        private static XElement[] CreateDoorwayTransition(ref int id, Vector2 pos, string destLevel, int destId, int gateId = -1)
        {
            if (exitRando)
            {
                string code = $"{destLevel}|{destId}";
                if (exitSwaps.TryGetValue(code, out string newCode))
                {
                    string[] destSplits = newCode.Split('|');
                    destLevel = destSplits[0];
                    if (destSplits[1].EndsWith("*"))
                        destSplits[1] = destSplits[1].Substring(0, destSplits[1].Length - 1);
                    destId = int.Parse(destSplits[1]);
                }
            }

            XElement[] scriptNodes = new XElement[2];
            int rectangleShapeNodeId = id++;
            scriptNodes[0] = NodeHelper.CreateRectangleShapeNode(rectangleShapeNodeId, pos, 2f, 1f, 15);
            scriptNodes[1] = NodeHelper.CreateLevelExitNode(id++, true, pos + new Vector2(0, -3), destLevel, destId, new[] { rectangleShapeNodeId });

            return scriptNodes;
        }
        public static string GetGoalText(ArchipelagoData archipelagoData)
        {
            string goal = "";
            if (archipelagoData.mapType == ArchipelagoData.MapType.Castle)
                goal += "Hey! ";
            goal += "Your goal is ";
            switch (archipelagoData.goalType)
            {
                case ArchipelagoData.GoalType.BeatAllBosses:
                    goal += "to defeat ";
                    switch (archipelagoData.mapType)
                    {
                        case ArchipelagoData.MapType.Castle:
                            goal += "all four bosses.";
                            break;
                        case ArchipelagoData.MapType.Temple:
                            goal += "all three bosses.";
                            break;
                    }
                    break;
                case ArchipelagoData.GoalType.PlankHunt:
                    goal += "to collect ";
                    bool needsAllPlanks = false;
                    if ((int)(archipelagoData.plankHuntRequirement * archipelagoData.GetOption(SlotDataKeys.extraPlankPercent) / 100f) == archipelagoData.plankHuntRequirement)
                    {
                        needsAllPlanks = true;
                    }
                    if (needsAllPlanks)
                    {
                        goal += "all ";
                    }
                    goal += $"{archipelagoData.plankHuntRequirement} strange planks.";
                    break;
                case ArchipelagoData.GoalType.FullCompletion:
                    goal += "to defeat ";
                    switch (archipelagoData.mapType)
                    {
                        case ArchipelagoData.MapType.Castle:
                            goal += "the dragon and escape, collecting ";
                            break;
                        case ArchipelagoData.MapType.Temple:
                            goal += "the Sun Guardian Sha'Rand and collect ";
                            break;
                    }
                    bool needsAllPlanks2 = false;
                    if ((int)(archipelagoData.plankHuntRequirement * archipelagoData.GetOption(SlotDataKeys.extraPlankPercent) / 100f) == archipelagoData.plankHuntRequirement)
                    {
                        needsAllPlanks2 = true;
                    }
                    if (needsAllPlanks2)
                    {
                        goal += "all ";
                    }
                    goal += $"{archipelagoData.plankHuntRequirement} strange planks.";
                    break;
                case ArchipelagoData.GoalType.Alternate:
                    switch (archipelagoData.mapType)
                    {
                        case ArchipelagoData.MapType.Castle:
                            goal += "to find and complete all four bonus levels.";
                            break;
                        case ArchipelagoData.MapType.Temple:
                            goal += "to make it to the end of the Pyramid of Fear.";
                            break;
                    }
                    break;
            }
            goal += " Good luck!";
            if (archipelagoData.GetOption(SlotDataKeys.deathLink) > 0)
            {
                goal += " And try not to die!";
            }

            return goal;
        }
        public static string GetGateType(string key)
        {
            if (gateTypes.TryGetValue(key, out string gateType))
                return gateType;
            return GetKeyNameFromCode(0);
        }
    }
}
