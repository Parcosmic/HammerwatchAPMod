using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using ARPGGame;
using ARPGGame.GUI;
using ARPGGame.Menus;
using ARPGGame.Networking;
using ARPGGame.Behaviors.Players;
using ARPGGame.WorldItemBehaviors;
using ARPGGame.ScriptNodes;
using HammerwatchAP.Util;
using HammerwatchAP.Game;
using HammerwatchAP.Menus;
using HammerwatchAP.Hooks;
using HammerwatchAP.Controls;
using TiltedEngine;
using TiltedEngine.Drawing;
using TiltedEngine.Networking;
using TiltedEngine.WorldObjects;
using TiltedEngine.WorldObjects.WorldObjectProducers;
using OpenTK;
using SDL2;
using Newtonsoft.Json;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;

namespace HammerwatchAP.Archipelago
{
    public static class ArchipelagoManager
    {
        public static readonly Version MOD_VERSION = new Version(2, 0, 0);
        public static readonly Version APWORLD_VERSION = new Version(4, 0, 0);
        public static readonly Version AP_VERSION = new Version(0, 6, 0);

        public const string ERROR_MESSAGE = "The game just crashed and created an error file (error.txt). Please send this and the game.log file to Parcosmic on the Archipelago Discord to help me fix it!";

        public const string BUTTON_EVENT_NAME = "got_button_event";

        private static Random random;

        public static ConnectionInfo connectionInfo = new ConnectionInfo();
        public static ArchipelagoData archipelagoData = new ArchipelagoData();
        public static GenerateInfo generateInfo = new GenerateInfo();
        public static string saveFileName;

        public static int reconnectTimer;

        public static bool inArchipelagoMenu;
        public static bool playingArchipelagoSave;

        static DeathLink deathlinkQueue;
        public static int trapLinkIndex;
        public static List<string> trapLinkQueue = new List<string>();
        public static Dictionary<string, Dictionary<long, string>> remoteItemToXmlNameCache = new Dictionary<string, Dictionary<long, string>>();

        public static GameState gameState;
        public static bool gameReady
        {
            get
            {
                return gameState == GameState.InGame;
            }
        }
        public static bool mapFinishedGenerating
        {
            get
            {
                return gameState > GameState.Generating;
            }
        }

        //Game data
        public static bool datapackageUpToDate = false;
        public static Dictionary<string, GameData> gameData = new Dictionary<string, GameData>();
        public static Dictionary<string, string> gameChecksums = new Dictionary<string, string>();

        //Config values
        public static bool Deathlink = false;
        public static bool ExploreSpeed = true;
        public static bool ExploreSpeedPing = true;
        public static bool FragileBreakables = true;
        public static bool APChatMirroring = true;
        public static bool ShopItemHinting = true;
        public static bool TrapLink = false;

        public static bool DEBUG_MODE = true;

        public static string LastConnectedIP = "archipelago.gg";
        public static string LastConnectedSlotName = "PlayerName";

        //Item matching
        public static Dictionary<string, List<string[]>> activeGameFuzzyItemNameToXMLDict;
        public static Dictionary<string, string> activeTrapLinkDict;
        public const string ITEM_MATCH_FILE = "ap_item_matching.json";
        public const string TRAP_LINK_MATCH_FILE = "trap_link_matching.json";

        public static void LoadMod()
        {
            ResourceContext.Log($"Load Hammerwatch Archipelago Mod - Ver {MOD_VERSION}");

            SoundHelper.LoadSounds();

            random = new Random();

            //Keyword item mapping
            string fuzzyDictFilePath = Path.Combine(Directory.GetCurrentDirectory(), APMapPatcher.AP_ASSETS_FOLDER, ITEM_MATCH_FILE);
            bool alwaysOverride = false;
            if (DEBUG_MODE)
                alwaysOverride = true;
            if (File.Exists(fuzzyDictFilePath) && !alwaysOverride)
            {
                ResourceContext.Log("Reading ap_item_matching list");
                string readFuzzyDict = File.ReadAllText(fuzzyDictFilePath);
                try
                {
                    Dictionary<string, List<string[]>> readGameFuzzyItemNameToXMLDict = JsonConvert.DeserializeObject<Dictionary<string, List<string[]>>>(readFuzzyDict);
                    activeGameFuzzyItemNameToXMLDict = readGameFuzzyItemNameToXMLDict;
                }
                catch (Exception)
                {
                    ResourceContext.Log("Error while reading ap_item_matching list, falling back to the default list");
                    activeGameFuzzyItemNameToXMLDict = APData.gameFuzzyItemNameToXML;
                    GameBase.Instance.SetMenu(MenuType.MESSAGE, "Mod Load Error", "Failed to load the ap_item_matching.txt file, falling back to the default list");
                }
            }
            else
            {
                ResourceContext.Log("ap_item_matching list does not exist, falling back to the default list");
                activeGameFuzzyItemNameToXMLDict = APData.gameFuzzyItemNameToXML;

                ResourceContext.Log("Creating ap_item_matching list file");
                string fuzzyDictString = JsonConvert.SerializeObject(APData.gameFuzzyItemNameToXML, Formatting.Indented);
                fuzzyDictString = fuzzyDictString.Replace("\",\r\n      ", "\", ");
                fuzzyDictString = fuzzyDictString.Replace("[\r\n      ", "[ ");
                fuzzyDictString = fuzzyDictString.Replace("\"\r\n    ", "\" ");
                File.WriteAllText(fuzzyDictFilePath, fuzzyDictString);
            }
            remoteItemToXmlNameCache = new Dictionary<string, Dictionary<long, string>>
            {
                [""] = new Dictionary<long, string>()
            };

            //Trap link item mapping
            string trapLinkDictFilePath = Path.Combine(Directory.GetCurrentDirectory(), APMapPatcher.AP_ASSETS_FOLDER, TRAP_LINK_MATCH_FILE);
            if (File.Exists(trapLinkDictFilePath) && !alwaysOverride)
            {
                ResourceContext.Log("Reading trap_link_matching list");
                string readTrapLinkDictString = File.ReadAllText(trapLinkDictFilePath);
                try
                {
                    activeTrapLinkDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(readTrapLinkDictString);
                }
                catch (Exception)
                {
                    ResourceContext.Log("Error while reading trap_link_matching list, falling back to the default list");
                    activeTrapLinkDict = APData.trapLinkInterpretNames;
                    GameBase.Instance.SetMenu(MenuType.MESSAGE, "Mod Load Error", "Failed to load the trap_link_matching.txt file, falling back to the default list");
                }
            }
            else
            {
                ResourceContext.Log("trap_link_matching list does not exist, falling back to the default list");
                activeTrapLinkDict = APData.trapLinkInterpretNames;

                ResourceContext.Log("Creating trap_link_matching list file");
                string trapLinkDictString = JsonConvert.SerializeObject(APData.trapLinkInterpretNames, Formatting.Indented);
                File.WriteAllText(trapLinkDictFilePath, trapLinkDictString);
            }

            QoL.Setup();
            GameInterface.Setup();
        }
        
        public static bool ConnectedToAP()
        {
            return connectionInfo.ConnectionActive;
        }

        public static void InitializeAPVariables(ArchipelagoData archipelagoData)
        {
            gameChecksums = new Dictionary<string, string>();

            ArchipelagoManager.archipelagoData = archipelagoData;

            ResetAPVars();
        }
        public static void ResetAPVars()
        {
            deathlinkQueue = null;
            trapLinkIndex = 0;
            trapLinkQueue.Clear();
        }

        public static bool LoadDatapackage()
        {
            string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string datapackageFolder = Path.Combine(localFolder, "Archipelago", "Cache", "datapackage");
            if (!Directory.Exists(datapackageFolder)) return false;

            foreach (string game in gameChecksums.Keys)
            {
                if (gameData.ContainsKey(game)) continue; //If we already set it when we received the datapackage don't load it
                string gameDatapackageFolder = Path.Combine(datapackageFolder, game);
                if (!Directory.Exists(gameDatapackageFolder)) return false;
                string gameChecksum = gameChecksums[game];
                string datapackageFile = Path.Combine(gameDatapackageFolder, $"{gameChecksum}.json");
                if (!File.Exists(datapackageFile)) return false;
                StreamReader stream = File.OpenText(datapackageFile);
                gameData[game] = JsonConvert.DeserializeObject<GameData>(stream.ReadToEnd());
                stream.Close();
            }

            return true;
        }
        public static void StartConnection(string ip, string slotName, string password)
        {
            connectionInfo = new ConnectionInfo(slotName, ip, password);
            //Start connecting
            GameBase.Instance.SetMenu(MenuType.MESSAGE, "Connecting", "Connecting to Archipelago server...");
            connectionInfo.StartConnection(null, true, false);
        }
        public static void DisconnectFromArchipelago(string reason=null)
        {
            connectionInfo.connectedToAP = false;
            connectionInfo.failedConnectMsg = "Disconnected from Archipelago server";
            if (reason != null)
                connectionInfo.failedConnectMsg = reason;
            connectionInfo.Disconnect();
            MainMenu mainMenu = GameBase.Instance.GetMenu<MainMenu>();
            if (mainMenu != null)
            {
                HookMainMenu.SetArchipelagoButtons(mainMenu, false, false);
                ((TextWidget)mainMenu.Document.GetWidget("ap-connection")).SetText("Not connected to Archipelago");
                ((TextWidget)mainMenu.Document.GetWidget("ap-slot-name")).SetText("");
            }
        }
        public static void CompletedGeneration()
        {
            gameState = GameState.Generated;
            GameBase.Instance.CloseMenu(MenuType.MESSAGE);

            MainMenu mainMenu = GameBase.Instance.GetMenu<MainMenu>();
            if (mainMenu != null)
                HookMainMenu.SetArchipelagoButtons(mainMenu, true, archipelagoData.saveFileName != null);

            LobbyMenu apMenu = GameBase.Instance.GetMenu<LobbyMenu>();
            if(apMenu != null)
                SetArchipelagoLevel(apMenu, archipelagoData.mapFileName);
        }
        public static void SetArchipelagoLevel(LobbyMenu apMenu, string mapName)
        {
            IList<ModificationInfo> allMods = GameBase.Instance.modList.GetModifications();
            ModificationInfo toPlay = null;
            foreach (ModificationInfo mod in allMods)
            {
                if (mod.Path == Path.Combine("levels", mapName))
                {
                    toPlay = mod;
                    break;
                }
            }

            if (toPlay != null)
            {
                apMenu.SetLevel(toPlay.ID, Path.GetFileName(toPlay.Path));
            }
            else
            {
                apMenu.SetLevel("", "");
            }
        }

        public static void GameUpdate(int ms)
        {
            if (connectionInfo != null)
            {
                connectionInfo.GameUpdate(ms);
            }
            if (playingArchipelagoSave)
            {
                switch(gameState)
                {
                    case GameState.Generating:
                        if(GameBase.Instance.GetMenu<MessageMenu>() == null)
                        {
                            GameBase.Instance.SetMenu(MenuType.MESSAGE, "Generation In Progress", "Generating map file...");
                        }
                        else
                        {
                            Task waitForGeneratedThread = new Task(() =>
                            {
                                while (!mapFinishedGenerating) { }
                            });
                            waitForGeneratedThread.Start();
                            try
                            {
                                waitForGeneratedThread.Wait();
                            }
                            catch (AggregateException e)
                            {
                                foreach (var ex in e.InnerExceptions)
                                {
                                    throw ex;
                                }
                            }
                        }
                        break;
                    case GameState.InGame:
                        if (!ConnectedToAP() && reconnectTimer > 0) //&& !offlineMode
                        {
                            reconnectTimer -= ms;
                            if (reconnectTimer <= 0)
                            {
                                reconnectTimer = 15000;
                                connectionInfo.StartConnection(archipelagoData, false, true, false);
                            }
                        }
                        if (GameBase.Instance.Players[0] != null && GameBase.Instance.Players[0].Actor != null)
                        {
                            if (archipelagoData != null)
                            {
                                archipelagoData.GameUpdate(ms);
                            }
                            ArchipelagoMessageManager.GameUpdate(ms);
                            //Floor master keys
                            if (archipelagoData.GetOption(SlotDataKeys.keyMode) == 2)
                            {
                                int floorIndex = APData.GetFloorIndex(archipelagoData.currentLevelName, archipelagoData);
                                int[] mkeys = new int[] { 0, 1, 2 };
                                if (archipelagoData.GetOption(SlotDataKeys.randomizeBonusKeys) > 0 && archipelagoData.currentLevelName.StartsWith("level_bonus_"))
                                {
                                    mkeys = new int[] { 10 };
                                }
                                for (int k = 0; k < mkeys.Length; k++)
                                {
                                    int arrayIndex = k;
                                    if (mkeys.Length == 1)
                                        arrayIndex = 3;
                                    int correctAmount = floorIndex != -1 && archipelagoData.hasMasterFloorKeys[arrayIndex, floorIndex] ? 1 : 0;
                                    if (GameBase.Instance.Players[0].GetKey(mkeys[k]) != correctAmount)
                                        GameBase.Instance.Players[0].SetKeys(mkeys[k], correctAmount);
                                }
                            }
                            //Traplink
                            while(trapLinkIndex < trapLinkQueue.Count)
                            {
                                ReceiveTrapLinkItem(trapLinkQueue[trapLinkIndex++]);
                            }
                        }
                        GameInterface.GameUpdate(ms);
                        break;
                }
            }
            ControlManager.Update(ms);
            //Explore speed
            if (GameBase.Instance.Players != null)
            {
                QoL.UpdateImmortalPlayers();
                QoL.TickExploreSpeed(ms);
            }
        }

        public static bool IsHostPlayer()
        {
            return GameBase.Instance.Players != null && GameBase.Instance.Players[0].IsLocalPlayer || Network.IsServer;
        }
        public static void APStartGameSetup()
        {
            Logging.Log($"Doing AP game start setup as {(IsHostPlayer() ? "host" : "remote")} with {GameBase.Instance.Players.Count} player(s)");
            random = new Random();
            gameState = GameState.InGame;

            //Refresh killedBosses array if we resume a game
            for (int b = 0; b < archipelagoData.killedBosses.Length; b++)
            {
                archipelagoData.killedBosses[b] = GameInterface.GetGlobalFlag($"killed_boss_{1 + b}");
            }
            CheckBossCompletion();

            for (int i = 0; i < archipelagoData.GetOption(SlotDataKeys.openCastlePortals); i++)
            {
                GameInterface.SetGlobalFlag($"portal_a{i + 1}", true);
            }
            //Force the quest item sidebar to always show, we might not be starting in the hub area
            if (archipelagoData.mapType == ArchipelagoData.MapType.Temple)
            {
                GameInterface.SetGlobalFlag($"quest_bar", true);
            }

            //Add hammer if hammer fragments are zero
            if (archipelagoData.totalHammerFragments == 0)
                GameInterface.SetGlobalFlag($"has_hammer", true);

            ArchipelagoMessageManager.Setup();

            SyncUpgrades();

            if (!connectionInfo.ConnectionActive) return;
            connectionInfo.SetClientReady();
        }
        public static void ChangedLevel(string levelId)
        {
            if (playingArchipelagoSave)
            {
                ResourceContext.Log("Changed level to id: " + levelId);
                if (!gameReady)
                {
                    APStartGameSetup();
                }
                int keyMode = archipelagoData.GetOption(SlotDataKeys.keyMode);

                archipelagoData.currentLevelId = levelId;
                archipelagoData.currentLevelName = APData.GetLevelFileNameFromId(levelId, archipelagoData);
                int newAct = APData.GetActFromLevelId(levelId, archipelagoData);
                switch (keyMode)
                {
                    case 0: //Vanilla keys
                        break;
                    case 1: //Act keys
                        if (archipelagoData.currentAct != newAct && archipelagoData.currentAct != 0)
                        {
                            ResourceContext.Log($"Swapping keys for new act {newAct}");
                            int[] akeys = new int[] { 0, 1, 2, 10 };
                            for (int k = 0; k < akeys.Length; k++)
                            {
                                archipelagoData.actKeys[archipelagoData.currentAct - 1, k] = GameBase.Instance.Players[0].GetKey(akeys[k]);
                                GameBase.Instance.Players[0].SetKeys(akeys[k], archipelagoData.actKeys[newAct - 1, k]);
                            }
                        }
                        break;
                    case 2: //Floor master keys
                        ResourceContext.Log($"Swapping floor master keys for new level {archipelagoData.currentLevelName}");
                        int floorIndex = APData.GetFloorIndex(archipelagoData.currentLevelName, archipelagoData);
                        int[] mkeys = new int[] { 0, 1, 2 };
                        if (archipelagoData.GetOption(SlotDataKeys.randomizeBonusKeys) > 0)
                        {
                            if (archipelagoData.currentLevelName.StartsWith("level_bonus_"))
                            {
                                mkeys = new int[] { 10 };
                            }
                        }
                        else
                        {
                            //Switch act keys for bonus keys only
                            if (archipelagoData.currentAct != newAct && archipelagoData.currentAct != 0)
                            {
                                ResourceContext.Log($"Swapping bonus keys for new act {newAct}");
                                archipelagoData.actKeys[archipelagoData.currentAct - 1, 3] = GameBase.Instance.Players[0].GetKey(10);
                                GameBase.Instance.Players[0].SetKeys(10, archipelagoData.actKeys[newAct - 1, 3]);
                            }
                        }
                        for (int k = 0; k < mkeys.Length; k++)
                        {
                            int arrayIndex = k;
                            if (mkeys.Length == 1)
                                arrayIndex = 3;
                            GameBase.Instance.Players[0].SetKeys(mkeys[k], floorIndex != -1 && archipelagoData.hasMasterFloorKeys[arrayIndex, floorIndex] ? 1 : 0);
                        }
                        break;
                }
                archipelagoData.currentAct = newAct;

                //Auto tab switching
                if (connectionInfo.connectedToAP)
                {
                    //session.DataStorage[$"{connectionInfo.playerTeam}:{connectionInfo.playerId}:CurrentRegion"] = levelId;
                    connectionInfo.SetMapTrackingKey(levelId);
                }

                //Apply deathlink if it was queued
                if (deathlinkQueue != null)
                    ReceivedDeathlink(deathlinkQueue);
            }
        }
        public static void RefreshEvents()
        {
            if (playingArchipelagoSave)
            {
                //Resubscribe goal events when the level is reloaded
                void KillBoss1()
                {
                    archipelagoData.killedBosses[0] = true;
                    CheckBossCompletion();
                    DataStorageMarkBossCompleted(1);
                }
                void KillBoss2()
                {
                    archipelagoData.killedBosses[1] = true;
                    CheckBossCompletion();
                    DataStorageMarkBossCompleted(2);
                }
                void KillBoss3()
                {
                    archipelagoData.killedBosses[2] = true;
                    CheckBossCompletion();
                    DataStorageMarkBossCompleted(3);
                }
                void KillBoss4()
                {
                    archipelagoData.killedBosses[3] = true;
                    CheckBossCompletion();
                    DataStorageMarkBossCompleted(4);
                }
                void FullCompletion() => CompleteGoal(ArchipelagoData.GoalType.FullCompletion);
                void AlternateGoal() => CompleteGoal(ArchipelagoData.GoalType.Alternate);
                switch (archipelagoData.mapType)
                {
                    case ArchipelagoData.MapType.Castle:
                        EventSystem.Instance.Subscibe("killed_boss_1", KillBoss1);
                        EventSystem.Instance.Subscibe("killed_boss_2", KillBoss2);
                        EventSystem.Instance.Subscibe("killed_boss_3", KillBoss3);
                        EventSystem.Instance.Subscibe("killed_boss_4", KillBoss4);
                        EventSystem.Instance.Subscibe("end_game_survive", FullCompletion);
                        if (archipelagoData.currentLevelName == "level_1.xml")
                            EventSystem.Instance.Subscibe("ap_check_end_game", CheckCompletion);
                        break;
                    case ArchipelagoData.MapType.Temple:
                        EventSystem.Instance.Subscibe("killed_boss_1", KillBoss1);
                        EventSystem.Instance.Subscibe("killed_boss_2", KillBoss2);
                        EventSystem.Instance.Subscibe("killed_boss_3", KillBoss3);
                        EventSystem.Instance.Subscibe("quest_pyramid_solved_door", AlternateGoal);
                        //EventSystem.Instance.Subscibe("puzzle_bonus_solved", CompletePoFPuzzle);
                        if (archipelagoData.currentLevelName == "level_boss_1.xml")
                            EventSystem.Instance.Subscibe("Boss Died", () => archipelagoData.wormCounter++);
                        if (archipelagoData.currentLevelName == "level_hub.xml")
                            EventSystem.Instance.Subscibe("ap_check_end_game", CheckCompletion);
                        break;
                }
                //Subscribe buttonsanity events
                if (archipelagoData.GetOption(SlotDataKeys.buttonsanity) > 0)
                {
                    Dictionary<string, int> buttonEventDict = new Dictionary<string, int>(0);
                    switch (archipelagoData.mapType)
                    {
                        case ArchipelagoData.MapType.Castle:
                            if (APData.castleButtonEventToLocationId.TryGetValue(archipelagoData.currentLevelName, out var castleButtonValues))
                                buttonEventDict = castleButtonValues;
                            break;
                        default:
                            if (APData.templeButtonEventToLocationId.TryGetValue(archipelagoData.currentLevelName, out var templeButtonValues))
                                buttonEventDict = templeButtonValues;
                            break;
                    }
                    void SpawnButtonEventItem(int location)
                    {
                        long locId = APData.GetAdjustedLocationId(location, archipelagoData);
                        NetworkItem item = archipelagoData.GetItemFromLoc(locId);
                        if (item.Item == -1) return;
                        archipelagoData.CheckLocation(locId, true);

                        if (item.Player != connectionInfo.playerId)
                            return;

                        CreateItemInWorld(item, false);
                    }
                    foreach (string buttonEvent in buttonEventDict.Keys)
                    {
                        EventSystem.Instance.Subscibe(buttonEvent, () => SpawnButtonEventItem(buttonEventDict[buttonEvent]));
                    }
                }
                //Subscribe trap events
                EventSystem.Instance.Subscibe(APData.SPEECH_TRAP_EVENT_NAME, () => { GameInterface.StartSpeechTrap(); });
            }
        }
        public static void DataStorageMarkBossCompleted(int boss)
        {
            //if (connectionInfo.ConnectionActive)
            //{
            //    session.DataStorage[$"{connectionInfo.playerTeam}:{connectionInfo.playerId}:boss_{boss}"] = 1;
            //}
            connectionInfo.SetDataStorageValue($"{connectionInfo.playerTeam}:{connectionInfo.playerId}:boss_{boss}", "1");
        }

        public static void GetBossRuneFlagFromItem(string itemName, bool receive)
        {
            int bossItemActIndex = -1;
            int bossRuneIndex = -1;
            string progressBaseName = itemName.Replace(" Progress", "");
            int bossRuneProgressCount = progressBaseName == itemName ? 4 : 1;
            string[] runeWords = progressBaseName.Split(' ');
            bossItemActIndex = APData.GetActIndexFromActName(runeWords[0], archipelagoData);
            switch (runeWords[3])
            {
                case "y":
                case "Y":
                    bossRuneIndex = 0;
                    break;
                case "n":
                case "N":
                    bossRuneIndex = 1;
                    break;
                case "a":
                case "A":
                    bossRuneIndex = 2;
                    break;
            }
            int levelIndex = 3 * bossItemActIndex + bossRuneIndex;
            string flagName = $"a{bossItemActIndex + 1}l{levelIndex + 1}_boss{bossItemActIndex + 1}";
            if (receive)
            {
                archipelagoData.bossRunes[levelIndex] += 1;
                for (int i = 1; i <= 3; i++)
                {
                    if (archipelagoData.bossRunes[bossItemActIndex] < bossRuneProgressCount) continue;
                    GameInterface.SetGlobalFlag(flagName, true);
                    EventSystem.Instance.FireEvent(BUTTON_EVENT_NAME);
                }
            }
        }
        public static Upgrade GetNextShopUpgrade(Upgrade baseUpgrade)
        {
            if (!playingArchipelagoSave)
                return baseUpgrade;
            if (baseUpgrade.ID.StartsWith("ap-"))
            {
                int locId = int.Parse(baseUpgrade.ID.Substring(3));
                NetworkItem item = archipelagoData.GetItemFromLoc(locId);
                Upgrade? upgrade = GetNextShopUpgrade(item, false);
                if (upgrade.HasValue)
                {
                    return upgrade.Value;
                }
            }
            return baseUpgrade;
        }
        public static Upgrade? GetNextShopUpgrade(NetworkItem item, bool receive)
        {
            int itemId = (int)item.Item;
            Upgrade? focusedUpgrade = null;
            if (IsItemOurs(item, receive) && APData.IsItemShopUpgrade(itemId))
            {
                PlayerClass itemClass = APData.GetShopItemClass(itemId);
                string[] vanillaShopIds = APData.GetShopIdsFromItemId(itemId);
                Upgrade? highestOwnedUpgrade = null;
                int highestOwnedUpgradeIndex = -1;
                foreach (PlayerInfo player in GameBase.Instance.Players)
                {
                    if (player.Class != itemClass) continue;
                    Dictionary<string, Upgrade> upgradeIdToUpgrade = new Dictionary<string, Upgrade>();
                    foreach (Upgrade upgrade in player.TweakData.GetAllUpgrades())
                    {
                        if (upgrade.ID.StartsWith("ap-")) continue;
                        upgradeIdToUpgrade[upgrade.ID] = upgrade;
                    }
                    HashSet<string> activeUpgradeIds = new HashSet<string>();
                    foreach (Upgrade upgrade in player.TweakData.GetActiveUpgrades())
                    {
                        activeUpgradeIds.Add(upgrade.ID);
                    }
                    for (int u = vanillaShopIds.Length - 1; u >= 0; u--)
                    {
                        string vanillaId = vanillaShopIds[u];
                        if (activeUpgradeIds.Contains(vanillaId))
                        {
                            highestOwnedUpgrade = upgradeIdToUpgrade[vanillaId];
                            highestOwnedUpgradeIndex = u;
                            break;
                        }
                    }
                    if (highestOwnedUpgradeIndex >= vanillaShopIds.Length - 1)
                        break;
                    highestOwnedUpgrade = upgradeIdToUpgrade[vanillaShopIds[highestOwnedUpgradeIndex + 1]];
                    break;
                }
                if (highestOwnedUpgrade != null)
                {
                    focusedUpgrade = highestOwnedUpgrade.Value;
                }
            }
            return focusedUpgrade;
        }
        public static void SyncUpgrades(PlayerInfo updatePlayer = null)
        {
            Dictionary<PlayerClass, Dictionary<int, int>> playerReceivedItemUpgradeCounts = new Dictionary<PlayerClass, Dictionary<int, int>>();
            Dictionary<PlayerClass, List<string>> classActiveAPUpgrades = new Dictionary<PlayerClass, List<string>>();
            foreach (PlayerClass playerClass in Enum.GetValues(typeof(PlayerClass)))
            {
                playerReceivedItemUpgradeCounts[playerClass] = new Dictionary<int, int>();
            }
            Dictionary<int, List<string>> playerActiveUpgradeIds = new Dictionary<int, List<string>>();
            HashSet<int> activeAPUpgrades = new HashSet<int>();
            foreach (PlayerInfo player in GameBase.Instance.Players)
            {
                playerActiveUpgradeIds[player.PeerID] = new List<string>();
                foreach (Upgrade upgrade in player.TweakData.GetActiveUpgrades())
                {
                    if (upgrade.ID.StartsWith("ap-"))
                    {
                        int apLocId = int.Parse(upgrade.ID.Substring(3));
                        activeAPUpgrades.Add(apLocId);
                    }
                    else
                        playerActiveUpgradeIds[player.PeerID].Add(upgrade.ID);
                }
            }
            List<NetworkItem> itemsToSync = new List<NetworkItem>(archipelagoData.itemsToReceive.Where((itm) => APData.IsItemShopUpgrade(itm.Item)));
            itemsToSync.AddRange(archipelagoData.checkedLocations.Where((locId) => APData.IsItemShopUpgrade(archipelagoData.locationToItem[locId].Item)).Select((locId) => archipelagoData.locationToItem[locId]));
            //itemsToSync.AddRange(activeAPUpgrades.Select((locId) => locationToItem[locId]).Where((itm) => APData.IsItemShopUpgrade(itm.Item)));
            foreach (NetworkItem item in itemsToSync)
            {
                int itemId = (int)item.Item;
                PlayerClass itemClass = APData.GetShopItemClass(itemId);
                if (!playerReceivedItemUpgradeCounts[itemClass].ContainsKey(itemId))
                    playerReceivedItemUpgradeCounts[itemClass][itemId] = 0;
                playerReceivedItemUpgradeCounts[itemClass][itemId]++;
            }
            foreach (PlayerInfo player in GameBase.Instance.Players)
            {
                bool appliedUpgrades = false;
                bool appliedUpgradesThisLoop = false;
                Dictionary<int, int> upgradesMissingPrereqs = new Dictionary<int, int>();
                Dictionary<string, Upgrade> upgradeIdToUpgrade = new Dictionary<string, Upgrade>();
                foreach (Upgrade upgrade in player.TweakData.GetAllUpgrades())
                {
                    upgradeIdToUpgrade[upgrade.ID] = upgrade;
                }
                Dictionary<int, int> receivedItemUpgradeCounts = new Dictionary<int, int>(playerReceivedItemUpgradeCounts[player.Class]);
                while (receivedItemUpgradeCounts.Count > 0)
                {
                    appliedUpgradesThisLoop = false;
                    foreach (KeyValuePair<int, int> receivedItemUpgrades in receivedItemUpgradeCounts)
                    {
                        string[] vanillaShopIds = APData.GetShopIdsFromItemId(receivedItemUpgrades.Key);
                        //Does the first upgrade in the series require another upgrade we don't have?
                        Upgrade firstUpgrade = upgradeIdToUpgrade[vanillaShopIds[0]];
                        if (firstUpgrade.Requrements.Length > 0)
                        {
                            //Check if we have all the requirements, elsewise we can't use this upgrade!
                            bool hasAllPrereqs = true;
                            foreach (string req in firstUpgrade.Requrements)
                            {
                                if (playerActiveUpgradeIds[player.PeerID].Contains(req)) continue;
                                hasAllPrereqs = false;
                                break;
                            }
                            if (!hasAllPrereqs)
                            {
                                upgradesMissingPrereqs[receivedItemUpgrades.Key] = receivedItemUpgrades.Value;
                                //Skip to the next item to apply
                                continue;
                            }
                        }
                        int maxUpgrades = Math.Min(receivedItemUpgrades.Value, vanillaShopIds.Length);
                        for (int r = 0; r < maxUpgrades; r++)
                        {
                            string currentUpgradeId = vanillaShopIds[r];
                            if (playerActiveUpgradeIds[player.PeerID].Contains(currentUpgradeId)) continue;
                            player.TweakData.ActivateUpgrade(currentUpgradeId);
                            playerActiveUpgradeIds[player.PeerID].Add(currentUpgradeId);
                            appliedUpgrades = true;
                            appliedUpgradesThisLoop = true;
                        }
                    }
                    receivedItemUpgradeCounts.Clear();
                    //If we applied upgrades we may be able to apply some upgrades we didn't have the prereqs for before
                    if (upgradesMissingPrereqs.Count > 0 && appliedUpgradesThisLoop)
                    {
                        receivedItemUpgradeCounts = upgradesMissingPrereqs;
                        upgradesMissingPrereqs = new Dictionary<int, int>();
                    }
                }

                if (player.IsLocalPlayer && appliedUpgrades)
                {
                    Network.SendToAll("SetPlayerUpgrades", new object[]
                    {
                    player.TweakData.SaveActiveUpgrades()
                    });
                    if (player.Actor != null)
                        ((PlayerActorBehavior)player.Actor.Behavior).RefreshTweakData(GameBase.Instance.resources);
                    else
                        Console.WriteLine("Player actor doesn't exist for player " + player.PeerID);
                }
            }
        }
        public static void BoughtAPShopItem(PlayerInfo player, string shopId)
        {
            if (shopId.StartsWith("ap-"))
            {
                string locIdStr = shopId.Split('-').Last();
                int locId = int.Parse(locIdStr);
                archipelagoData.CheckLocation(locId, false);
                NetworkItem item = archipelagoData.GetItemFromLoc(locId);
                if (IsItemOurs(item, false))
                    CreateItemInWorld(item, false);
            }
        }

        public static void CheckCompletion()
        {
            bool didGoal = false;
            switch (archipelagoData.goalType)
            {
                case ArchipelagoData.GoalType.BeatAllBosses:
                    didGoal = CheckBossCompletion();
                    break;
                case ArchipelagoData.GoalType.PlankHunt:
                    if (archipelagoData.planks >= archipelagoData.plankHuntRequirement)
                    {
                        CompleteGoal(ArchipelagoData.GoalType.PlankHunt);
                        didGoal = true;
                    }
                    break;
                case ArchipelagoData.GoalType.Alternate:
                    if (archipelagoData.mapType == ArchipelagoData.MapType.Temple && GameInterface.GetGlobalFlag("quest_pyramid_solved"))
                    {
                        CompleteGoal(ArchipelagoData.GoalType.Alternate);
                        didGoal = true;
                    }
                    break;
            }
            if (didGoal)
            {
                ShowEndGameScreen(archipelagoData.goalType);
            }
        }
        public static bool CheckBossCompletion()
        {
            for (int b = 0; b < archipelagoData.killedBosses.Length; b++)
            {
                if (!archipelagoData.killedBosses[b]) return false;
            }
            CompleteGoal(ArchipelagoData.GoalType.BeatAllBosses);
            return true;
        }

        //Archipelago connection methods
        public static void CheckLocation(Vector2 position)
        {
            archipelagoData.CheckLocation(position);
        }
        public static void SetDeathlink(bool on)
        {
            Deathlink = on;
            connectionInfo.SetDeathlink(on);
        }
        public static void ReceivedDeathlink(DeathLink deathlink)
        {
            if (!Deathlink)
                return;
            deathlinkQueue = deathlink;
            if (GameBase.Instance.Players == null)
                return;
            foreach (PlayerInfo player in GameBase.Instance.Players)
            {
                player.ChangeHealth(-9999);
                if (player.Actor != null && (player.Actor.Behavior is PlayerActorBehavior actBehavior))
                {
                    actBehavior.CheckDead();
                }
            }
            deathlinkQueue = null;
            string message;
            if(deathlink.Cause == null || deathlink.Cause == "")
            {
                message = $"{deathlink.Source} sent a Deathlink!";
            }
            else
            {
                message = deathlink.Cause;
            }
            ArchipelagoMessageManager.SendHWMessage(message, TiltedEngine.Color.Red);
        }
        public static void PlayerDeath(PlayerInfo player) //Only is run by the host
        {
            if (Deathlink && deathlinkQueue == null)
            {
                DeathLink deathlink = new DeathLink(connectionInfo.slotName, GetDeathlinkMessage(player));
                connectionInfo.SendDeathlink(deathlink);
                ReceivedDeathlink(deathlink);
            }
        }
        private static string GetDeathlinkMessage(PlayerInfo player)
        {
            Random random = new Random();
            List<string> potentialMessages = new List<string>(APData.deathlinkMessages);
            List<string> specialMessages = new List<string>(APData.classDeathlinkMessages[player.Class]);
            //Special death messages based on damage source
            //if (GameBase.Instance.Controls.PlayerControls[player.PeerID] is PlayerJoystickControls) //Commented for now as PlayerJoystickControls is inaccessible
            //{
            //    potentialMessages.Add("It was just _'s controller acting up.");
            //}
            if (GameBase.Instance.Difficulty != Difficulty.EASY)
            {
                potentialMessages.Add("_ should've played on an easier difficulty.");
            }
            if (player.Actor != null)
            {
                if (player.Potion == PotionType.Rejuvenation)
                {
                    specialMessages.Add("_ forgot to use their rejuvenation potion.");
                }
            }
            string baseMessage;
            if (specialMessages.Count > 0 && random.Next(4) == 0)
            {
                baseMessage = specialMessages[random.Next(specialMessages.Count)];
            }
            else
            {
                baseMessage = potentialMessages[random.Next(potentialMessages.Count)];
            }
            return baseMessage.Replace("_", player.Name);
        }
        public static void SetTrapLink(bool on)
        {
            if (TrapLink == on) return;
            TrapLink = on;
            connectionInfo.UpdateTags();
            if(!TrapLink)
            {
                trapLinkIndex = 0;
                trapLinkQueue.Clear();
            }
        }
        public static void AddTrapLinkTrapToQueue(string linkedItemName)
        {
            trapLinkQueue.Add(linkedItemName);
        }
        public static void ReceiveTrapLinkItem(string linkedItemName)
        {
            if(APData.trapLinkCustomTraps.Contains(linkedItemName))
            {
                ReceiveCustomTrap(linkedItemName);
            }
            else
            {
                CreateItemInWorld(linkedItemName);
            }
        }
        public static void ReceiveCustomTrap(string customTrapName)
        {
            switch(customTrapName)
            {
                case "home_tp_trap":
                    string startCode = archipelagoData.GetStartExitCode();
                    string[] startSplits = startCode.Split('|');
                    string levelId = startSplits[0];
                    int startId = int.Parse(startSplits[1], CultureInfo.InvariantCulture);
                    GameBase.Instance.ChangeLevel(levelId, startId);
                    ArchipelagoMessageManager.SendHWMessage("Whoosh!");
                    break;
                case "ap_beetle_tp_trap":
                    GameBase.Instance.ChangeLevel("ap_hub", 1);
                    ArchipelagoMessageManager.SendHWMessage("Whoosh!");
                    break;
            }
        }

        public static void KilledEnemy(WorldActor actor)
        {
        }
        public static LootTableWrapper OverrideLootTable(WorldActor actor, LootTableWrapper lootTable)
        {
            if (actor == null || actor.Producer == null || actor.Producer.Name == null) return lootTable;
            string enemyType = actor.Producer.Name;
            if (archipelagoData.GetOption(SlotDataKeys.randomizeEnemyLoot) == 0 && !enemyType.EndsWith("boss_worm_key.xml")) return lootTable;
            if (enemyType.Contains("tower_flower") || enemyType.Contains("tower_nova") || enemyType.Contains("tower_tracking"))
            {
                lootTable.entries.Clear();
                return lootTable;
            }
            int enemyId = actor.NodeId;
            Dictionary<string, Dictionary<int, List<int>>> idDict;
            if (archipelagoData.mapType == ArchipelagoData.MapType.Castle)
            {
                idDict = APData.castleMinibossIdToLocations;
            }
            else
            {
                idDict = APData.templeMinibossIdToLocations;
            }
            List<int> lootLocations = new List<int>();
            bool customLootLocations = false;
            //We have a manual hack for minibosses summoned via script node (the Dune Sharks and the lich in the dragon boss room)
            if (enemyType.EndsWith("boss_worm.xml"))
            {
                int incAmount = 2 * archipelagoData.wormCounter;
                lootLocations = new List<int>() { 582 + incAmount, 583 + incAmount };
                customLootLocations = true;
            }
            else if (enemyType.EndsWith("boss_worm_key.xml"))
            {
                lootLocations = new List<int>() { 590 };
                customLootLocations = true;
            }
            else if (archipelagoData.currentLevelName == "level_boss_4.xml" && enemyType.EndsWith("lich_1_mb.xml"))
            {
                if(archipelagoData.droppedBoss4MinibossLoot)
                {
                    return lootTable;
                }
                lootLocations = new List<int>() { 1323, 1324 };
                customLootLocations = true;
                archipelagoData.droppedBoss4MinibossLoot = true;
            }
            string logString = $"Killed miniboss ({enemyType}) with id: {enemyId}";
            if (enemyType.EndsWith("mb.xml"))
            {
                Logging.Log(logString);
            }
            if (!customLootLocations)
            {
                if (!idDict.TryGetValue(archipelagoData.currentLevelName, out var levelDict) || !levelDict.TryGetValue(enemyId, out lootLocations))
                {
                    if (enemyType.EndsWith("mb.xml"))
                    {
                        Logging.Log($"{logString} that has no valid loot locations!");
                    }
                    return lootTable;
                }
            }

            bool createdPrefab = false;
            List<int> activeLootLocations = new List<int>();
            for (int i = 0; i < lootLocations.Count; i++)
            {
                long locId = APData.GetAdjustedLocationId(lootLocations[i], archipelagoData);
                NetworkItem item = archipelagoData.GetItemFromLoc(locId);
                if (item.Item == -1) continue;

                //bool itemOurs = IsItemOurs(item, false);
                //if(!itemOurs)
                //{
                //    archipelagoData.CheckLocation(locId, true);
                //    if (lootTable.lootTable == null) continue;
                //}

                string lootXmlName = GetItemXmlName(item);
                if (lootXmlName == APData.bombTrapXmlName)
                {
                    createdPrefab = true;
                    ARPGScriptNodeTypeCollection scriptCollection = new ARPGScriptNodeTypeCollection(null, GameBase.Instance.resources);
                    PrefabProducer pref = GameBase.Instance.resources.GetResource<PrefabProducer>("prefabs/bomb_roof_trap.xml");
                    if (pref == null)
                        ResourceContext.Log("ERROR: failed to get bomb trap resource!!!");
                    pref.ProduceInGame(actor.Position, GameBase.Instance.resources, GameBase.Instance.world, scriptCollection);
                    archipelagoData.CheckLocation(locId, true);
                    continue;
                }
                else if(lootXmlName == APData.chaserTrapXmlName)
                {
                    lootXmlName = APData.chaserTrapActorXmlName;
                }
                else if (lootXmlName == APData.SPEECH_TRAP_XML_NAME)
                {
                    GameInterface.StartSpeechTrap();
                    archipelagoData.CheckLocation(locId, true);
                    continue;
                }
                if (!APData.IsItemXmlNameCorporeal(lootXmlName))
                {
                    AddPickupMessageToQueue(item);
                    PickupItemEffects(item, false);
                    archipelagoData.CheckLocation(locId, true);
                    continue;
                }

                IWorldItemProducer itemProducer = GameBase.Instance.resources.GetResource<IWorldItemProducer>(lootXmlName);
                if (itemProducer == null)
                {
                    ResourceContext.Log($"Failed to get WorldItemProducer for item {item.Item}, xml name {lootXmlName}");
                }
                else
                {
                    lootTable.entries.Add(new Tuple<int, List<Tuple<int, IWorldItemProducer>>>(1000, new List<Tuple<int, IWorldItemProducer>>() { new Tuple<int, IWorldItemProducer>(1000, itemProducer) }));
                    activeLootLocations.Add((int)locId);
                }
            }
            if (lootTable.lootTable == null)// || (lootTable.lootAPLocations.Count == 0 && !createdPrefab && !customLootLocations))
                return lootTable;

            //Remove existing item spawn entries
            int entriesToRemove = 2;
            if ((enemyType.StartsWith("actors/boss_") || enemyType.EndsWith("mummy_1_mb.xml")) && !enemyType.StartsWith("actors/boss_worm"))
                entriesToRemove = 1;
            for (int r = 0; r < entriesToRemove; r++)
            {
                lootTable.entries.RemoveAt(lootTable.entries.Count - activeLootLocations.Count - 1);
            }
            archipelagoData.lootTableAPLocationIDs.Add(lootTable.lootTable, activeLootLocations);

            return lootTable;
        }

        public static void CompleteGoal()
        {
            connectionInfo.SendGoal();
            archipelagoData.completedGoal = true;
            GameInterface.SetGlobalFlag("goal");
        }
        public static void CompleteGoal(ArchipelagoData.GoalType winType)
        {
            ResourceContext.Log($"Completed goal type: {winType}");
            if (winType == archipelagoData.goalType)
            {
                CompleteGoal();
            }
        }
        public static void ShowEndGameScreen(ArchipelagoData.GoalType winType)
        {
            //Create the end game screen for certain goals
            bool showEndGameScreen = true;
            string endGameScreenHeader = "e.win-congrats";
            string endGameScreenMessage = "";
            switch (archipelagoData.goalType)
            {
                case ArchipelagoData.GoalType.BeatAllBosses:
                    endGameScreenMessage = "You killed all the bosses";
                    endGameScreenMessage += archipelagoData.mapType == ArchipelagoData.MapType.Castle ? " in the castle!" : " around the temple!";
                    break;
                case ArchipelagoData.GoalType.PlankHunt:
                    endGameScreenMessage = "You collected the required strange planks";
                    endGameScreenMessage += archipelagoData.mapType == ArchipelagoData.MapType.Castle ? " and repaired the bridge!" : "!";
                    break;
                case ArchipelagoData.GoalType.Alternate:
                    endGameScreenMessage = "You completed the Pyramid of Fear!";
                    break;
                default:
                    showEndGameScreen = false;
                    break;
            }
            if (showEndGameScreen)
            {
                if (!Network.IsClient)
                {
                    GameInterface.ShowEndGameScreen(endGameScreenHeader, endGameScreenMessage);
                }
                SoundHelper.PlayCountdownFinishSound();
            }
        }

        public static void AddTranslatedMessageToQueue(string translationKey)
        {
            string message = ArchipelagoMessageManager.GetLanguageString(translationKey, new string[0]);
            ArchipelagoMessageManager.announceMessageQueue.Add(message);
        }
        public static void AddMessageToQueue(string message)
        {
            ArchipelagoMessageManager.announceMessageQueue.Add(message);
        }
        public static void AddPickupMessageToQueue(NetworkItem item)
        {
            if (!IsItemOurs(item, false))
            {
                string popupMessage = GetSendItemMessage(item);
                ArchipelagoMessageManager.pickupTextQueue.Add(popupMessage);
            }
        }

        public static string GetReceiveItemName(NetworkItem item)
        {
            return GetItemName(item.Item);
        }
        public static string GetItemName(NetworkItem item)
        {
            return connectionInfo.session.Items.GetItemName(item.Item, archipelagoData.playerGames[item.Player]);
        }
        public static string GetItemName(long itemId, string gameName = "Hammerwatch")
        {
            //TODO: probably convert this to just call session.Items.GetItemName
            if (gameData.TryGetValue(gameName, out GameData game))
            {
                foreach (KeyValuePair<string, long> pair in game.ItemLookup)
                {
                    if (pair.Value == itemId)
                    {
                        return pair.Key;
                    }
                }
            }

            return connectionInfo.session.Items.GetItemName(itemId, gameName);
        }
        public static string GetSendItemMessageFromPos(Vector2 pos)
        {
            NetworkItem item = archipelagoData.GetItemFromPos(pos, archipelagoData.currentLevelName);
            return GetSendItemMessage(item);
        }
        public static string GetSendItemMessageFromLoc(long loc)
        {
            NetworkItem item = archipelagoData.GetItemFromLoc(loc);
            return GetSendItemMessage(item);
        }
        public static string GetSendItemMessage(NetworkItem item)
        {
            if (item.Item == -1)
                return "";
            string itemName = GetItemName(item);
            string playerName = connectionInfo.GetPlayerName(item.Player);
            string playerString = item.Player == connectionInfo.playerId ? "" : $"{playerName}'s ";
            string endString = GetItemMessageEndString(item);
            return $"{playerString}{itemName}{endString}";
        }
        public static string GetReceiveItemMessage(NetworkItem item)
        {
            var playerInfo = connectionInfo.session.Players.Players[connectionInfo.playerTeam][item.Player];
            string itemName = GetReceiveItemName(item);
            string playerName = playerInfo.Alias;
            string endString = GetItemMessageEndString(item);
            return $"Received {itemName} from {playerName}{endString}";
        }
        public static string GetItemMessageEndString(NetworkItem item)
        {
            string endString = "";
            if (item.Flags.HasFlag(ItemFlags.Trap))
                endString += "...";
            if (item.Flags.HasFlag(ItemFlags.Advancement))
                endString += "!";
            return endString;
        }

        public static bool IsRandomized(string xmlName)
        {
            if (!APData.itemNameToXML.ContainsValue(xmlName)) return false;
            if (xmlName.StartsWith("items/valuable_") && !xmlName.StartsWith("items/valuable_d")) return false; //Don't randomize coins normally, diamonds are ok
            return true;

        }
        public static bool IsItemOurs(NetworkItem item, bool receive)
        {
            return receive || item.Player == connectionInfo.playerId;
        }

        public static string GetItemXmlName(NetworkItem item, bool receive = false, bool trueItem = false)
        {
            if (connectionInfo.IsPlayerPlayingSameGame(item.Player) || receive)
            {
                string itemName = receive ? GetReceiveItemName(item) : GetItemName(item);
                if (!APData.itemNameToXML.TryGetValue(itemName, out string xmlName))
                {
                    if (APData.IsItemButton(item.Item))
                    {
                        if (itemName.Contains("Boss Rune"))
                        {
                            string nonProgressName = itemName.Replace(" Progress", "");
                            switch (nonProgressName[nonProgressName.Length - 1])
                            {
                                case 'y':
                                case 'Y':
                                    xmlName = APData.bossRune1ItemXmlName;
                                    break;
                                case 'n':
                                case 'N':
                                    xmlName = APData.bossRune2ItemXmlName;
                                    break;
                                case 'A':
                                    xmlName = APData.bossRune3ItemXmlName;
                                    break;
                                default: //This actually isn't a boss rune!
                                    xmlName = APData.buttonEffectItemXmlName;
                                    break;
                            }
                        }
                        else
                        {
                            xmlName = APData.buttonEffectItemXmlName;
                        }
                    }
                    else if (APData.IsItemShopUpgrade(item.Item))
                    {
                        xmlName = APData.GetShopItemXml((int)item.Item);
                    }
                    else if (APData.IsItemFloorMasterKey(item.Item)) //Floor master keys
                    {
                        string[] nameSplits = itemName.Split(' ');
                        string masterKeyName = $"Master {nameSplits[nameSplits.Length - 2]} Key";
                        xmlName = APData.itemNameToXML[masterKeyName];
                    }
                }
                //Get the holo effect for a Hammerwatch item
                if (!IsItemOurs(item, receive) && !trueItem)
                {
                    return APData.GetAPHoloXmlItemFromItemFlags(item.Flags, xmlName);
                }
                if (xmlName == null)
                {
                    ResourceContext.Log($"xmlName was null for item {itemName}!");
                }
                return xmlName;
            }
            string apXmlItemName = GetRemoteItemXmlName(item);
            if (!IsItemOurs(item, receive) && !trueItem)
            {
                return APData.GetAPHoloXmlItemFromItemFlags(item.Flags, apXmlItemName);
            }
            return apXmlItemName;
        }
        private static string GetRemoteItemXmlName(NetworkItem item)
        {
            string itemGame = connectionInfo.GetPlayerGame(item.Player);
            string itemName = GetItemName(item);
            if (remoteItemToXmlNameCache.ContainsKey(itemGame))
            {
                if (remoteItemToXmlNameCache[itemGame].TryGetValue(item.Item, out string cachedXmlName))
                    return cachedXmlName;
            }
            else
            {
                remoteItemToXmlNameCache[itemGame] = new Dictionary<long, string>();
            }
            if (activeGameFuzzyItemNameToXMLDict.ContainsKey(itemGame))
            {
                foreach (var keywords in activeGameFuzzyItemNameToXMLDict[itemGame])
                {
                    if (itemName.Contains(keywords[0]))
                    {
                        remoteItemToXmlNameCache[itemGame][item.Item] = keywords[1];
                        return keywords[1];
                    }
                }
            }
            foreach (var keywords in activeGameFuzzyItemNameToXMLDict[""])
            {
                if (itemName.Contains(keywords[0]))
                {
                    remoteItemToXmlNameCache[""][item.Item] = keywords[1];
                    return keywords[1];
                }
            }
            //string itemNameWithoutParens = itemName.Replace("(", "").Replace(")", "");
            //string[] itemWords = itemNameWithoutParens.Split(new char[] { ' ', '_', '-' });
            //foreach (var keywords in APData.fuzzyItemWordToXML)
            //{
            //    if (itemWords.Contains(keywords.Item1))
            //    {
            //        remoteItemToXmlNameCache[itemGame][item.Item] = keywords.Item2;
            //        return keywords.Item2;
            //    }
            //}
            return APData.GetAPXmlItemFromItemFlags(item.Flags);
        }

        public static void CreateItemInWorld(NetworkItem item, bool receivedFromServer)
        {
            string itemName = GetReceiveItemName(item);

            bool remote = !IsHostPlayer();

            PickupItemEffects(item, receivedFromServer, remote);

            if (!APData.itemNameToXML.ContainsKey(itemName))
            {
                //This is likely a button or a master key, skip spawning it
                if (APData.IsItemFloorMasterKey(item.Item) && !remote)
                {
                    TiltedEngine.Audio.Sound keySound;
                    if (itemName.Contains("Bonus"))
                    {
                        keySound = GameBase.Instance.resources.GetResource<TiltedEngine.Audio.Sound>("sound/bonus.xml:bonus_key");
                    }
                    else
                    {
                        keySound = GameBase.Instance.resources.GetResource<TiltedEngine.Audio.Sound>("sound/misc.xml:take_key");
                    }
                    keySound.Play2D();
                }
                return;
            }

            //If we're not the host, don't spawn the item but pretend like we picked it up
            if (remote)
            {
                PickupItemEffectsXml(GetItemXmlName(item, receivedFromServer), receivedFromServer);
                return;
            }

            CreateItemInWorld(itemName);
        }
        public static void CreateItemInWorld(string itemName)
        {
            //If this is an item with a custom effect don't create the item
            if(itemName == APData.SPEECH_TRAP_NAME)
            {
                GameInterface.StartSpeechTrap();
                return;
            }

            Vector2 spawnPos;
            //Determine player who needs the item the most
            List<PlayerInfo> players = GameBase.Instance.Players.Where(playerInfo => playerInfo != null).ToList();
            List<PlayerInfo> allAvailablePlayers = new List<PlayerInfo>(players);
            if (itemName.Contains("Potion"))
            {
                foreach (PlayerInfo player in GameBase.Instance.Players)
                {
                    if (player.Potion != PotionType.None)
                    {
                        players.Remove(player);
                    }
                }
            }
            if (APData.foodItemNames.Contains(itemName))
            {
                foreach (PlayerInfo player in GameBase.Instance.Players)
                {
                    if (player.Health == player.MaxHealth)
                    {
                        players.Remove(player);
                    }
                }
            }
            if (itemName.Contains("Mana"))
            {
                foreach (PlayerInfo player in GameBase.Instance.Players)
                {
                    if (player.Mana == player.MaxMana)
                    {
                        players.Remove(player);
                    }
                }
            }

            PlayerInfo defaultPlayer;
            if (players.Count > 0)
            {
                defaultPlayer = players[random.Next(players.Count)];
            }
            else
            {
                defaultPlayer = allAvailablePlayers[random.Next(allAvailablePlayers.Count)];
            }

            WorldActor playerActor = defaultPlayer?.Actor;
            spawnPos = playerActor?.Position ?? GameBase.Instance.GetSpawnPos(defaultPlayer); //Null coalescing operator, accesses Position if not null, returns spawn pos if it is

            //Spawn an item or a prefab
            if (itemName == "Bomb Trap")
            {
                ARPGScriptNodeTypeCollection scriptCollection = new ARPGScriptNodeTypeCollection(null, GameBase.Instance.resources);
                PrefabProducer pref = GameBase.Instance.resources.GetResource<PrefabProducer>("prefabs/bomb_roof_trap.xml");
                if (pref == null)
                    ResourceContext.Log("ERROR: failed to get bomb trap resource!!!");
                pref.ProduceInGame(spawnPos, GameBase.Instance.resources, GameBase.Instance.world, scriptCollection);
                QoL.ResetExploreSpeed(defaultPlayer);
            }
            else if (itemName == "Chaser Trap")
            {
                spawnPos = GameBase.Instance.GetSpawnPos(defaultPlayer);
                ActorType actorType = GameBase.Instance.resources.GetResource<ActorType>(APData.chaserTrapActorXmlName);
                WorldObject worldActor = actorType.Produce(spawnPos);
                GameBase.Instance.world.Place(spawnPos.X, spawnPos.Y, worldActor, true);
                Network.SendToAll("SpawnUnit", new object[]
                {
                    ResourceBank.PathHash(actorType.Name),
                    worldActor.NodeId,
                    spawnPos
                });
            }
            else
            {
                ItemType type;
                if (APData.trapNameToXML.ContainsKey(itemName))
                {
                    spawnPos += new Vector2(0, .25f);
                    QoL.ResetExploreSpeed(defaultPlayer);
                }
                if (itemName == "Random Stat Upgrade")
                {
                    string[] upgrades = { "Damage Upgrade", "Defense Upgrade", "Health Upgrade", "Mana Upgrade" };
                    itemName = upgrades[random.Next(upgrades.Length)];
                }
                //Trap items just create the chest on top of you, easier this way
                type = GameBase.Instance.resources.GetResource<ItemType>(APData.itemNameToXML[itemName]);
                if (type == null)
                {
                    ResourceContext.Log($"ERROR: failed to get item resource for item {itemName}!!!");
                    return;
                }
                WorldObject worldItem = type.Produce(spawnPos);
                GameBase.Instance.world.Place(spawnPos.X, spawnPos.Y, worldItem, true);
                Network.SendToAll("SpawnItem", new object[]
                {
                    ResourceBank.PathHash(type.Name),
                    worldItem.NodeId,
                    spawnPos
                });
                worldItem.Hit(playerActor, 1, null);
            }
        }
        public static bool PickupItemEffects(Vector2 pos)
        {
            NetworkItem item = archipelagoData.GetItemFromPos(pos, archipelagoData.currentLevelName);
            if (item.Item == -1) return false;
            PickupItemEffects(item, false);
            return true;
        }
        public static bool PickupItemEffects(long locationId)
        {
            NetworkItem item = archipelagoData.GetItemFromLoc(locationId);
            if (item.Item == -1) return false;
            PickupItemEffects(item, false);
            return true;
        }
        public static void PickupItemEffectsXml(string xmlName, bool receive) //For item effects based on the xml name of the item
        {
            archipelagoData.PickupItemEffectsXml(xmlName, receive);
        }
        //Should only be called on static items or when receiving an item from the server (NOT from enemy loot)
        public static void PickupItemEffects(NetworkItem item, bool receiveFromServer, bool silent = false)
        {
            //If this is someone else's item in our world don't run this
            if (!IsItemOurs(item, receiveFromServer))
                return;

            string itemName = receiveFromServer ? GetReceiveItemName(item) : GetItemName(item);
            string xmlName = GetItemXmlName(item, receiveFromServer);

            if (xmlName == APData.buttonEffectItemXmlName)
            {
                //Logging.GameLog($"receive {receiveFromServer} | silent {silent}");
                if (!receiveFromServer && !silent)
                {
                    AddMessageToQueue(APData.GetButtonEventName(itemName));
                }

                GameInterface.SetGlobalFlag(itemName, true);
                EventSystem.Instance.FireEvent(BUTTON_EVENT_NAME);
            }

            //Button effect specific
            string progressBaseName = itemName.Replace(" Progress", "");
            if (archipelagoData.mapType == ArchipelagoData.MapType.Castle)
            {
                if (itemName.Contains("Boss Rune"))
                {
                    string[] runeWords = progressBaseName.Split(' ');
                    int bossItemActIndex = APData.GetActIndexFromActName(runeWords[0], archipelagoData);
                    if (bossItemActIndex != -1)
                    {
                        int bossRuneIndex = -1;
                        int bossRuneProgressCount = progressBaseName == itemName ? 1 : 4;
                        switch (runeWords[3])
                        {
                            case "y":
                            case "Y":
                                bossRuneIndex = 0;
                                break;
                            case "n":
                            case "N":
                                bossRuneIndex = 1;
                                break;
                            case "a":
                            case "A":
                                bossRuneIndex = 2;
                                break;
                        }
                        int levelIndex = 3 * bossItemActIndex + bossRuneIndex;
                        string flagName = $"a{bossItemActIndex + 1}l{levelIndex + 1}_boss{bossItemActIndex + 1}";
                        archipelagoData.bossRunes[levelIndex] += 1;
                        if (archipelagoData.bossRunes[levelIndex] >= bossRuneProgressCount)
                        {
                            GameInterface.SetGlobalFlag(flagName, true);
                            EventSystem.Instance.FireEvent(BUTTON_EVENT_NAME);
                            if (!silent)
                                AddMessageToQueue(APData.GetButtonEventName(progressBaseName));
                        }
                    }
                }
            }
            else
            {
                int newPofRaiseLevel = archipelagoData.pofRaiseLevel;
                switch (itemName)
                {
                    case "Elevate PoF Pyramid":
                        newPofRaiseLevel += 4;
                        break;
                    case "Elevate PoF Pyramid Progress":
                        newPofRaiseLevel += 1;
                        break;
                }
                if (newPofRaiseLevel != archipelagoData.pofRaiseLevel)
                {
                    int pyramids = newPofRaiseLevel / 4;
                    string[] pyramidFlags = new[] {
                        "puzzle_bonus_solved_c1",
                        "puzzle_bonus_solved_c2",
                        "puzzle_bonus_solved_c3",
                        "puzzle_bonus_solved_t1",
                        "puzzle_bonus_solved_t2",
                        "puzzle_bonus_solved_hub",
                    };
                    for (int p = 0; p < pyramids; p++)
                    {
                        GameInterface.SetGlobalFlag(pyramidFlags[p], true);
                    }
                    archipelagoData.pofRaiseLevel = newPofRaiseLevel;
                    EventSystem.Instance.FireEvent("pyramid_update");
                }
            }

            if (itemName.Contains("Master") || itemName.StartsWith("Dune Sharks Arena"))
            {
                if (!silent)
                    ArchipelagoMessageManager.pickupTextQueue.Add(itemName + "!");
                string[] nameSplits = itemName.Split(' ');
                if (int.TryParse(nameSplits[2], out int floorIndex))
                {
                    floorIndex -= 1;
                }
                int keyIndex = 0;
                switch (nameSplits[nameSplits.Length - 2])
                {
                    case "Bronze":
                        keyIndex = 0;
                        break;
                    case "Silver":
                        keyIndex = 1;
                        break;
                    case "Gold":
                        keyIndex = 2;
                        break;
                    case "Bonus":
                        keyIndex = 3;
                        break;
                }
                if (archipelagoData.mapType == ArchipelagoData.MapType.Castle)
                {
                    if (keyIndex == 3)
                    {
                        floorIndex = APData.GetActIndexFromActName(nameSplits[0], archipelagoData);
                    }
                }
                else
                {
                    if (itemName.StartsWith("Dune Sharks Arena"))
                    {
                        floorIndex = 2; //There are no gates on the third floor
                    }
                    else if (itemName.StartsWith("Pyramid of Fear"))
                    {
                        floorIndex = 3;
                    }

                }
                archipelagoData.hasMasterFloorKeys[keyIndex, floorIndex] = true;
            }

            //Shop upgrade handling
            if (APData.IsItemShopUpgrade(item.Item))
            {
                SyncUpgrades();
                if (!silent)
                    ArchipelagoMessageManager.ShowPickupMessage(itemName);
            }
        }

        public static void AddDynamicLocation(int itemNodeID, int locationID)
        {
            if (!archipelagoData.dynamicItemLocations.ContainsKey(archipelagoData.currentLevelName))
                archipelagoData.dynamicItemLocations[archipelagoData.currentLevelName] = new Dictionary<int, int>();
            archipelagoData.dynamicItemLocations[archipelagoData.currentLevelName][itemNodeID] = locationID;
        }
        public static int GetDynamicLocation(int itemNodeID)
        {
            if (!archipelagoData.dynamicItemLocations.TryGetValue(archipelagoData.currentLevelName, out Dictionary<int, int> levelDynamicLocs))
            {
                //Logging.Log($"Dynamic location doesn't exist for current level (item node ID: {itemNodeID})!");
                return -1;
            }
            if (!levelDynamicLocs.TryGetValue(itemNodeID, out int dynamicLoc))
            {
                //Logging.Log($"Dynamic location doesn't exist for item node ID: {itemNodeID}!");
                return -1;
            }
            levelDynamicLocs.Remove(itemNodeID);
            return dynamicLoc;
        }

        public static void OutputError(Exception e)
        {
            ResourceContext.OutputError($"UNHANDLED EXCEPTION (GENERATION)", e.ToString());
            SDL.SDL_ShowSimpleMessageBox(SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR, "Crash! :(", ERROR_MESSAGE, IntPtr.Zero);
            Environment.FailFast("Exception occured while generating Archipelago map file", e);
        }

        public class GenerateInfo
        {
            public bool gameReady;
            public bool saveIsLoaded;

            public string saveFileName;

            public GenerateState generateState;

            public GenerateInfo()
            {
            }

            public void SetGameReady(bool ready = true)
            {
                gameReady = ready;
            }
            public void Reset()
            {
                gameState = GameState.MainMenu;
                gameReady = false;
                saveIsLoaded = false;
            }

            public enum GenerateState
            {
                FinishGenerate,
                GenerateError,
            }
        }

        public enum GameState
        {
            MainMenu,
            Connecting,
            StartGenerate,
            Generating,
            FailedGenerate,
            Generated,
            Lobby,
            InGame,
        }
    }
}
