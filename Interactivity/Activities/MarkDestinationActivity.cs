using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.Interactivity.Core.Activity;
using HPNS.Interactivity.Core.Data;
using static CitizenFX.Core.Native.API;

namespace HPNS.Interactivity.Activities
{
    public class MarkDestinationActivity : ActivityBase
    {
        public IParameter<Vector3> Destination;

        private int _blipHandle;
        
        public MarkDestinationActivity() : base(nameof(MarkDestinationActivity)) { }

        protected override Task ExecutePrepare()
        {
            return TaskHelper.CompletedTask;
        }

        protected override void ExecuteStart()
        {
            AddBlip();
        }

        protected override void ExecuteAbort()
        {
            RemoveBlip();
        }

        protected override void ExecuteReset() { }

        private void AddBlip()
        {
            var dest = Destination.GetValue();
            
            _blipHandle = AddBlipForCoord(dest.X, dest.Y, dest.Z);
            SetBlipSprite(_blipHandle, 146);
            SetBlipColour(_blipHandle, 5);
            SetBlipScale(_blipHandle, 0.75f);
            SetBlipRoute(_blipHandle, true);
        }

        private void RemoveBlip()
        {
            CitizenFX.Core.Native.API.RemoveBlip(ref _blipHandle);
        }
    }
}