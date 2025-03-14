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
                    //if (itemLocation == -1)
                    //{
                    //    ArchipelagoManager.archipelagoData.CheckLocation(item.Position);
                    //}
                    //else
                    //{
                    //    //This entire block is only ran on items you might not be able to pick up, we'll never show pickup messages for these
                    //    ArchipelagoManager.archipelagoData.CheckLocation(itemLocation, false);
                    //}
                }
            }
        }

        [HarmonyPatch(typeof(WorldItemPickupBehavior), nameof(WorldItemPickupBehavior.NetPickup))]
        internal static class NetPickup
        {
            static bool Prefix(WorldItemPickupBehavior __instance, WorldItem item, ref bool ___taken, ref string ___pickupText)
            {
                ___taken = true;
                if (ArchipelagoManager.playingArchipelagoSave)
                {
                    WorldItemArchipelagoBehavior apBehavior = __instance as WorldItemArchipelagoBehavior;
                    if (apBehavior != null)
                    {
                        if (!apBehavior.isCheck && ArchipelagoManager.ConnectedToAP())
                        {
                            if (apBehavior.itemLocation == -1)
                            {
                                ___pickupText = ArchipelagoManager.GetSendItemMessageFromPos(item.Position);
                            }
                            else
                            {
                                ___pickupText = "";
                            }
                        }
                    }
                    if (apBehavior == null || apBehavior.itemLocation == -1)
                    {
                        ArchipelagoManager.archipelagoData.CheckLocation(item.Position, string.IsNullOrEmpty(___pickupText));
                        ArchipelagoManager.PickupItemEffects(item.Position);
                    }
                    else
                    {
                        ArchipelagoManager.archipelagoData.CheckLocation(apBehavior.itemLocation, true);
                        ArchipelagoManager.PickupItemEffects(apBehavior.itemLocation);
                    }
                    ArchipelagoManager.PickupItemEffectsXml(item.Producer.Name, false);
                }
                return true;
            }
        }
    }
}
