using System;
using System.Collections.Generic;
using CitizenFX.Core;
using HPNS.Interactivity.Core;
using HPNS.Interactivity.Support;
using static CitizenFX.Core.Native.API;

namespace HPNS.Interactivity.Tasks
{
    public class AttachEntityToPedTask : TaskBase
    {
        private int _entityHandle;
        private int _pedHandle;
        private int _boneIndex;
        private Vector3 _offset;
        private Vector3 _rotation;
        private int _duration;

        private ITask _currentTask;
        
        public AttachEntityToPedTask(int entityHandle, int pedHandle, int boneId, Vector3 offset, Vector3 rotation, int duration)
        {
            _entityHandle = entityHandle;
            _pedHandle = pedHandle;
            _boneIndex = GetPedBoneIndex(pedHandle, boneId);
            _offset = offset;
            _rotation = rotation;
            _duration = duration;
        }
        
        protected override void ExecuteStarting()
        {
            var tasks = new List<ITask>();
            tasks.Add(new LambdaTask(AttachEntityToPed));
            tasks.Add(new WaitTask(_duration));
            tasks.Add(new LambdaTask(DetachEntityFromPed));

            var sequence = new SequentialSetTask(tasks);
            sequence.TaskDidEnd += CurrentTaskTaskDidEnd;
            sequence.Start();

            _currentTask = sequence;
        }

        protected override void ExecuteAborting()
        {
            _currentTask.TaskDidEnd -= CurrentTaskTaskDidEnd;
            _currentTask.Abort();
        }

        private void CurrentTaskTaskDidEnd(object sender, EventArgs e)
        {
            _currentTask.TaskDidEnd -= CurrentTaskTaskDidEnd;
            NotifyTaskDidEnd();
        }

        private void AttachEntityToPed()
        { 
            AttachEntityToEntity(_entityHandle, _pedHandle, _boneIndex,
            _offset.X, _offset.Y, _offset.Z, _rotation.X, _rotation.Y, _rotation.Z,
            true, false, false, false, 0, true);
        }

        private void DetachEntityFromPed()
        {
            DetachEntity(_entityHandle, true, true);
        }
    }
}