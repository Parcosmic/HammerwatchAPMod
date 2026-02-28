using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ARPGGame;
using ARPGGame.Menus;
using ARPGGame.GUI;
using TiltedEngine;
using TiltedEngine.Drawing;
using HammerwatchAP.Archipelago;
using HammerwatchAP.Util;
using HammerwatchAP.Game;

namespace HammerwatchAP.Hooks
{
    internal class HookGameBase
    {
        internal static void Hook()
        {
            HooksHelper.Hook(typeof(InitializeResources));
            HooksHelper.Hook(typeof(StartGame));
            HooksHelper.Hook(typeof(ChangeLevel));
            HooksHelper.Hook(typeof(ResetGame));
            HooksHelper.Hook(typeof(PlayerDeath));
            HooksHelper.Hook(typeof(ClientDisconnected));
            HooksHelper.Hook(typeof(ClientDropin));
            HooksHelper.Hook(typeof(ClearLevel));
            HooksHelper.Hook(typeof(Update));
        }

        [HarmonyPatch(typeof(GameBase), "InitializeResources")]
        internal static class InitializeResources
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
                MethodInfo _mi_PatchNetworkDoc = typeof(InitializeResources).GetMethod(nameof(PatchNetworkDoc), BindingFlags.NonPublic | BindingFlags.Static);

                for (int c = 0; c < codes.Count; c++)
                {
                    if (codes[c].opcode == OpCodes.Ldstr && (string)codes[c].operand == "network.xml")
                    {
                        codes[c] = new CodeInstruction(OpCodes.Nop);
                        codes[c + 1] = new CodeInstruction(OpCodes.Nop);
                        codes[c + 2] = new CodeInstruction(OpCodes.Call, _mi_PatchNetworkDoc);
                        break;
                    }
                }

                return codes;
            }

            static XDocument PatchNetworkDoc(ResourceContext context)
            {
                XDocument networkXml = context.LoadXML("network.xml", new ErrorLogger());
                foreach (XElement element in networkXml.Root.Elements("message"))
                {
                    string str1 = element.Attribute("name").Value;
                    if (str1 != "SetNumKeys") continue;
                    element.Element("int").SetAttributeValue("max", APData.PLAYER_KEYS - 1);
                }
                Logging.Log("---Patched newtork.xml!");

                return networkXml;
            }
        }

        [HarmonyPatch(typeof(GameBase), "StartGame")]
        internal static class StartGame
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
                MethodInfo _mi_GetStartPosition = typeof(StartGame).GetMethod(nameof(GetStartPosition), BindingFlags.NonPublic | BindingFlags.Static);

                for (int c = 0; c < codes.Count; c++)
                {
                    if (codes[c].opcode == OpCodes.Ldc_I4_0)
                    {
                        codes[c] = new CodeInstruction(OpCodes.Call, _mi_GetStartPosition);
                        break;
                    }
                }

                return codes;
            }

            static int GetStartPosition()
            {
                int startPosition = 0;
                if (ArchipelagoManager.playingArchipelagoSave)
                {
                    string startCode = ArchipelagoManager.archipelagoData.GetStartExitCode();
                    string[] startSplits = startCode.Split('|');
                    startPosition = int.Parse(startSplits[1]);
                }
                return startPosition;
            }
        }

        [HarmonyPatch(typeof(GameBase), nameof(GameBase.ResetGame))]
        internal static class ResetGame
        {
            static void Postfix(bool loadMenu)
            {
                ArchipelagoManager.ResetGame(loadMenu);
            }
        }

        [HarmonyPatch(typeof(GameBase), "ChangeLevel")]
        internal static class ChangeLevel
        {
            static void Postfix(string levelId)
            {
                ArchipelagoManager.ChangedLevel(levelId);
            }
        }

        [HarmonyPatch(typeof(GameBase), "PlayerDeath")]
        internal static class PlayerDeath
        {
            static void Postfix(int player, int delay)
            {
                if(ArchipelagoManager.playingArchipelagoSave && delay > 0)
                {
                    ArchipelagoManager.PlayerDeath(GameBase.Instance.Players[player]);
                }
            }
        }

        [HarmonyPatch(typeof(GameBase), "ClientDisconnected")]
        internal static class ClientDisconnected
        {
            static void Prefix(int peerId, string reason)
            {
                if (GameBase.Instance.Players != null && GameBase.Instance.Players[peerId] != null)
                    ResourceContext.Log($"Player {GameBase.Instance.Players[peerId].Name} disconnected: " + reason);
            }
        }

        [HarmonyPatch(typeof(GameBase), "ClientDropin")]
        internal static class ClientDropin
        {
            static void Postfix(PlayerInfo plr)
            {
                ResourceContext.Log($"Player {plr.Name} dropped in");
            }
        }

        [HarmonyPatch(typeof(GameBase), "ClearLevel")]
        internal static class ClearLevel
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
                MethodInfo _mi_ArchipelagoManager_RefreshEvents = typeof(ArchipelagoManager).GetMethod(nameof(ArchipelagoManager.RefreshEvents), BindingFlags.Public | BindingFlags.Static);

                bool refreshCodeInserted = false;
                for (int c = 0; c < codes.Count - 7; c++)
                {
                    if (codes[c].opcode == OpCodes.Ldc_I4_0 && !refreshCodeInserted)
                    {
                        //Insert call to refresh AP events
                        codes.Insert(c + 2, new CodeInstruction(OpCodes.Call, _mi_ArchipelagoManager_RefreshEvents));
                        refreshCodeInserted = true;
                        continue;
                    }
                    if (codes[c].opcode == OpCodes.Endfinally)
                    {
                        //Skip the code that resets keys when entering a new act
                        codes[c + 4] = new CodeInstruction(OpCodes.Br, codes[c + 7].operand);
                        break;
                    }
                }

                return codes;
            }
        }

        [HarmonyPatch(typeof(GameBase), "Update")]
        internal static class Update
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
                MethodInfo _mi_ArchipelagoManager_GameUpdate = typeof(ArchipelagoManager).GetMethod(nameof(ArchipelagoManager.GameUpdate), BindingFlags.Public | BindingFlags.Static);

                for (int c = 0; c < codes.Count; c++)
                {
                    if (codes[c].opcode == OpCodes.Starg_S)
                    {
                        List<Label> labels = codes[c + 1].labels;
                        codes[c + 1].labels = new List<Label>();
                        codes.Insert(c + 1, new CodeInstruction(OpCodes.Ldarg_1));
                        codes.Insert(c + 2, new CodeInstruction(OpCodes.Call, _mi_ArchipelagoManager_GameUpdate));
                        codes[c + 1].labels = labels;
                        break;
                    }
                }

                return codes;
            }
        }
    }
}
