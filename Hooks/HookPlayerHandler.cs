using System;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using ARPGGame;
using ARPGGame.Networking;
using HammerwatchAP.Archipelago;

namespace HammerwatchAP.Hooks
{
    internal class HookPlayerHandler
	{
		static Type _t_PlayerHandler = HammerwatchAP.hammerwatchAssembly.GetType("ARPGGame.Networking.PlayerHandler");
		static MethodInfo _mi_PlayerHandler_GetPlayer = _t_PlayerHandler.GetMethod("GetPlayer", BindingFlags.Static | BindingFlags.Public);
		internal static void Hook()
		{
			HooksHelper.Hook(typeof(SetPlayerUpgrades));
		}

		[HarmonyPatch("PlayerHandler", "SetPlayerUpgrades")]
		internal static class SetPlayerUpgrades
		{
			static void Postfix(int peerId)
			{
				PlayerInfo playerInfo = (PlayerInfo)_mi_PlayerHandler_GetPlayer.Invoke(null, new object[] { peerId });
				if (playerInfo == null)
					return;
				ArchipelagoManager.SyncUpgrades(playerInfo);
			}
        }
	}
}
