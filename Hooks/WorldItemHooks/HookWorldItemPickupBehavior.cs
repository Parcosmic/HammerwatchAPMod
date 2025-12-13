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
    internal class HookWorldItemPickupBehavior
    {
        internal static void Hook()
        {
            HooksHelper.Hook(typeof(TryPickup));
            HooksHelper.Hook(typeof(NetPickup));
        }

        [HarmonyPatch(typeof(WorldItemPickupBehavior), nameof(WorldItemPickupBehavior.TryPickup))]
        internal static class TryPickup
        {
            //For consumables that you might not be able to pick up, this lets us report to the server that we found the check
            static void Postfix(WorldItemPickupBehavior __instance, WorldItem item, bool ___taken)
            {
                if (!___taken)
                {
                    ArchipelagoManager.archipelagoData.CheckItemLocation(item.NodeId, item.Position);
                }
            }
        }

        [HarmonyPatch(typeof(WorldItemPickupBehavior), nameof(WorldItemPickupBehavior.NetPickup))]
        internal static class NetPickup
        {
            static void Prefix(WorldItemPickupBehavior __instance, WorldItem item, ref bool ___taken, ref string ___pickupText)
            {
                ___taken = true;
                if (ArchipelagoManager.playingArchipelagoSave)
                {
                    if (__instance is WorldItemArchipelagoBehavior apBehavior)
                    {
                        if (!apBehavior.isCheck)
                        {
                            ___pickupText = ArchipelagoManager.GetSendItemMessageFromPos(item.Position); //For floor master keys/boss runes?
                        }
                    }
                    int location = ArchipelagoManager.archipelagoData.CheckItemLocation(item.NodeId, item.Position, string.IsNullOrEmpty(___pickupText));
                    ArchipelagoManager.PickupItemEffects(location);
                    ArchipelagoManager.PickupItemEffectsXml(item.Producer.Name, false);
                }
            }
        }
    }
}
