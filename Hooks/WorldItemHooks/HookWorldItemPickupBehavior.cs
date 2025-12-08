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
            static void Postfix(WorldItemPickupBehavior __instance, WorldItem item, bool ___taken)
            {
                if (!___taken)
                {
                    ArchipelagoManager.archipelagoData.CheckLocation(item.Position);
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
                    int dynamicLocationID = ArchipelagoManager.GetDynamicLocation(item.NodeId);
                    if (dynamicLocationID == -1)
                    {
                        int locId = ArchipelagoManager.archipelagoData.CheckLocation(item.Position, string.IsNullOrEmpty(___pickupText));
                        ArchipelagoManager.PickupItemEffects(locId);
                    }
                    else
                    {
                        ArchipelagoManager.archipelagoData.CheckLocation(dynamicLocationID, true);
                        ArchipelagoManager.PickupItemEffects(dynamicLocationID);
                    }
                    ArchipelagoManager.PickupItemEffectsXml(item.Producer.Name, false);
                }
            }
        }
    }
}
