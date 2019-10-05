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
            
            var pedHandle = await Utility.CreatePedAtPositionAsync(pedPosition, pedHeading, (uint) GetHashKey("a_m_m_ktown_01"));
            SetBlockingOfNonTemporaryEvents(pedHandle, true);
            SetPedCanRagdollFromPlayerImpact(pedHandle, false);
            PlaceObjectOnGroundProperly(pedHandle);

            ShopRobberyScenario shopRobberyScenario;
            Pickup bagPickup = null;

            var pedDependedTasks = new List<ITask>();
            pedDependedTasks.Add(new GoToRadiusAreaTask(new Vector3(-55.11107f, -1759.414f, 28.98155f), 10.0f));
            pedDependedTasks.Add(new GoToRadiusAreaTask(new Vector3(-51.2039f, -1754.137f, 29.42102f), 5.0f));
            pedDependedTasks.Add(new StateRecoverWaitTask(new AimingAtEntityState(pedHandle)));
            pedDependedTasks.Add(shopRobberyScenario = new ShopRobberyScenario(pedHandle));
            pedDependedTasks.Add(new LambdaTask(() =>
            {
                bagPickup = World.Current.ObjectManager.AddObject(new Pickup(shopRobberyScenario.BagEntityHandle, 0.5f));
                
                SetBlockingOfNonTemporaryEvents(pedHandle, false);
                SetPedCanRagdollFromPlayerImpact(pedHandle, true);
                TaskSmartFleePed(pedHandle, Game.PlayerPed.Handle, 50f, -1, true, true);
            }));

            var pedDependedRaceParallelTasks = new List<ITask>();
            pedDependedRaceParallelTasks.Add(new SequentialSetTask(pedDependedTasks));
            pedDependedRaceParallelTasks.Add(new StateBreakWaitTask(new EntityIsAliveState(pedHandle)));
            
            var questTasks = new List<ITask>();
            questTasks.Add(new RaceParallelSetTask(pedDependedRaceParallelTasks));
            questTasks.Add(new ConditionalTask(() => World.Current.EntityDeathTracker.IsEntityAlive(pedHandle),
                new DeferredCreationTask(() =>
                {
                    Debug.WriteLine("Cashier is alive. God bless you!!!");
                    return new TakePickupTask(bagPickup);
                })));
            questTasks.Add(new LeaveRadiusAreaTask(new Vector3(-55.11107f, -1759.414f, 28.98155f), 100f));

            var questSequence = new SequentialSetTask(questTasks);
            questSequence.TaskDidEnd += CurrentTaskOnTaskDidEnd;
            questSequence.Start();

            _currentTask = questSequence;
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