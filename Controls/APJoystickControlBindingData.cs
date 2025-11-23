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
    public class APJoystickControlBindingData : APControlBindingData
    {
        static readonly Type _t_PlayerJoystickControls = HammerwatchAP.hammerwatchAssembly.GetType("ARPGGame.PlayerJoystickControls");
		static readonly FieldInfo _fi_PlayerJoystickControls_joystick = _t_PlayerJoystickControls.GetField("joystick", BindingFlags.NonPublic | BindingFlags.Instance);

		static readonly Type _t_PlayerJoystickControlBinding = HammerwatchAP.hammerwatchAssembly.GetType("ARPGGame.PlayerJoystickControlBinding");
        static readonly FieldInfo _fi_PlayerJoystickControlBinding_controller = _t_PlayerJoystickControlBinding.GetField("controller", BindingFlags.NonPublic | BindingFlags.Instance);

        static readonly Type _t_JoystickDevice = HammerwatchAP.hammerwatchAssembly.GetType("ARPGGame.JoystickDevice");
        static readonly FieldInfo _fi_JoystickDevice_Button = _t_JoystickDevice.GetField("Button", BindingFlags.Public | BindingFlags.Instance);
        static readonly FieldInfo _fi_JoystickDevice_IsController = _t_JoystickDevice.GetField("IsController", BindingFlags.Public | BindingFlags.Instance);

        Dictionary<ControlManager.APControllerAction, int> buttonBindings = new Dictionary<ControlManager.APControllerAction, int>();

		public APJoystickControlBindingData(IPlayerControlBinding baseControlBinding)
        {
			controller = baseControlBinding;
			Reset();
        }

		public bool IsController()
		{
			object playerJoystickControls = _fi_PlayerJoystickControlBinding_controller.GetValue(controller);
			if (playerJoystickControls == null)
				return false;
			return (bool)_fi_JoystickDevice_IsController.GetValue(_fi_PlayerJoystickControls_joystick.GetValue(playerJoystickControls));
		}
		private bool IsActionActive(ControlManager.APControllerAction action)
        {
			int button = ActionToButton(action);
			if(controller.Controller != null && ((Dictionary<int, bool>)_fi_JoystickDevice_Button.GetValue(_fi_PlayerJoystickControls_joystick.GetValue(controller.Controller))).TryGetValue(button, out bool value))
            {
				return value;
            }
			return false;
        }

        public override void Update(int ms)
		{
			ExploreSpeed = IsActionActive(ControlManager.APControllerAction.ExploreSpeed);
			ExploreSpeedPress = ExploreSpeed && !exploreSpeedLastFrame;
			exploreSpeedLastFrame = ExploreSpeed;
		}

        public override void Reset()
		{
			if(IsController())
			{
				buttonBindings = new Dictionary<ControlManager.APControllerAction, int>()
				{
					{ControlManager.APControllerAction.ExploreSpeed, 1 }
				};
			}
			else
			{
				buttonBindings = new Dictionary<ControlManager.APControllerAction, int>()
				{
					{ControlManager.APControllerAction.ExploreSpeed, 1 }
				};
			}
        }

        public override bool RebindAction(ControlManager.APControllerAction action)
        {
            object joystick = _fi_PlayerJoystickControls_joystick.GetValue(controller.Controller);
            if (joystick == null)
            {
                return false;
            }
            Dictionary<int, bool> buttons = (Dictionary<int, bool>)_fi_JoystickDevice_Button.GetValue(joystick);
            foreach(KeyValuePair<int, bool> button in buttons)
            {
                if(button.Value)
                {
                    buttonBindings[action] = button.Key;
                    return true;
                }
            }
            return false;
        }

		public void SetBinding(ControlManager.APControllerAction action, int button)
        {
			buttonBindings[action] = button;
        }

        public override string ActionBoundName(ControlManager.APControllerAction action)
        {
            int button = ActionToButton(action);
			if (button == -100)
			{
				return "";
			}
			if (IsController())
			{
				switch (button)
				{
					case -2:
						return "RTrigger";
					case -1:
						return "LTrigger";
					case 0:
						return "A";
					case 1:
						return "B";
					case 2:
						return "X";
					case 3:
						return "Y";
					case 4:
						return "Back";
					case 5:
						return "Guide";
					case 6:
						return "Start";
					case 7:
						return "LStick";
					case 8:
						return "RStick";
					case 9:
						return "LBump";
					case 10:
						return "RBump";
					case 11:
						return "Up";
					case 12:
						return "Down";
					case 13:
						return "Left";
					case 14:
						return "Right";
				}
			}
			return "Button " + (button + 1);
		}

        public int ActionToButton(ControlManager.APControllerAction action)
        {
            return buttonBindings[action];
        }
    }
}
