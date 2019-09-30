using System;
using CitizenFX.Core;
using HPNS.Interactivity.Core;
using HPNS.Interactivity.States;
using HPNS.Interactivity.Support;
using HPNS.Interactivity.Tasks;

namespace QuestTestClient.Tests
{
    public class BeingInVehicleStateTest : TaskBase
    {
        private const float VEHICLE_DISTANCE = 5.0f;
        private const float AREA_RADIUS = 25f;
        private const string VEHICLE_MODEL = "toros";
        
        private static readonly Vector3 AREA_CENTER = new Vector3(55.84977f, -1572.498f, 28.95687f);
        
        private ITask _currentTask;
        
        protected override async void ExecuteStarting()
        {
            var vehicle = await World.CreateVehicle(VEHICLE_MODEL, Game.PlayerPed.Position + Game.PlayerPed.RightVector * VEHICLE_DISTANCE, Game.PlayerPed.Heading);

            ITask GoToRadiusAreaTaskProvider() => new GoToRadiusAreaTask(AREA_CENTER, AREA_RADIUS);
            var beingInVehicleState = new BeingInVehicleState(vehicle.Handle);
                
            var stateSuspendTask = new StateSuspendTask(GoToRadiusAreaTaskProvider, beingInVehicleState);
            stateSuspendTask.TaskDidEnd += CurrentTaskTaskDidEnd;
            stateSuspendTask.Start();

            _currentTask = stateSuspendTask;
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
    }
}