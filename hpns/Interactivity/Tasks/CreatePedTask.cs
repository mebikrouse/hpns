using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.CoreClient;
using HPNS.Interactivity.Core.Data;
using HPNS.Interactivity.Core.Task;
using static CitizenFX.Core.Native.API;

namespace HPNS.Interactivity.Tasks
{
    public class CreatePedTask : TaskBase
    {
        private uint _pedHash;

        public IParameter<Vector3> Position = new Parameter<Vector3>(Vector3.Zero);
        public IParameter<float> Heading = new Parameter<float>(0f);
        public IResult<int> PedHandle;
        
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
            var position = Position.GetValue();
            var heading = Heading.GetValue();
            
            var pedHandle = Utility.CreatePedAtPosition(position, heading, _pedHash);
            PedHandle?.SetValue(pedHandle);
            
            NotifyTaskDidEnd();
        }

        protected override void ExecuteAbort() { }

        protected override void ExecuteReset() { }
    }
}