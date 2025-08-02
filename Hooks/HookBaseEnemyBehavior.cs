using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using HarmonyLib;
using ARPGGame.Behaviors;
using TiltedEngine.Networking;
using TiltedEngine.WorldObjects;
using HammerwatchAP.Archipelago;
using HammerwatchAP.Game;

namespace HammerwatchAP.Hooks
{
    internal class HookBaseEnemyBehavior
	{
		static Type _t_BaseEnemyBehavior = HammerwatchAP.hammerwatchAssembly.GetType("ARPGGame.Behaviors.BaseEnemyBehavior");
		static FieldInfo _fi_BaseEnemyBehavior_lootTable = _t_BaseEnemyBehavior.GetField("lootTable", BindingFlags.NonPublic | BindingFlags.Instance);

		internal static void Hook()
		{
			HooksHelper.Hook(typeof(DamagedFromNet));
		}

		[HarmonyPatch("BaseEnemyBehavior", "DamagedFromNet")]
		internal static class DamagedFromNet
		{
			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
				MethodInfo _mi_OverrideLootTable = typeof(DamagedFromNet).GetMethod(nameof(DamagedFromNet.OverrideLootTable), BindingFlags.NonPublic | BindingFlags.Static);

				List<CodeInstruction> updateAPOptionsInstructions = new List<CodeInstruction>()
				{
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldarg_0),
					codes[codes.Count - 8],
					new CodeInstruction(OpCodes.Ldarg_0),
					codes[codes.Count - 4],
					new CodeInstruction(OpCodes.Call, _mi_OverrideLootTable),
					new CodeInstruction(OpCodes.Stfld, _fi_BaseEnemyBehavior_lootTable),
				};
				codes.InsertRange(codes.Count - 9, updateAPOptionsInstructions);

				return codes;
			}

			static object OverrideLootTable(object lootTable, WorldActor actor)
            {
				LootTableWrapper wrapper = ArchipelagoManager.OverrideLootTable(actor, new LootTableWrapper(lootTable));
				if (wrapper == null)
					return null;
				return wrapper.lootTable;
            }
        }
	}
}
