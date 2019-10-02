using System;
using CitizenFX.Core;
using HPNS.Interactivity.Core;
using Checkpoint = HPNS.Core.Environment.Checkpoint;
using World = HPNS.Core.World;

namespace HPNS.Interactivity.Tasks
{
    public class LeaveRadiusAreaTask : TaskBase
    {
        private Vector3 _center;
        private float _radius;

        private Checkpoint _checkpoint;
        
        public LeaveRadiusAreaTask(Vector3 center, float radius)
        {
            _center = center;
            _radius = radius;
        }
        
        protected override void ExecuteStarting()
        {
            AddCheckpoint();
        }

        protected override void ExecuteAborting()
        {
            RemoveCheckpoint();
        }

        private void CheckpointOnPlayerLeft(object sender, EventArgs e)
        {
            RemoveCheckpoint();
            NotifyTaskDidEnd();
        }

        private void AddCheckpoint()
        {
            _checkpoint = World.Current.ObjectManager.AddObject(new Checkpoint(_center, _radius));
            _checkpoint.PlayerLeft += CheckpointOnPlayerLeft;
        }

        private void RemoveCheckpoint()
        {
            _checkpoint.PlayerEntered -= CheckpointOnPlayerLeft;
            World.Current.ObjectManager.DestroyObject(_checkpoint);
        }
    }
}