﻿using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using HarmonyLib;
using ARPGGame;
using ARPGGame.Menus;
using HammerwatchAP.Archipelago;
using HammerwatchAP.Util;

namespace HammerwatchAP.Hooks
{
    internal class HookShopMenu
	{
		static FieldInfo _fi_ShopMenu_upgrades = typeof(ShopMenu).GetField("upgrades", BindingFlags.NonPublic | BindingFlags.Instance);

		internal static void Hook()
		{
			HooksHelper.Hook(typeof(BuySharedUpgrade));
			HooksHelper.Hook(typeof(Focus));
		}

		[HarmonyPatch(typeof(ShopMenu), nameof(ShopMenu.BuySharedUpgrade))]
		internal static class BuySharedUpgrade
		{
			static void Postfix(PlayerInfo buyer, string id)
            {
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

                //Upgrade focusedUpgrade = ArchipelagoManager.GetNextShopUpgrade(upgrades[slot]);
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
