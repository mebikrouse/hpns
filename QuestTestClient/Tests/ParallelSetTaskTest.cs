using System;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.Interactivity.Core;
using HPNS.Interactivity.States;
using HPNS.Interactivity.Support;

namespace QuestTestClient.Tests
{
    public class ParallelSetTaskTest : TaskBase
    {
        private const float PED_DISTANCE = 3f;
        
        private ITask _currentTask;
        
        protected override async void ExecuteStarting()
        {
            var pedsHeading = Game.PlayerPed.Heading - 180f;

            var playerPosition = Game.PlayerPed.Position;
            var forwardVector = Game.PlayerPed.ForwardVector;
            var lineDirection = Game.PlayerPed.RightVector;
            
            var pedAPosition = playerPosition + (-lineDirection + forwardVector) * PED_DISTANCE;
            var pedATask = Utility.CreateRandomPedAsync(pedAPosition, pedsHeading);

            var pedBPosition = playerPosition + forwardVector * PED_DISTANCE;
            var pedBTask = Utility.CreateRandomPedAsync(pedBPosition, pedsHeading);

            var pedCPosition = playerPosition + (lineDirection + forwardVector) * PED_DISTANCE;
            var pedCTask = Utility.CreateRandomPedAsync(pedCPosition, pedsHeading);

            var pedAHandle = await pedATask;
            var pedBHandle = await pedBTask;
            var pedCHandle = await pedCTask;
            
            var aimingAState = new AimingAtEntityState(pedAHandle);
            var stateWaitATask = new StateRecoverWaitTask(aimingAState);
            
            var aimingBState = new AimingAtEntityState(pedBHandle);
            var stateWaitBTask = new StateRecoverWaitTask(aimingBState);
            
            var aimingCState = new AimingAtEntityState(pedCHandle);
            var stateWaitCTask = new StateRecoverWaitTask(aimingCState);
            
            var parallelSetTask = new ParallelSetTask(new [] {stateWaitATask, stateWaitBTask, stateWaitCTask});
            parallelSetTask.TaskDidEnd += CurrentTaskOnTaskDidEnd;
            parallelSetTask.Start();

            _currentTask = parallelSetTask;
        }

        protected override void ExecuteAborting()
        {
            _currentTask.TaskDidEnd -= CurrentTaskOnTaskDidEnd;
            _currentTask.Abort();
        }

        private void CurrentTaskOnTaskDidEnd(object sender, EventArgs e)
        {
            _currentTask.TaskDidEnd -= CurrentTaskOnTaskDidEnd;
            NotifyTaskDidEnd();
        }
    }
}