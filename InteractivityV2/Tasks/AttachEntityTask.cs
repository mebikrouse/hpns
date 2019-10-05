using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.InteractivityV2.Core;
using HPNS.InteractivityV2.Support;
using static CitizenFX.Core.Native.API;

namespace HPNS.InteractivityV2.Tasks
{
    public class AttachEntityTask : TaskBase
    {
        public Property<int> PedHandle;
        public Property<int> EntityHandle;
        public Property<int> BoneId;
        public Property<Vector3> Offset = new Property<Vector3>(Vector3.Zero);
        public Property<Vector3> Rotation = new Property<Vector3>(Vector3.Zero);
        public Property<int> Duration;

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
            var entityHandle = EntityHandle.Value;
            var pedHandle = PedHandle.Value;
            var boneIndex = GetPedBoneIndex(pedHandle, BoneId.Value);
            var offset = Offset.Value;
            var rotation = Rotation.Value;
            
            AttachEntityToEntity(entityHandle, pedHandle, boneIndex,
                offset.X, offset.Y, offset.Z, rotation.X, rotation.Y, rotation.Z,
                true, false, false, false, 0, true);
        }

        private void DetachEntity()
        {
            var entityHandle = EntityHandle.Value;
            CitizenFX.Core.Native.API.DetachEntity(entityHandle, true, true);
        }
    }
}