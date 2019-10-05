using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.InteractivityV2.Core;
using static CitizenFX.Core.Native.API;

namespace HPNS.InteractivityV2.Tasks
{
    public class CreateObjectTask : TaskBase
    {
        private uint _modelHash;
        private Vector3 _position;
        private Vector3 _rotation;

        public CreateObjectTask(string model, Vector3 position, Vector3 rotation)
        {
            _modelHash = (uint) GetHashKey(model);
            _position = position;
            _rotation = rotation;
        }
        
        protected override async Task ExecutePrepare()
        {
            if (!IsModelInCdimage(_modelHash))
                throw new Exception();
            
            await Utility.LoadObject(_modelHash);
        }

        protected override void ExecuteStart()
        {
            var objectHandle = CreateObject((int) _modelHash, _position.X, _position.Y, _position.Z, true, true, true);
            SetEntityRotation(objectHandle, _rotation.X, _rotation.Y, _rotation.Z, 1, true);
            
            NotifyTaskDidEnd();
        }

        protected override void ExecuteAbort() { }

        protected override void ExecuteReset() { }
    }
}