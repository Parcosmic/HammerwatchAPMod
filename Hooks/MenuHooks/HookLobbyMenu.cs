using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ARPGGame;
using ARPGGame.Menus;
using ARPGGame.GUI;
using TiltedEngine;
using TiltedEngine.Networking;
using TiltedEngine.Drawing;
using HammerwatchAP.Archipelago;
using HammerwatchAP.Util;
using HammerwatchAP.Game;

namespace HammerwatchAP.Hooks
{
    internal class HookLobbyMenu
    {
        static Widget loadButton;

        static readonly FieldInfo _fi_LobbyMenu_myPeerId = typeof(LobbyMenu).GetField("myPeerId", BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo _fi_LobbyMenu_slotWidgets = typeof(LobbyMenu).GetField("slotWidgets", BindingFlags.Instance | BindingFlags.NonPublic);
        static Widget[] GetSlotWidgets(LobbyMenu lobbyMenu)
        {
            return (Widget[])_fi_LobbyMenu_slotWidgets.GetValue(lobbyMenu);
        }
        static int GetMyPeerId(LobbyMenu lobbyMenu)
        {
            return (int)_fi_LobbyMenu_myPeerId.GetValue(lobbyMenu);
        }

        internal static void Hook()
        {
            HooksHelper.Hook(typeof(LobbyMenuConstructor));
            HooksHelper.Hook(typeof(LobbyMenuConstructor2));
            HooksHelper.Hook(typeof(LoadGUI));
            HooksHelper.Hook(typeof(JoinRequestResponseDropin));
            HooksHelper.Hook(typeof(PlayerJoined));
            HooksHelper.Hook(typeof(RefreshSlots));
            HooksHelper.Hook(typeof(UpdateModifiers));
            HooksHelper.Hook(typeof(GetFunction));
        }

        [HarmonyPatch(typeof(LobbyMenu), MethodType.Constructor, new Type[] { typeof(GameBase), typeof(ResourceBank), typeof(GamePlayers) })]
        internal static class LobbyMenuConstructor
        {
            static void Postfix(LobbyMenu __instance)
            {
                __instance.SetDifficulty(ArchipelagoManager.archipelagoData.GetDifficulty());
            }
        }

        [HarmonyPatch(typeof(LobbyMenu), MethodType.Constructor, new Type[] { typeof(GameBase), typeof(ResourceBank), typeof(bool), typeof(bool), typeof(int) })]
        internal static class LobbyMenuConstructor2
        {
            static void Postfix(LobbyMenu __instance, bool host)
            {
                if (!ArchipelagoManager.playingArchipelagoSave)
                    return;
                __instance.SetDifficulty(ArchipelagoManager.archipelagoData.GetDifficulty());
                bool shopsanityPlayerSet = false;
                for (int p = 3; p >= 0; p--)
                {
                    if (ArchipelagoManager.archipelagoData.shopsanityClasses[p].HasValue)
                    {
                        shopsanityPlayerSet = true;
                        if (__instance.slots[p] == null)
                        {
                            __instance.slots[p] = new PlayerSlot
                            {
                                Name = "Player " + (p + 1),
                                PlayerClass = ArchipelagoManager.archipelagoData.shopsanityClasses[p].Value
                            };
                            __instance.AssignClassVariation(p, false);
                            __instance.slots[p].Ready = true;
                            __instance.RefreshSlots();
                        }
                        else
                        {
                            __instance.ChangeClass(p, ArchipelagoManager.archipelagoData.shopsanityClasses[p].Value);
                        }
                    }
                    else if (shopsanityPlayerSet)
                    {
                        __instance.slots[p] = new PlayerSlot
                        {
                            Name = "Player " + (p + 1),
                            PlayerClass = PlayerClass.KNIGHT
                        };
                        __instance.AssignClassVariation(p, false);
                        __instance.slots[p].Ready = true;
                        __instance.RefreshSlots();
                    }
                }
                __instance.UpdateModifiers(null);
                //We just set the game to a blank save while we wait for Archipelago to load the right one
                if (ArchipelagoManager.saveFileName == null)
                {
                    if (ArchipelagoManager.mapFinishedGenerating)
                    {
                        ArchipelagoManager.SetArchipelagoLevel(__instance, ArchipelagoManager.archipelagoData.mapFileName);
                    }
                }
                else
                {
                    __instance.SetLevel("", "");
                }
                //ArchipelagoManager.OutputAbsorbedMessages();
                //if (!ArchipelagoManager.mapFinishedGenerating)
                //{
                //    ArchipelagoManager.gameState = ArchipelagoManager.GameState.Generating;
                //}
                HooksHelper.SetWidgetVisible(loadButton, host);
            }
        }

        [HarmonyPatch(typeof(LobbyMenu), "LoadGUI")]
        internal static class LoadGUI
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
                MethodInfo _mi_PatchLobbyMenuDoc = typeof(LoadGUI).GetMethod(nameof(PatchLobbyMenuDoc), BindingFlags.NonPublic | BindingFlags.Static);

                //for (int c = 0; c < codes.Count; c++)
                //{
                //    if (codes[c].opcode == OpCodes.Callvirt)
                //    {
                //        codes[c + 1] = new CodeInstruction(OpCodes.Nop);
                //        codes[c + 2] = new CodeInstruction(OpCodes.Nop);
                //        codes[c + 3] = new CodeInstruction(OpCodes.Call, _mi_PatchMainMenuDoc);
                //        break;
                //    }
                //}
                HooksHelper.PatchLoadMenu(codes, _mi_PatchLobbyMenuDoc);

                return codes;
            }

            static void Postfix(LobbyMenu __instance)
            {
                loadButton = __instance.Document.GetWidget("load");
                __instance.Document.GetWidget("start").Offset = new Point(60, -12);
            }

            static XDocument PatchLobbyMenuDoc(ResourceContext context)
            {
                XDocument doc = context.LoadXML("menus/gui/lobby.xml", new ErrorLogger());

                //Hacking in a button for our Archipelago menu
                XElement button = new XElement("textbutton");
                button.SetAttributeValue("click", "load");
                button.SetAttributeValue("id", "load");
                button.SetAttributeValue("anchor", "0% 100%");
                button.SetAttributeValue("offset", "108 -12");
                button.SetAttributeValue("text", "Load");
                doc.Root.Element("base").Element("sprite").AddFirst(button);

                return doc;
            }
        }

        [HarmonyPatch(typeof(LobbyMenu), "JoinRequestResponseDropin")]
        internal static class JoinRequestResponseDropin
        {
            static void Postfix(LobbyMenu __instance, int peerId)
            {
                if (ArchipelagoManager.playingArchipelagoSave)
                {
                    if (ArchipelagoManager.archipelagoData.shopsanityClasses[peerId].HasValue)
                    {
                        __instance.ChangeClass(peerId, ArchipelagoManager.archipelagoData.shopsanityClasses[peerId].Value);
                        Network.SendToAll("ChangeClass", new object[]
                        {
                        ArchipelagoManager.archipelagoData.shopsanityClasses[peerId].Value
                        });
                        __instance.RefreshSlots();
                    }
                }
            }
        }

        [HarmonyPatch(typeof(LobbyMenu), "PlayerJoined")]
        internal static class PlayerJoined
        {
            static void Postfix(LobbyMenu __instance, int peerId)
            {
                if (!ArchipelagoManager.playingArchipelagoSave)
                    return;
                if (ArchipelagoManager.archipelagoData.shopsanityClasses[peerId].HasValue)
                {
                    __instance.ChangeClass(peerId, ArchipelagoManager.archipelagoData.shopsanityClasses[peerId].Value);
                }
            }
        }

        [HarmonyPatch(typeof(LobbyMenu), "RefreshSlots")]
        internal static class RefreshSlots
        {
            static void Postfix(LobbyMenu __instance)
            {
                if (ArchipelagoManager.playingArchipelagoSave)
                {
                    if (GetMyPeerId(__instance) != -1)
                    {
                        for (int p = 0; p < 4; p++)
                        {
                            if (ArchipelagoManager.archipelagoData.shopsanityClasses[p].HasValue)
                                HooksHelper.SetWidgetEnabled((ButtonWidget)GetSlotWidgets(__instance)[p].GetWidget("class").Children[0], false);
                                //((ButtonWidget)GetSlotWidgets(__instance)[p].GetWidget("class").Children[0]).Enabled = false;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(LobbyMenu), "UpdateModifiers")]
        internal static class UpdateModifiers
        {
            static bool Prefix(LobbyMenu __instance, SObject modData)
            {
                Widget modPnl = __instance.Document.GetWidget("pick-modifiers");
                if (ArchipelagoManager.playingArchipelagoSave)
                {
                    foreach (string widgetId in widgetIdToModName.Keys)
                    {
                        if (ArchipelagoManager.archipelagoData.gameModifiers.TryGetValue(widgetIdToModName[widgetId], out bool active))
                        {
                            ((CheckboxWidget)modPnl.GetWidget(widgetId).Children[0]).Checked = active;
                        }
                    }
                }

                return true;
            }
            static void Postfix(LobbyMenu __instance, SObject modData)
            {
                Widget modPnl = __instance.Document.GetWidget("pick-modifiers");
                if (ArchipelagoManager.playingArchipelagoSave)
                {
                    foreach (string widgetId in widgetIdToModName.Keys)
                    {
                        if (ArchipelagoManager.archipelagoData.gameModifiers.ContainsKey(widgetIdToModName[widgetId]))
                        {
                            //((CheckboxWidget)modPnl.GetWidget(widgetId).Children[0]).Enabled = false;
                            HooksHelper.SetWidgetEnabled((CheckboxWidget)modPnl.GetWidget(widgetId).Children[0], false);
                        }
                    }
                }
            }
        }
        private static Dictionary<string, string> widgetIdToModName = new Dictionary<string, string>()
        {
            {"nolives", "no_extra_lives"},
            {"1hp", "1_hp"},
            {"sharehp", "shared_hp_pool"},
            {"nohppup", "no_hp_pickups"},
            {"nomanaregen", "no_mana_regen"},
            {"revhpregen", "reverse_hp_regen"},
            {"inflives", "infinite_lives"},
            {"hpregen", "hp_regen"},
            {"doubledmg", "double_damage"},
            {"doublelife", "double_lives"},
            {"quickmana", "5x_mana_regen"},
        };

        [HarmonyPatch(typeof(LobbyMenu), "GetFunction")]
        internal static class GetFunction
        {
            static bool Prefix(LobbyMenu __instance, string name, ref Action<Widget> __result)
            {
                switch(name)
                {
                    case "levels":
                        return false;
                    case "difficulty":
                        return false;
                    case "load":
                        __result = delegate (Widget param0)
                        {
                            GameBase.Instance.SetMenu(MenuType.LOAD, new object[] { __instance.slots.Length == 1 });
                        };
                        return false;
                }
                return true;
            }
        }
    }
}
