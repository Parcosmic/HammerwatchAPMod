using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using ARPGGame;
using HarmonyLib;
using HammerwatchAP.Archipelago;
using HammerwatchAP.Util;

namespace HammerwatchAP.Hooks
{
    internal class HookARPGGame
    {
        internal static void Hook()
        {
            HooksHelper.Hook(typeof(OnLoad));
        }

        [HarmonyPatch("ARPGGame.ARPGGame", "OnLoad")]
        internal static class OnLoad
        {
            static void Prefix(XElement ___options)
            {
                if (___options == null)
                    return;
                XElement apOptions = ___options.Element("Archipelago");
                if (apOptions != null)
                {
                    XElement deathlinkOption = apOptions.Element("Deathlink");
                    XElement exploreSpeedOption = apOptions.Element("ExploreSpeed");
                    XElement exploreSpeedPingOption = apOptions.Element("ExploreSpeedPing");
                    XElement fragileBreakablesOption = apOptions.Element("FragileBreakables");
                    XElement chatMirroringOption = apOptions.Element("APChatMirroring");
                    XElement shopItemHintingOption = apOptions.Element("ShopItemHinting");
                    XElement apDebugModeOption = apOptions.Element("APDebugMode");
                    XElement lastConnectedIPOption = apOptions.Element("LastConnectedIP");
                    XElement lastConnectedSlotNameOption = apOptions.Element("LastConnectedSlotName");
                    if (deathlinkOption != null)
                        ArchipelagoManager.Deathlink = bool.Parse(deathlinkOption.Value);
                    if (exploreSpeedOption != null)
                        ArchipelagoManager.ExploreSpeed = bool.Parse(exploreSpeedOption.Value);
                    if (exploreSpeedPingOption != null)
                        ArchipelagoManager.ExploreSpeedPing = bool.Parse(exploreSpeedPingOption.Value);
                    if (fragileBreakablesOption != null)
                        ArchipelagoManager.FragileBreakables = bool.Parse(fragileBreakablesOption.Value);
                    if (chatMirroringOption != null)
                        ArchipelagoManager.APChatMirroring = bool.Parse(chatMirroringOption.Value);
                    if (shopItemHintingOption != null)
                        ArchipelagoManager.ShopItemHinting = bool.Parse(shopItemHintingOption.Value);
                    if (apDebugModeOption != null)
                        ArchipelagoManager.DEBUG_MODE = bool.Parse(apDebugModeOption.Value);
                    if (lastConnectedIPOption != null)
                        ArchipelagoManager.LastConnectedIP = lastConnectedIPOption.Value;
                    if (lastConnectedSlotNameOption != null)
                        ArchipelagoManager.LastConnectedSlotName = lastConnectedSlotNameOption.Value;
                }
            }
            static void Postfix()
            {
                ArchipelagoManager.LoadMod();
            }
        }
    }
}
