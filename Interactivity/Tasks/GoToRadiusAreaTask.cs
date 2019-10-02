using System;
using CitizenFX.Core;
using HPNS.Interactivity.Core;

using static CitizenFX.Core.Native.API;

using Checkpoint = HPNS.Core.Environment.Checkpoint;
using World = HPNS.Core.World;

namespace HPNS.Interactivity.Tasks
{
    public class GoToRadiusAreaTask : TaskBase
    {
        private Vector3 _center;
        private float _radius;

        private Checkpoint _checkpoint;
        private int _blipHandle;

        public GoToRadiusAreaTask(Vector3 center, float radius)
        {
            _center = center;
            _radius = radius;
        }

        protected override void ExecuteStarting()
        {
            AddCheckpointAndBlip();
        }

        protected override void ExecuteAborting()
        {
            RemoveCheckpointAndBlip();
        }

        private void AddCheckpointAndBlip()
        {
            _checkpoint = World.Current.ObjectManager.AddObject(new Checkpoint(_center, _radius));
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
            World.Current.ObjectManager.DestroyObject(_checkpoint);
            
            RemoveBlip(ref _blipHandle);
        }

        private void CheckpointOnPlayerEntered(object sender, EventArgs e)
        {
            RemoveCheckpointAndBlip();
            NotifyTaskDidEnd();
        }
    }
}