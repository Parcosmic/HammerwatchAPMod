using System.Xml.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ARPGGame;
using ARPGGame.Menus;
using ARPGGame.GUI;
using HammerwatchAP.Archipelago;
using HammerwatchAP.Util;

namespace HammerwatchAP.Hooks
{
    internal class HookGameHUD
	{
		internal static void Hook()
		{
			HooksHelper.Hook(typeof(UpdateHUD));
		}

		[HarmonyPatch(typeof(GameHUD), "UpdateHUD")]
		internal static class UpdateHUD
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

                for (int c = 0; c < codes.Count; c++)
                {
                    if (codes[c].opcode == OpCodes.Ldc_I4_S && codes[c].operand is sbyte codeOperand && codeOperand == 10)
                    {
                        codes[c].operand = (sbyte)APData.PLAYER_KEYS;
                        break;
                    }
                }

                return codes;
            }

            static void Postfix(GameHUD __instance, int ___refreshTimer1)
			{
				if (ArchipelagoManager.playingArchipelagoSave)
				{
					if (__instance.Document.GetWidget("hammer") != null)
					{
						SpriteWidget hammerWidget = (SpriteWidget)__instance.Document.GetWidget("hammer");
						if (ArchipelagoManager.archipelagoData.totalHammerFragments > 0)
						{
							HooksHelper.SetWidgetVisible(hammerWidget, true);
							string hammerTextString = ArchipelagoManager.archipelagoData.hammerFragments.ToString();
							if (ArchipelagoManager.archipelagoData.hammerFragments >= ArchipelagoManager.archipelagoData.totalHammerFragments)
								hammerTextString = "";
							((TextWidget)__instance.Document.GetWidget("hammer-fragments")).SetText(hammerTextString);
						}
						else
						{
							HooksHelper.SetWidgetVisible(hammerWidget, false);
						}
					}
					Widget extraInfo = __instance.Document.GetWidget("extra-info");
					if (extraInfo.Visible && ___refreshTimer1 <= 0 && GameBase.Instance.lvlList.CurrentLevel != null)
					{
						//APMod
						//If goal is not plank hunt or escape castle planks don't matter
						if (ArchipelagoManager.archipelagoData.goalType == ArchipelagoData.GoalType.PlankHunt || ArchipelagoManager.archipelagoData.goalType == ArchipelagoData.GoalType.FullCompletion)
						{
							if (extraInfo.GetWidget("strange-plank") != null)
							{
								//((SpriteWidget)extraInfo.GetWidget("strange-plank")).Visible = true;
								HooksHelper.SetWidgetVisible((SpriteWidget)extraInfo.GetWidget("strange-plank"), true);
								((TextWidget)extraInfo.GetWidget("planks")).SetText(ArchipelagoManager.archipelagoData.planks.ToString());
							}
						}
						//End APMod
						((TextWidget)extraInfo.GetWidget("act")).SetText(ArchipelagoMessageManager.GetLanguageString(GameBase.Instance.lvlList.CurrentLevel.ActName, new string[0]));
						((TextWidget)extraInfo.GetWidget("level")).SetText(ArchipelagoMessageManager.GetLanguageString(GameBase.Instance.lvlList.CurrentLevel.Name, new string[0]));
					}
					if (ArchipelagoManager.archipelagoData.mapType == ArchipelagoData.MapType.Temple) //Extra menu stuff for tools
					{
						Widget extraInfo2 = __instance.Document.GetWidget("extra-info-2");
						//extraInfo2.Visible = GameBase.Instance.WorldDrawer.ShowMinimap;
						HooksHelper.SetWidgetVisible(extraInfo2, GameBase.Instance.WorldDrawer.ShowMinimap);
						TextWidget leverWidget = ((TextWidget)extraInfo2.GetWidget("lever-fragments"));
						TextWidget panWidget = ((TextWidget)extraInfo2.GetWidget("pan-fragments"));
						TextWidget pickaxeWidget = ((TextWidget)extraInfo2.GetWidget("pickaxe-fragments"));
						if (ArchipelagoManager.archipelagoData.totalPanFragments > 1 && ArchipelagoManager.archipelagoData.panFragments != ArchipelagoManager.archipelagoData.totalPanFragments)
							HooksHelper.SetWidgetVisible(panWidget, true);
						else
							HooksHelper.SetWidgetVisible(panWidget, false);
						if (ArchipelagoManager.archipelagoData.totalLeverFragments > 1 && ArchipelagoManager.archipelagoData.leverFragments != ArchipelagoManager.archipelagoData.totalLeverFragments)
							HooksHelper.SetWidgetVisible(leverWidget, true);
						else
							HooksHelper.SetWidgetVisible(leverWidget, false);
						if (ArchipelagoManager.archipelagoData.totalPickaxeFragments > 1 && ArchipelagoManager.archipelagoData.pickaxeFragments != ArchipelagoManager.archipelagoData.totalPickaxeFragments)
							HooksHelper.SetWidgetVisible(pickaxeWidget, true);
						else
							HooksHelper.SetWidgetVisible(pickaxeWidget, false);
						panWidget.SetText(ArchipelagoManager.archipelagoData.panFragments.ToString());
						leverWidget.SetText(ArchipelagoManager.archipelagoData.leverFragments.ToString());
						pickaxeWidget.SetText(ArchipelagoManager.archipelagoData.pickaxeFragments.ToString());

					}
				}
			}
        }
	}
}
