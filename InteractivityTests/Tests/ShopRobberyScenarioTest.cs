using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.InteractivityV2.Core;
using HPNS.InteractivityV2.Scenarios;
using HPNS.InteractivityV2.Support;
using HPNS.InteractivityV2.Tasks;
using InteractivityTests.Core;

namespace InteractivityTests.Tests
{
    public class ShopRobberyScenarioTest : TaskBase, ITest
    {
        public string Name => nameof(ShopRobberyScenarioTest);

        private ITask _testSequence;
        
        protected override async Task ExecutePrepare()
        {
            var pedPosition = new Parameter<Vector3>();
            var pedHeading = new Parameter<float>();
            
            var pedHandle = new ResultCapturer<int>();
            
            var tasks = new List<ITask>();
            tasks.Add(new LambdaTask(() =>
            {
                pedPosition.SetValue(Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 3f);
                pedHeading.SetValue(Game.PlayerPed.Heading - 180f);
            }));
            tasks.Add(new CreatePedTask(Utility.GetRandomPedHash())
            {
                Position = pedPosition,
                Heading = pedHeading,
                PedHandle = pedHandle
            });
            tasks.Add(new ShopRobberyScenario
            {
                PedHandle = pedHandle
            });
            tasks.Add(new LambdaTask(NotifyTaskDidEnd));

            _testSequence = new SequenceTask(tasks);

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