using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HammerwatchAP.Util;

namespace HammerwatchAP.Hooks
{
    public static class HooksMain
    {
        public static void ApplyHooks()
        {
            Logging.Log("Applying APMod hooks");
            HookARPGGame.Hook();
            Logging.Debug("Applying Menu hooks");
            MenuHooksMain.ApplyHooks();
            Logging.Debug("Applying GameBase hooks");
            HookGameBase.Hook();
            Logging.Debug("Applying GameSaver hooks");
            HookGameSaver.Hook();
            Logging.Debug("Applying BaseEnemyBehavior hooks");
            HookBaseEnemyBehavior.Hook();
            Logging.Debug("Applying PlayerHandler hooks");
            HookPlayerHandler.Hook();
            Logging.Debug("Applying SharedPlayerInfo hooks");
            HookSharedPlayerInfo.Hook();
            Logging.Debug("Applying PlayerInfo hooks");
            HookPlayerInfo.Hook();
            Logging.Debug("Applying PlayerActorBehavior hooks");
            HookPlayerActorBehavior.Hook();
            Logging.Debug("Applying BehaviorFactory hooks");
            HookBehaviorFactory.Hook();
            Logging.Debug("Applying WorldItemPickupBehavior hooks");
            HookWorldItemPickupBehavior.Hook();
            Logging.Debug("Applying WorldItemBreakableBehavior hooks");
            HookWorldItemBreakableBehavior.Hook();
            Logging.Debug("Finished applying all hooks!");
        }
    }
}
