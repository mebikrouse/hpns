using System.Threading.Tasks;
using HPNS.Core;
using HPNS.Core.Environment;
using HPNS.InteractivityV2.Core.Data;
using HPNS.InteractivityV2.Core.Task;

namespace HPNS.InteractivityV2.Tasks
{
    public class CreatePickupTask : TaskBase
    {
        public IParameter<int> EntityHandle;
        public IResult<Pickup> Pickup;
        
        protected override async Task ExecutePrepare() { }

        protected override void ExecuteStart()
        {
            Pickup?.SetValue(World.Current.ObjectManager.AddObject(new Pickup(EntityHandle.GetValue(), 0.75f)));
            NotifyTaskDidEnd();
        }

        protected override void ExecuteAbort() { }

        protected override void ExecuteReset() { }
    }
}