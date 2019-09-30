using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Interactivity.Core;
using HPNS.Interactivity.Scenarios;
using HPNS.Interactivity.States;
using HPNS.Interactivity.Support;
using HPNS.Interactivity.Tasks;

using static CitizenFX.Core.Native.API;

namespace QuestTestClient.Tests
{
    public class ShopRobberyScenarioTest : TaskBase
    {
        private ITask _currentTask;
        
        protected override async void ExecuteStarting()
        {
            var pedPosition = Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 3;
            var pedHeading = Game.PlayerPed.Heading - 180f;
                
            var pedHandle = await CreatePedAtPosition(pedPosition, pedHeading, (uint) GetHashKey("a_m_m_ktown_01"));
            SetBlockingOfNonTemporaryEvents(pedHandle, true);
            PlaceObjectOnGroundProperly(pedHandle);

            var tasks = new List<ITask>();
            tasks.Add(new StateWaitTask(new AimingAtEntityState(pedHandle)));
            tasks.Add(new ShopRobberyScenario(pedHandle));
            tasks.Add(new LambdaTask(() =>
            {
                SetBlockingOfNonTemporaryEvents(pedHandle, false);
                TaskSmartFleePed(pedHandle, Game.PlayerPed.Handle, 50f, -1, true, true);
            }));

            var sequentialSetTask = new SequentialSetTask(tasks);
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

        private static async Task<int> CreatePedAtPosition(Vector3 position, float heading, uint pedHash)
        {
            while (!HasModelLoaded(pedHash))
            {
                RequestModel(pedHash);
                await BaseScript.Delay(100);
            }
            
            var ped = CreatePed(0, pedHash, position.X, position.Y, position.Z, heading, true, false);
            return ped;
        }
    }
}