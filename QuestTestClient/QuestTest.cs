using System;
using System.Collections.Generic;
using CitizenFX.Core;
using HPNS.Tasks;
using HPNS.Tasks.Core;
using HPNS.Tasks.Support;
using static CitizenFX.Core.Native.API;

namespace QuestTestClient
{
    public class QuestTest : BaseScript, ITaskDelegate
    {
        public QuestTest()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
            
            RegisterCommand("quest1", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                var model = "toros";
                var vehicle = await World.CreateVehicle(model, Game.PlayerPed.Position + Game.PlayerPed.RightVector * 5.0f, Game.PlayerPed.Heading);

                var goToRadiusAreaTask = new GoToRadiusAreaTask(new Vector3(55.84977f, -1572.498f, 28.95687f), 25f);
                var stayInVehicleState = new StayInVehicleState(vehicle.Handle);
                
                var stateConjunction = new StateConjunction(goToRadiusAreaTask, stayInVehicleState);
                stateConjunction.Delegate = this;
                stateConjunction.Start();
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