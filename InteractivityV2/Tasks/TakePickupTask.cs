using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.InteractivityV2.Core.Data;
using HPNS.InteractivityV2.Core.Task;
using Pickup = HPNS.Core.Environment.Pickup;

namespace HPNS.InteractivityV2.Tasks
{
    public class TakePickupTask : TaskBase
    {
        public IParameter<Pickup> Pickup;
        
        protected override async Task ExecutePrepare() { }

        protected override void ExecuteStart()
        {
            if (Pickup.GetValue().IsPickedUp)
            {
                NotifyTaskDidEnd();
                return;
            }

            Pickup.GetValue().PlayerPickedUp += OnPlayerPickedUp;
        }

        protected override void ExecuteAbort()
        {
            Pickup.GetValue().PlayerPickedUp -= OnPlayerPickedUp;
        }

        protected override void ExecuteReset() { }

        private void OnPlayerPickedUp(object sender, EventArgs e)
        {
            Pickup.GetValue().PlayerPickedUp -= OnPlayerPickedUp;
            NotifyTaskDidEnd();
        }
    }
}