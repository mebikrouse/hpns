using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.InteractivityV2.Core;

using static CitizenFX.Core.Native.API;

namespace HPNS.InteractivityV2.Tasks
{
    public class CreatePedTask : TaskBase
    {
        private uint _pedHash;
        
        public Property<Vector3> Position = new Property<Vector3>(Vector3.Zero);
        public Property<float> Heading = new Property<float>(0f);

        public Property<int> PedHandle = new Property<int>();
        
        public CreatePedTask(uint pedHash)
        {
            _pedHash = pedHash;
        }
        
        public CreatePedTask(string pedModel) : this((uint) GetHashKey(pedModel)) { }
        
        protected override async Task ExecutePrepare()
        {
            if (!IsModelInCdimage(_pedHash))
                throw new Exception();
            
            await Utility.LoadObject(_pedHash);
        }

        protected override void ExecuteStart()
        {
            var position = Position.Value;
            var heading = Heading.Value;
            
            var pedHandle = Utility.CreatePedAtPosition(position, heading, _pedHash);
            PedHandle.Value = pedHandle;
            
            NotifyTaskDidEnd();
        }

        protected override void ExecuteAbort() { }

        protected override void ExecuteReset() { }
    }
}