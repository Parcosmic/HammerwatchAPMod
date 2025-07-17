using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Globalization;
using ARPGGame;
using HarmonyLib;
using HammerwatchAP.Archipelago;
using HammerwatchAP.Controls;
using HammerwatchAP.Util;

namespace HammerwatchAP.Hooks
{
    internal class HookARPGGame
    {
        static Type _t_ARPGGAME = HammerwatchAP.hammerwatchAssembly.GetType("ARPGGame.ARPGGame");
        public static FieldInfo _fi_ARPGGAME_Instance = _t_ARPGGAME.GetField("Instance", BindingFlags.Public | BindingFlags.Static);
        public static FieldInfo _fi_ARPGGAME_ControlBindings = _t_ARPGGAME.GetField("ControlBindings", BindingFlags.Public | BindingFlags.Instance);
        public static FieldInfo _fi_ARPGGAME_options = _t_ARPGGAME.GetField("options", BindingFlags.NonPublic | BindingFlags.Instance);
        public static MethodInfo _mi_ARPGGAME_RefreshControls = _t_ARPGGAME.GetMethod("RefreshControls", BindingFlags.Public | BindingFlags.Instance);

        public static IPlayerControlBinding[] GetARPGGameControlBindings()
        {
            return (IPlayerControlBinding[])_fi_ARPGGAME_ControlBindings.GetValue(_fi_ARPGGAME_Instance.GetValue(null));
        }
        public static void ARPGGameRefreshControls()
        {
            _mi_ARPGGAME_RefreshControls.Invoke(_fi_ARPGGAME_Instance.GetValue(null), null);
        }

        internal static void Hook()
        {
            HooksHelper.Hook(typeof(OnLoad));
            HooksHelper.Hook(typeof(RefreshControls));
        }

        [HarmonyPatch("ARPGGame.ARPGGame", "OnLoad")]
        internal static class OnLoad
        {
            static void Prefix(XElement ___options)
            {
                if (___options == null)
                    return;
                Logging.Log("Loading Archipelago config settings");
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

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
                MethodInfo _mi_LoadAPControls = typeof(OnLoad).GetMethod(nameof(LoadAPControls), BindingFlags.NonPublic | BindingFlags.Static);

                for (int c = 0; c < codes.Count; c++)
                {
                    if (codes[c].opcode == OpCodes.Newarr)
                    {
                        List<CodeInstruction> loadAPControlsInstructions = new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Ldfld, _fi_ARPGGAME_options),
                            new CodeInstruction(OpCodes.Call, _mi_LoadAPControls),
                        };
                        codes.InsertRange(c+20, loadAPControlsInstructions);
                        break;
                    }
                }

                return codes;
            }

            static void Postfix(XElement ___options)
            {
                ArchipelagoManager.LoadMod();
            }

            static void LoadAPControls(XElement options)
            {
                XElement apControls = options.Element("ArchipelagoControls");
                if (apControls != null)
                {
                    IPlayerControlBinding[] controlBindings = GetARPGGameControlBindings();
                    APControlBindingData[] apBindingDatas = new APControlBindingData[4];
                    for(int p = 0; p < apBindingDatas.Length; p++)
                    {
                        apBindingDatas[p] = LoadAPControl(apControls.Element($"Player{p+1}"), controlBindings[p]);
                    }
                    ControlManager.apControlBindings = new Dictionary<IPlayerControlBinding, APControlBindingData>();
                    for (int c = 0; c < controlBindings.Length; c++)
                    {
                        ControlManager.SetAPControlBinding(controlBindings[c], apBindingDatas[c]);
                    }
                }
            }
        }

        [HarmonyPatch("ARPGGame.ARPGGame", "RefreshControls")]
        internal static class RefreshControls
        {
            static void Postfix(object __instance)
            {
                if (GameBase.Instance == null || GameBase.Instance.Controls == null || GameBase.Instance.Controls.PlayerControls == null) return;
                IPlayerControlBinding[] controlBindings = GetARPGGameControlBindings();
                Dictionary<IPlayerControlBinding, APControlBindingData> newAPControlBindings = new Dictionary<IPlayerControlBinding, APControlBindingData>();
                for(int p = 0; p < controlBindings.Length; p++)
                {
                    if(ControlManager.apControlBindings.TryGetValue(controlBindings[p], out APControlBindingData data) && data != null)
                    {
                        newAPControlBindings[controlBindings[p]] = data;
                        data.controller = controlBindings[p];
                    }
                    else
                    {
                        if (controlBindings[p] is PlayerNullControlBinding)
                        {
                            newAPControlBindings[controlBindings[p]] = null;
                        }
                        else if (controlBindings[p] is PlayerJoystickControlBinding)
                        {
                            newAPControlBindings[controlBindings[p]] = new APJoystickControlBindingData(controlBindings[p]);
                        }
                        else if (controlBindings[p] is PlayerKeyboardControlBinding)
                        {
                            newAPControlBindings[controlBindings[p]] = new APKeyboardControlBindingData(controlBindings[p]);
                        }
                    }
                }
                ControlManager.apControlBindings = newAPControlBindings;
            }
        }

        static APControlBindingData LoadAPControl(XElement element, IPlayerControlBinding controlBinding)
        {
            XElement keyboard = element.Element("Keyboard");
            if(keyboard != null)
            {
                APKeyboardControlBindingData apKeyboardBinding = new APKeyboardControlBindingData(controlBinding);
                foreach(XElement e in keyboard.Elements())
                {
                    ControlManager.APControllerAction action = (ControlManager.APControllerAction)Enum.Parse(typeof(ControlManager.APControllerAction), e.Name.LocalName);
                    Keys key = (Keys)Enum.Parse(typeof(Keys), e.Value);
                    apKeyboardBinding.SetBinding(action, key);
                }
                return apKeyboardBinding;
            }
            XElement joystick = element.Element("Joystick");
            if (joystick != null)
            {
                APJoystickControlBindingData apJoystickBinding = new APJoystickControlBindingData(controlBinding);
                foreach (XElement e in joystick.Elements())
                {
                    ControlManager.APControllerAction action = (ControlManager.APControllerAction)Enum.Parse(typeof(ControlManager.APControllerAction), e.Name.LocalName);
                    int button = int.Parse(e.Value, CultureInfo.InvariantCulture);
                    apJoystickBinding.SetBinding(action, button);
                }
                return apJoystickBinding;
            }
            return null;
        }
    }
}
