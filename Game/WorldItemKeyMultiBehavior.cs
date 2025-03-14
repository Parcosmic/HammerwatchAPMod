using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARPGGame;
using ARPGGame.WorldItemBehaviors;
using TiltedEngine;
using TiltedEngine.WorldObjects.WorldObjectProducers;

namespace HammerwatchAP.Game
{
    public class WorldItemKeyMultiBehavior : WorldItemPickupBehavior
    {
        private readonly int type;
        private readonly int amount;

        public WorldItemKeyMultiBehavior(BehaviorData param, ResourceBank resBank) : base(param, resBank)
        {
            SValue amountValue = param.Get("amount");
            amount = amountValue.IsNull() ? 1 : amountValue.GetInteger();
        }

        protected override bool DoPickup(PlayerInfo plr)
        {
            for (int a = 0; a < amount; a++)
                plr.AddKey(type);
            return true;
        }
    }
}
