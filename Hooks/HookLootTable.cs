using System;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using ARPGGame;

namespace HammerwatchAP.Hooks
{
    internal class HookLootTable
	{
		internal static void Hook()
		{
			HooksHelper.Hook(typeof(HookName));
		}

		[HarmonyPatch(typeof(TemplateHook), nameof(TemplateHook.Hook))]
		internal static class HookName
		{
			static void Postfix(TemplateHook __instance)
            {
				
			}
        }
	}
}
