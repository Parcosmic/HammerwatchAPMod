using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using HarmonyLib;
using ARPGGame;
using ARPGGame.Menus;

using HammerwatchAP.Util;
using HammerwatchAP.Menus;

namespace HammerwatchAP.Hooks
{
    internal class HookMenuList
    {
        internal static void Hook()
        {
            HooksHelper.Hook(typeof(SetMenu));
            HooksHelper.Hook(typeof(CloseMenu));
        }

        static readonly FieldInfo _fi_MenuList_menus = typeof(MenuList).GetField("menus", BindingFlags.Instance | BindingFlags.NonPublic);
        static List<GameMenu> GetMenus(MenuList instance)
        {
            return (List<GameMenu>)_fi_MenuList_menus.GetValue(instance);
        }

        [HarmonyPatch(typeof(MenuList), nameof(MenuList.SetMenu))]
        internal static class SetMenu
        {
            static bool Prefix(MenuList __instance, MenuType menu, params object[] parameters)
            {
                GameMenu.ClearKeyboardFocus();
                //Archipelago versions of menus
                switch(menu)
                {
                    //case MenuType.LOBBY:
                    //    if (parameters.Length >= 4)
                    //    {
                    //        if (parameters[2] is GamePlayers players)
                    //        {
                    //            GetMenus(__instance).Add(new LobbyMenu(GameBase.Instance, GameBase.Instance.resources, players));
                    //            return false;
                    //        }
                    //        GetMenus(__instance).Add(new LobbyMenu(GameBase.Instance, GameBase.Instance.resources, (bool)parameters[0], (bool)parameters[1], (int)parameters[2]));
                    //        return false;
                    //    }
                    //    break;
                    case MenuType.TEXT_PROMPT:
                        if(parameters.Length >= 9)
                        {
                            GetMenus(__instance).Add(new TextPromptTripleMenu(GameBase.Instance, GameBase.Instance.resources,
                                (string)parameters[0], (string)parameters[1], (Action<bool, string, string, string>)parameters[2], (string)parameters[3], (string)parameters[4], (string)parameters[5],
                                (Func<string, bool>)parameters[6], (string)parameters[7], (bool)parameters[8]));
                            return false;
                        }
                        break;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(MenuList), nameof(MenuList.CloseMenu))]
        internal static class CloseMenu
        {
            static bool Prefix(MenuList __instance, MenuType menu)
            {
                GameMenu.ClearKeyboardFocus();
                //Archipelago versions of menus
                Type additionalType = null;
                switch (menu)
                {
                    case MenuType.TEXT_PROMPT:
                        additionalType = typeof(TextPromptTripleMenu);
                        break;
                }
                if(additionalType != null)
                {
                    GetMenus(__instance).RemoveAll((GameMenu m) => m.GetType() == additionalType);
                }
                return true;
            }
        }
    }
}
