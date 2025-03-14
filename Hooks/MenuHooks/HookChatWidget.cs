using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Xml.Linq;
using HarmonyLib;
using ARPGGame;
using ARPGGame.GUI;
using ARPGGame.Behaviors.Players;
using ARPGGame.Menus;
using TiltedEngine;
using TiltedEngine.WorldObjects;
using HammerwatchAP.Archipelago;

namespace HammerwatchAP.Hooks
{
    internal static class HookChatWidget
	{
		static FieldInfo _fi_ChatWidget_input = typeof(ChatWidget).GetField("input", BindingFlags.Instance | BindingFlags.NonPublic);
		internal static void Hook()
		{
			HooksHelper.Hook(typeof(OnChat));
		}

		[HarmonyPatch(typeof(ChatWidget), "OnChat")]
		internal static class OnChat
		{
			static void Prefix(ChatWidget __instance)
			{
				string text = ((InputWidget)_fi_ChatWidget_input.GetValue(__instance)).GetText();
				if (string.IsNullOrEmpty(text)) return;
				ArchipelagoMessageManager.SentChatMessage(text);
			}
		}
	}
}
