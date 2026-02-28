using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
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
    internal class HookMainMenu
    {
        internal static void Hook()
		{
			HooksHelper.Hook(typeof(LoadGUI));
			HooksHelper.Hook(typeof(GetFunction));
		}

        static readonly FieldInfo _fi_MainMenu_mainGrp = typeof(MainMenu).GetField("mainGrp", BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo _fi_MainMenu_campaignGrp = typeof(MainMenu).GetField("campaignGrp", BindingFlags.Instance | BindingFlags.NonPublic);
        static readonly FieldInfo _fi_MainMenu_multiplayerGrp = typeof(MainMenu).GetField("multiplayerGrp", BindingFlags.Instance | BindingFlags.NonPublic);

        [HarmonyPatch(typeof(MainMenu), "LoadGUI")]
        internal static class LoadGUI
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
				MethodInfo _mi_PatchMainMenuDoc = typeof(LoadGUI).GetMethod(nameof(PatchMainMenuDoc), BindingFlags.NonPublic | BindingFlags.Static);

				HooksHelper.PatchLoadMenu(codes, _mi_PatchMainMenuDoc);

				return codes;
			}

			static void Postfix(MainMenu __instance)
			{
				string modString = ArchipelagoManager.GetModVersionString();
				((TextWidget)__instance.Document.GetWidget("modVersion")).SetText(modString);

				if (ArchipelagoManager.ConnectedToAP())
				{
					SetArchipelagoButtons(__instance, true, ArchipelagoManager.saveFileName != null);
					if (ArchipelagoManager.generateInfo.gameReady)
					{
						ArchipelagoManager.ResetAPVars();
					}
				}
				else
				{
					SetArchipelagoButtons(__instance, false, false);
				}
			}

			static XDocument PatchMainMenuDoc(ResourceContext context)
			{
				XDocument doc = context.LoadXML("menus/gui/main.xml", new ErrorLogger());
				//Hacking in a button for our Archipelago menu
				XElement button = new XElement("textbutton");
				button.SetAttributeValue("click", "archipelago");
				button.SetAttributeValue("offset", "0 60");
				button.SetAttributeValue("text", "Archipelago");
				button.SetAttributeValue("enabled", "true");
				doc.Root.Element("base").Element("group").Element("group").Add(button);
				//Edit version
				XElement version = doc.Root.Element("base").Element("text");
				//Mod version
				XElement modVersion = new XElement(version);
				modVersion.SetAttributeValue("id", "modVersion");
				doc.Root.Element("base").Add(modVersion);
				//Archipelago connection
				XElement apConnection = new XElement(version);
				apConnection.SetAttributeValue("anchor", "50% 100%");
				apConnection.SetAttributeValue("id", "ap-connection");
				apConnection.SetAttributeValue("offset", "0 -30");
				string connectionMsg = "Not connected to Archipelago";
				connectionMsg = ArchipelagoManager.connectionInfo.connectedToAP ? $"Connected to Archipelago at {ArchipelagoManager.connectionInfo.ip.Replace("ws://", "")}" : "Not connected to Archipelago";
				apConnection.SetAttributeValue("text", connectionMsg);
				doc.Root.Element("base").Add(apConnection);
				XElement apConnectionSlotName = new XElement(apConnection);
				apConnectionSlotName.SetAttributeValue("id", "ap-slot-name");
				apConnectionSlotName.SetAttributeValue("offset", "0 -20");
				apConnectionSlotName.SetAttributeValue("text", ArchipelagoManager.connectionInfo.connectedToAP ? $"Slot name: {ArchipelagoManager.connectionInfo.slotName}" : "");
				doc.Root.Element("base").Add(apConnectionSlotName);
				//Bump existing version up
				version.SetAttributeValue("offset", "0 -10");
				//Add info to credits screen
				XElement apModCreditsHeader = new XElement("text");
				apModCreditsHeader.SetAttributeValue("anchor", "50% 0%");
				apModCreditsHeader.SetAttributeValue("offset", "75 135");
				apModCreditsHeader.SetAttributeValue("font", "menus/px-20.xml");
				apModCreditsHeader.SetAttributeValue("text", "ARCHIPELAGO MOD");
				apModCreditsHeader.SetAttributeValue("color", "255 201 54");
				apModCreditsHeader.SetAttributeValue("ignore-lang", "true");
				XElement apModCreditsName = new XElement("text");
				apModCreditsName.SetAttributeValue("anchor", "50% 0%");
				apModCreditsName.SetAttributeValue("offset", "75 153");
				apModCreditsName.SetAttributeValue("font", "menus/px-10.xml");
				apModCreditsName.SetAttributeValue("text", "Parcosmic");
				apModCreditsName.SetAttributeValue("ignore-lang", "true");
				XElement apModCreditsName2 = new XElement("text");
				apModCreditsName2.SetAttributeValue("anchor", "50% 0%");
				apModCreditsName2.SetAttributeValue("offset", "75 163");
				apModCreditsName2.SetAttributeValue("font", "menus/px-10.xml");
				apModCreditsName2.SetAttributeValue("text", "kl3cks7r");
				apModCreditsName2.SetAttributeValue("ignore-lang", "true");
				XElement creditsNode = null;
				foreach (XElement node in doc.Root.Element("base").Element("group").Elements("sprite"))
				{
					if (node.Attribute("id") != null && node.Attribute("id").Value == "credits")
					{
						creditsNode = node;
						break;
					}
				}
				creditsNode.Add(apModCreditsHeader);
				creditsNode.Add(apModCreditsName);
				creditsNode.Add(apModCreditsName2);

				return doc;
			}
		}

		public static void SetArchipelagoButtons(MainMenu mainMenu, bool set, bool resume, bool multi = false)
		{
			Widget mainGrp = (Widget)_fi_MainMenu_mainGrp.GetValue(mainMenu);
			Widget multiplayerGrp = (Widget)_fi_MainMenu_multiplayerGrp.GetValue(mainMenu);
			TextWidget firstTextButton = (TextWidget)mainGrp.Children[3].Children[0].Children[0];
			TextWidget thirdTextButton = (TextWidget)mainGrp.Children[5].Children[0].Children[0];
			TextWidget multiplayerLocalButton = (TextWidget)multiplayerGrp.Children[0].Children[0].Children[0];
			TextWidget multiplayerHostButton = (TextWidget)multiplayerGrp.Children[1].Children[0].Children[0];
			if (set)
			{
				firstTextButton.SetText(resume ? "Resume" : multi ? "Multi" : "Single");
				thirdTextButton.SetText("Disconnect");
				if (resume)
				{
					multiplayerLocalButton.SetText("Resume");
					multiplayerHostButton.SetText("Resume");
				}
			}
			else
			{
				firstTextButton.SetText("Single");
				thirdTextButton.SetText("Archipelago");
				multiplayerLocalButton.SetText(ArchipelagoMessageManager.GetLanguageString( "m.localg", new string[0] ));
				multiplayerHostButton.SetText(ArchipelagoMessageManager.GetLanguageString("m.hostg", new string[0] ));
			}
		}

		[HarmonyPatch(typeof(MainMenu), "GetFunction")]
		internal static class GetFunction
		{
			static bool Prefix(MainMenu __instance, string name, ref Action<Widget> __result)
            {
				switch(name)
				{
					case "local":
						if (ArchipelagoManager.ConnectedToAP())
						{
							__result = delegate (Widget widget)
							{
								if (ArchipelagoManager.saveFileName != null)
								{
									APSaveManager.LoadGame(ArchipelagoManager.saveFileName);
									return;
								}
								GameBase.Instance.SetMenu(MenuType.LOBBY, new object[]
								{
									true,
									true,
									GameBase.Instance.Controls.PlayerControls.Length
								});
							};
							return false;
						}
						return true;
					case "campaign":
						if (ArchipelagoManager.ConnectedToAP())
						{
							__result = delegate (Widget widget)
							{
								if (ArchipelagoManager.saveFileName != null)
								{
									APSaveManager.LoadGame(ArchipelagoManager.saveFileName);
									return;
								}
								GameBase.Instance.SetMenu(MenuType.LOBBY, new object[]
								{
									true,
									true,
									ArchipelagoManager.archipelagoData.neededPlayers
								});
							};
							return false;
						}
						return true;
					case "host":
						if (ArchipelagoManager.ConnectedToAP())
						{
							__result = delegate (Widget widget)
							{
								if (ArchipelagoManager.saveFileName != null)
								{
									APSaveManager.LoadGame(ArchipelagoManager.saveFileName);
									return;
								}
								GameBase.Instance.SetMenu(MenuType.LOBBY, new object[]
								{
									true,
									false,
									4
								});
							};
							return false;
						}
						return true;
					case "archipelago":
						__result = delegate (Widget widget)
						{
							if (ArchipelagoManager.ConnectedToAP())
							{
								ArchipelagoManager.DisconnectFromArchipelago("User disconnected from server");
								return;
							}
							string defaultIp = ArchipelagoManager.LastConnectedIP;
							string defaultSlotName = ArchipelagoManager.LastConnectedSlotName;
                            object[] array = new object[9];
							array[0] = "Connect to Archipelago";
							array[1] = "Enter IP address, slot name, and password";
							array[2] = new Action<bool, string, string, string>(delegate (bool cancel, string answer, string answer2, string answer3)
							{
								if (cancel) return;
								string ip = answer ?? "";
								int port = 38281;
								string[] splits = ip.Split(':');
								//Add the default port if it doesn't have it
								if (!int.TryParse(splits[splits.Length - 1], out _))
								{
									ip = $"{ip}:{port}";
								}
								string slotName = answer2 ?? "";
								string password = answer3 ?? "";
								ArchipelagoManager.StartConnection(ip, slotName, password);
							});
							array[3] = defaultIp;
							array[4] = defaultSlotName;
							array[5] = "";
							array[6] = new Func<string, bool>((string answer) => true);
							array[7] = "";
							array[8] = true;
							GameBase.Instance.SetMenu(MenuType.TEXT_PROMPT, array);
						};
						return false;
				}
				return true;
            }
        }

	}
}
