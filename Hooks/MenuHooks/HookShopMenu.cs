using System;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using ARPGGame;
using ARPGGame.Menus;
using HammerwatchAP.Archipelago;

namespace HammerwatchAP.Hooks
{
    internal class HookShopMenu
	{
		FieldInfo _fi_ShopMenu_upgrades = typeof(ShopMenu).GetField("upgrades", BindingFlags.NonPublic | BindingFlags.Instance);

		internal static void Hook()
		{
			HooksHelper.Hook(typeof(BuySharedUpgrade));
		}

		[HarmonyPatch(typeof(ShopMenu), nameof(ShopMenu.BuySharedUpgrade))]
		internal static class BuySharedUpgrade
		{
			static void Postfix(PlayerInfo buyer, string id)
            {
				ArchipelagoManager.BoughtAPShopItem(buyer, id);
			}
		}

		[HarmonyPatch(typeof(ShopMenu), "Focus")]
		internal static class Focus
		{
			//Going to probably need a transpiler (for handling progressive items)
		}
	}
}
