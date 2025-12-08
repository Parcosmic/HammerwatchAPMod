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
using TiltedEngine.Drawing;
using HammerwatchAP.Archipelago;
using HammerwatchAP.Util;
using HammerwatchAP.Game;

namespace HammerwatchAP.Hooks
{
    internal class HookGameSaver
    {
        internal static void Hook()
        {
            HooksHelper.Hook(typeof(SaveGame));
            HooksHelper.Hook(typeof(LoadGame));
        }

        [HarmonyPatch("GameSaver", "SaveGame")]
        internal static class SaveGame
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
                Type _type_GameSaver = HammerwatchAP.hammerwatchAssembly.GetType("ARPGGame.GameSaver");
                FieldInfo _fi_GameSaver_saveName = _type_GameSaver.GetField("saveName", BindingFlags.NonPublic | BindingFlags.Instance);
                MethodInfo _mi_ArchipelagoSave = typeof(SaveGame).GetMethod(nameof(ArchipelagoSave), BindingFlags.NonPublic | BindingFlags.Static);

                for (int c = 0; c < codes.Count; c++)
                {
                    if (codes[c].opcode == OpCodes.Ldstr && (string)codes[c].operand == "saves")
                    {
                        List<CodeInstruction> apSaveInstructions = new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Ldfld, _fi_GameSaver_saveName),
                            new CodeInstruction(OpCodes.Ldloc_0),
                            new CodeInstruction(OpCodes.Call, _mi_ArchipelagoSave),
                        };
                        codes.InsertRange(c, apSaveInstructions);
                        break;
                    }
                }

                return codes;
            }

            static void ArchipelagoSave(string saveName, SObject save)
            {
                if(ArchipelagoManager.playingArchipelagoSave)
                {
                    APSaveManager.SaveGame(saveName + ".hws", save);
                }
            }
        }

        [HarmonyPatch("GameSaver", "LoadGame")]
        internal static class LoadGame
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
                MethodInfo _mi_ValidateAPLoad = typeof(LoadGame).GetMethod(nameof(ValidateAPLoad), BindingFlags.NonPublic | BindingFlags.Static);
                MethodInfo _mi_ArchipelagoLoad = typeof(LoadGame).GetMethod(nameof(ArchipelagoLoad), BindingFlags.NonPublic | BindingFlags.Static);

                for (int c = 0; c < codes.Count; c++)
                {
                    if (codes[c].opcode == OpCodes.Ret)
                    {
                        List<CodeInstruction> apValidateInstructions = new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldloc_0),
                            new CodeInstruction(OpCodes.Ldarg_1),
                            new CodeInstruction(OpCodes.Call, _mi_ValidateAPLoad),
                            new CodeInstruction(OpCodes.Brfalse, codes[c].labels[0]),
                        };
                        codes.InsertRange(c + 5, apValidateInstructions);
                        break;
                    }
                }
                for (int c = 0; c < codes.Count; c++)
                {
                    if (codes[c].opcode == OpCodes.Ldstr && (string)codes[c].operand == "levels")
                    {
                        List<CodeInstruction> apLoadInstructions = new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldloc_0),
                            new CodeInstruction(OpCodes.Call, _mi_ArchipelagoLoad),
                        };
                        codes.InsertRange(c - 11, apLoadInstructions);
                        break;
                    }
                }

                return codes;
            }

            static bool ValidateAPLoad(SObject save, string saveName)
            {
                if (save.Get("ap").Type != SValueType.Null && save.Get("ap").GetBoolean())
                {
                    if (ArchipelagoManager.ConnectedToAP())
                    {
                        ArchipelagoManager.autoloadSave = false;
                        string exitMessage = null;
                        string loadedSlotName = save.Get("ap-slot-name").GetString();
                        if (ArchipelagoManager.connectionInfo.slotName != null && ArchipelagoManager.connectionInfo.slotName != loadedSlotName)
                            exitMessage = "Slot name in save does not match server";
                        string loadedSeed = save.Get("ap-seed").GetString();
                        if (ArchipelagoManager.archipelagoData.seed != null && ArchipelagoManager.archipelagoData.seed != loadedSeed)
                            exitMessage = "Seed in save does not match server";
                        if (exitMessage != null)
                        {
                            GameBase.Instance.SetMenu(MenuType.MESSAGE, new object[] { "Cannot Load Save", exitMessage });
                            return false;
                        }
                    }
                    else
                    {
                        if (!ArchipelagoManager.connectionInfo.ConnectionActive)
                        {
                            Logging.Log("Attempted to load Archipelago save without connecting, halting loading to connect first");
                            ArchipelagoManager.autoloadSave = true;
                            ArchipelagoManager.saveFileName = saveName;
                            Logging.Debug("Autoload save file " + ArchipelagoManager.saveFileName);
                            string loadedIp = save.Get("ap-ip").GetString();
                            string loadedSlotName = save.Get("ap-slot-name").GetString();
                            string password = save.Get("ap-password").GetString();
                            ArchipelagoManager.StartConnection(loadedIp, loadedSlotName, password);
                        }
                        return false;
                    }
                }
                return true;
            }

            static void ArchipelagoLoad(SObject save)
            {
                if (save.Get("ap").Type != SValueType.Null && save.Get("ap").GetBoolean())
                {
                    if (!APSaveManager.LoadSave(save))
                    {
                        Logging.Debug("Failed to load archipelago save!!!!");
                    }
                }
            }
        }
    }
}
