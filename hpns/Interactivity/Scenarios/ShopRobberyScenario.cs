using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Interactivity.Core.Data;
using HPNS.Interactivity.Core.Task;
using HPNS.Interactivity.Support;
using HPNS.Interactivity.Tasks;

namespace HPNS.Interactivity.Scenarios
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

            var scenarioTasks = new List<ITask>();
            scenarioTasks.Add(new SequenceTask(bagAttachmentTasks));
            scenarioTasks.Add(new PlayAnimTask("mp_am_hold_up", "holdup_victim_20s")
            {
                PedHandle = PedHandle,
                Duration = new Parameter<int>(21750)
            });
            scenarioTasks.Add(new PlayFacialAnimTask("facials@gen_male@base", "shocked_1")
            {
                PedHandle = PedHandle,
                Duration = new Parameter<int>(21750)
            });

            _scenarioTask = new ParallelAllTask(scenarioTasks);

            await _scenarioTask.Prepare();
        }

        protected override void ExecuteStart()
        {
            _scenarioTask.DidEnd += ScenarioTaskOnDidEnd;
            _scenarioTask.Start();
        }

        protected override void ExecuteAbort()
        {
            _scenarioTask.DidEnd -= ScenarioTaskOnDidEnd;
            _scenarioTask.Abort();
        }

        protected override void ExecuteReset()
        {
            _scenarioTask.Reset();
        }

        private void ScenarioTaskOnDidEnd(object sender, EventArgs e)
        {
            _scenarioTask.DidEnd -= ScenarioTaskOnDidEnd;
            NotifyTaskDidEnd();
        }
    }
}