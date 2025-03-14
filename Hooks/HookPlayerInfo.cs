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
    internal class HookPlayerInfo
	{
		internal static void Hook()
		{
			HooksHelper.Hook(typeof(Save));
		}

		[HarmonyPatch(typeof(PlayerInfo), "Save")]
		internal static class Save
		{
			static void Postfix(PlayerInfo __instance, ref SValue __result)
            {
				int[] keys = new int[2 * APData.PLAYER_KEYS];
				for (int i = 0; i < keys.Length; i += 2)
				{
					keys[i] = i / 2;
					keys[i + 1] = __instance.GetKey(i / 2);
				}
				__result.GetObject().Set("keys", keys);
			}
        }
	}
}
