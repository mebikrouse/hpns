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
        private int _pedHandle;
        private int _entityHandle;
        private int _boneIndex;
        private Vector3 _offset;
        private Vector3 _rotation;
        private int _duration;

        private ITask _attachSequence;
        
        public AttachEntityTask(int pedHandle, int entityHandle, int boneId, Vector3 offset, Vector3 rotation, int duration)
        {
            _pedHandle = pedHandle;
            _entityHandle = entityHandle;
            _boneIndex = GetPedBoneIndex(pedHandle, boneId);
            _offset = offset;
            _rotation = rotation;
            _duration = duration;
        }

        protected override async Task ExecutePrepare()
        {
            var tasks = new List<ITask>();
            tasks.Add(new LambdaTask(AttachEntity));
            tasks.Add(new WaitTask(_duration));
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
            AttachEntityToEntity(_entityHandle, _pedHandle, _boneIndex,
                _offset.X, _offset.Y, _offset.Z, _rotation.X, _rotation.Y, _rotation.Z,
                true, false, false, false, 0, true);
        }

        private void DetachEntity()
        {
            CitizenFX.Core.Native.API.DetachEntity(_entityHandle, true, true);
        }
    }
}