using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HammerwatchAP.Util;

namespace HammerwatchAP.Hooks
{
    public static class MenuHooksMain
    {
        public static void ApplyHooks()
        {
            Logging.Debug("Applying MainMenu hooks");
            HookMainMenu.Hook();
            Logging.Debug("Applying OptionsMenu hooks");
            HookOptionsMenu.Hook();
            Logging.Debug("Applying MenuList hooks");
            HookMenuList.Hook();
            Logging.Debug("Applying LobbyMenu hooks");
            HookLobbyMenu.Hook();
            Logging.Debug("Applying GameHUD hooks");
            HookGameHUD.Hook();
            Logging.Debug("Applying ShopMenu hooks");
            HookShopMenu.Hook();
            Logging.Debug("Applying ChatWidget hooks");
            HookChatWidget.Hook();
        }
    }
}
