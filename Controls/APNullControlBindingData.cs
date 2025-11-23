using ARPGGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HammerwatchAP.Controls
{
    public class APNullControlBindingData : APControlBindingData
    {

        public APNullControlBindingData(IPlayerControlBinding baseControlBinding)
        {
            controller = baseControlBinding;
            Reset();
        }

        public override string ActionBoundName(ControlManager.APControllerAction action)
        {
            return "null";
        }

        public override bool RebindAction(ControlManager.APControllerAction action)
        {
            return false;
        }

        public override void Reset()
        {
            
        }

        public override void Update(int ms)
        {
            
        }
    }
}
