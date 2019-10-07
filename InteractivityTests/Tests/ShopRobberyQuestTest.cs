using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.InteractivityV2.Activities;
using HPNS.InteractivityV2.Conditions;
using HPNS.InteractivityV2.Core.Data;
using HPNS.InteractivityV2.Core.Task;
using HPNS.InteractivityV2.Scenarios;
using HPNS.InteractivityV2.Support;
using HPNS.InteractivityV2.Tasks;
using InteractivityTests.Core;

using static CitizenFX.Core.Native.API;

using Pickup = HPNS.Core.Environment.Pickup;

namespace InteractivityTests.Tests
{
    public class ShopRobberyQuestTest : TaskBase, ITest
    {
        private ITask _testSequence;

        public string Name => nameof(ShopRobberyQuestTest);
        
        protected override async Task ExecutePrepare()
        {
            var pedPosition = new Parameter<Vector3>();
            var pedHeading = new Parameter<float>();
            var pedHandle = new ResultCapturer<int>();
            var bagHandle = new ResultCapturer<int>();
            var pickup = new ResultCapturer<Pickup>();
            
            var pedCreationTasks = new List<ITask>();
            pedCreationTasks.Add(new LambdaTask(() =>
            {
                pedPosition.SetValue(Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 3f);
                pedHeading.SetValue(Game.PlayerPed.Heading - 180f);
            }));
            pedCreationTasks.Add(new CreatePedTask(Utility.GetRandomPedHash())
            {
                Position = pedPosition,
                Heading = pedHeading,
                PedHandle = pedHandle
            });
            
            var cutsceneTasks = new List<ITask>();
            cutsceneTasks.Add(new ConditionWaitRecoverTask(new AimingAtEntityCondition
            {
                EntityHandle = pedHandle
            }));
            cutsceneTasks.Add(new ShopRobberyScenario
            {
                PedHandle = pedHandle,
                BagHandle = bagHandle
            });

            var aftermathTasks = new List<ITask>();
            aftermathTasks.Add(new CreatePickupTask
            {
                EntityHandle = bagHandle,
                Pickup = pickup
            });
            aftermathTasks.Add(new TakePickupTask
            {
                Pickup = pickup
            });

            var testTasks = new List<ITask>();
            testTasks.Add(new SequenceTask(pedCreationTasks));
            testTasks.Add(new ParallelActivityTask(new SequenceTask(cutsceneTasks), new CalmPedActivity {PedHandle = pedHandle}));
            testTasks.Add(new SequenceTask(aftermathTasks));
            testTasks.Add(new LambdaTask(NotifyTaskDidEnd));

            _testSequence = new SequenceTask(testTasks);
            
            await _testSequence.Prepare();
        }

        protected override void ExecuteStart()
        {
            _testSequence.Start();
        }

        protected override void ExecuteAbort()
        {
            _testSequence.Abort();
        }

        protected override void ExecuteReset()
        {
            _testSequence.Reset();
        }
    }
}