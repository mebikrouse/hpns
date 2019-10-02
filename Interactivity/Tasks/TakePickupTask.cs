using System;
using HPNS.Core.Environment;
using HPNS.Interactivity.Core;

namespace HPNS.Interactivity.Tasks
{
    public class TakePickupTask : TaskBase
    {
        private Pickup _pickup;
        
        public TakePickupTask(Pickup pickup)
        {
            _pickup = pickup;
        }
        
        protected override void ExecuteStarting()
        {
            if (_pickup.IsPickedUp)
            {
                NotifyTaskDidEnd();
                return;
            }
            
            _pickup.PlayerPickedUp += PickupOnPlayerPickedUp;
        }

        protected override void ExecuteAborting()
        {
            _pickup.PlayerPickedUp -= PickupOnPlayerPickedUp;
        }

        private void PickupOnPlayerPickedUp(object sender, EventArgs e)
        {
            _pickup.PlayerPickedUp -= PickupOnPlayerPickedUp;
            NotifyTaskDidEnd();
        }
    }
}