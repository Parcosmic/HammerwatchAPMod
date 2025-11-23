using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARPGGame;
using HammerwatchAP.Archipelago;
using HammerwatchAP.Util;

namespace HammerwatchAP.Controls
{
    public class APKeyboardControlBindingData : APControlBindingData
    {
        static readonly Type _t_KeyboardState = HammerwatchAP.hammerwatchAssembly.GetType("ARPGGame.KeyboardState");
        static readonly FieldInfo _fi_KeyboardState_KeyNames = _t_KeyboardState.GetField("KeyNames", BindingFlags.Public | BindingFlags.Static);

        static readonly Type _t_PlayerKeyboardControls = HammerwatchAP.hammerwatchAssembly.GetType("ARPGGame.PlayerKeyboardControls");
        static readonly FieldInfo _fi_PlayerKeyboardControls_keyboard = _t_PlayerKeyboardControls.GetField("keyboard", BindingFlags.NonPublic | BindingFlags.Instance);

        static readonly FieldInfo _fi_KeyboardState_Button = _t_KeyboardState.GetField("Button", BindingFlags.Public | BindingFlags.Instance);
        static readonly FieldInfo _fi_KeyboardState_IsController = _t_KeyboardState.GetField("IsController", BindingFlags.Public | BindingFlags.Instance);

        static readonly PropertyInfo _pi_KeyboardState = _t_KeyboardState.GetProperty("Item", BindingFlags.Public | BindingFlags.Instance);

        Dictionary<ControlManager.APControllerAction, Keys> keyBindings = new Dictionary<ControlManager.APControllerAction, Keys>();

        public APKeyboardControlBindingData(IPlayerControlBinding baseControlBinding)
        {
            controller = baseControlBinding;
            Reset();
        }

        public bool GetKeyPress(Keys key)
        {
            if (controller == null || controller.Controller == null) return false;
            object keyboard = _fi_PlayerKeyboardControls_keyboard.GetValue(controller.Controller);
            if (keyboard == null)
                return false;
            return (bool)_pi_KeyboardState.GetValue(keyboard, new object[] { key });
        }

        public override void Update(int ms)
        {
            ExploreSpeed = GetKeyPress(keyBindings[ControlManager.APControllerAction.ExploreSpeed]);
            ExploreSpeedPress = ExploreSpeed && !exploreSpeedLastFrame;
            exploreSpeedLastFrame = ExploreSpeed;
        }

        public override void Reset()
        {
            keyBindings = new Dictionary<ControlManager.APControllerAction, Keys>()
            {
                { ControlManager.APControllerAction.ExploreSpeed, Keys.Space },
            };
        }

        public override bool RebindAction(ControlManager.APControllerAction action)
        {
            object keyboard = _fi_PlayerKeyboardControls_keyboard.GetValue(controller.Controller);
            if (keyboard == null)
            {
                return false;
            }
            foreach(Keys key in Enum.GetValues(typeof(Keys)))
            {
                if(key != Keys.Escape && (bool)_pi_KeyboardState.GetValue(keyboard, new object[] { key }))
                {
                    keyBindings[action] = key;
                    return true;
                }
            }
            return false;
        }

        public void SetBinding(ControlManager.APControllerAction action, Keys key)
        {
            keyBindings[action] = key;
        }

        public override string ActionBoundName(ControlManager.APControllerAction action)
        {
            Keys key = keyBindings[action];
            if(((Dictionary<Keys, string>)_fi_KeyboardState_KeyNames.GetValue(null)).TryGetValue(key, out string name))
            {
                return name;
            }
            return key.ToString();
        }
    }
}
