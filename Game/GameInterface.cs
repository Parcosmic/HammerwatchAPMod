using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ARPGGame;
using ARPGGame.ScriptNodes;
using OpenTK;
using TiltedEngine;
using TiltedEngine.Networking;
using Archipelago.MultiClient.Net.Models;
using HammerwatchAP.Archipelago;
using HammerwatchAP.Util;

namespace HammerwatchAP.Game
{
    public static class GameInterface
    {
        static readonly MethodInfo _mi_SystemHandler_GetStatisticsData = HammerwatchAP.hammerwatchAssembly.GetType("ARPGGame.Networking.SystemHandler").GetMethod("GetStatisticsData", BindingFlags.Public | BindingFlags.Static);

        static SpeechStyle speechTrapStyle;
        static SpeechBubble lastSpawned;

        static int speechTrapCounter = 0;
        static int SPEECH_TRAP_COOLDOWN = 25;
        static int speechTrapDurationCounter = -5000;
        static int SPEECH_TRAP_DURATION = 5000;
        static int MAX_SPEECH_TRAP_DIST = 10;

        private static Random random;

        public static void Setup()
        {
            speechTrapStyle = GameBase.Instance.resources.GetResource<SpeechStyle>("menus/speech/normal_speech.xml");

            random = new Random();
        }

        public static void GameUpdate(int ms)
        {
            if (GameBase.Instance.Players == null || GameBase.Instance.Players[0].Actor == null) return;
            speechTrapDurationCounter -= ms;
            if (speechTrapDurationCounter > 0)
            {
                speechTrapCounter -= ms;
                if (speechTrapCounter <= 0)
                {
                    speechTrapCounter = SPEECH_TRAP_COOLDOWN;
                    Vector2 deviance = new Vector2(((float)random.NextDouble() - 0.5f), ((float)random.NextDouble() - 0.5f));
                    deviance = new Vector2(deviance.X * Math.Abs(deviance.X), deviance.Y * Math.Abs(deviance.Y)) * 2 * MAX_SPEECH_TRAP_DIST;
                    Vector2 bubblePosition = GameBase.Instance.Players[0].Actor.Position + deviance;
                    lastSpawned = new SpeechBubble(null, bubblePosition, "Hey!", speechTrapStyle, 1000, 80, 100, () => { });
                    GameBase.Instance.world.Place(bubblePosition.X, bubblePosition.Y, lastSpawned, false);
                }
            }
            else
            {
                speechTrapCounter = 0;
                SoundHelper.StopAaaSound();
            }
        }

        public static void SetGlobalFlag(string flag, bool value=true)
        {
            TiltedEngine.WorldObjects.ScriptNodes.SetGlobalFlag.GlobalFlags[flag] = value;
        }
        public static bool GetGlobalFlag(string flag)
        {
            return TiltedEngine.WorldObjects.ScriptNodes.SetGlobalFlag.GlobalFlags.ContainsKey(flag) && TiltedEngine.WorldObjects.ScriptNodes.SetGlobalFlag.GlobalFlags[flag];
        }
        public static bool TryGetGlobalFlag(string flag, out bool value)
        {
            return TiltedEngine.WorldObjects.ScriptNodes.SetGlobalFlag.GlobalFlags.TryGetValue(flag, out value);
        }

        public static void StartSpeechTrap()
        {
            SoundHelper.PlayAaaSound();
            speechTrapDurationCounter = SPEECH_TRAP_DURATION;
        }

        public static void ShowEndGameScreen(string endGameScreenHeader, string endGameScreenMessage)
        {
            SValue stats = (SValue)_mi_SystemHandler_GetStatisticsData.Invoke(null, new object[0]);
            GameBase.Instance.SetMenu(MenuType.END, new object[]
            {
                    endGameScreenHeader,
                    endGameScreenMessage,
                    stats
            });
            Network.SendToAll("EndGame", new object[]
            {
                    endGameScreenHeader,
                    endGameScreenMessage,
                    stats
            });
        }
    }
}
