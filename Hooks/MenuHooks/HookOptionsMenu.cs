using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Globalization;
using HarmonyLib;
using ARPGGame;
using ARPGGame.Menus;
using ARPGGame.GUI;
using TiltedEngine;
using TiltedEngine.Drawing;
using HammerwatchAP.Archipelago;
using HammerwatchAP.Util;
using HammerwatchAP.Controls;

namespace HammerwatchAP.Hooks
{
    internal class HookOptionsMenu
	{
		static FieldInfo _fi_ArchipelagoManager_inAPOptionsMenu = typeof(ArchipelagoManager).GetField(nameof(ArchipelagoManager.inArchipelagoMenu), BindingFlags.Public | BindingFlags.Static);

		public static Widget archipelagoWidget;
		static FieldInfo _fi_archipelagoWidget = typeof(HookOptionsMenu).GetField(nameof(archipelagoWidget), BindingFlags.Public | BindingFlags.Static);

		static FieldInfo _fi_OptionsMenu_mainPanel = typeof(OptionsMenu).GetField("mainPanel", BindingFlags.NonPublic | BindingFlags.Instance);
		static FieldInfo _fi_OptionsMenu_stateMain = typeof(OptionsMenu).GetField("stateMain", BindingFlags.NonPublic | BindingFlags.Instance);
		static FieldInfo _fi_OptionsMenu_stateRebind = typeof(OptionsMenu).GetField("stateRebind", BindingFlags.NonPublic | BindingFlags.Instance);
		static FieldInfo _fi_OptionsMenu_stateRebindKeyboard = typeof(OptionsMenu).GetField("stateRebindKeyboard", BindingFlags.NonPublic | BindingFlags.Instance);
		static FieldInfo _fi_OptionsMenu_stateRebindJoystick = typeof(OptionsMenu).GetField("stateRebindJoystick", BindingFlags.NonPublic | BindingFlags.Instance);

		static FieldInfo _fi_OptionsMenu_oState = typeof(OptionsMenu).GetField("oState", BindingFlags.NonPublic | BindingFlags.Instance);
		static FieldInfo _fi_OptionsMenu_readyToBind = typeof(OptionsMenu).GetField("readyToBind", BindingFlags.NonPublic | BindingFlags.Instance);
		static FieldInfo _fi_OptionsMenu_controlToRebind = typeof(OptionsMenu).GetField("controlToRebind", BindingFlags.NonPublic | BindingFlags.Instance);
		static FieldInfo _fi_OptionsMenu_noInput = typeof(OptionsMenu).GetField("noInput", BindingFlags.NonPublic | BindingFlags.Instance);
		static MethodInfo _mi_OptionsMenu_RefreshState = typeof(OptionsMenu).GetMethod("RefreshState", BindingFlags.NonPublic | BindingFlags.Instance);

		static PropertyInfo _pi_IPlayerControlBinding_IsReadyToBind = typeof(IPlayerControlBinding).GetProperty("IsReadyToBind", BindingFlags.Public | BindingFlags.Instance);
		static MethodInfo _mi_IPlayerControlBinding_RebindAction = typeof(IPlayerControlBinding).GetMethod("RebindAction", BindingFlags.Public | BindingFlags.Instance);

		static ControlManager.APControllerAction apActionToRebind;

		internal static void Hook()
		{
			HooksHelper.Hook(typeof(LoadGUI));
			HooksHelper.Hook(typeof(Update));
			HooksHelper.Hook(typeof(RefreshState));
			HooksHelper.Hook(typeof(Back));
			HooksHelper.Hook(typeof(SaveOptions));
			HooksHelper.Hook(typeof(GetFunction));
		}

		[HarmonyPatch(typeof(OptionsMenu), "LoadGUI")]
		internal static class LoadGUI
		{
			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
				MethodInfo _mi_PatchOptionsMenuDoc = typeof(LoadGUI).GetMethod(nameof(LoadGUI.PatchOptionsMenuDoc), BindingFlags.NonPublic | BindingFlags.Static);
				MethodInfo _mi_UpdateArchipelagoOptionsc = typeof(LoadGUI).GetMethod(nameof(LoadGUI.UpdateArchipelagoOptions), BindingFlags.NonPublic | BindingFlags.Static);

				HooksHelper.PatchLoadMenu(codes, _mi_PatchOptionsMenuDoc);
				List<CodeInstruction> updateAPOptionsInstructions = new List<CodeInstruction>()
				{
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Call, _mi_UpdateArchipelagoOptionsc),
				};
				codes.InsertRange(codes.Count - 3, updateAPOptionsInstructions);

				return codes;
			}

			static XDocument PatchOptionsMenuDoc(ResourceContext context)
			{
				XDocument doc = context.LoadXML("menus/gui/options.xml", new ErrorLogger());
				//Hacking in a button for our Archipelago menu
				XElement button = new XElement("textbutton");
				button.SetAttributeValue("click", "archipelago");
				button.SetAttributeValue("anchor", "50% 0%");
				button.SetAttributeValue("offset", "0 84");
				button.SetAttributeValue("text", "Archipelago");
				doc.Element("gui").Element("base").Element("sprite").Element("group").Add(button);
				var spriteNodes = doc.Element("gui").Element("base").Elements("sprite").GetEnumerator();
				spriteNodes.MoveNext();
				spriteNodes.MoveNext();
				foreach (XElement groupElement in spriteNodes.Current.Elements())
				{
					if (groupElement.Name == "group")
					{
						var rebindNodes = groupElement.Elements("keybind");
						XElement autofireKeybind = null;
						foreach (XElement element in rebindNodes)
						{
							if (element.Attribute("id").Value == "Autofire")
							{
								autofireKeybind = element;
								XElement autofireLabel = element.Element("text");
								autofireLabel.SetAttributeValue("offset", "-22 4");
								autofireLabel.SetAttributeValue("text", "Autofire");
								break;
							}
						}
						XElement exploreSpeedKeybind = new XElement(autofireKeybind);
						exploreSpeedKeybind.SetAttributeValue("id", "ExploreSpeed");
						string offset = autofireKeybind.Attribute("offset").Value;
						offset = offset.Replace("100", "5");
						exploreSpeedKeybind.SetAttributeValue("offset", offset);
						exploreSpeedKeybind.SetAttributeValue("click", "bind ExploreSpeed");
						XElement exploreSpeedLabel = exploreSpeedKeybind.Element("text");
						exploreSpeedLabel.SetAttributeValue("offset", "-22 -1");
						exploreSpeedLabel.SetAttributeValue("width", "30");
						exploreSpeedLabel.SetAttributeValue("text", "Explore Speed");
						groupElement.Add(exploreSpeedKeybind);
					}
				}
				//Archipelago menu
				XElement optionGroupRoot = doc.Element("gui").Element("base").Element("sprite");
				XElement archipelagoGroup = new XElement("group");
				archipelagoGroup.SetAttributeValue("id", "archipelago");
				archipelagoGroup.SetAttributeValue("anchor", "50% 50%");
				archipelagoGroup.SetAttributeValue("offset", "0 0");
				archipelagoGroup.SetAttributeValue("width", "150");
				archipelagoGroup.SetAttributeValue("height", "100");
				archipelagoGroup.SetAttributeValue("visible", "false");
				XElement apTitle = new XElement("text");
				apTitle.SetAttributeValue("anchor", "50% 0%");
				apTitle.SetAttributeValue("offset", "0 -30");
				apTitle.SetAttributeValue("font", "menus/px-20.xml");
				apTitle.SetAttributeValue("text", "Archipelago");
				apTitle.SetAttributeValue("color", "255 201 54");
				archipelagoGroup.Add(apTitle);
				int baseYPos = 0;
				int ySpacing = 15;
				Tuple<string, string>[] apOptionNames = new Tuple<string, string>[]
				{
					new Tuple<string, string>( "ap-deathlink", "Deathlink" ),
					new Tuple<string, string>( "ap-explore-speed", "Explore Speed" ),
					new Tuple<string, string>( "ap-explore-speed-ping", "Explore Speed Ping" ),
					new Tuple<string, string>( "ap-fragile-breakables", "Fragile Breakables" ),
					new Tuple<string, string>( "ap-chat-mirroring", "AP Chat Mirroring" ),
					new Tuple<string, string>( "ap-shop-item-hinting", "Shop Item Hinting" ),
					new Tuple<string, string>( "ap-traplink", "Trap Link" ),
				};
				for(int i = 0; i < apOptionNames.Length; i++)
                {
					XElement optionElement = new XElement("textcheck");
					optionElement.SetAttributeValue("id", apOptionNames[i].Item1);
					optionElement.SetAttributeValue("check", apOptionNames[i].Item1);
					optionElement.SetAttributeValue("offset", $"30 {baseYPos + ySpacing * i}");
					optionElement.SetAttributeValue("text", apOptionNames[i].Item2);
					archipelagoGroup.Add(optionElement);
				}
				optionGroupRoot.Add(archipelagoGroup);

				return doc;
			}

			static void UpdateArchipelagoOptions(OptionsMenu optionsMenu)
			{
				archipelagoWidget = optionsMenu.Document.GetWidget("archipelago");
				((CheckboxWidget)optionsMenu.Document.GetWidget("ap-deathlink").Children[0]).Checked = ArchipelagoManager.Deathlink;
				((CheckboxWidget)optionsMenu.Document.GetWidget("ap-explore-speed").Children[0]).Checked = ArchipelagoManager.ExploreSpeed;
				((CheckboxWidget)optionsMenu.Document.GetWidget("ap-explore-speed-ping").Children[0]).Checked = ArchipelagoManager.ExploreSpeedPing;
				((CheckboxWidget)optionsMenu.Document.GetWidget("ap-fragile-breakables").Children[0]).Checked = ArchipelagoManager.FragileBreakables;
				((CheckboxWidget)optionsMenu.Document.GetWidget("ap-chat-mirroring").Children[0]).Checked = ArchipelagoManager.APChatMirroring;
				((CheckboxWidget)optionsMenu.Document.GetWidget("ap-shop-item-hinting").Children[0]).Checked = ArchipelagoManager.ShopItemHinting;
				((CheckboxWidget)optionsMenu.Document.GetWidget("ap-traplink").Children[0]).Checked = ArchipelagoManager.TrapLink;
			}
		}

		[HarmonyPatch(typeof(OptionsMenu), "RefreshState")]
		internal static class RefreshState
		{
			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
			{
				List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
				MethodInfo _mi_HandleArchipelagoMenus = typeof(RefreshState).GetMethod(nameof(RefreshState.HandleArchipelagoMenus), BindingFlags.NonPublic | BindingFlags.Static);

				Label switchLabel = iLGenerator.DefineLabel();
				Label bindVisibleLabel = iLGenerator.DefineLabel();

				for (int c = 0; c < codes.Count; c++)
				{
					if (codes[c].opcode == OpCodes.Stloc_S)
					{
						codes[c - 2].labels.Add(switchLabel);
						List<CodeInstruction> apMenuInstructions = new List<CodeInstruction>()
						{
							new CodeInstruction(OpCodes.Ldsfld, _fi_ArchipelagoManager_inAPOptionsMenu),
							new CodeInstruction(OpCodes.Brfalse, switchLabel),
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Call, _mi_HandleArchipelagoMenus),
							new CodeInstruction(OpCodes.Brtrue, bindVisibleLabel),
							new CodeInstruction(OpCodes.Ret),
						};
						codes.InsertRange(c - 2, apMenuInstructions);
						break;
					}
				}

				codes[codes.Count - 12].labels.Add(bindVisibleLabel);

				return codes;
			}

			static void Prefix(OptionsMenu __instance)
            {
				HooksHelper.SetWidgetVisible(archipelagoWidget, false);

				string optionsState = _fi_OptionsMenu_oState.GetValue(__instance).ToString();
				switch(optionsState)
                {
					case "Rebind":
						IPlayerControlBinding controlToRebind = (IPlayerControlBinding)_fi_OptionsMenu_controlToRebind.GetValue(__instance);
						if(controlToRebind is PlayerKeyboardControlBinding)
                        {
							foreach(ControlManager.APControllerAction apControllerAction in Enum.GetValues(typeof(ControlManager.APControllerAction)))
							{
								Widget widget = ((Widget)_fi_OptionsMenu_stateRebindKeyboard.GetValue(__instance)).GetWidget(apControllerAction.ToString());
								if (widget != null)
								{
									((TextWidget)widget.GetWidget("key")).SetText(ControlManager.apControlBindings[controlToRebind].ActionBoundName(apControllerAction));
								}
							}
							break;
						}
						foreach (ControlManager.APControllerAction apControllerAction in Enum.GetValues(typeof(ControlManager.APControllerAction)))
						{
							Widget widget = ((Widget)_fi_OptionsMenu_stateRebindJoystick.GetValue(__instance)).GetWidget(apControllerAction.ToString());
							if (widget != null)
							{
								((TextWidget)widget.GetWidget("key")).SetText(ControlManager.apControlBindings[controlToRebind].ActionBoundName(apControllerAction));
							}
						}
						break;
                }
            }

			static bool HandleArchipelagoMenus(OptionsMenu optionsMenu)
            {
				switch(_fi_OptionsMenu_oState.GetValue(optionsMenu).ToString())
                {
					case "Rebind":
						HooksHelper.SetWidgetEnabled((Widget)_fi_OptionsMenu_mainPanel.GetValue(optionsMenu), false);
						HooksHelper.SetWidgetVisible((Widget)_fi_OptionsMenu_stateRebind.GetValue(optionsMenu), false);
						break;
					case "Bind":
						return true;
					default:
						HooksHelper.SetWidgetVisible(archipelagoWidget, true);
						break;
                }
				return false;
            }
		}

		[HarmonyPatch(typeof(OptionsMenu), "Back")]
		internal static class Back
        {
			static bool Prefix(OptionsMenu __instance, int ___noInput)
            {
				if(___noInput <= 0 && ArchipelagoManager.inArchipelagoMenu)
                {
					OptionsMenu.SaveOptions();
					ArchipelagoManager.inArchipelagoMenu = false;
					_mi_OptionsMenu_RefreshState.Invoke(__instance, new object[0]);
					return false;
				}
				return true;
            }
        }

		[HarmonyPatch(typeof(OptionsMenu), nameof(OptionsMenu.SaveOptions))]
		internal static class SaveOptions
		{
			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
				MethodInfo _mi_CreateArchipelagoOptionsNode = typeof(SaveOptions).GetMethod(nameof(SaveOptions.CreateArchipelagoOptionsNode), BindingFlags.NonPublic | BindingFlags.Static);

				for (int c = 0; c < codes.Count; c++)
				{
					if (codes[c].opcode == OpCodes.Ldc_I4_0)
					{
						List<CodeInstruction> saveAPOptionsInstructions = new List<CodeInstruction>()
						{
							new CodeInstruction(OpCodes.Ldloc_2),
							new CodeInstruction(OpCodes.Call, _mi_CreateArchipelagoOptionsNode),
						};
						codes.InsertRange(c, saveAPOptionsInstructions);
						break;
					}
				}

				return codes;
			}

			static void CreateArchipelagoOptionsNode(XElement optionsNode)
			{
				XElement archipelago = new XElement("Archipelago");
				archipelago.Add(new XElement("Deathlink", ArchipelagoManager.Deathlink));
				archipelago.Add(new XElement("ExploreSpeed", ArchipelagoManager.ExploreSpeed));
				archipelago.Add(new XElement("ExploreSpeedPing", ArchipelagoManager.ExploreSpeedPing));
				archipelago.Add(new XElement("FragileBreakables", ArchipelagoManager.FragileBreakables));
				archipelago.Add(new XElement("APChatMirroring", ArchipelagoManager.APChatMirroring));
				archipelago.Add(new XElement("ShopItemHinting", ArchipelagoManager.ShopItemHinting));
				archipelago.Add(new XElement("TrapLink", ArchipelagoManager.TrapLink));
				archipelago.Add(new XElement("APDebugMode", ArchipelagoManager.DEBUG_MODE));
				archipelago.Add(new XElement("LastConnectedIP", ArchipelagoManager.LastConnectedIP));
				archipelago.Add(new XElement("LastConnectedSlotName", ArchipelagoManager.LastConnectedSlotName));
				optionsNode.Add(archipelago);
				XElement apControls = new XElement("ArchipelagoControls");
				IPlayerControlBinding[] controlBindings = HookARPGGame.GetARPGGameControlBindings();
				for (int p = 0; p < 4; p++)
                {
					XElement control;
					if(controlBindings[p] is PlayerKeyboardControlBinding keyboardBinding)
                    {
						APKeyboardControlBindingData apKeyboardBinding = (APKeyboardControlBindingData)ControlManager.apControlBindings[keyboardBinding];
						control = new XElement("Keyboard");
						foreach(ControlManager.APControllerAction action in Enum.GetValues(typeof(ControlManager.APControllerAction)))
                        {
							control.Add(new XElement(action.ToString(), apKeyboardBinding.ActionBoundName(action)));
						}
					}
					else if (controlBindings[p] is PlayerJoystickControlBinding joystickBinding)
					{
						APJoystickControlBindingData apJoystickBinding = (APJoystickControlBindingData)ControlManager.apControlBindings[joystickBinding];
						control = new XElement("Joystick");
						foreach (ControlManager.APControllerAction action in Enum.GetValues(typeof(ControlManager.APControllerAction)))
						{
							control.Add(new XElement(action.ToString(), apJoystickBinding.ActionToButton(action).ToString(CultureInfo.InvariantCulture)));
						}
					}
                    else
                    {
						control = new XElement("Null");
					}
					apControls.Add(new XElement($"Player{1 + p}", control));
				}
				optionsNode.Add(apControls);
			}
		}

		[HarmonyPatch(typeof(OptionsMenu), "GetFunction")]
		internal static class GetFunction
        {
			static bool Prefix(OptionsMenu __instance, string name, ref Action<Widget> __result)
            {
				switch(name)
                {
					case "archipelago":
						__result = delegate (Widget w)
						{
							ArchipelagoManager.inArchipelagoMenu = true;
							_mi_OptionsMenu_RefreshState.Invoke(__instance, new object[0]);
						};
						return false;
					case "ap-deathlink":
						__result = delegate (Widget w)
						{
							ArchipelagoManager.SetDeathlink(((CheckboxWidget)w).Checked);
						};
						return false;
					case "ap-explore-speed":
						__result = delegate (Widget w)
						{
							ArchipelagoManager.ExploreSpeed = ((CheckboxWidget)w).Checked;
						};
						return false;
					case "ap-explore-speed-ping":
						__result = delegate (Widget w)
						{
							ArchipelagoManager.ExploreSpeedPing = ((CheckboxWidget)w).Checked;
						};
						return false;
					case "ap-fragile-breakables":
						__result = delegate (Widget w)
						{
							ArchipelagoManager.FragileBreakables = ((CheckboxWidget)w).Checked;
						};
						return false;
					case "ap-chat-mirroring":
						__result = delegate (Widget w)
						{
							ArchipelagoManager.APChatMirroring = ((CheckboxWidget)w).Checked;
						};
						return false;
					case "ap-shop-item-hinting":
						__result = delegate (Widget w)
						{
							ArchipelagoManager.ShopItemHinting = ((CheckboxWidget)w).Checked;
						};
						return false;
					case "ap-traplink":
						__result = delegate (Widget w)
						{
							ArchipelagoManager.SetTrapLink(((CheckboxWidget)w).Checked);
						};
						return false;
				}
				if(name != null)
                {
					string[] splits = name.Split(' ');
					string verb = splits[0];
					if (verb != null && verb == "bind" && splits[1] == "ExploreSpeed")
                    {
                        __result = delegate (Widget w)
                        {
                            ArchipelagoManager.inArchipelagoMenu = true;
                            apActionToRebind = (ControlManager.APControllerAction)Enum.Parse(typeof(ControlManager.APControllerAction), splits[1]);
							_fi_OptionsMenu_readyToBind.SetValue(__instance, false);
							_fi_OptionsMenu_oState.SetValue(__instance, 7); //OptionState.Bind
                            _mi_OptionsMenu_RefreshState.Invoke(__instance, new object[0]);
                        };
                        return false;
                    }
                }
				return true;
            }
		}

		[HarmonyPatch(typeof(OptionsMenu), nameof(OptionsMenu.Update))]
		internal static class Update
        {
			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator)
			{
				List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
				MethodInfo _mi_APBindUpdate = typeof(Update).GetMethod(nameof(Update.APBindUpdate), BindingFlags.NonPublic | BindingFlags.Static);

				Label noInputCounterLabel = iLGenerator.DefineLabel();

				codes[codes.Count - 13].labels.Add(noInputCounterLabel);

				List<CodeInstruction> apBindUpdateInstructions = new List<CodeInstruction>()
				{
					new CodeInstruction(OpCodes.Ldarg_0),
					new CodeInstruction(OpCodes.Ldarg_2),
					new CodeInstruction(OpCodes.Call, _mi_APBindUpdate),
					new CodeInstruction(OpCodes.Brfalse, noInputCounterLabel),
				};
				codes.InsertRange(4, apBindUpdateInstructions);

				return codes;
			}

			static bool APBindUpdate(OptionsMenu __instance, bool updateControls)
			{
				string optionsState = _fi_OptionsMenu_oState.GetValue(__instance).ToString();
				if (ArchipelagoManager.inArchipelagoMenu && updateControls && optionsState == "Bind")
				{
					object controlToRebind = _fi_OptionsMenu_controlToRebind.GetValue(__instance);
					if (!(bool)_fi_OptionsMenu_readyToBind.GetValue(__instance))
					{
						_fi_OptionsMenu_readyToBind.SetValue(__instance, _pi_IPlayerControlBinding_IsReadyToBind.GetValue(controlToRebind, null));
					}
					else
					{
						IPlayerControls playerControl = null;
						for (int c = 0; c < GameBase.Instance.Controls.PlayerControls.Length; c++)
						{
							if (GameBase.Instance.Controls.PlayerControls[c].Binding == controlToRebind)
							{
								playerControl = GameBase.Instance.Controls.PlayerControls[c];
								break;
							}
						}
						if (ControlManager.apControlBindings[playerControl.Binding].RebindAction(apActionToRebind))
						{
							ArchipelagoManager.inArchipelagoMenu = false;
							HookARPGGame.ARPGGameRefreshControls();
							GameBase.Instance.menus.ReloadUI();
							__instance.Back(false);
							_fi_OptionsMenu_noInput.SetValue(__instance, 0);
						}
					}
					return false;
				}
				return true;
			}
        }
	}
}
