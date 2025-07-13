using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using HarmonyLib;
using ARPGGame;
using TiltedEngine.WorldObjects;
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
					new CodeInstruction(OpCodes.Ldloc_S, (byte)7),
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

			static void AddDynamicItem(object lootTable, WorldObject lootWorldObject)
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
			}
		}
	}
}
