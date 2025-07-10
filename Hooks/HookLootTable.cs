using System;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using ARPGGame;
using TiltedEngine.WorldObjects;

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
			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

				return codes;
			}

			static void AddDynamicItem(object lootTable, WorldObject lootWorldObject)
            {

            }
        }
	}
}
