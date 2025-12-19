using System;
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
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using static HammerwatchAP.Archipelago.ArchipelagoData;

namespace HammerwatchAP.Archipelago
{
    public class ConnectionInfo
    {
        const string TAG_DEATHLINK = "DeathLink";
        const string TAG_TRAPLINK = "TrapLink";

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

        private int reconnectTimer = 0;
        private int lastReconnectWaitTime = 0;
        private const int RECONNECT_TIMER_START = 5000;

        public ArchipelagoSession session;
        private DeathLinkService deathLinkService;
        private bool socketClosed;
        public bool ConnectionActive
        {
            get
            {
                return session != null && session.Socket.Connected;
            }
        }

        private Task connectTask;
        private Exception connectException;

        private LoginResult loginResult;

        private List<int> hintedLocationIds;

        //Message vars
        private int absorbMessages;
        private List<string> absorbedMessages = new List<string>();

        public ConnectionInfo()
        {
            slotName = "";
            ip = "";
            apPassword = "";

            playerId = -1;
            playerTeam = -1;

            failedConnectMsg = null;

            hintedLocationIds = new List<int>();
        }
        public ConnectionInfo(string slotName, string ip, string password)
        {
            this.slotName = slotName;
            this.ip = ip;
            this.apPassword = password;

            playerId = -1;
            playerTeam = -1;

            failedConnectMsg = null;

            hintedLocationIds = new List<int>();
        }

        public void SetConnectionState(ConnectionState state)
        {
            Logging.Debug("ConnectionState set to " + state.ToString());
            connectionState = state;
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
                    lastReconnectWaitTime = 0;
                }
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
            SetConnectionState(ConnectionState.SetupSession);

            Logging.LogConnectionInfo(ip);

            socketClosed = false;
            failedConnectMsg = null;
            session = ArchipelagoSessionFactory.CreateSession(ip);
            if (session == null)
            {
                failedConnectMsg = "Couldn't create Archipelago session";
                return false;
            }

            SetConnectionState(ConnectionState.ServerAuth);

            ArchipelagoData archipelagoData = loadedArchipelagoData ?? new ArchipelagoData();
            if (resetVars)
            {
                ArchipelagoManager.InitializeAPVariables(archipelagoData);
            }
            archipelagoData.itemIndexCounter = 0; //This ALWAYS needs to be reset, regardless of reconnecting. This represents which index from the server we've processed up to

            this.ip = ip;
            this.slotName = slotName;
            apPassword = password;

            string apworldVersion = "Unknown Version";

            //Server message mirroring in client
            session.MessageLog.OnMessageReceived += message =>
            {
                if (ArchipelagoMessageManager.sentMessage)
                {
                    ArchipelagoMessageManager.sentMessage = false;
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
                    case CommandResultLogMessage commandMessage:
                        if (!ArchipelagoMessageManager.sentCommand)
                        {
                            ArchipelagoMessageManager.SendHWMessage(wholeMessage);
                        }
                        break;
                    case HintItemSendLogMessage hintMessage:
                        if (hintMessage.IsRelatedToActivePlayer)
                        {
                            ArchipelagoMessageManager.SendHWMessage(wholeMessage);
                        }
                        break;
                    case ItemSendLogMessage itemSendLogMessage:
                        //Ignore all item send/receive messages
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
                        if(wholeMessage.Contains("Warning: your client does not support compressed websocket connections! It may stop working in the future. If you are a player, please report this to the client's developer."))
                        {
                            //Block the compressed websocket message, it's a lib problem anyways
                            break;
                        }
                        ArchipelagoMessageManager.SendHWMessage(wholeMessage);
                        break;
                }
                ArchipelagoMessageManager.sentCommand = false;
            };
            session.Socket.PacketReceived += packet =>
            {
                try
                {
                    switch (packet.PacketType)
                    {
                        case ArchipelagoPacketType.DataPackage:
                            DataPackagePacket dataPackagePacket = (DataPackagePacket)packet;
                            Logging.Debug("Got DatapackagePacket with " + dataPackagePacket.DataPackage.Games.Count);
                            foreach (string game in dataPackagePacket.DataPackage.Games.Keys)
                            {
                                ArchipelagoManager.gameData[game] = dataPackagePacket.DataPackage.Games[game];
                            }
                            ArchipelagoManager.datapackageUpToDate = true;
                            //if(connectionState == ConnectionState.WaitForDatapackage)
                            //    SetConnectionState(ConnectionState.ConnectionResult);
                            break;
                        case ArchipelagoPacketType.RoomInfo:
                            RoomInfoPacket rPacket = (RoomInfoPacket)packet;

                            Logging.Log("Archipelago server version: " + rPacket.Version.ToVersion());
                            Version version = rPacket.Version.ToVersion();
                            if (rPacket.Version.ToVersion() < ArchipelagoManager.AP_VERSION)
                            {
                                //The client version is greater than the server!
                                Logging.Log("  WARNING: The server is outdated, some features may not work properly!");
                            }
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
                            List<string> neededGameDatapackages = new List<string>();
                            foreach (string game in ArchipelagoManager.gameChecksums.Keys)
                            {
                                string gameFileName = game.Replace(":", "");
                                string gameFolder = Path.Combine(apDatapackageCacheFolder, gameFileName);
                                if (Directory.Exists(gameFolder))
                                {
                                    string[] checkSums = Directory.GetFiles(gameFolder).Select(Path.GetFileName).ToArray();
                                    if (checkSums.Contains($"{ArchipelagoManager.gameChecksums[game]}.json"))
                                    {
                                        continue;
                                    }
                                }
                                Logging.Debug($"Local datapackage for game {game} does not exist!!");
                                neededGameDatapackages.Add(game);
                            }
                            if(neededGameDatapackages.Count > 0)
                            {
                                return;
                            }
                            ArchipelagoManager.datapackageUpToDate = true;
                            break;
                        case ArchipelagoPacketType.RoomUpdate:
                            RoomUpdatePacket ruPacket = (RoomUpdatePacket)packet;
                            if(ruPacket.CheckedLocations != null)
                            {
                                foreach (long loc in ruPacket.CheckedLocations)
                                {
                                    if (archipelagoData.checkedLocations.Contains(loc)) continue;
                                    archipelagoData.checkedLocations.Add(loc);
                                }
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
                                    SetConnectionState(ConnectionState.ConnectionFailure);
                                    ArchipelagoManager.DisconnectFromArchipelago($"Server requires higher mod version ({clientVersion}) than is currently installed. Update your mod!");
                                    return;
                                case MiscHelper.VersionMisMatch.Build:
                                    if (!connectIgnoringWarnings)
                                    {
                                        SetConnectionState(ConnectionState.ConnectionFailure);
                                        ArchipelagoManager.DisconnectFromArchipelago("There is a newer version of the mod, it is recommended to update before you start playing!");
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
                                Logging.Log("  WARNING: APWorld is outdated!");
                                ArchipelagoManager.DisconnectFromArchipelago("Mod version mismatch between mod and server!");
                                failedConnectMsg = "The Hammerwatch APWorld used to generate the multiworld is out of date, please update it and re-generate!";
                                SetConnectionState(ConnectionState.ConnectionFailure);
                                return;
                            }

                            ArchipelagoManager.LastConnectedIP = ip;
                            ArchipelagoManager.LastConnectedSlotName = slotName;
                            OptionsMenu.SaveOptions(); //Need to call this here so we can save the last connected info

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

                            if (loadedArchipelagoData != null)
                            {
                                if (loadedArchipelagoData.seed != null && session.RoomState.Seed != loadedArchipelagoData.seed)
                                {
                                    //Uh oh the server is hosting the wrong seed!
                                    ArchipelagoManager.DisconnectFromArchipelago("Seed mismatch between save and server");
                                    SetConnectionState(ConnectionState.ConnectionFailure);
                                    return;
                                }
                                if (loadedArchipelagoData.mapType != archipelagoData.mapType)
                                {
                                    //Big problem the server has a different map than us!
                                    ArchipelagoManager.DisconnectFromArchipelago("Map mismatch between save and server");
                                    SetConnectionState(ConnectionState.ConnectionFailure);
                                    return;
                                }
                            }

                            //If we don't have a save with the seed name, then we have to scout all the locations to generate a map
                            if (Directory.Exists("saves") && !ArchipelagoManager.autoloadSave)
                            {
                                ArchipelagoManager.saveFileName = APSaveManager.GetLatestSaveNameWithConnectionInfo(ip, archipelagoData.seed, slotName);
                            }
                            if (ArchipelagoManager.saveFileName == null)
                            {
                                session.Socket.SendPacketAsync(new LocationScoutsPacket() { CreateAsHint = 0, Locations = archipelagoData.allLocalLocations.ToArray() });
                            }

                            SetConnectionState(ConnectionState.ConnectionResult);
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
                                }
                                failedConnectMsg = errorMessage;
                            }
                            break;
                        case ArchipelagoPacketType.ReceivedItems:
                            ReceivedItemsPacket rIPacket = (ReceivedItemsPacket)packet;

                            //If the server's received items index is less than ours, reset ours to the server so we can at least receive new items
                            if (rIPacket.Index == 0 && rIPacket.Items.Length < archipelagoData.itemsReceived)
                            {
                                Logging.Log("Server has less received items than we do! This means item history will likely be divergent between the server and this save!!");
                                archipelagoData.itemsReceived = rIPacket.Items.Length;
                            }

                            NetworkItem[] items = rIPacket.Items;
                            List<NetworkItem> itemsToReceive = new List<NetworkItem>(rIPacket.Items.Length);
                            foreach (NetworkItem item in items)
                            {
                                string itemName = ArchipelagoManager.GetReceiveItemName(item);
                                if (++archipelagoData.itemIndexCounter <= archipelagoData.itemsReceived)
                                {
                                    if (APData.IsReceiveItemUnique(item.Item))
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
                                Logging.Debug($"Queued to receive: {itemName}");
                                itemsToReceive.Add(item);
                            }

                            ArchipelagoManager.archipelagoData.itemsToReceive.AddRange(itemsToReceive);
                            break;
                        case ArchipelagoPacketType.LocationInfo:
                            if (archipelagoData.locationToItem.Count > 0) return; //We can receive more LocationInfo packets after broadcasting hints, ignore those
                            //Save initial location scouts data and proceed with generation
                            LocationInfoPacket locPacket = (LocationInfoPacket)packet;
                            Dictionary<long, NetworkItem> locationToItem = new Dictionary<long, NetworkItem>(locPacket.Locations.Length);
                            foreach (NetworkItem item in locPacket.Locations)
                            {
                                locationToItem[item.Location] = item;
                            }
                            archipelagoData.locationToItem = locationToItem;

                            if (ArchipelagoManager.archipelagoData.saveFileName == null)
                            {
                                ArchipelagoManager.SetGameState(ArchipelagoManager.APGameState.StartGenerate);
                            }
                            else
                            {
                                ArchipelagoManager.CompletedGeneration();
                            }
                            break;
                        case ArchipelagoPacketType.Bounced:
                            BouncedPacket bouncedPacket = (BouncedPacket)packet;
                            if(bouncedPacket.Tags.Contains(TAG_TRAPLINK) && ArchipelagoManager.TrapLink)
                            {
                                if (!bouncedPacket.Data.TryGetValue("source", out Newtonsoft.Json.Linq.JToken source))
                                    break;
                                string trapLinkPlayerName = source.ToString();
                                if (trapLinkPlayerName == GetPlayerName(playerId))
                                {
                                    break;
                                }
                                if (!bouncedPacket.Data.TryGetValue("trap_name", out Newtonsoft.Json.Linq.JToken trap_name))
                                    break;
                                //string trapLinkSentTime = bouncedPacket.Data["time"].ToString();
                                string trapLinkTrapName = trap_name.ToString();
                                string hwTrapName = APData.GetTrapLinkTrap(trapLinkTrapName);
                                if(hwTrapName != null)
                                    ArchipelagoManager.AddTrapLinkTrapToQueue(trapLinkPlayerName, trapLinkTrapName, hwTrapName);
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    ArchipelagoManager.OutputError(e);
                }
            };
            session.Socket.SocketClosed += reason =>
            {
                if(socketClosed) //For some reason this event gets run twice when the socket closes???
                {
                    return;
                }
                socketClosed = true;
                if (failedConnectMsg == null)
                    Logging.Log($"Disconnected from AP socket: {reason}");
                else
                    Logging.Log($"Disconnected from AP socket: {failedConnectMsg}");
                DisconnectedFromArchipelago();
            };
            ArchipelagoManager.datapackageUpToDate = false;
            loginResult = session.TryConnectAndLogin("Hammerwatch", slotName, ItemsHandlingFlags.IncludeStartingInventory, ArchipelagoManager.AP_VERSION, null, null, password);

            if(loginResult is LoginFailure loginFailure)
            {
                failedConnectMsg = loginFailure.ToString();
                SetConnectionState(ConnectionState.ConnectionFailure);
            }

            return failedConnectMsg == null;
        }
        public bool ConnectionResponse()
        {
            Logging.Log($"AP connection result: {loginResult}");

            connectedToAP = loginResult.Successful && connectionState == ConnectionState.ConnectionResult;

            if (!connectedToAP)
            {
                if (loginResult is LoginFailure loginFailure)
                    ConnectionResponseError(loginFailure.Errors[0]);
                else
                    ConnectionResponseError(failedConnectMsg);
                SetConnectionState(ConnectionState.Disconnected);
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

            //ArchipelagoManager.archipelagoData.raceMode = session.DataStorage["_read_race_mode"];

            //Setup death link
            deathLinkService = session.CreateDeathLinkService();
            ArchipelagoManager.SetDeathlink(ArchipelagoManager.archipelagoData.GetOption(SlotDataKeys.deathLink) > 0);
            deathLinkService.OnDeathLinkReceived += (deathLinkObject) =>
            {
                if (ArchipelagoManager.InGame)
                    ArchipelagoManager.ReceivedDeathlink(deathLinkObject);
            };

            SetConnectionState(ConnectionState.Connected);
            ArchipelagoManager.FinishConnectingToAP();
            return true;
        }
        public void ConnectionResponseError(string errorMessage)
        {
            Logging.Log("ConnectionError: " + errorMessage);
            if(ArchipelagoManager.GameState == ArchipelagoManager.APGameState.InGame)
            {
                RefreshReconnectTimer();
            }
            else
            {
                GameBase.Instance.SetMenu(MenuType.MESSAGE, "Connection Error", errorMessage);
            }
        }
        public void DisconnectFromArchipelago(string reason = null)
        {
            SetConnectionState(ConnectionState.Disconnecting);
            failedConnectMsg = "Disconnected from Archipelago server";
            if (reason != null)
                failedConnectMsg = reason;
            connectedToAP = false;
            session?.Socket.DisconnectAsync();
        }
        private void DisconnectedFromArchipelago()
        {
            connectedToAP = false;
            deathLinkService = null;
            ArchipelagoMessageManager.SendHWErrorMessage(failedConnectMsg ?? "Disconnected from Archipelago server");
            if(connectionState != ConnectionState.ConnectionFailure)
            {
                SetConnectionState(ConnectionState.Disconnected);
                RefreshReconnectTimer();
            }
            ArchipelagoManager.DisconnectedFromArchipelago(failedConnectMsg);
        }
        private void RefreshReconnectTimer()
        {
            reconnectTimer = Math.Max(lastReconnectWaitTime * 2, RECONNECT_TIMER_START);
            lastReconnectWaitTime = reconnectTimer;
            ArchipelagoMessageManager.SendHWErrorMessage($"Reconnecting in {reconnectTimer / 1000} seconds...");
            Logging.Debug($"Refreshed reconnect timer, now waiting {reconnectTimer} ms to reconnect");
        }

        public void GameUpdate(int ms)
        {
            //if(connectionState == ConnectionState.WaitForDatapackage)
            //{
            //    session.Socket.SendPacket(new GetDataPackagePacket() { Games = neededGameDatapackages.ToArray()});
            //    return;
            //}
            switch(connectionState)
            {
                case ConnectionState.ConnectionResult:
                case ConnectionState.ConnectionFailure:
                    ConnectionResponse();
                    break;
                case ConnectionState.Connected:
                    if (session != null && session.Socket != null && !session.Socket.Connected)
                    {
                        Logging.Debug("The Socket Disconnected!");
                        DisconnectFromArchipelago("Lost connection to Archipelago server");
                    }
                    break;
                case ConnectionState.Disconnected:
                    //Reconnect if the reconnect timer is done ticking
                    if(reconnectTimer > 0)
                    {
                        reconnectTimer -= ms;
                        if(reconnectTimer <= 0)
                        {
                            ArchipelagoMessageManager.SendHWErrorMessage("Reconnecting to Archipelago server...");
                            StartConnection(ArchipelagoManager.archipelagoData, false, true, false);
                        }
                    }
                    break;
            }
            switch(ArchipelagoManager.GameState)
            {
                case ArchipelagoManager.APGameState.StartGenerate:
                    if (GameBase.Instance.GetMenu<MessageMenu>() == null)
                    {
                        GameBase.Instance.SetMenu(MenuType.MESSAGE, "Generation In Progress", "Generating map file...");
                        break;
                    }
                    try
                    {
                        ArchipelagoManager.SetGameState(ArchipelagoManager.APGameState.Generating);
                        string baseFile = ArchipelagoManager.archipelagoData.mapType == MapType.Castle ? "levels\\campaign.hwm" : "levels\\campaign2.hwm";
                        if (!APMapPatcher.CreateAPMapFile(baseFile, session.RoomState.Seed, session.ConnectionInfo.Slot, out ArchipelagoManager.archipelagoData.mapFileName, this, ArchipelagoManager.archipelagoData))
                        {
                            Logging.Log("Save failed to generate!");
                        }
                    }
                    catch (Exception e)
                    {
                        ArchipelagoManager.SetGameState(ArchipelagoManager.APGameState.FailedGenerate);
                        ArchipelagoManager.OutputError(e);
                    }
                    ArchipelagoManager.CompletedGeneration();
                    break;
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
        public void SetDataStorageValue(string key, string value)
        {
            if (!ConnectionActive) return;
            session.Socket.SendPacketAsync(new SetPacket() { Key = key, WantReply = false, Operations = new OperationSpecification[] { new OperationSpecification() { OperationType = OperationType.Replace, Value = value} } });
        }
        public void SetMapTrackingKey(string key)
        {
            SetDataStorageValue($"{playerTeam}:{playerId}:CurrentRegion", key);
        }

        public void SetClientReady()
        {
            if(ConnectionActive)
            {
                session.Socket.SendPacketAsync(new StatusUpdatePacket() { Status = ArchipelagoClientState.ClientPlaying });
                SetDeathlink(ArchipelagoManager.Deathlink);
                UpdateTags();
            }
        }
        public void UpdateTags()
        {
            List<string> tags = new List<string>();
            if (ArchipelagoManager.TrapLink)
                tags.Add(TAG_TRAPLINK);
            if (ArchipelagoManager.Deathlink)
                tags.Add(TAG_DEATHLINK);
            if(ConnectionActive)
                session.Socket.SendPacketAsync(new ConnectUpdatePacket() { ItemsHandling = ItemsHandlingFlags.IncludeStartingInventory, Tags = tags.ToArray() });
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
        public void SendTrapLink(NetworkItem item)
        {
            if (!ConnectionActive) return;
            string itemName = ArchipelagoManager.GetReceiveItemName(item);
            SendTrapLink(itemName);
        }
        public void SendTrapLink(string itemName)
        {
            Dictionary<string, Newtonsoft.Json.Linq.JToken> data = new Dictionary<string, Newtonsoft.Json.Linq.JToken>()
            {
                { "source", GetPlayerName(playerId) },
                { "time", DateTime.Now },
                { "trap_name", itemName }
            };
            session.Socket.SendPacketAsync(new BouncePacket() { Tags = new List<string>() { TAG_TRAPLINK }, Data = data });
        }
        public void SendCheckedLocations(ArchipelagoData archipelagoData)
        {
            session.Socket.SendPacketAsync(new LocationChecksPacket { Locations = archipelagoData.checkedLocations.ToArray() });
        }

        public void SendAPChatMessage(string message)
        {
            session.Socket.SendPacketAsync(new SayPacket { Text = message });
        }

        public void HintLocations(List<int> locationIds)
        {
            List<long> locationsToHint = new List<long>();
            foreach(int locId in locationIds)
            {
                if (hintedLocationIds.Contains(locId)) continue;
                locationsToHint.Add(locId);
                hintedLocationIds.Add(locId);
            }
            if (locationsToHint.Count == 0) return;
            session.Socket.SendPacketAsync(new LocationScoutsPacket() { Locations = locationsToHint.ToArray(), CreateAsHint = 2 });
        }

        public enum ConnectionState
        {
            Disconnected,
            SetupSession,
            ServerAuth,
            WaitForDatapackage,
            ConnectionResult,
            ConnectionFailure,
            Connected,
            Disconnecting,

        }
    }
}
