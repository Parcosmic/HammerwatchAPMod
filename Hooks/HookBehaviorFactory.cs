using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using ARPGGame;
using ARPGGame.WorldItemBehaviors;
using TiltedEngine.WorldObjects.WorldItemBehaviors;
using TiltedEngine.WorldObjects.WorldObjectProducers;
using HammerwatchAP.Game;

namespace HammerwatchAP.Hooks
{
    internal class HookBehaviorFactory
	{
		internal static void Hook()
		{
			HooksHelper.Hook(typeof(ProduceItemBehavior));
		}

		[HarmonyPatch(typeof(BehaviorFactory), nameof(BehaviorFactory.ProduceItemBehavior))]
		internal static class ProduceItemBehavior
		{
			static bool Prefix(BehaviorFactory __instance, string id, BehaviorData param, ref IWorldItemBehavior __result)
            {
				//Technically we would need to set the difficulty variant before this but we don't care what it is for archipelago items
				switch(id)
                {
					case "archipelago":
						__result = new WorldItemArchipelagoBehavior(param, __instance.ResourceBank);
						return false;
					case "keymulti":
						__result = new WorldItemKeyMultiBehavior(param, __instance.ResourceBank);
						return false;
				}
				return true;
			}
        }
	}
}
