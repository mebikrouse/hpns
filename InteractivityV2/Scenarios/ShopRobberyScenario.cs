using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.InteractivityV2.Core;
using HPNS.InteractivityV2.Support;
using HPNS.InteractivityV2.Tasks;

using static CitizenFX.Core.Native.API;

namespace HPNS.InteractivityV2.Scenarios
{
    public class ShopRobberyScenario : TaskBase
    {
        public IParameter<int> PedHandle;
        public IResult<int> BagHandle;

        private ITask _scenarioTask;
        
        protected override async Task ExecutePrepare()
        {
            var bagHandle = new ResultCapturer<int>(BagHandle);
            
            var bagAttachmentTasks = new List<ITask>();
            bagAttachmentTasks.Add(new WaitTask
            {
                Duration = new Parameter<int>(11250)
            });
            bagAttachmentTasks.Add(new CreateObjectTask("prop_poly_bag_01")
            {
                ObjectHandle = bagHandle
            });
            bagAttachmentTasks.Add(new AttachEntityTask
            {
                PedHandle = PedHandle,
                EntityHandle = bagHandle,
                BoneId = new Parameter<int>(4138),
                Offset = new Parameter<Vector3>(new Vector3(-0.1f, -0.04f, -0.13f)),
                Duration = new Parameter<int>(9750)
            });

            var parallelTasks = new List<ITask>();
            parallelTasks.Add(new SequenceTask(bagAttachmentTasks));
            parallelTasks.Add(new PlayAnimTask("mp_am_hold_up", "holdup_victim_20s")
            {
                PedHandle = PedHandle,
                Duration = new Parameter<int>(21750)
            });
            parallelTasks.Add(new PlayFacialAnimTask("facials@gen_male@base", "shocked_1")
            {
                PedHandle = PedHandle,
                Duration = new Parameter<int>(21750)
            });

            var scenarioTasks = new List<ITask>();
            scenarioTasks.Add(new LambdaTask(() => { SetBlockingOfNonTemporaryEvents(PedHandle.GetValue(), true); }));
            scenarioTasks.Add(new ParallelTask(parallelTasks));
            scenarioTasks.Add(new LambdaTask(() =>
            {
                SetBlockingOfNonTemporaryEvents(PedHandle.GetValue(), false);
                BagHandle?.SetValue(bagHandle.GetValue());
                NotifyTaskDidEnd();
            }));

            _scenarioTask = new SequenceTask(scenarioTasks);

            await _scenarioTask.Prepare();
        }

        protected override void ExecuteStart()
        {
            _scenarioTask.Start();
        }

        protected override void ExecuteAbort()
        {
            SetBlockingOfNonTemporaryEvents(PedHandle.GetValue(), false);
            _scenarioTask.Abort();
        }

        protected override void ExecuteReset()
        {
            _scenarioTask.Reset();
        }
    }
}