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

        public TaskState CurrentState { get; private set; } = TaskState.Waiting;
        
        public event EventHandler TaskDidEnd;

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
        }

        public void Abort()
        {
            if (CurrentState != TaskState.Running) return;
            
            RemoveCheckpointAndBlip();

            CurrentState = TaskState.Aborted;
        }

        private void AddCheckpointAndBlip()
        {
            _checkpoint = World.Current.CheckpointManager.AddCheckpoint(_center, _radius);
            _checkpoint.PlayerEntered += CheckpointOnPlayerEntered;

            _blipHandle = AddBlipForCoord(_center.X, _center.Y, _center.Z);
            SetBlipSprite(_blipHandle, 146);
            SetBlipColour(_blipHandle, 5);
            SetBlipScale(_blipHandle, 0.75f);
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
            TaskDidEnd?.Invoke(this, EventArgs.Empty);
        }
    }
}