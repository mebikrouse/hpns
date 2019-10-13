using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.Interactivity.Core.Activity;
using HPNS.Interactivity.Core.Data;
using static CitizenFX.Core.Native.API;

namespace HPNS.Interactivity.Activities
{
    public class MarkAreaActivity : ActivityBase
    {
        public IParameter<Vector3> Center;
        public IParameter<float> Radius;
        public IParameter<BlipColor> Color;

        private int _blipHandle;
        
        public MarkAreaActivity() : base(nameof(MarkAreaActivity)) { }

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
            var center = Center.GetValue();
            var radius = Radius.GetValue();
            var color = Color.GetValue();

            _blipHandle = AddBlipForRadius(center.X, center.Y, center.Z, radius);
            SetBlipAlpha(_blipHandle, 64);
            SetBlipColour(_blipHandle, (int) color);
        }

        private void RemoveBlip()
        {
            CitizenFX.Core.Native.API.RemoveBlip(ref _blipHandle);
        }
    }
}