using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.InteractivityV2.Core.Data;
using HPNS.InteractivityV2.Core.Task;
using static CitizenFX.Core.Native.API;

namespace HPNS.InteractivityV2.Tasks
{
    public class CreateObjectTask : TaskBase
    {
        private uint _modelHash;

        public IParameter<Vector3> Position = new Parameter<Vector3>(Vector3.Zero);
        public IParameter<Vector3> Rotation = new Parameter<Vector3>(Vector3.Zero);
        public IResult<int> ObjectHandle;

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
            var position = Position.GetValue();
            var rotation = Rotation.GetValue();
            
            var objectHandle = CreateObject((int) _modelHash, position.X, position.Y, position.Z, true, true, true);
            SetEntityRotation(objectHandle, rotation.X, rotation.Y, rotation.Z, 1, true);

            ObjectHandle?.SetValue(objectHandle);
            
            NotifyTaskDidEnd();
        }

        protected override void ExecuteAbort() { }

        protected override void ExecuteReset() { }
    }
}