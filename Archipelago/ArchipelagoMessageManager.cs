using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using TiltedEngine;
using ARPGGame;
using ARPGGame.GUI;
using ARPGGame.Menus;
using ARPGGame.Behaviors.Players;
using OpenTK;
using HammerwatchAP.Game;

namespace HammerwatchAP.Archipelago
{
    public static class ArchipelagoMessageManager
    {
        public static bool sentMessage;
        public static bool sentCommand;

        public static int receivedItemPopupItems;
        public const int MAX_RECEIVE_ITEM_UPDATE_COUNT = 100;
        public const int RECEIVED_ITEM_POPUP_CYCLE_TIME = 2500;
        public const int PICKUP_TEXT_CYCLE_TIME = 200;

        public static List<string> announceMessageQueue = new List<string>();
        public static int receivedItemPopupTimer;

        public static List<string> pickupTextQueue = new List<string>();
        public static int pickupTextTimer;

        static readonly MethodInfo _mi_LanguageManager_Get = HammerwatchAP.hammerwatchAssembly.GetType("ARPGGame.LanguageManager").GetMethod("Get", BindingFlags.Public | BindingFlags.Static);
        public static string GetLanguageString(string key, params string[] replaceStrings)
        {
            return (string)_mi_LanguageManager_Get.Invoke(null, new object[] { key, replaceStrings });
        }
        static readonly FieldInfo _fi_ChatWidget_chatWidget = typeof(ChatWidget).GetField("chatWidget", BindingFlags.Static | BindingFlags.NonPublic);

        public static void Setup()
        {
            receivedItemPopupTimer = 0;
            pickupTextTimer = 0;
            announceMessageQueue = new List<string>();
            pickupTextQueue = new List<string>();
        }

        public static void GameUpdate(int ms)
        {
            if(announceMessageQueue.Count > 0)
            {
                if (receivedItemPopupTimer <= 0)
                {
                    int count = 1;
                    string currentDisplayMessage = announceMessageQueue[0];
                    for (int a = 1; a < announceMessageQueue.Count; a++)
                    {
                        if (currentDisplayMessage != announceMessageQueue[a]) break;
                        count++;
                    }
                    if (count > 1)
                    {
                        for (int c = 1; c < count; c++)
                        {
                            announceMessageQueue.RemoveAt(0);
                        }
                        currentDisplayMessage = currentDisplayMessage.Replace("Received ", $"Received {count}x ");
                    }
                    SetAnnounceText(currentDisplayMessage);
                    announceMessageQueue.RemoveAt(0);
                    receivedItemPopupTimer = RECEIVED_ITEM_POPUP_CYCLE_TIME * count / receivedItemPopupItems;
                }
                else
                {
                    receivedItemPopupTimer -= ms;
                }
            }
            else
            {
                receivedItemPopupTimer = -1;
                receivedItemPopupItems = 1;
            }
            if (pickupTextQueue.Count > 0)
            {
                if (pickupTextTimer <= 0)
                {
                    ShowPickupMessage(pickupTextQueue[0]);
                    pickupTextQueue.RemoveAt(0);
                    pickupTextTimer = PICKUP_TEXT_CYCLE_TIME;
                }
                else
                {
                    pickupTextTimer -= ms;
                }
            }
            else
            {
                if (pickupTextTimer > 0)
                    pickupTextTimer -= ms;
            }
        }

        public static Color GetPlayerColor(string slotName)
        {
            Random random = new Random(slotName.GetHashCode());
            double h = random.NextDouble() * 360;
            const double minSat = 0.9;
            double sat = minSat + (1 - minSat) * random.NextDouble();
            const double minV = 0.7;
            double v = minV + (1 - minV) * random.NextDouble();

            float chroma = (float)(v * sat);
            float segment = (float)(h / 60);
            float x = chroma * (1 - Math.Abs((segment % 2) - 1));

            float m = (float)v - chroma;

            Color playerColor;
            if (segment < 1)
                playerColor = new Color(chroma + m, x + m, 0 + m);
            else if (segment < 2)
                playerColor = new Color(x + m, chroma + m, 0 + m);
            else if (segment < 3)
                playerColor = new Color(0 + m, chroma + m, x + m);
            else if (segment < 4)
                playerColor = new Color(0 + m, x + m, chroma + m);
            else if (segment < 5)
                playerColor = new Color(x + m, 0 + m, chroma + m);
            else
                playerColor = new Color(chroma + m, 0 + m, x + m);

            return playerColor;
        }
        public static void AddColorLine(string text, Color color)
        {
            ((ChatWidget)_fi_ChatWidget_chatWidget.GetValue(null))?.AddLine(text, color);
        }
        public static void SetAnnounceMessage(string translationKey, AnnounceTextType announceType = AnnounceTextType.Text)
        {
            string plankMsg = GetLanguageString(translationKey, new string[0]);
            GameHUD hud = GameBase.Instance.GetMenu<GameHUD>();
            hud?.SetAnnounceText(plankMsg ?? translationKey, 2500, announceType);
        }
        public static void SetAnnounceText(string text, AnnounceTextType announceType = AnnounceTextType.Text)
        {
            GameHUD hud = GameBase.Instance.GetMenu<GameHUD>();
            hud?.SetAnnounceText(text, 2500, announceType);
        }
        public static void ShowPickupMessage(string text)
        {
            foreach (PlayerInfo player in GameBase.Instance.Players)
            {
                if (player != null && player.Actor != null)
                {
                    WorldDrawer.AddHitText(player.Actor.Position + new Vector2(0f, -0.5f), text, HitTextType.PlayerPickup, null);
                }
            }
        }
        public static void SendHWMessage(string message)
        {
            AddColorLine(message, Color.White);
        }
        public static void SendHWMessage(string message, string senderName)
        {
            AddColorLine(message, GetPlayerColor(senderName));
        }
        public static void SendHWMessage(string message, Color color)
        {
            AddColorLine(message, color);
        }
        public static void SendHWErrorMessage(string message)
        {
            AddColorLine(message, Color.PaleVioletRed);
        }

        public static string SanitizeString(string str)
        {
            return Regex.Replace(str, @"[/\\]", "", RegexOptions.None);
        }
        public static void SentChatMessage(string message)
        {
            if (message.StartsWith("/") && message.Length > 1) //Is this a command
            {
                if (ExecuteCommand(message))
                    return;
            }
            if (!ArchipelagoManager.ConnectedToAP()) return;
            sentMessage = true; //So we ignore our own message we sent
            if (message[0] == '!') sentCommand = true; //Mark if we sent a command
            ArchipelagoManager.connectionInfo.SendAPChatMessage(message);
        }
        public static bool ExecuteCommand(string message)
        {
            List<string> parts = new List<string>();
            bool inQuotes = false;
            string curPart = "";
            for (int m = 1; m < message.Length; m++)
            {
                char c = message[m];
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ' ' && !inQuotes && curPart != "")
                {
                    parts.Add(curPart);
                    curPart = "";
                }
                else
                {
                    curPart += c;
                }
            }
            if (curPart != "")
                parts.Add(curPart);
            switch (parts[0])
            {
                case "help":
                    SendHWMessage("/tp [levelId] [startId]");
                    SendHWMessage("    Teleports to the given levelId and startId, or to the map start if no arguments are provided");
                    SendHWMessage("/flag <flagName> [true/false]");
                    SendHWMessage("    Sets the value of the supplied flag, or returns the value of the flag if only 1 argument is provided");
                    SendHWMessage("/deathlink");
                    SendHWMessage("    Toggles deathlink");
                    break;
                case "teleport":
                case "tp":
                case "t":
                    if (!ArchipelagoManager.IsHostPlayer())
                    {
                        SendHWErrorMessage($"Only the host can use the teleport command.");
                        break;
                    }
                    string levelId;
                    int startId = 0;
                    if (parts.Count == 1)
                    {
                        string startCode = ArchipelagoManager.archipelagoData.GetStartExitCode();
                        string[] startSplits = startCode.Split('|');
                        levelId = startSplits[0];
                        startId = int.Parse(startSplits[1]);
                    }
                    else
                    {
                        levelId = parts[1];
                    }
                    if (!GameBase.Instance.lvlList.Levels.ContainsKey(levelId))
                    {
                        SendHWErrorMessage($"Level code \"{levelId}\" is invalid.");
                        break;
                    }
                    if (parts.Count == 3 && !int.TryParse(parts[2], out startId))
                    {
                        SendHWErrorMessage($"Argument 2 of command \"tp\" should be an integer.");
                        break;
                    }
                    GameBase.Instance.ChangeLevel(levelId, startId);
                    break;
                case "flag":
                case "f":
                    bool flagValue;
                    switch (parts.Count)
                    {
                        case 1:
                            SendHWErrorMessage($"Command \"flag\" requires at least 1 argument.");
                            return true;
                        case 2: //Report the value of the flag with no extra argument
                            if (GameInterface.TryGetGlobalFlag(parts[1], out flagValue))
                            {
                                SendHWMessage($"Flag \"{parts[1]}\" is {flagValue}");
                            }
                            else
                            {
                                SendHWMessage($"Flag \"{parts[1]}\" has not been set");
                            }
                            return true;
                        case 3: //Set the flag
                            //Try to see if the argument is a number
                            if (int.TryParse(parts[2], out int flagIntValue))
                            {
                                flagValue = flagIntValue != 0;
                            }
                            else
                            {
                                string arg = parts[2].ToLower();
                                flagValue = arg != "false";
                            }
                            GameInterface.SetGlobalFlag(parts[1], flagValue);
                            //SetGlobalFlag.GlobalFlags[parts[1]] = flagValue;
                            EventSystem.Instance.FireEvent(ArchipelagoManager.BUTTON_EVENT_NAME);
                            SendHWMessage($"Flag \"{parts[1]}\" set to {flagValue}");
                            return true;
                    }
                    break;
                case "deathlink":
                case "d":
                    ArchipelagoManager.SetDeathlink(!ArchipelagoManager.Deathlink);
                    break;
                case "i":
                    if (GameBase.Instance.Players == null)
                        return false;
                    bool? immortal = null;
                    foreach (PlayerInfo player in GameBase.Instance.Players)
                    {
                        if (player == null) continue;
                        QoL.immortalPlayers[player.PeerID] = immortal ?? !QoL.immortalPlayers[player.PeerID];
                        immortal = QoL.immortalPlayers[player.PeerID];
                    }
                    QoL.UpdateImmortalPlayers();
                    SendHWMessage($"Toggled {(immortal.HasValue && immortal.Value ? "on" : "off")} immortality");
                    break;
                case "kill":
                case "k":
                    if (GameBase.Instance.Players == null)
                        return true;
                    //If there is a second argument and it's somewhat indicative that we want to kill everyone do it >:)
                    bool killAllPlayers = parts.Count > 1 && (parts[1].ToLower() == "all" || (bool.TryParse(parts[1], out bool arg1) && arg1) || (int.TryParse(parts[1], out int arg1Int) && arg1Int > 0));
                    foreach (PlayerInfo player in GameBase.Instance.Players)
                    {
                        if (!player.IsLocalPlayer && !killAllPlayers) continue;
                        player.ChangeHealth(-9999);
                        if (player.Actor != null && (player.Actor.Behavior is PlayerActorBehavior actBehavior))
                        {
                            actBehavior.CheckDead();
                        }
                    }
                    break;
                default:
                    return false;
            }
            return true;
        }
    }
}
