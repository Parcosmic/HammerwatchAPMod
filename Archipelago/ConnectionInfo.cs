﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using TiltedEngine.Drawing;
using Archipelago.MultiClient.Net;
using ARPGGame.Menus;
using ARPGGame;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using HammerwatchAP.Util;
using HammerwatchAP.Game;
using HammerwatchAP.Menus;
using HammerwatchAP.Archipelago;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using SDL2;
using static HammerwatchAP.Archipelago.ArchipelagoData;
using HammerwatchAP.Hooks;

namespace HammerwatchAP.Archipelago
{
    public class ConnectionInfo
    {
        public string slotName;
        public string ip;
        public string apPassword;

        public bool connectedToAP;
        public ConnectionState connectionState;

        public int playerId;
        public int playerTeam;

        public string failedConnectMsg;

        public bool gotTryConnectResult = false;
        public bool connectIgnoringWarnings = false;

        int reconnectTimer = 0;

        public ArchipelagoSession session;
        private DeathLinkService deathLinkService;
        public bool ConnectionActive
        {
            get
            {
                return session != null && session.Socket.Connected;
            }
        }

        Task connectTask;
        Exception connectException;

        LoginResult loginResult;

        //Message vars
        bool sentMessage;
        bool sentCommand;
        int absorbMessages;
        List<string> absorbedMessages = new List<string>();

        public ConnectionInfo()
        {
            slotName = "";
            ip = "";
            apPassword = "";

            playerId = -1;
            playerTeam = -1;

            failedConnectMsg = null;
        }
        public ConnectionInfo(string slotName, string ip, string password)
        {
            this.slotName = slotName;
            this.ip = ip;
            this.apPassword = password;

            playerId = -1;
            playerTeam = -1;

            failedConnectMsg = null;
        }

        public void StartConnection(ArchipelagoData loadedData, bool resetVars = false, bool inGame = false, bool wait = true)
        {
            connectTask = new Task(() => ConnectThread(loadedData, resetVars, inGame));
            connectTask.Start();
            if (wait)
                connectTask.Wait();

            if (connectException != null)
                throw connectException;
        }
        private void ConnectThread(ArchipelagoData loadedData, bool resetVars, bool inGame)
        {
            try
            {
                if (TryConnectToArchipelago(loadedData, resetVars))
                {
                    reconnectTimer = 0;
                }
                if (inGame) return;
                //if (connectedToAP)
                //{
                //    GameBase.Instance.SetMenu(MenuType.MAIN);
                //}
                //else
                //{
                //    GameBase.Instance.SetMenu(MenuType.MESSAGE, "Connection Error", failedConnectMsg);
                //}
            }
            catch (Exception e)
            {
                connectException = e;
                ArchipelagoManager.generateInfo.generateState = ArchipelagoManager.GenerateInfo.GenerateState.GenerateError;
                throw e;
            }
        }
        private bool TryConnectToArchipelago(ArchipelagoData loadedData, bool resetVars)
        {
            bool connected = ConnectToArchipelago(ip, slotName, apPassword, loadedData, resetVars);
            if (connected) return true;
            if (session.Socket.Connected)
                session.Socket.DisconnectAsync();
            return false;
        }
        private bool ConnectToArchipelago(string ip, string slotName, string password, ArchipelagoData loadedArchipelagoData, bool resetVars)
        {
            connectionState = ConnectionState.SetupSession;

            Logging.LogConnectionInfo(ip);

            session = ArchipelagoSessionFactory.CreateSession(ip);
            if(session == null)
            {
                failedConnectMsg = "Couldn't create Archipelago session";
                return false;
            }

            connectionState = ConnectionState.ServerAuth;

            ArchipelagoData archipelagoData = new ArchipelagoData();
            if (resetVars)
            {
                archipelagoData.seed = null;
                ArchipelagoManager.InitializeAPVariables(archipelagoData);
            }
            archipelagoData.itemIndexCounter = 0; //This ALWAYS needs to be reset, regardless of reconnecting. The server will always send all of our items again to us upon a new connection

            this.ip = ip;
            this.slotName = slotName;
            apPassword = password;

            string apworldVersion = "Unknown Version";

            //Server message mirroring in client
            session.MessageLog.OnMessageReceived += message =>
            {
                if (sentMessage)
                {
                    sentMessage = false;
                    return;
                }
                string wholeMessage = string.Join("", message.Parts.Select(str => str.Text));
                if (absorbMessages-- == 2)
                {
                    absorbedMessages.Add(wholeMessage);
                    return;
                }

                if (!ArchipelagoManager.APChatMirroring && !(message is CountdownLogMessage))
                    return;

                switch (message)
                {
                    case ChatLogMessage chatMessage:
                        var playerColor = ArchipelagoMessageManager.GetPlayerColor(chatMessage.Player.Name);
                        ArchipelagoMessageManager.SendHWMessage(wholeMessage, playerColor);
                        break;
                    //case Archipelago.MultiClient.Net.MessageLog.Messages.:
                    //    break;
                    case CommandResultLogMessage commandMessage:
                        if (!sentCommand)
                        {
                            ArchipelagoMessageManager.SendHWMessage(wholeMessage);
                        }
                        break;
                    case HintItemSendLogMessage hintMessage:
                        //if (hintMessage.Receiver.Slot == playerId || hintMessage.Sender.Slot == playerId)
                        if (hintMessage.IsRelatedToActivePlayer)
                        {
                            ArchipelagoMessageManager.SendHWMessage(wholeMessage);
                        }
                        break;
                    case ItemSendLogMessage itemSendLogMessage:
                        //Ignore all item send/receive messages
                        //if (itemSendLogMessage.IsRelatedToActivePlayer)
                        //{
                        //    SendHWMessage(wholeMessage);
                        //}
                        break;
                    case CountdownLogMessage countdownMessage:
                        if (GameBase.Instance.Players == null) break;
                        if (countdownMessage.RemainingSeconds == 0)
                        {
                            SoundHelper.PlayCountdownFinishSound();
                            ArchipelagoMessageManager.SetAnnounceText("GO!!!", AnnounceTextType.Title);
                            ArchipelagoMessageManager.SetAnnounceText("", AnnounceTextType.SubTitle);
                        }
                        else
                        {
                            SoundHelper.PlayCountdownSound();
                            ArchipelagoMessageManager.SetAnnounceText("Starting countdown:", AnnounceTextType.Title);
                            ArchipelagoMessageManager.SetAnnounceText(countdownMessage.RemainingSeconds.ToString(), AnnounceTextType.SubTitle);
                        }
                        break;
                    default:
                        ArchipelagoMessageManager.SendHWMessage(wholeMessage);
                        break;
                }
                sentCommand = false;
            };
            session.Socket.PacketReceived += packet =>
            {
                //Logging.Debug("Received packet " + packet.PacketType);
                try
                {
                    switch (packet.PacketType)
                    {
                        case ArchipelagoPacketType.DataPackage:
                            DataPackagePacket dataPackagePacket = (DataPackagePacket)packet;
                            foreach (string game in dataPackagePacket.DataPackage.Games.Keys)
                            {
                                ArchipelagoManager.gameData[game] = dataPackagePacket.DataPackage.Games[game];
                            }
                            ArchipelagoManager.datapackageUpToDate = true;
                            break;
                        case ArchipelagoPacketType.RoomInfo:
                            RoomInfoPacket rPacket = (RoomInfoPacket)packet;

                            Console.WriteLine("Archipelago server version: " + rPacket.Version.ToVersion());
                            Version version = rPacket.Version.ToVersion();
                            if (rPacket.Version.ToVersion() < ArchipelagoManager.AP_VERSION)
                            {
                                //The client version is greater than the server!
                                Console.WriteLine("Server is outdated!");
                            }
                            //////TODO: Probably move to ArchipelagoManager
                            //Sanitize datapackage checksum data
                            foreach (var checksumKey in rPacket.DataPackageChecksums)
                            {
                                string sanitizedGame = ArchipelagoMessageManager.SanitizeString(checksumKey.Key);
                                string sanitizeChecksum = ArchipelagoMessageManager.SanitizeString(checksumKey.Value);
                                if (sanitizedGame != checksumKey.Key || sanitizeChecksum != checksumKey.Value)
                                {
                                    Logging.Log($"WARNING: Game checksums contained unexpected characters for game \"{checksumKey.Key}\", checksum \"{checksumKey.Value}\". Either the network protocol changed, or the server has been tampered with!");
                                }
                                ArchipelagoManager.gameChecksums[sanitizedGame] = sanitizeChecksum;
                            }
                            // Check our datapackage checksums and see if all of them exist already
                            string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                            string apDatapackageCacheFolder = Path.Combine(localFolder, "Archipelago", "Cache", "datapackage");
                            if (!Directory.Exists(apDatapackageCacheFolder)) return;
                            foreach (string game in ArchipelagoManager.gameChecksums.Keys)
                            {
                                string gameFolder = Path.Combine(apDatapackageCacheFolder, game);
                                if (!Directory.Exists(gameFolder)) return;
                                string[] checkSums = Directory.GetFiles(gameFolder).Select(Path.GetFileName).ToArray();
                                if (!checkSums.Contains($"{ArchipelagoManager.gameChecksums[game]}.json")) return;
                            }
                            ArchipelagoManager.datapackageUpToDate = true;
                            break;
                        case ArchipelagoPacketType.RoomUpdate:
                            RoomUpdatePacket ruPacket = (RoomUpdatePacket)packet;
                            foreach (long loc in ruPacket.CheckedLocations)
                            {
                                if (archipelagoData.checkedLocations.Contains(loc)) continue;
                                archipelagoData.checkedLocations.Add(loc);
                            }
                            break;
                        case ArchipelagoPacketType.Connected:
                            ConnectedPacket cPacket = ((ConnectedPacket)packet);

                            //Check version
                            string clientVersion = (string)cPacket.SlotData["Hammerwatch Mod Version"];
                            switch (MiscHelper.CheckVersion(ArchipelagoManager.MOD_VERSION, clientVersion))
                            {
                                case MiscHelper.VersionMisMatch.Major:
                                case MiscHelper.VersionMisMatch.Minor:
                                    //ArchipelagoManager.failedConnectMsg = $"Server requires higher mod version ({clientVersion}) than is currently installed. Update your mod!";
                                    ArchipelagoManager.DisconnectFromArchipelago($"Server requires higher mod version ({clientVersion}) than is currently installed. Update your mod!");
                                    connectionState = ConnectionState.ConnectionFailure;
                                    return;
                                case MiscHelper.VersionMisMatch.Build:
                                    if (!connectIgnoringWarnings)
                                    {
                                        ArchipelagoManager.DisconnectFromArchipelago("There is a newer version of the mod, it is recommended to update before you start playing!");
                                        connectionState = ConnectionState.ConnectionFailure;
                                    }
                                    connectIgnoringWarnings = true;
                                    break;
                            }

                            playerTeam = cPacket.Team;
                            playerId = cPacket.Slot;
                            archipelagoData.seed = session.RoomState.Seed;
                            archipelagoData.SetSlotData(cPacket.SlotData);

                            //Check required ap version
                            apworldVersion = archipelagoData.GetSlotValue<string>("APWorld Version");
                            Logging.Log("Server APWorld version: " + apworldVersion);
                            string[] splits = apworldVersion.Split('.');
                            if (int.Parse(splits[0]) < ArchipelagoManager.APWORLD_VERSION.Major || (splits.Length > 1 && int.Parse(splits[0]) == ArchipelagoManager.APWORLD_VERSION.Major && int.Parse(splits[1]) < ArchipelagoManager.APWORLD_VERSION.Minor)) //Only check major and minor, bugfix shouldn't break anything
                            {
                                Console.WriteLine("APWorld is outdated!");
                                ArchipelagoManager.DisconnectFromArchipelago("Mod version mismatch between mod and server!");
                                failedConnectMsg = "The Hammerwatch APWorld used to generate the multiworld is out of date, please update it and re-generate!";
                                connectionState = ConnectionState.ConnectionFailure;
                                return;
                            }
                            //Console.WriteLine("Slot data dump:");
                            //foreach (string key in archipelagoData.slotData.Keys)
                            //{
                            //    Console.WriteLine($"{key} : {archipelagoData.slotData[key]}");
                            //}

                            archipelagoData.playerGames = new string[session.Players.Players[playerTeam].Count];
                            foreach (var playerInfo in session.Players.Players[playerTeam])
                            {
                                archipelagoData.playerGames[playerInfo.Slot] = playerInfo.Game;
                            }

                            archipelagoData.allLocalLocations = new HashSet<long>();
                            archipelagoData.allLocalLocations.UnionWith(cPacket.LocationsChecked);
                            archipelagoData.allLocalLocations.UnionWith(cPacket.MissingChecks);

                            //Sync locations between server and client
                            List<long> unsyncedLocations = new List<long>();
                            foreach (long loc in archipelagoData.checkedLocations)
                            {
                                if (cPacket.LocationsChecked.Contains(loc)) continue;
                                unsyncedLocations.Add(loc);
                            }
                            if (unsyncedLocations.Count > 0)
                                session.Locations.CompleteLocationChecksAsync(unsyncedLocations.ToArray());
                            foreach (long loc in cPacket.LocationsChecked)
                            {
                                if (archipelagoData.checkedLocations.Contains(loc)) continue;
                                archipelagoData.checkedLocations.Add(loc);
                            }

                            //TODO: Sync loaded AP data with info on the server
                            if (loadedArchipelagoData != null)
                            {
                                if (loadedArchipelagoData.seed != null && session.RoomState.Seed != loadedArchipelagoData.seed)
                                {
                                    //Uh oh the server is hosting the wrong seed!
                                    ArchipelagoManager.DisconnectFromArchipelago("Seed mismatch between save and server");
                                    connectionState = ConnectionState.ConnectionFailure;
                                    return;
                                }
                                if (loadedArchipelagoData.mapType != archipelagoData.mapType)
                                {
                                    //Big problem the server has a different map than us!
                                    ArchipelagoManager.DisconnectFromArchipelago("Map mismatch between save and server!");
                                    connectionState = ConnectionState.ConnectionFailure;
                                    return;
                                }
                                archipelagoData.Update(loadedArchipelagoData);
                            }

                            //If we don't have a save with the seed name, then we have to scout all the locations to generate a map
                            if (Directory.Exists("saves"))
                            {
                                ArchipelagoManager.saveFileName = APSaveManager.GetLatestSaveNameWithConnectionInfo(ip, archipelagoData.seed, slotName);
                            }

                            if (ArchipelagoManager.saveFileName == null)
                            {
                                session.Socket.SendPacketAsync(new LocationScoutsPacket() { CreateAsHint = 0, Locations = archipelagoData.allLocalLocations.ToArray() });
                            }

                            connectionState = ConnectionState.ConnectionResult;
                            break;
                        case ArchipelagoPacketType.ConnectionRefused:
                            ConnectionRefusedPacket cRPacket = ((ConnectionRefusedPacket)packet);
                            string errorMessage = null;
                            foreach (ConnectionRefusedError error in cRPacket.Errors)
                            {
                                errorMessage = error.ToString();
                                switch (error)
                                {
                                    case ConnectionRefusedError.IncompatibleVersion:
                                        break;
                                    case ConnectionRefusedError.InvalidPassword:
                                        break;
                                    case ConnectionRefusedError.InvalidItemsHandling:
                                        break;
                                    case ConnectionRefusedError.InvalidSlot:
                                        break;
                                    case ConnectionRefusedError.InvalidGame:
                                        break;
                                    case ConnectionRefusedError.SlotAlreadyTaken:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                ResourceContext.Log(errorMessage);
                                failedConnectMsg = errorMessage;
                            }
                            connectionState = ConnectionState.ConnectionFailure;
                            break;
                        case ArchipelagoPacketType.ReceivedItems:
                            ReceivedItemsPacket rIPacket = (ReceivedItemsPacket)packet;

                            //If the server's received items index is less than ours, reset ours to the server so we can at least receive new items
                            if (rIPacket.Index == 0 && rIPacket.Items.Length < archipelagoData.itemsReceived)
                            {
                                archipelagoData.itemsReceived = rIPacket.Items.Length;
                            }

                            NetworkItem[] items = rIPacket.Items;
                            foreach (NetworkItem item in items)
                            {
                                string itemName = ArchipelagoManager.GetReceiveItemName(item);
                                if (++archipelagoData.itemIndexCounter <= archipelagoData.itemsReceived)
                                {
                                    if (APData.IsItemUnique(item.Item))
                                    {
                                        Logging.Debug($"Re-receiving {itemName}");
                                        ArchipelagoManager.PickupItemEffects(item, true, true);
                                    }
                                    else
                                    {
                                        Logging.Debug($"Skip receiving {itemName}");
                                    }
                                    continue;
                                }
                                //ReceiveItem(item);
                                Logging.Debug($"Received {itemName}");
                            }
                            ArchipelagoManager.archipelagoData.itemsToReceive.AddRange(items);
                            break;
                        case ArchipelagoPacketType.LocationInfo:
                            LocationInfoPacket locPacket = (LocationInfoPacket)packet;
                            Dictionary<long, NetworkItem> locationToItem = new Dictionary<long, NetworkItem>(locPacket.Locations.Length);
                            foreach (NetworkItem item in locPacket.Locations)
                            {
                                locationToItem[item.Location] = item;
                            }
                            archipelagoData.locationToItem = locationToItem;

                            if (ArchipelagoManager.archipelagoData.saveFileName == null)
                            {
                                try
                                {
                                    string baseFile = ArchipelagoManager.archipelagoData.mapType == MapType.Castle ? "levels\\campaign.hwm" : "levels\\campaign2.hwm";
                                    ArchipelagoManager.gameState = ArchipelagoManager.GameState.Generating;
                                    if (!APMapPatcher.CreateAPMapFile(baseFile, session.RoomState.Seed, session.ConnectionInfo.Slot, out ArchipelagoManager.archipelagoData.mapFileName, this, archipelagoData))
                                    {
                                        Logging.Log("Save failed to generate!");
                                    }
                                }
                                catch (Exception e)
                                {
                                    ArchipelagoManager.gameState = ArchipelagoManager.GameState.FailedGenerate;
                                    ArchipelagoManager.OutputError(e);
                                }
                            }
                            ArchipelagoManager.CompletedGeneration();
                            break;
                    }
                }
                catch (Exception e)
                {
                    ArchipelagoManager.OutputError(e);
                }
            };
            ArchipelagoManager.datapackageUpToDate = false;
            loginResult = session.TryConnectAndLogin("Hammerwatch", slotName, ItemsHandlingFlags.IncludeStartingInventory, ArchipelagoManager.AP_VERSION, null, null, password);

            //while ((!gotTryConnectResult || !ArchipelagoManager.datapackageUpToDate) && loginResult.Successful)
            //{
            //    //Wait for it to connect
            //}

            return failedConnectMsg == null;
        }
        public bool ConnectionResponse()
        {
            Logging.Log($"AP connection result: {loginResult}");

            connectedToAP = loginResult.Successful && connectionState == ConnectionState.ConnectionResult;

            if (!connectedToAP)
            {
                if (loginResult is LoginFailure loginFailure)
                    ConnectionError(loginFailure.Errors[0]);
                else
                    ConnectionError(failedConnectMsg);
                return false;
            }

            ArchipelagoManager.LoadDatapackage();

            Logging.Log("Building AP item > name dictionary");
            List<string> gameNames = new List<string>();
            foreach (var player in session.Players.Players[playerTeam])
            {
                if (gameNames.Contains(player.Game)) continue;
                gameNames.Add(player.Game);
            }

            ArchipelagoManager.playingArchipelagoSave = true;

            //Setup death link
            deathLinkService = session.CreateDeathLinkService();
            SetDeathlink(ArchipelagoManager.Deathlink);
            deathLinkService.OnDeathLinkReceived += (deathLinkObject) =>
            {
                if (ArchipelagoManager.gameReady)
                    ArchipelagoManager.ReceivedDeathlink(deathLinkObject);
            };
            session.Socket.SocketClosed += reason =>
            {
                if (failedConnectMsg == null)
                    ResourceContext.Log($"Disconnected from AP socket: {reason}");
                else
                    ResourceContext.Log(failedConnectMsg);
                DisconnectedFromArchipelago();
                failedConnectMsg = null;
            };

            connectionState = ConnectionState.Connected;
            GameBase.Instance.SetMenu(MenuType.MAIN);
            return true;
        }
        public void Disconnect()
        {
            session?.Socket.DisconnectAsync();
        }
        public void ConnectionError(string errorMessage)
        {
            GameBase.Instance.SetMenu(MenuType.MESSAGE, "Connection Error", errorMessage);
            if (session.Socket.Connected)
                session.Socket.DisconnectAsync();
            connectionState = ConnectionState.Disconnected;
        }

        public void GameUpdate(int ms)
        {
            //Logging.Log($"State {connectionState}, datapackage up to date: {ArchipelagoManager.datapackageUpToDate}");
            if((connectionState == ConnectionState.ConnectionResult || connectionState == ConnectionState.ConnectionFailure) && ArchipelagoManager.datapackageUpToDate)
            {
                ConnectionResponse();
            }
        }

        public string GetPlayerName(int slot)
        {
            return session.Players.Players[playerTeam][slot].Alias;
        }
        public string GetPlayerGame(int slot)
        {
            return session.Players.Players[playerTeam][slot].Game;
        }
        public bool IsPlayerPlayingSameGame(int slot)
        {
            return session.Players.Players[playerTeam][slot].Game == session.Players.Players[playerTeam][playerId].Game;
        }
        private void DisconnectedFromArchipelago()
        {
            //archipelagoSave = false;
            connectedToAP = false;
            session = null;
            deathLinkService = null;
            ArchipelagoMessageManager.SendHWErrorMessage(failedConnectMsg ?? "Disconnected from Archipelago server");
            reconnectTimer = 15000;

            //ResetAbsorb();
        }

        public void SetClientReady(bool deathlink)
        {
            if(ConnectionActive)
            {
                session.Socket.SendPacketAsync(new StatusUpdatePacket() { Status = ArchipelagoClientState.ClientPlaying });
                string[] tags = new string[0];
                if (deathlink)
                    tags = new[] { "DeathLink" };
                session.Socket.SendPacketAsync(new ConnectUpdatePacket() { ItemsHandling = ItemsHandlingFlags.IncludeStartingInventory, Tags = tags });
            }
        }
        public void SendGoal()
        {
            if(ConnectionActive)
                session.Socket.SendPacketAsync(new StatusUpdatePacket() { Status = ArchipelagoClientState.ClientGoal });
        }

        public void SetDeathlink(bool on)
        {
            if (deathLinkService == null) return;
            if (on)
            {
                deathLinkService.EnableDeathLink();
            }
            else
            {
                deathLinkService.DisableDeathLink();
            }
        }
        public void SendDeathlink(DeathLink deathlink)
        {
            if (connectedToAP)
                deathLinkService.SendDeathLink(deathlink);
        }
        public void SendCheckedLocations(ArchipelagoData archipelagoData)
        {
            session.Socket.SendPacketAsync(new LocationChecksPacket { Locations = archipelagoData.checkedLocations.ToArray() });
        }

        public void SendAPChatMessage(string message)
        {
            session.Socket.SendPacketAsync(new SayPacket { Text = message });
        }

        public enum ConnectionState
        {
            Unconnected,
            Disconnected,
            SetupSession,
            ServerAuth,
            ConnectionResult,
            ConnectionFailure,
            Connected,

        }
    }
}
