using System;
using System.Threading.Tasks;
using HPNS.Core;
using HPNS.CoreClient.Environment;
using HPNS.Interactivity.Core.Data;
using HPNS.Interactivity.Core.Task;

namespace HPNS.Interactivity.Tasks
{
    public class TakePickupTask : TaskBase
    {
        public IParameter<Pickup> Pickup;

        protected override Task ExecutePrepare()
        {
            return TaskHelper.CompletedTask;
        }

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