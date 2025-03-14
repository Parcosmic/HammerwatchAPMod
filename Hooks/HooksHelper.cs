using ARPGGame.GUI;
using HammerwatchAP.Archipelago;
using HammerwatchAP.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace HammerwatchAP.Hooks
{
    public static class HooksHelper
    {
        public static MethodInfo _mi_Logging_DebugObject = typeof(Logging).GetMethod(nameof(Logging.DebugObject), BindingFlags.Public | BindingFlags.Static);

        public static void Hook(Type hookType)
        {
            Logging.Debug("- Hooking "+hookType.Name);
            Harmony.CreateAndPatchAll(hookType);
        }

        public static void PrintCodes(List<CodeInstruction> codes)
        {
            for (int i = 0; i < codes.Count; i++)
            {
                Logging.Log($"{codes[i].opcode}: {codes[i].operand}");
            }
        }

        public static CodeInstruction GetLogDebugInstruction()
        {
            return new CodeInstruction(OpCodes.Call, _mi_Logging_DebugObject);
        }

        public static readonly MethodInfo _mi_Widget_Enabled = typeof(Widget).GetProperty(nameof(Widget.Enabled), BindingFlags.Instance | BindingFlags.Public).GetSetMethod(true);
        public static readonly MethodInfo _mi_Widget_Visible = typeof(Widget).GetProperty(nameof(Widget.Visible), BindingFlags.Instance | BindingFlags.Public).GetSetMethod(true);
        public static void SetWidgetEnabled(Widget widget, bool enabled)
        {
            _mi_Widget_Enabled.Invoke(widget, new object[] { enabled });
        }
        public static void SetWidgetVisible(Widget widget, bool visible)
        {
            _mi_Widget_Visible.Invoke(widget, new object[] { visible });
        }

        public static void PatchLoadMenu(List<CodeInstruction> codes, MethodInfo patchMethodInfo)
        {
            for (int c = 0; c < codes.Count; c++)
            {
                if (codes[c].opcode == OpCodes.Callvirt)
                {
                    codes[c + 1] = new CodeInstruction(OpCodes.Nop);
                    codes[c + 2] = new CodeInstruction(OpCodes.Nop);
                    codes[c + 3] = new CodeInstruction(OpCodes.Call, patchMethodInfo);
                    break;
                }
            }
        }
    }
}
