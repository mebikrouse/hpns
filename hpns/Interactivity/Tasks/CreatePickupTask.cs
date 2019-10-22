using System.Threading.Tasks;
using HPNS.Core;
using HPNS.CoreClient;
using HPNS.CoreClient.Environment;
using HPNS.Interactivity.Core.Data;
using HPNS.Interactivity.Core.Task;

namespace HPNS.Interactivity.Tasks
{
    public class CreatePickupTask : TaskBase
    {
        public IParameter<int> EntityHandle;
        public IResult<Pickup> Pickup;

        protected override Task ExecutePrepare()
        {
            return TaskHelper.CompletedTask;
        }

        protected override void ExecuteStart()
        {
            Pickup?.SetValue(World.Current.ObjectManager.AddObject(new Pickup(EntityHandle.GetValue(), 0.75f)));
            NotifyTaskDidEnd();
        }

        protected override void ExecuteAbort() { }

        protected override void ExecuteReset() { }
    }
}