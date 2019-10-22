using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.Interactivity.Core.Data;
using HPNS.Interactivity.Core.Task;
using Checkpoint = HPNS.CoreClient.Environment.Checkpoint;
using World = HPNS.CoreClient.World;

namespace HPNS.Interactivity.Tasks
{
    public class EnterAreaTask : TaskBase
    {
        public IParameter<Vector3> Center;
        public IParameter<float> Radius;

        private Checkpoint _checkpoint;

        protected override Task ExecutePrepare()
        {
            return TaskHelper.CompletedTask;
        }

        protected override void ExecuteStart()
        {
            var center = Center.GetValue();
            var radius = Radius.GetValue();

            _checkpoint = World.Current.ObjectManager.AddObject(new Checkpoint(center, radius));
            _checkpoint.PlayerEntered += CheckpointOnPlayerEntered;

            if (_checkpoint.IsPlayerInside())
            {
                _checkpoint.PlayerEntered -= CheckpointOnPlayerEntered;
                World.Current.ObjectManager.DestroyObject(_checkpoint);
                
                NotifyTaskDidEnd();
            }
        }

        protected override void ExecuteAbort()
        {
            _checkpoint.PlayerEntered -= CheckpointOnPlayerEntered;
            World.Current.ObjectManager.DestroyObject(_checkpoint);
        }

        protected override void ExecuteReset() { }

        private void CheckpointOnPlayerEntered(object sender, EventArgs e)
        {
            _checkpoint.PlayerEntered -= CheckpointOnPlayerEntered;
            World.Current.ObjectManager.DestroyObject(_checkpoint);
            
            NotifyTaskDidEnd();
        }
    }
}