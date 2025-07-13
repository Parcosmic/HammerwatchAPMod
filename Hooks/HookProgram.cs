using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using HarmonyLib;
using ARPGGame;
using HammerwatchAP.Archipelago;
using HammerwatchAP.Util;

namespace HammerwatchAP.Hooks
{
    internal class HookProgram
	{
		internal static void Hook()
		{
			HooksHelper.Hook(typeof(Main));
		}

		[HarmonyPatch("Program", "Main")]
		internal static class Main
		{
			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

				foreach(CodeInstruction code in codes)
                {
					if(code.opcode == OpCodes.Ldstr && ((string)code.operand).StartsWith("The game just crashed"))
                    {
						code.operand = ArchipelagoManager.ERROR_MESSAGE;
						break;
                    }
                }

				return codes;
			}
        }
	}
}
