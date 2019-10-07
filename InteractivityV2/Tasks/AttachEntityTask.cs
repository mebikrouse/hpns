using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.InteractivityV2.Core.Data;
using HPNS.InteractivityV2.Core.Task;
using HPNS.InteractivityV2.Support;
using static CitizenFX.Core.Native.API;

namespace HPNS.InteractivityV2.Tasks
{
    public class AttachEntityTask : TaskBase
    {
        public IParameter<int> PedHandle;
        public IParameter<int> EntityHandle;
        public IParameter<int> BoneId;
        public IParameter<Vector3> Offset = new Parameter<Vector3>(Vector3.Zero);
        public IParameter<Vector3> Rotation = new Parameter<Vector3>(Vector3.Zero);
        public IParameter<int> Duration;

        private ITask _attachSequence;

        protected override async Task ExecutePrepare()
        {
            var tasks = new List<ITask>();
            tasks.Add(new LambdaTask(AttachEntity));
            tasks.Add(new WaitTask {Duration = Duration});
            tasks.Add(new LambdaTask(DetachEntity));
            tasks.Add(new LambdaTask(NotifyTaskDidEnd));

            _attachSequence = new SequenceTask(tasks);

            await _attachSequence.Prepare();
        }

        protected override void ExecuteStart()
        {
            _attachSequence.Start();
        }

        protected override void ExecuteAbort()
        {
            _attachSequence.Abort();
            DetachEntity();
        }

        protected override void ExecuteReset()
        {
            _attachSequence.Reset();
        }

        private void AttachEntity()
        {
            var entityHandle = EntityHandle.GetValue();
            var pedHandle = PedHandle.GetValue();
            var boneIndex = GetPedBoneIndex(pedHandle, BoneId.GetValue());
            var offset = Offset.GetValue();
            var rotation = Rotation.GetValue();
            
            AttachEntityToEntity(entityHandle, pedHandle, boneIndex,
                offset.X, offset.Y, offset.Z, rotation.X, rotation.Y, rotation.Z,
                true, false, false, false, 0, true);
        }

        private void DetachEntity()
        {
            var entityHandle = EntityHandle.GetValue();
            CitizenFX.Core.Native.API.DetachEntity(entityHandle, true, true);
        }
    }
}