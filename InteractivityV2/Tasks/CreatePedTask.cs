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
        private Vector3 _position;
        private float _heading;
        
        public CreatePedTask(uint pedHash, Vector3 position, float heading)
        {
            _pedHash = pedHash;
            _position = position;
            _heading = heading;
        }
        
        public CreatePedTask(string pedModel, Vector3 position, float heading) : this((uint) GetHashKey(pedModel), position, heading) { }
        
        protected override async Task ExecutePrepare()
        {
            if (!IsModelInCdimage(_pedHash))
                throw new Exception();
            
            await Utility.LoadObject(_pedHash);
        }

        protected override void ExecuteStart()
        {
            Utility.CreatePedAtPosition(_position, _heading, _pedHash);
            NotifyTaskDidEnd();
        }

        protected override void ExecuteAbort() { }

        protected override void ExecuteReset() { }
    }
}