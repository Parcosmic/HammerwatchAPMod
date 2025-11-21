using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using HarmonyLib;
using SDL2;
using TiltedEngine.Drawing;
using HammerwatchAP.Archipelago;
using HammerwatchAP.Util;

namespace HammerwatchAP.Hooks
{
    internal class HookResourceContext
	{
		internal static void Hook()
		{
			HooksHelper.Hook(typeof(OutputError));
		}

		[HarmonyPatch(typeof(ResourceContext), nameof(ResourceContext.OutputError))]
		internal static class OutputError
		{
			static void Postfix()
			{
				SDL.SDL_ShowSimpleMessageBox(SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR, "Crash! :(", ArchipelagoManager.ERROR_MESSAGE, IntPtr.Zero);
				//throw new Exception(ArchipelagoManager.ERROR_MESSAGE);
			}
        }
	}
}
