using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Xml.Linq;
using HarmonyLib;
using ARPGGame;
using ARPGGame.Menus;
using TiltedEngine;
using HammerwatchAP.Archipelago;

namespace HammerwatchAP.Hooks
{
    internal class HookSharedPlayerInfo
	{
		internal static void Hook()
		{
			HooksHelper.Hook(typeof(SharedPlayerInfoConstructor));
		}

        [HarmonyPatch(typeof(SharedPlayerInfo), MethodType.Constructor, new Type[] { })]
        internal static class SharedPlayerInfoConstructor
        {
            static void Postfix(ref int[] ____keys)
            {
                ____keys = new int[APData.PLAYER_KEYS];
            }
        }
    }
}
