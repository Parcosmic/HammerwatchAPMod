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
			HooksHelper.Hook(typeof(Damaged));
			HooksHelper.Hook(typeof(DamagedRanger));
			HooksHelper.Hook(typeof(DamagedThief));
		}

		[HarmonyPatch(typeof(PlayerActorBehavior), nameof(PlayerActorBehavior.TargetedByEnemy))]
		internal static class TargetedByEnemy
		{
			static void Postfix(PlayerActorBehavior __instance)
            {
				QoL.ResetExploreSpeed(__instance.PlayerInfo);
			}
		}

		[HarmonyPatch(typeof(PlayerActorBehavior), nameof(PlayerActorBehavior.Damaged))]
		internal static class Damaged
		{
			static void Prefix(PlayerActorBehavior __instance, WorldObject attacker)
			{
                if (attacker is WorldActor attackerActor && attackerActor.Category == __instance.Category)
                    return;
                QoL.ResetExploreSpeed(__instance.PlayerInfo);
			}
		}

		[HarmonyPatch("PlayerRangerActorBehavior", "Damaged")]
		internal static class DamagedRanger
		{
			static void Prefix(PlayerActorBehavior __instance, WorldObject attacker)
			{
				if (attacker is WorldActor attackerActor && attackerActor.Category == __instance.Category)
					return;
				QoL.ResetExploreSpeed(__instance.PlayerInfo);
			}
		}

		[HarmonyPatch("PlayerThiefActorBehavior", "Damaged")]
		internal static class DamagedThief
		{
			static void Prefix(PlayerActorBehavior __instance, WorldObject attacker)
			{
				if (attacker is WorldActor attackerActor && attackerActor.Category == __instance.Category)
					return;
				QoL.ResetExploreSpeed(__instance.PlayerInfo);
			}
		}
	}
}
