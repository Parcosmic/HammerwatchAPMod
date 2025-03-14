using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Xml.Linq;
using HarmonyLib;
using ARPGGame;
using ARPGGame.Behaviors.Players;
using ARPGGame.Menus;
using TiltedEngine;
using TiltedEngine.WorldObjects;
using HammerwatchAP.Game;

namespace HammerwatchAP.Hooks
{
    internal class HookPlayerActorBehavior
	{
		static readonly FieldInfo _fi_PlayerActorBehavior_activeHitEffects = typeof(PlayerActorBehavior).GetField("_activeHitEffects", BindingFlags.Instance | BindingFlags.NonPublic);
		public static List<IBuff> GetPlayerActorBehaviorActiveHitEffects(PlayerActorBehavior behavior)
        {
			return (List<IBuff>)_fi_PlayerActorBehavior_activeHitEffects.GetValue(behavior);
        }
		public static void SetPlayerActorBehaviorActiveHitEffects(PlayerActorBehavior behavior, List<IBuff> activeHitEffects)
        {
			_fi_PlayerActorBehavior_activeHitEffects.SetValue(behavior, activeHitEffects);
        }
		internal static void Hook()
		{
			HooksHelper.Hook(typeof(TargetedByEnemy));
		}

		[HarmonyPatch(typeof(PlayerActorBehavior), nameof(PlayerActorBehavior.TargetedByEnemy))]
		internal static class TargetedByEnemy
		{
			static void Postfix(PlayerActorBehavior __instance)
            {
				QoL.ResetExploreSpeed(__instance.PlayerInfo);
			}
        }
	}
}
