using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using HarmonyLib;
using TiltedEngine;
using ARPGGame;
using ARPGGame.Menus;
using ARPGGame.ScriptNodes;
using HammerwatchAP.Archipelago;
using HammerwatchAP.Util;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;

namespace HammerwatchAP.Hooks
{
    internal class HookShopMenu
	{
		static FieldInfo _fi_ShopMenu_upgrades = typeof(ShopMenu).GetField("upgrades", BindingFlags.NonPublic | BindingFlags.Instance);

		internal static void Hook()
		{
			HooksHelper.Hook(typeof(ShopMenuCtor));
			HooksHelper.Hook(typeof(Back));
			HooksHelper.Hook(typeof(BuySharedUpgrade));
			HooksHelper.Hook(typeof(Focus));
		}

		[HarmonyPatch(typeof(ShopMenu), MethodType.Constructor, new Type[] { typeof(GameBase), typeof(ResourceBank), typeof(PlayerInfo), typeof(ShopArea) })]
		internal static class ShopMenuCtor
		{
            static void Postfix()
            {
				if(ArchipelagoManager.playingArchipelagoSave && ArchipelagoManager.archipelagoData.IsShopSanityOn())
					ArchipelagoManager.connectionInfo.SetMapTrackingKey("shop");
            }
        }

		[HarmonyPatch(typeof(ShopMenu), nameof(ShopMenu.Back))]
		internal static class Back
		{
			static void Prefix(Upgrade[] ___upgrades)
			{
				if (!ArchipelagoManager.playingArchipelagoSave)
					return;
				if (ArchipelagoManager.archipelagoData.IsShopSanityOn())
					ArchipelagoManager.connectionInfo.SetMapTrackingKey(ArchipelagoManager.archipelagoData.currentLevelId);
				List<int> hintLocationIds = new List<int>();
				foreach(Upgrade upgrade in ___upgrades)
				{
					if (upgrade.ID.StartsWith("ap-"))
					{
						int locId = int.Parse(upgrade.ID.Substring(3));
						NetworkItem item = ArchipelagoManager.archipelagoData.GetItemFromLoc(locId);
						//Only hint for progression items
						if((item.Flags & ItemFlags.Advancement) != ItemFlags.None)
						{
							hintLocationIds.Add(locId);

						}
					}
                }
				if(hintLocationIds.Count > 0 && ArchipelagoManager.ShopItemHinting)
				{
					ArchipelagoManager.connectionInfo.HintLocations(hintLocationIds);
				}
			}
		}

		[HarmonyPatch(typeof(ShopMenu), nameof(ShopMenu.BuySharedUpgrade))]
		internal static class BuySharedUpgrade
		{
			static void Postfix(PlayerInfo buyer, string id)
			{
				if (!ArchipelagoManager.playingArchipelagoSave)
					return;
				Logging.Debug($"Bought upgrade: {id}");
				ArchipelagoManager.BoughtAPShopItem(buyer, id);
			}
		}

		[HarmonyPatch(typeof(ShopMenu), "Focus")]
		internal static class Focus
		{
			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
			{
				List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
				MethodInfo _mi_ArchipelagoManager_GetNextShopUpgrade = typeof(ArchipelagoManager).GetMethod(nameof(ArchipelagoManager.GetNextShopUpgrade), BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(Upgrade) }, null);

                LocalBuilder focusedUpgradeLocalBuilder = il.DeclareLocal(typeof(Upgrade));

                List<CodeInstruction> getFocusedUpgradeInstructions = new List<CodeInstruction>()
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, _fi_ShopMenu_upgrades),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldelem, typeof(Upgrade)),
                    new CodeInstruction(OpCodes.Call, _mi_ArchipelagoManager_GetNextShopUpgrade),
                    new CodeInstruction(OpCodes.Stloc_S, focusedUpgradeLocalBuilder.LocalIndex),
                };
                int ldarg0Counter = 0;
				for (int c = 0; c < codes.Count; c++)
				{
					if (codes[c].opcode == OpCodes.Ldarg_0)
					{
                        ldarg0Counter++;
                        switch (ldarg0Counter)
                        {
                            case 2:
                                codes.InsertRange(c, getFocusedUpgradeInstructions);
                                c += getFocusedUpgradeInstructions.Count;
                                break;
                            case 3:
                            case 5:
                                codes[c].opcode = OpCodes.Nop;
                                codes[c + 1].opcode = OpCodes.Nop;
                                codes[c + 2].opcode = OpCodes.Nop;
                                codes[c + 3] = new CodeInstruction(OpCodes.Ldloc_S, focusedUpgradeLocalBuilder.LocalIndex);
                                break;
                        }
                        if (ldarg0Counter >= 5)
                            break;
                    }
				}

				return codes;
			}
		}
	}
}
