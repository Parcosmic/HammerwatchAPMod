using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ARPGGame;
using TiltedEngine;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Enums;
using HammerwatchAP.Util;

namespace HammerwatchAP.Archipelago
{
    public static class APSaveManager
    {
        static readonly Type _type_GameSaver = HammerwatchAP.hammerwatchAssembly.GetType("ARPGGame.GameSaver");
        static readonly MethodInfo _mi_GameSaver_LoadGame = _type_GameSaver.GetMethod("LoadGame", BindingFlags.Public | BindingFlags.Static);

        public static void LoadGame(string saveName)
        {
            _mi_GameSaver_LoadGame.Invoke(null, new object[] { GameBase.Instance, saveName });
        }

        public static bool SaveExists(string mapFileName)
        {
            return File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "levels", mapFileName));
        }

        public static bool LoadSave(SObject save)
        {
            //loadedSave = true;
            ConnectionInfo connectionData = ArchipelagoManager.connectionInfo;
            ArchipelagoData archipelagoData = ArchipelagoManager.archipelagoData;
            ArchipelagoManager.playingArchipelagoSave = save.Get("ap").GetBoolean();
            string loadedIp = save.Get("ap-ip").GetString();
            string loadedSeed = save.Get("ap-seed").GetString();
            if (archipelagoData.seed != null && archipelagoData.seed != loadedSeed)
                return false;
            connectionData.ip = loadedIp;
            archipelagoData.seed = loadedSeed;
            string loadedSlotName = save.Get("ap-slot-name").GetString();
            if (connectionData.slotName != null && connectionData.slotName != loadedSlotName)
                return false;
            connectionData.slotName = loadedSlotName;
            connectionData.apPassword = save.Get("ap-password").GetString();
            archipelagoData.mapType = (ArchipelagoData.MapType)save.Get("ap-map").GetInteger();
            archipelagoData.completedGoal = save.Get("ap-goal").GetBoolean();
            if (archipelagoData.completedGoal) ArchipelagoManager.CompleteGoal();
            archipelagoData.itemsReceived = save.Get("ap-items-received").GetInteger();
            SValue[] locations = save.Get("ap-checked-locations").GetArray();
            foreach (SValue loc in locations)
            {
                int savedCheckedLocation = loc.GetInteger();
                if (archipelagoData.checkedLocations.Contains(savedCheckedLocation)) continue;
                archipelagoData.checkedLocations.Add(savedCheckedLocation);
            }

            archipelagoData.planks = save.Get("ap-planks").GetInteger();
            if (archipelagoData.mapType == ArchipelagoData.MapType.Castle)
            {
                SValue bossRunesValue = save.Get("ap-boss-runes");
                if (!bossRunesValue.IsNull())
                    archipelagoData.bossRunes = bossRunesValue.GetIntegerArray();
            }
            else
            {
                archipelagoData.panFragments = save.Get("ap-pan-fragments").GetInteger();
                archipelagoData.leverFragments = save.Get("ap-lever-fragments").GetInteger();
                archipelagoData.pickaxeFragments = save.Get("ap-pickaxe-fragments").GetInteger();
                archipelagoData.totalPanFragments = save.Get("ap-total-pan-fragments").GetInteger();
                archipelagoData.totalLeverFragments = save.Get("ap-total-lever-fragments").GetInteger();
                archipelagoData.totalPickaxeFragments = save.Get("ap-total-pickaxe-fragments").GetInteger();
                archipelagoData.pofRaiseLevel = SaveTryGetInteger(save, "ap-pof-raise-level");
                archipelagoData.wormCounter = SaveTryGetInteger(save, "ap-worm-counter");
            }
            archipelagoData.hammerFragments = SaveTryGetInteger(save, "ap-hammer-fragments");
            archipelagoData.totalHammerFragments = SaveTryGetInteger(save, "ap-total-hammer-fragments");
            archipelagoData.killedBosses = new bool[archipelagoData.GetActCount()];

            SValue[] actKeysValue = save.Get("ap-act-keys").GetArray();
            for (int a = 0; a < actKeysValue.Length; a++)
            {
                int[] keysValues = actKeysValue[a].GetIntegerArray();
                for (int k = 0; k < 4; k++)
                {
                    archipelagoData.actKeys[a, k] = keysValues[k];
                }
            }
            SValue floorMasterKeysBaseValue = save.Get("ap-floor-master-keys");
            if (floorMasterKeysBaseValue.IsNull())
            {
                archipelagoData.hasMasterFloorKeys = new bool[4, 12];
            }
            else
            {
                SValue[] floorMasterKeysValue = floorMasterKeysBaseValue.GetArray();
                for (int a = 0; a < floorMasterKeysValue.Length; a++)
                {
                    int[] keysValues = floorMasterKeysValue[a].GetIntegerArray();
                    for (int k = 0; k < keysValues.Length; k++)
                    {
                        archipelagoData.hasMasterFloorKeys[a, k] = keysValues[k] > 0;
                    }
                }
            }

            //Only load checksums if they haven't been loaded already
            if (ArchipelagoManager.gameChecksums == null || ArchipelagoManager.gameChecksums.Count == 0)
            {
                ArchipelagoManager.gameChecksums = new Dictionary<string, string>();
                SValue gameNameValue = save.Get("ap-game-names");
                if (!gameNameValue.IsNull())
                {
                    SValue[] gameNames = gameNameValue.GetArray();
                    SValue[] checksums = save.Get("ap-checksums").GetArray();
                    for (int g = 0; g < gameNames.Length; g++)
                    {
                        ArchipelagoManager.gameChecksums[gameNames[g].GetString()] = checksums[g].GetString();
                    }
                }
                if (!ArchipelagoManager.LoadDatapackage())
                {
                    //Have an issue, we're trying to load the datapackage offline and are missing the right files!
                    GameBase.Instance.SetMenu(MenuType.MESSAGE, "Datapackage Error", "Failed to load the datapackage, connect to the server to update!");
                }
            }

            SValue saveModVersion = save.Get("ap-save-mod-version");
            switch (saveModVersion.GetString())
            {
                default:
                    break;
            }

            SValue[] playerGame = save.Get("ap-player-games").GetArray();
            for (int p = 0; p < playerGame.Length; p++)
                archipelagoData.playerGames[p] = playerGame[p].GetString();

            archipelagoData.locationToItem = new Dictionary<long, NetworkItem>();
            SValue[] itemIds = save.Get("ap-item-ids").GetArray();
            SValue[] itemLocationIds = save.Get("ap-item-location-ids").GetArray();
            SValue[] itemPlayers = save.Get("ap-item-players").GetArray();
            SValue[] itemClassifications = save.Get("ap-item-flags").GetArray();
            for (int i = 0; i < itemIds.Length; i++)
            {
                NetworkItem item = new NetworkItem
                {
                    Item = long.Parse(itemIds[i].GetString()),
                    Location = long.Parse(itemLocationIds[i].GetString()),
                    Player = itemPlayers[i].GetInteger(),
                    Flags = (ItemFlags)itemClassifications[i].GetInteger()
                };
                archipelagoData.locationToItem[item.Location] = item;
            }

            if (!connectionData.ConnectionActive)
            {
                connectionData.StartConnection(archipelagoData);
                if (!connectionData.connectedToAP)
                {
                    GameBase.Instance.SetMenu(MenuType.MESSAGE, "Connection Error", "Failed to connect to Archipelago server");
                    ArchipelagoManager.DisconnectFromArchipelago();
                    GameBase.Instance.ResetGame(false);
                }
            }
            else
            {
                //We're already connected, but sync our loaded checked locations with the server here
                connectionData.SendCheckedLocations(archipelagoData);
            }
            return true;
        }
        private static int SaveTryGetInteger(SObject save, string key)
        {
            SValue saveInt = save.Get(key);
            if (saveInt.IsNull()) return 0;
            return saveInt.GetInteger();
        }
        public static void SaveGame(string saveName, SObject save)
        {
            ConnectionInfo connectionData = ArchipelagoManager.connectionInfo;
            ArchipelagoData archipelagoData = ArchipelagoManager.archipelagoData;
            archipelagoData.saveFileName = saveName;
            save.Set("ap", true);
            save.Set("ap-ip", connectionData.ip);
            save.Set("ap-seed", archipelagoData.seed);
            save.Set("ap-slot-name", connectionData.slotName);
            save.Set("ap-password", connectionData.apPassword);
            save.Set("ap-map", (int)archipelagoData.mapType);
            save.Set("ap-goal", archipelagoData.completedGoal);
            save.Set("ap-items-received", archipelagoData.itemsReceived);
            SValue[] checkedLocations = new SValue[archipelagoData.checkedLocations.Count];
            for (int c = 0; c < checkedLocations.Length; c++)
            {
                checkedLocations[c] = (int)archipelagoData.checkedLocations[c];
            }
            save.Set("ap-checked-locations", checkedLocations);
            save.Set("ap-planks", archipelagoData.planks);

            if (archipelagoData.mapType == ArchipelagoData.MapType.Castle)
            {
                save.Set("ap-boss-runes", archipelagoData.bossRunes);
            }
            else
            {
                save.Set("ap-pan-fragments", archipelagoData.panFragments);
                save.Set("ap-lever-fragments", archipelagoData.leverFragments);
                save.Set("ap-pickaxe-fragments", archipelagoData.pickaxeFragments);
                save.Set("ap-total-pan-fragments", archipelagoData.totalPanFragments);
                save.Set("ap-total-lever-fragments", archipelagoData.totalLeverFragments);
                save.Set("ap-total-pickaxe-fragments", archipelagoData.totalPickaxeFragments);
                save.Set("ap-pof-raise-level", archipelagoData.pofRaiseLevel);
                save.Set("ap-worm-counter", archipelagoData.wormCounter);
            }
            save.Set("ap-hammer-fragments", archipelagoData.hammerFragments);
            save.Set("ap-total-hammer-fragments", archipelagoData.totalHammerFragments);
            SValue[] actKeysValues = new SValue[4];
            for (int a = 0; a < 4; a++)
            {
                int[] keys = new int[4];
                for (int k = 0; k < 4; k++)
                {
                    keys[k] = archipelagoData.actKeys[a, k];
                }
                actKeysValues[a] = new SValue(keys);
            }
            save.Set("ap-act-keys", actKeysValues);
            SValue[] floorMasterKeysValues = new SValue[4];
            for (int a = 0; a < archipelagoData.hasMasterFloorKeys.GetLength(0); a++)
            {
                int[] floors = new int[archipelagoData.hasMasterFloorKeys.GetLength(1)];
                for (int k = 0; k < floors.Length; k++)
                {
                    floors[k] = archipelagoData.hasMasterFloorKeys[a, k] ? 1 : 0;
                }
                floorMasterKeysValues[a] = new SValue(floors);
            }
            save.Set("ap-floor-master-keys", floorMasterKeysValues);
            SValue[] gameNames = new SValue[ArchipelagoManager.gameChecksums.Count];
            SValue[] checksums = new SValue[ArchipelagoManager.gameChecksums.Count];
            int gameCounter = 0;
            foreach (string game in ArchipelagoManager.gameChecksums.Keys)
            {
                gameNames[gameCounter] = game;
                checksums[gameCounter++] = ArchipelagoManager.gameChecksums[game];
            }
            save.Set("ap-game-names", gameNames);
            save.Set("ap-checksums", checksums);

            save.Set("ap-save-mod-version", ArchipelagoManager.MOD_VERSION.ToString());

            //Save player info and scouted items
            SValue[] playerGame = new SValue[archipelagoData.playerGames.Length];
            for (int p = 0; p < archipelagoData.playerGames.Length; p++)
                playerGame[p] = archipelagoData.playerGames[p];
            save.Set("ap-player-games", playerGame);

            SValue[] itemIds = new SValue[archipelagoData.locationToItem.Count];
            SValue[] itemLocationIds = new SValue[archipelagoData.locationToItem.Count];
            SValue[] itemPlayers = new SValue[archipelagoData.locationToItem.Count];
            SValue[] itemClassifications = new SValue[archipelagoData.locationToItem.Count];
            int itemSaveCounter = 0;
            foreach (NetworkItem item in archipelagoData.locationToItem.Values)
            {
                itemIds[itemSaveCounter] = item.Item.ToString();
                itemLocationIds[itemSaveCounter] = item.Location.ToString();
                itemPlayers[itemSaveCounter] = item.Player;
                itemClassifications[itemSaveCounter] = (int)item.Flags;
                itemSaveCounter++;
            }
            save.Set("ap-item-ids", itemIds);
            save.Set("ap-item-location-ids", itemLocationIds);
            save.Set("ap-item-players", itemPlayers);
            save.Set("ap-item-flags", itemClassifications);

            Logging.Debug("Saved Archipelago data");
        }
        public static APSaveDataInfo LoadSaveInfo(string saveFileName)
        {
            string[] saveFiles = Directory.GetFiles("saves");
            using (MemoryStream memStream = new MemoryStream())
            {
                foreach (string saveFile in saveFiles)
                {
                    if (saveFile != saveFileName) continue;
                    try
                    {
                        string saveIp;
                        string saveSeed;
                        string saveSlotName;
                        using (FileStream fileStream = File.Open(saveFile, FileMode.Open, FileAccess.Read))
                        {
                            memStream.Position = 0L;
                            fileStream.CopyTo(memStream);
                            memStream.Position = 0L;
                            BinaryReader reader = new BinaryReader(memStream);
                            saveIp = SValue.FindDictionaryEntry(reader, "ap-ip").GetString();
                            memStream.Position = 0L;
                            saveSeed = SValue.FindDictionaryEntry(reader, "ap-seed").GetString();
                            memStream.Position = 0L;
                            saveSlotName = SValue.FindDictionaryEntry(reader, "ap-slot-name").GetString();
                        }
                        return new APSaveDataInfo(saveIp, saveSeed, saveSlotName);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            return new APSaveDataInfo();
        }
        public static string GetLatestSaveNameWithConnectionInfo(string gameIp, string gameSeed, string gameSlotName)
        {
            string[] saveFiles = Directory.GetFiles("saves");
            string saveName = null;
            DateTime latestSaveTime = DateTime.MinValue;
            bool portMatch = false;
            string gamePort = gameIp.Substring(gameIp.Length - 5);
            using (MemoryStream memStream = new MemoryStream())
            {
                foreach (string saveFile in saveFiles)
                {
                    try
                    {
                        string saveIp;
                        string savePort;
                        string saveSeed;
                        string saveSlotName;
                        using (FileStream fileStream = File.Open(saveFile, FileMode.Open, FileAccess.Read))
                        {
                            memStream.Position = 0L;
                            fileStream.CopyTo(memStream);
                            memStream.Position = 0L;
                            BinaryReader reader = new BinaryReader(memStream);
                            SValue apValue = SValue.FindDictionaryEntry(reader, "ap");
                            if (apValue == null) continue;
                            memStream.Position = 0L;
                            saveIp = SValue.FindDictionaryEntry(reader, "ap-ip").GetString();
                            if (saveIp != gameIp) continue;
                            memStream.Position = 0L;
                            savePort = saveIp.Substring(gameIp.Length - 5);
                            if (savePort == gamePort) //Ignore port, doubt there would be multiple saves hosted on the same seed, server, and different ports
                                portMatch = true;
                            if (gameSeed != null)
                            {
                                saveSeed = SValue.FindDictionaryEntry(reader, "ap-seed").GetString();
                                if (saveSeed != gameSeed) continue;
                            }
                            memStream.Position = 0L;
                            saveSlotName = SValue.FindDictionaryEntry(reader, "ap-slot-name").GetString();
                            if (saveSlotName != gameSlotName) continue;
                            DateTime fileWriteTime = File.GetLastWriteTime(saveFile);
                            if (fileWriteTime > latestSaveTime && (!portMatch || savePort == gamePort))
                            {
                                latestSaveTime = fileWriteTime;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        saveName = Path.GetFileName(saveFile);
                    }
                    catch (Exception e)
                    {
                        Logging.Log($"Error getting save file with connection info: {e}");
                    }
                }
            }
            return saveName;
        }
    }

    public struct APSaveDataInfo
    {
        public string ip;
        public string seed;
        public string slotName;

        public APSaveDataInfo(string ip, string seed, string slotName)
        {
            this.ip = ip;
            this.seed = seed;
            this.slotName = slotName;
        }
    }
}
