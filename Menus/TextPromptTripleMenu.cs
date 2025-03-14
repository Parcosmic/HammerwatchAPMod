using System;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using ARPGGame;
using ARPGGame.GUI;
using ARPGGame.Menus;
using TiltedEngine;

namespace HammerwatchAP.Menus
{
	public class TextPromptTripleMenu : GameMenu
	{

		static readonly PropertyInfo _pi_Widget_Visible = typeof(Widget).GetProperty("Visible", BindingFlags.Instance | BindingFlags.Public);

		public TextPromptTripleMenu(GameBase gameBase, ResourceBank resBank, string title, string message, Action<bool, string, string, string> onAnswer, string defaultAnswer1, string defaultAnswer2, string defaultAnswer3,
			Func<string, bool> okayAnswer, string notOkayMsg, bool cancelable) : base(gameBase)
		{
			this.title = title;
			this.message = message;
			this.defaultAnswer1 = defaultAnswer1;
			this.defaultAnswer2 = defaultAnswer2;
			this.defaultAnswer3 = defaultAnswer3;
			this.notOkayMsg = notOkayMsg;
			this.cancelable = cancelable;
			this.onAnswer = onAnswer;
			this.okayAnswer = okayAnswer;
			LoadGUI(new GUILoader(resBank, this), resBank);
		}

		public override void LoadGUI(GUILoader loader, ResourceBank resBank)
		{
			if (!loader.HasProducer(typeof(BGSpriteWidgetProducer)))
			{
				loader.AddWidgetProducer(new BGSpriteWidgetProducer());
			}
			XDocument doc = resBank.Context.LoadXML("menus/gui/textprompt.xml", new ErrorLogger());
			XElement inputGroup = doc.Root.Element("base").Element("group");
			XElement input1Sprite = inputGroup.Elements().ToArray()[5];
			input1Sprite.SetAttributeValue("offset", "0 -2");
			XElement input2Sprite = new XElement(input1Sprite);
			XElement input2 = input2Sprite.Element("input");
			input2Sprite.SetAttributeValue("offset", "0 10");
			input2.SetAttributeValue("id", "input2");
			XElement input3Sprite = new XElement(input1Sprite);
			XElement input3 = input3Sprite.Element("input");
			input3Sprite.SetAttributeValue("offset", "0 22");
			input3.SetAttributeValue("id", "input3");
			//input2.SetAttributeValue("offset", "5 1");
			inputGroup.Add(input2Sprite);
			inputGroup.Add(input3Sprite);
			Document = loader.LoadGUI(doc, game.Width / MenuScale, game.Height / MenuScale);
			((TextWidget)Document.GetWidget("title")).SetText(title);
			((TextWidget)Document.GetWidget("message")).SetText(message);
			((TextWidget)Document.GetWidget("needinput")).SetText(notOkayMsg);
			input1 = (InputWidget)Document.GetWidget("input");
			this.input2 = (InputWidget)Document.GetWidget("input2");
			this.input3 = (InputWidget)Document.GetWidget("input3");
			input1.SetText(defaultAnswer1);
			this.input2.SetText(defaultAnswer2);
			this.input3.SetText(defaultAnswer3);
			SetKeyboardFocus(input1);
			BGSpriteWidget bg = (BGSpriteWidget)Document.GetWidget("bg");
			if (bg != null)
			{
				bg.SetDimensions(game.Width + MenuScale, Document.Height + MenuScale);
			}
			Widget okWidget = Document.GetWidget("ok");
			Widget okCancelWidget = Document.GetWidget("okcancel");
			_pi_Widget_Visible.SetValue(okWidget, !cancelable, null);
			_pi_Widget_Visible.SetValue(okCancelWidget, cancelable, null);
		}

		public override void Back(bool menu)
		{
			Okay(cancelable);
		}

		public void Okay(bool cancel)
		{
			if (cancel || (okayAnswer(input1.GetText()) && okayAnswer(input2.GetText()) && okayAnswer(input3.GetText())))
			{
				onAnswer(cancel, input1.GetText(), input2.GetText(), input3.GetText());
				game.CloseMenu(MenuType.TEXT_PROMPT);
				return;
			}
			Widget needinputWidget = Document.GetWidget("needinput");
			_pi_Widget_Visible.SetValue(needinputWidget, true, null);
			SetKeyboardFocus(input1);
		}

		public override Action<Widget> GetFunction(string name)
		{
			if (name == "ok")
			{
				return delegate (Widget param0)
				{
					Okay(false);
				};
			}
			if (name == "cancel")
			{
				return delegate (Widget param0)
				{
					Okay(true);
				};
			}
			return null;
		}

		private InputWidget input1;
		private InputWidget input2;
		private InputWidget input3;
		private string title;
		private string message;
		private string defaultAnswer1;
		private string defaultAnswer2;
		private string defaultAnswer3;
		private string notOkayMsg;
		private bool cancelable;
		private Action<bool, string, string, string> onAnswer;
		private Func<string, bool> okayAnswer;
	}
}
