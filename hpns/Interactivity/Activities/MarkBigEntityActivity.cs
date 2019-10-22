using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.Interactivity.Core.Activity;
using HPNS.Interactivity.Core.Data;
using static CitizenFX.Core.Native.API;

namespace HPNS.Interactivity.Activities
{
    public class MarkBigEntityActivity : ActivityBase
    {
        public IParameter<int> EntityHandle;
        public IParameter<BlipColor> Color;

        private int _blipHandle;

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
            var entityHandle = EntityHandle.GetValue();
            var color = Color.GetValue();
            
            _blipHandle = AddBlipForEntity(entityHandle);
            SetBlipSprite(_blipHandle, 143);
            SetBlipColour(_blipHandle, (int) color);
            SetBlipScale(_blipHandle, 0.75f);
        }

        private void RemoveBlip()
        {
            CitizenFX.Core.Native.API.RemoveBlip(ref _blipHandle);
        }
    }
}