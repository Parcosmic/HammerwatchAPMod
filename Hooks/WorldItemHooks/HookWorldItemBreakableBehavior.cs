using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ARPGGame.WorldItemBehaviors;
using TiltedEngine.WorldObjects;
using HammerwatchAP.Archipelago;
using HammerwatchAP.Game;
using HammerwatchAP.Util;

namespace HammerwatchAP.Hooks
{
    internal class HookWorldItemBreakableBehavior
    {
        static Type _t_WorldItemBreakableBehavior = HammerwatchAP.hammerwatchAssembly.GetType("ARPGGame.WorldItemBehaviors.WorldItemBreakableBehavior");
        static MethodInfo _mi_WorldItemBreakableBehavior_OnHit = _t_WorldItemBreakableBehavior.GetMethod("OnHit", BindingFlags.Instance | BindingFlags.Public);
        internal static void Hook()
        {
            HooksHelper.Hook(typeof(OnCollision));
            HooksHelper.Hook(typeof(OnHit));
        }

        [HarmonyPatch("WorldItemBreakableBehavior", "OnCollision")]
        internal static class OnCollision
        {
            static void Prefix(object __instance, WorldActor actor, WorldItem worldItem)
            {
                if (ArchipelagoManager.FragileBreakables && actor.Category == ActorCategory.Player)
                {
                    _mi_WorldItemBreakableBehavior_OnHit.Invoke(__instance, new object[] { actor, worldItem, 1 });
                }
            }
        }

        [HarmonyPatch("WorldItemBreakableBehavior", "OnHit")]
        internal static class OnHit
        {
            static void Postfix(WorldObject attacker, WorldItem item, int dmg, ref bool __result)
            {
                if (!__result) return;
                //TODO: fix
                //if (itemLocation == -1)
                //{
                //    ArchipelagoManager.CheckLocation(item.Position);
                //}
                //else
                //{
                //    ArchipelagoManager.CheckLocation(itemLocation, false);
                //}
                ArchipelagoManager.CheckLocation(item.Position);
            }
        }
    }
}
