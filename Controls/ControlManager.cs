using ARPGGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HammerwatchAP.Controls
{
    public static class ControlManager
    {
        public static Dictionary<IPlayerControlBinding, APControlBindingData> apControlBindings = new Dictionary<IPlayerControlBinding, APControlBindingData>(4);

        public static void SetAPControlBinding(IPlayerControlBinding playerControlBinding, APControlBindingData apControlBindingData)
        {
            apControlBindings[playerControlBinding] = apControlBindingData;
        }

        public static void Update(int ms)
        {
            foreach(var control in apControlBindings)
            {
                if (control.Value == null) return;
                control.Value.Update(ms);
            }
        }
        public enum APControllerAction
        {
            ExploreSpeed,
        }
    }
}
