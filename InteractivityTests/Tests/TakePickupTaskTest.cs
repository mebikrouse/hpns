using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.InteractivityV2.Core.Data;
using HPNS.InteractivityV2.Core.Task;
using HPNS.InteractivityV2.Support;
using HPNS.InteractivityV2.Tasks;
using InteractivityTests.Core;
using Pickup = HPNS.Core.Environment.Pickup;

namespace InteractivityTests.Tests
{
    public class TakePickupTaskTest : TaskBase, ITest
    {
        public string Name => nameof(TakePickupTaskTest);

        private ITask _testSequence;
        
        protected override async Task ExecutePrepare()
        {
            var objectPosition = new Parameter<Vector3>();
            var objectHandle = new ResultCapturer<int>();
            var pickup = new ResultCapturer<Pickup>();
            
            var tasks = new List<ITask>();
            tasks.Add(new LambdaTask(() =>
            {
                objectPosition.SetValue(Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 3f);
            }));
            tasks.Add(new CreateObjectTask("prop_poly_bag_01")
            {
                Position = objectPosition,
                ObjectHandle = objectHandle
            });
            tasks.Add(new CreatePickupTask
            {
                EntityHandle = objectHandle,
                Pickup = pickup
            });
            tasks.Add(new TakePickupTask
            {
                Pickup = pickup
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