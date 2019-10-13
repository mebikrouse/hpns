using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.CoreClient;
using HPNS.Interactivity.Core.Data;
using HPNS.Interactivity.Core.Task;
using HPNS.Interactivity.Scenarios;
using HPNS.Interactivity.Support;
using HPNS.Interactivity.Tasks;
using InteractivityTests.Core;

namespace InteractivityTests.Tests
{
    public class ShopRobberyScenarioTest : TaskBase, ITest
    {
        private ITask _testSequence;

        public string TestName => nameof(ShopRobberyScenarioTest);
        
        public ShopRobberyScenarioTest() : base(nameof(ShopRobberyScenarioTest)) { }
        
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