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

using World = HPNS.Core.World;
using Pickup = HPNS.Core.Environment.Pickup;

namespace QuestTestClient.Tests
{
    public class ShopRobberyQuestTest : TaskBase
    {
        private ITask _currentTask;
        
        protected override async void ExecuteStarting()
        {
            var pedPosition = new Vector3(-46.10585f, -1757.487f, 29.421f);
            var pedHeading = 52.37571f;
            
            var pedHandle = await CreatePedAtPosition(pedPosition, pedHeading, (uint) GetHashKey("a_m_m_ktown_01"));
            SetBlockingOfNonTemporaryEvents(pedHandle, true);
            PlaceObjectOnGroundProperly(pedHandle);

            ShopRobberyScenario shopRobberyScenario;
            Pickup bagPickup = null;

            var tasks = new List<ITask>();
            tasks.Add(new GoToRadiusAreaTask(new Vector3(-55.11107f, -1759.414f, 28.98155f), 10.0f));
            tasks.Add(new GoToRadiusAreaTask(new Vector3(-51.2039f, -1754.137f, 29.42102f), 5.0f));
            tasks.Add(new StateRecoverWaitTask(new AimingAtEntityState(pedHandle)));
            tasks.Add(shopRobberyScenario = new ShopRobberyScenario(pedHandle));
            tasks.Add(new LambdaTask(() =>
            {
                SetBlockingOfNonTemporaryEvents(pedHandle, false);
                TaskSmartFleePed(pedHandle, Game.PlayerPed.Handle, 50f, -1, true, true);
            }));
            tasks.Add(new LambdaTask(() =>
            {
                bagPickup = World.Current.ObjectManager.AddObject(new Pickup(shopRobberyScenario.BagEntityHandle, 0.5f));
            }));
            tasks.Add(new DeferredCreationTask(() => new TakePickupTask(bagPickup)));
            tasks.Add(new LeaveRadiusAreaTask(new Vector3(-55.11107f, -1759.414f, 28.98155f), 50f));

            var sequentialSetTask = new SequentialSetTask(tasks);
            sequentialSetTask.TaskDidEnd += CurrentTaskOnTaskDidEnd;
            sequentialSetTask.Start();

            _currentTask = sequentialSetTask;
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