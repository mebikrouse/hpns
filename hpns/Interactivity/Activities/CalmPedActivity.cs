using System.Threading.Tasks;
using HPNS.Core;
using HPNS.Interactivity.Core.Activity;
using HPNS.Interactivity.Core.Data;
using static CitizenFX.Core.Native.API;

namespace HPNS.Interactivity.Activities
{
    public class CalmPedActivity : ActivityBase
    {
        public IParameter<int> PedHandle;
        
        public CalmPedActivity() : base(nameof(CalmPedActivity)) { }

        protected override Task ExecutePrepare()
        {
            return TaskHelper.CompletedTask;
        }

        protected override void ExecuteStart()
        {
            SetBlockingOfNonTemporaryEvents(PedHandle.GetValue(), true);
        }

        protected override void ExecuteAbort()
        {
            SetBlockingOfNonTemporaryEvents(PedHandle.GetValue(), false);
        }

        protected override void ExecuteReset() { }
    }
}