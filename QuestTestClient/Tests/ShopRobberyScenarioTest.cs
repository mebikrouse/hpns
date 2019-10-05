using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.Interactivity.Core;
using HPNS.Interactivity.Scenarios;
using HPNS.Interactivity.States;
using HPNS.Interactivity.Support;
using HPNS.Interactivity.Tasks;

using static CitizenFX.Core.Native.API;

using Pickup = HPNS.Core.Environment.Pickup;
using World = HPNS.Core.World;

namespace QuestTestClient.Tests
{
    public class ShopRobberyScenarioTest : TaskBase
    {
        private ITask _currentTask;
        
        protected override async void ExecuteStarting()
        {
            var pedPosition = Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 3f;
            var pedHeading = Game.PlayerPed.Heading - 180f;
                
            var pedHandle = await Utility.CreatePedAtPositionAsync(pedPosition, pedHeading, (uint) GetHashKey("a_m_m_ktown_01"));
            SetBlockingOfNonTemporaryEvents(pedHandle, true);
            PlaceObjectOnGroundProperly(pedHandle);

            var tasks = new List<ITask>();
            
            tasks.Add(new StateRecoverWaitTask(new AimingAtEntityState(pedHandle)));
            var shopRobberyScenario = new ShopRobberyScenario(pedHandle);
            tasks.Add(shopRobberyScenario);
            tasks.Add(new LambdaTask(() =>
            {
                SetBlockingOfNonTemporaryEvents(pedHandle, false);
                TaskSmartFleePed(pedHandle, Game.PlayerPed.Handle, 50f, -1, true, true);
            }));
            Pickup pickup = null;
            tasks.Add(new LambdaTask(() => { pickup = World.Current.ObjectManager.AddObject(new Pickup(shopRobberyScenario.BagEntityHandle, 0.5f)); }));
            tasks.Add(new DeferredCreationTask(() => new TakePickupTask(pickup)));

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
    }
}