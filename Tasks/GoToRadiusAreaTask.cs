using System;
using CitizenFX.Core;
using HPNS.Tasks.Core;

using Checkpoint = HPNS.Core.Environment.Checkpoint;
using World = HPNS.Core.World;

using static CitizenFX.Core.Native.API;

namespace HPNS.Tasks
{
    public class GoToRadiusAreaTask : ITask
    {
        private Vector3 _center;
        private float _radius;

        private Checkpoint _checkpoint;
        private int _blipHandle;
        
        public ITaskDelegate Delegate { get; set; }
        
        public TaskState CurrentState { get; private set; }

        public GoToRadiusAreaTask(Vector3 center, float radius)
        {
            _center = center;
            _radius = radius;
        }
        
        public void Start()
        {
            if (CurrentState != TaskState.Waiting) return;
            
            AddCheckpointAndBlip();

            CurrentState = TaskState.Running;
            Delegate?.TaskDidStart(this);
        }

        public void Abort()
        {
            if (CurrentState != TaskState.Running) return;
            
            RemoveCheckpointAndBlip();

            CurrentState = TaskState.Aborted;
            Delegate?.TaskDidAbort(this);
        }

        public void Suspend()
        {
            if (CurrentState != TaskState.Running) return;
            
            RemoveCheckpointAndBlip();

            CurrentState = TaskState.Suspended;
            Delegate?.TaskDidSuspend(this);
        }

        public void Resume()
        {
            if (CurrentState != TaskState.Suspended) return;
            
            AddCheckpointAndBlip();

            CurrentState = TaskState.Running;
            Delegate?.TaskDidResume(this);
        }

        private void AddCheckpointAndBlip()
        {
            _checkpoint = World.Current.CheckpointManager.AddCheckpoint(_center, _radius);
            _checkpoint.PlayerEntered += CheckpointOnPlayerEntered;

            _blipHandle = AddBlipForRadius(_center.X, _center.Y, _center.Z, _radius);
            SetBlipColour(_blipHandle, (int) BlipColor.Yellow);
            SetBlipAlpha(_blipHandle, 128);
            SetBlipRoute(_blipHandle, true);
        }

        private void RemoveCheckpointAndBlip()
        {
            _checkpoint.PlayerEntered -= CheckpointOnPlayerEntered;
            World.Current.CheckpointManager.RemoveCheckpoint(_checkpoint);
            
            RemoveBlip(ref _blipHandle);
        }

        private void CheckpointOnPlayerEntered(object sender, EventArgs e)
        {
            RemoveCheckpointAndBlip();

            CurrentState = TaskState.Ended;
            Delegate?.TaskDidEnd(this);
        }
    }
}