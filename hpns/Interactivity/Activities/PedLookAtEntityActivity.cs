using System.Threading.Tasks;
using HPNS.Core;
using HPNS.Interactivity.Core.Activity;
using HPNS.Interactivity.Core.Data;
using static CitizenFX.Core.Native.API;

namespace HPNS.Interactivity.Activities
{
    public class PedLookAtEntityActivity : ActivityBase
    {
        public IParameter<int> PedHandle;
        public IParameter<int> EntityHandle;

        protected override Task ExecutePrepare()
        {
            return TaskHelper.CompletedTask;
        }

        protected override void ExecuteStart()
        {
            var pedHandle = PedHandle.GetValue();
            var entityHande = EntityHandle.GetValue();
            
            TaskLookAtEntity(pedHandle, entityHande, -1, 2048, 3);
        }

        protected override void ExecuteAbort()
        {
            var pedHandle = PedHandle.GetValue();
            TaskClearLookAt(pedHandle);
        }

        protected override void ExecuteReset() { }
    }
}