using System;
using System.Collections.Generic;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.Interactivity.Core;
using HPNS.Interactivity.States;
using HPNS.Interactivity.Support;
using HPNS.Interactivity.Tasks;

namespace QuestTestClient.Tests
{
    public class AimingAtEntityStateTest : TaskBase
    {
        private ITask _currentTask;
        
        protected override async void ExecuteStarting()
        {
            var pedPosition = Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 5f;
            var pedHandle = await Utility.CreateRandomPedAsync(pedPosition, Game.PlayerPed.Heading - 180f);
                
            var aimingAtEntityState = new AimingAtEntityState(pedHandle);
            var stateWaitTask = new StateRecoverWaitTask(aimingAtEntityState);
            
            var goToRadiusAreaTask = new GoToRadiusAreaTask(new Vector3(55.84977f, -1572.498f, 28.95687f), 25f);
            
            var sequentialSetTask = new SequentialSetTask(new List<ITask>() {stateWaitTask, goToRadiusAreaTask});
            sequentialSetTask.TaskDidEnd += CurrentTaskTaskDidEnd;
            sequentialSetTask.Start();

            _currentTask = sequentialSetTask;
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