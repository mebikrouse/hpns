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
        
        public Property<Vector3> Position = new Property<Vector3>(Vector3.Zero);
        public Property<Vector3> Rotation = new Property<Vector3>(Vector3.Zero);

        public Property<int> ObjectHandle;

        public CreateObjectTask(string model)
        {
            _modelHash = (uint) GetHashKey(model);
        }
        
        protected override async Task ExecutePrepare()
        {
            if (!IsModelInCdimage(_modelHash))
                throw new Exception();
            
            await Utility.LoadObject(_modelHash);
        }

        protected override void ExecuteStart()
        {
            var position = Position.Value;
            var rotation = Rotation.Value;
            
            var objectHandle = CreateObject((int) _modelHash, position.X, position.Y, position.Z, true, true, true);
            SetEntityRotation(objectHandle, rotation.X, rotation.Y, rotation.Z, 1, true);

            ObjectHandle.Value = objectHandle;
            
            NotifyTaskDidEnd();
        }

        protected override void ExecuteAbort() { }

        protected override void ExecuteReset() { }
    }
}