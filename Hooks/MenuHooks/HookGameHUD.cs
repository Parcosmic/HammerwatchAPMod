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

            static void Postfix(GameHUD __instance)
			{
				if (ArchipelagoManager.playingArchipelagoSave)
				{
					Widget bonusKeyWidget = __instance.Document.GetWidget("bonus-key");
					if (bonusKeyWidget != null)
					{
						SpriteWidget bonusKeySpriteWidget = (SpriteWidget)bonusKeyWidget;
						HooksHelper.SetWidgetVisible(bonusKeySpriteWidget, ArchipelagoManager.archipelagoData.GetSlotInt(SlotDataKeys.randomizeBonusKeys) > 0);
					}
					Widget hammerWidget = __instance.Document.GetWidget("hammer");
					if (hammerWidget != null)
					{
						SpriteWidget hammerSpriteWidget = (SpriteWidget)hammerWidget;
						if (ArchipelagoManager.archipelagoData.totalHammerFragments > 0)
						{
							HooksHelper.SetWidgetVisible(hammerSpriteWidget, true);
							string hammerTextString = ArchipelagoManager.archipelagoData.hammerFragments.ToString();
							if (ArchipelagoManager.archipelagoData.hammerFragments >= ArchipelagoManager.archipelagoData.totalHammerFragments)
								hammerTextString = "";
							((TextWidget)__instance.Document.GetWidget("hammer-fragments")).SetText(hammerTextString);
						}
						else
						{
							HooksHelper.SetWidgetVisible(hammerSpriteWidget, false);
						}
					}
					Widget extraInfo = __instance.Document.GetWidget("extra-info");
					if (extraInfo.Visible)
					{
						//If goal is not plank hunt or escape castle planks don't matter
						if (ArchipelagoManager.archipelagoData.goalType == ArchipelagoData.GoalType.PlankHunt || ArchipelagoManager.archipelagoData.goalType == ArchipelagoData.GoalType.FullCompletion)
						{
							if (extraInfo.GetWidget("strange-plank") != null)
							{
								HooksHelper.SetWidgetVisible((SpriteWidget)extraInfo.GetWidget("strange-plank"), true);
								int planksRequired;
								if(ArchipelagoManager.archipelagoData.goalType == ArchipelagoData.GoalType.PlankHunt)
                                {
									planksRequired = ArchipelagoManager.archipelagoData.plankHuntRequirement;
								}
								else
                                {
									planksRequired = 12;
                                }
								string planksRequiredString = (planksRequired < 10 ? " /  " : "/ ") + planksRequired.ToString();
								string planksString = (ArchipelagoManager.archipelagoData.planks < 10 ? " " : "") + ArchipelagoManager.archipelagoData.planks.ToString();
								string planksText = $"{planksString} {planksRequiredString}";
								((TextWidget)extraInfo.GetWidget("planks")).SetText(planksText);
							}
						}
					}
					if (ArchipelagoManager.archipelagoData.mapType == ArchipelagoData.MapType.Temple) //Extra menu stuff for tools
					{
						Widget extraInfo2 = __instance.Document.GetWidget("extra-info-2");
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
