using System.Threading.Tasks;
using HPNS.InteractivityV2.Core.Activity;
using HPNS.InteractivityV2.Core.Data;

using static CitizenFX.Core.Native.API;

namespace HPNS.InteractivityV2.Activities
{
    public class CalmPedActivity : ActivityBase
    {
        public IParameter<int> PedHandle;
        
        protected override async Task ExecutePrepare() { }

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