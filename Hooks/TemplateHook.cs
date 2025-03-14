using System;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;

namespace HammerwatchAP.Hooks
{
    internal class TemplateHook
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
