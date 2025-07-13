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
    public class WorldItemArchipelagoBehavior : WorldItemPickupBehavior
    {
        public bool isCheck;
        //public int itemLocation = -1;

        public WorldItemArchipelagoBehavior(BehaviorData param, ResourceBank resBank) : base(param, resBank)
        {
            SValue isCheckValue = param.Get("is-check");
            if (!isCheckValue.IsNull())
                isCheck = isCheckValue.GetBoolean();
        }

        protected override bool DoPickup(PlayerInfo plr)
        {
            return true;
        }
    }
}
