using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ARPGGame;
using HammerwatchAP.Archipelago;

namespace HammerwatchAP.Controls
{
    public abstract class APControlBindingData
    {
        protected bool exploreSpeedLastFrame;
        public IPlayerControlBinding controller;

        public bool ExploreSpeed
        {
            get;
            protected set;
        }

        public bool ExploreSpeedPress
        {
            get;
            protected set;
        }

        public abstract void Update(int ms);

        public abstract void Reset();

        public abstract bool RebindAction(ControlManager.APControllerAction action);

        public abstract string ActionBoundName(ControlManager.APControllerAction action);
    }
}
