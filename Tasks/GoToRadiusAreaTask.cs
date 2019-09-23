using System;
using CitizenFX.Core;
using HPNS.Core.Tools;
using HPNS.Tasks.Core;
using static CitizenFX.Core.Native.API;
using Checkpoint = HPNS.Core.Tools.Checkpoint;

namespace HPNS.Tasks
{
    public class GoToRadiusAreaTask : ITask
    {
        private Vector3 _center;
        private float _radius;

        private Checkpoint _checkpoint;
        private int _blipHandle;
        
        public ITaskDelegate Delegate { get; set; }

        public GoToRadiusAreaTask(Vector3 center, float radius)
        {
            _center = center;
            _radius = radius;
        }
        
        public void Start()
        {
            Delegate?.TaskDidStart(this);
            AddCheckpointAndBlip();
        }

        public void Abort()
        {
            RemoveCheckpointAndBlip();
            Delegate?.TaskDidAbort(this);
        }

        public void Suspend()
        {
            RemoveCheckpointAndBlip();
        }

        public void Resume()
        {
            AddCheckpointAndBlip();
        }

        private void RadiusAreaOnPlayerEntered(object sender, EventArgs e)
        {
            RemoveCheckpointAndBlip();
            Delegate?.TaskDidEnd(this);
        }

        private void AddCheckpointAndBlip()
        {
            _blipHandle = AddBlipForRadius(_center.X, _center.Y, _center.Z, _radius);
            SetBlipColour(_blipHandle, (int) BlipColor.Yellow);
            SetBlipRoute(_blipHandle, true);

            _checkpoint = CheckpointManager.Default.AddCheckpoint(_center, _radius);
            _checkpoint.PlayerEntered += RadiusAreaOnPlayerEntered;
        }

        private void RemoveCheckpointAndBlip()
        {
            RemoveBlip(ref _blipHandle);
            
            CheckpointManager.Default.RemoveCheckpoint(_checkpoint);
            _checkpoint.PlayerEntered -= RadiusAreaOnPlayerEntered;
        }
    }
}