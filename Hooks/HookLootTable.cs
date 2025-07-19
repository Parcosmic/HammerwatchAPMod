using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using HarmonyLib;
using ARPGGame;
using OpenTK;
using TiltedEngine;
using TiltedEngine.WorldObjects;
using Archipelago.MultiClient.Net.Models;
using HammerwatchAP.Archipelago;
using HammerwatchAP.Game;
using HammerwatchAP.Util;

namespace HammerwatchAP.Hooks
{
    internal class HookLootTable
	{
		internal static void Hook()
		{
			HooksHelper.Hook(typeof(SpawnLoot));
		}

		[HarmonyPatch("LootTable", "SpawnLoot")]
		internal static class SpawnLoot
		{
			static int entryCounter = 0;
			static int entries = 0;

			static void Prefix(object __instance)
            {
				entryCounter = 0;
				entries = new LootTableWrapper(__instance).entries.Count;
            }

			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
				MethodInfo _mi_AddDynamicItem = typeof(SpawnLoot).GetMethod(nameof(SpawnLoot.AddDynamicItem), BindingFlags.NonPublic | BindingFlags.Static);

				List<CodeInstruction> addDynamicItemInstructions = new List<CodeInstruction>()
				{
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldarg_1),
					new CodeInstruction(OpCodes.Ldloc_S, (byte)7),
					new CodeInstruction(OpCodes.Ldloc_S, (byte)4),
					new CodeInstruction(OpCodes.Call, _mi_AddDynamicItem),
				};
				for (int c = 0; c < codes.Count; c++)
                {
					if(codes[c].opcode == OpCodes.Ldstr && (string)codes[c].operand == "SpawnItem")
                    {
						codes.InsertRange(c, addDynamicItemInstructions);
						break;
                    }
                }

				return codes;
			}

			static void AddDynamicItem(object lootTable, WorldNode worldNode, WorldObject lootWorldObject, Vector2 itemPosition)
            {
				if(!ArchipelagoManager.playingArchipelagoSave)
					return;
				if(!ArchipelagoManager.archipelagoData.lootTableAPLocationIDs.TryGetValue(lootTable, out List<int> locationIDs))
                {
					return;
                }
				int apLocationIndex = entryCounter++ - entries + locationIDs.Count;
				if(apLocationIndex < 0)
                {
					return;
                }
				//ArchipelagoManager.archipelagoData.dynamicItemLocations[ArchipelagoManager.archipelagoData.currentLevelName][lootWorldObject.NodeId] = locationIDs[apLocationIndex];
				ArchipelagoManager.AddDynamicLocation(lootWorldObject.NodeId, locationIDs[apLocationIndex]);
				if(apLocationIndex == locationIDs.Count - 1)
                {
					ArchipelagoManager.archipelagoData.lootTableAPLocationIDs.Remove(lootTable);
				}
				//If this is an offworld item create an attached effect
				NetworkItem lootItem = ArchipelagoManager.archipelagoData.GetItemFromLoc(locationIDs[apLocationIndex]);
				string itemXmlName = ArchipelagoManager.GetItemXmlName(lootItem, false);
                if (APData.IsItemXmlNameOffworld(itemXmlName))
                {
					string holoItemXmlName = ArchipelagoManager.GetItemXmlName(lootItem, false, true);
					string spriteName = $"{holoItemXmlName}:";
					Sprite toPlay = GameBase.Instance.resources.GetResource<Sprite>(spriteName);
					if (toPlay != null)
					{
						ObjectAttachedEffect itemEffect = new ObjectAttachedEffect(lootWorldObject, Vector2.Zero, spriteName, toPlay.Copy(), true, 20);
						worldNode.Place(itemPosition.X, itemPosition.Y, itemEffect, false);
						itemEffect.SetPlaytime(60000);
					}
				}
			}
		}
	}
}
