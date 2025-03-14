using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ARPGGame;
using TiltedEngine;
using TiltedEngine.Networking;
using Archipelago.MultiClient.Net.Models;

namespace HammerwatchAP.Game
{
    public static class GameInterface
    {
        static readonly MethodInfo _mi_SystemHandler_GetStatisticsData = HammerwatchAP.hammerwatchAssembly.GetType("ARPGGame.Networking.SystemHandler").GetMethod("GetStatisticsData", BindingFlags.Public | BindingFlags.Static);

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
