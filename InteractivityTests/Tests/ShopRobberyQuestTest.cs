using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Interactivity.Activities;
using HPNS.Interactivity.Conditions;
using HPNS.Interactivity.Core.Data;
using HPNS.Interactivity.Core.Task;
using HPNS.Interactivity.Scenarios;
using HPNS.Interactivity.Support;
using HPNS.Interactivity.Tasks;
using HPNS.UI;
using InteractivityTests.Core;

using static CitizenFX.Core.Native.API;

using Pickup = HPNS.CoreClient.Environment.Pickup;

namespace InteractivityTests.Tests
{
    public class ShopRobberyQuestTest : TaskBase, ITest
    {
        private ITask _testSequence;
        
        public string TestName => nameof(ShopRobberyQuestTest);

        public ShopRobberyQuestTest() : base(nameof(ShopRobberyQuestTest)) { }
        
        protected override async Task ExecutePrepare()
        {
            var pedHandle = new ResultCapturer<int>();
            var bagHandle = new ResultCapturer<int>();
            var pickup = new ResultCapturer<Pickup>();

            var pedDependedTasks = new List<ITask>();
            pedDependedTasks.Add(new LambdaTask(() => { Hints.ShowNextHint("Отправляйтесь в магазин."); }));
            pedDependedTasks.Add(new ParallelActivityTask(
                new EnterAreaTask
                {
                    Center = new Parameter<Vector3>(new Vector3(-51.2039f, -1754.137f, 29.42102f)),
                    Radius = new Parameter<float>(10.0f)
                },
                new MarkDestinationActivity
                {
                    Destination = new Parameter<Vector3>(new Vector3(-51.2039f, -1754.137f, 29.42102f)),
                })
            );
            pedDependedTasks.Add(new LambdaTask(() => { Hints.ShowNextHint("Пригрозите продавцу, прицелившись в него."); }));
            pedDependedTasks.Add(new ParallelActivityTask(
                new ConditionWaitRecoverTask(new AimingAtEntityCondition {EntityHandle = pedHandle}),
                new MarkRegularEntityActivity
                {
                    EntityHandle = pedHandle,
                    Color = new Parameter<BlipColor>(BlipColor.Red)
                }
            ));
            pedDependedTasks.Add(new LambdaTask(() => { Hints.ShowNextHint("Подождите, пока продавец не соберет деньги из кассы."); }));
            pedDependedTasks.Add(new ShopRobberyScenario
            {
                PedHandle = pedHandle,
                BagHandle = bagHandle
            });

            var aftermathTasks = new List<ITask>();
            aftermathTasks.Add(new LambdaTask(() =>
            {
                TaskSmartFleePed(pedHandle.GetValue(), Game.PlayerPed.Handle, 100f, -1, true, true);
            }));
            aftermathTasks.Add(new CreatePickupTask
            {
                EntityHandle = bagHandle,
                Pickup = pickup
            });
            aftermathTasks.Add(new LambdaTask(() => { Hints.ShowNextHint("Возьмите пакет с деньгами."); }));
            aftermathTasks.Add(new ParallelActivityTask(
                new TakePickupTask {Pickup = pickup},
                new MarkSmallEntityActivity
                {
                    EntityHandle = bagHandle,
                    Color = new Parameter<BlipColor>(BlipColor.Green)
                }
            ));

            var testTasks = new List<ITask>();
            testTasks.Add(new CreatePedTask("a_m_m_ktown_01")
            {
                Position = new Parameter<Vector3>(new Vector3(-46.10585f, -1757.487f, 29.421f)),
                Heading = new Parameter<float>(52.37571f),
                PedHandle = pedHandle
            });
            testTasks.Add(new LambdaTask(() =>
            {
                Debug.WriteLine($"Ped handle is {pedHandle.GetValue()}.");
            }));
            testTasks.Add(new ParallelAnyTask(
                new ParallelActivityTask(new SequenceTask(pedDependedTasks), new CalmPedActivity {PedHandle = pedHandle}),
                new ConditionWaitBreakTask(new EntityAliveCondition
                {
                    EntityHandle = pedHandle
                })
            ));
            testTasks.Add(new ConditionalTask(new SequenceTask(aftermathTasks), () => !IsEntityDead(pedHandle.GetValue())));
            testTasks.Add(new LambdaTask(() => { Hints.ShowNextHint("Покиньте окрестности магазина."); }));
            testTasks.Add(new ParallelActivityTask(
                new LeaveAreaTask
                {
                    Center = new Parameter<Vector3>(new Vector3(-55.11107f, -1759.414f, 28.98155f)),
                    Radius = new Parameter<float>(100f)
                },
                new MarkAreaActivity
                {
                    Center = new Parameter<Vector3>(new Vector3(-55.11107f, -1759.414f, 28.98155f)),
                    Radius = new Parameter<float>(100f),
                    Color = new Parameter<BlipColor>(BlipColor.Red)
                }
            ));
            testTasks.Add(new LambdaTask(Hints.HideCurrentHint));
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
            Hints.HideCurrentHint();
            _testSequence.Abort();
        }

        protected override void ExecuteReset()
        {
            _testSequence.Reset();
        }
    }
}