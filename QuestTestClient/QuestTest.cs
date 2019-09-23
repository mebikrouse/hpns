using System;
using System.Collections.Generic;
using CitizenFX.Core;
using HPNS.Core.Tools;
using HPNS.Tasks;
using HPNS.Tasks.Core;
using static CitizenFX.Core.Native.API;
using Checkpoint = HPNS.Core.Tools.Checkpoint;

namespace QuestTestClient
{
    public class QuestTest : BaseScript, ITaskDelegate
    {
        private Checkpoint _checkpoint;
        private int _blipHandle;
        
        public QuestTest()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
            
            RegisterCommand("quest1", new Action<int, List<object>, string>((source, args, raw) =>
            {
                var goToTask = new GoToRadiusAreaTask(new Vector3(1494.855f, 3758.768f, 33.90023f), 50f);
                goToTask.Delegate = this;
                goToTask.Start();
            }), false);
            
            RegisterCommand("checkpoint", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (_checkpoint != null)
                {
                    CheckpointManager.Default.RemoveCheckpoint(_checkpoint);
                    RemoveBlip(ref _blipHandle);

                    _checkpoint = null;

                    return;
                }
                
                var position = Game.PlayerPed.Position;
                var radius = 50f;

                _checkpoint = CheckpointManager.Default.AddCheckpoint(position, radius);

                _checkpoint.PlayerEntered += (sender, e) => Debug.WriteLine("Player Entered");
                _checkpoint.PlayerLeft += (sender, e) => Debug.WriteLine("Player left");

                _blipHandle = AddBlipForRadius(position.X, position.Y, position.Z, radius);
                SetBlipAlpha(_blipHandle, 128);

            }), false);
        }

        public void TaskDidStart(ITask task)
        {
            Debug.WriteLine("Task did start");
        }

        public void TaskDidEnd(ITask task)
        {
            Debug.WriteLine("Task did end");
        }

        public void TaskDidAbort(ITask task)
        {
            Debug.WriteLine("Task did abort");
        }

        public void TaskDidSuspend(ITask task)
        {
            Debug.WriteLine("Task did suspend");
        }

        public void TaskDidResume(ITask task)
        {
            Debug.WriteLine("Task did resume");
        }
    }
}