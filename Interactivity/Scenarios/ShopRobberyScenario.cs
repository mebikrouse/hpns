using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Interactivity.Core;
using HPNS.Interactivity.Support;
using HPNS.Interactivity.Tasks;

using static CitizenFX.Core.Native.API;

namespace HPNS.Interactivity.Scenarios
{
    public class ShopRobberyScenario : TaskBase
    {
        private int _pedHandle;
        private ITask _sequenceTask;

        public ShopRobberyScenario(int pedHandle)
        {
            _pedHandle = pedHandle;
        }

        protected override async void ExecuteStarting()
        {
            var animDict = "mp_am_hold_up";
            var animName = "holdup_victim_20s";

            var propModelHash = (uint) GetHashKey("prop_poly_bag_01");
            await LoadObject(propModelHash);

            var propHandle = 0;
            var pedHandle = _pedHandle;

            var bagAttachmentTasks = new List<ITask>();
            bagAttachmentTasks.Add(new WaitTask(11250));
            bagAttachmentTasks.Add(new LambdaTask(() =>
            {
                propHandle = CreateObject((int) propModelHash, 0f, 0f, 0f, true, true, true);
                var boneIndex = GetPedBoneIndex(pedHandle, 4138);
                AttachEntityToEntity(propHandle, pedHandle, boneIndex,
                    -0.09999999f, -0.04f, -0.13f, 0, 0, 0, true,
                    false, false, false, 0, true);
            }));
            bagAttachmentTasks.Add(new WaitTask(9750));
            bagAttachmentTasks.Add(new LambdaTask(() =>
            {
                DetachEntity(propHandle, true, true);
            }));
            
            var parallelTasks = new List<ITask>();
            parallelTasks.Add(new PlayAnimTask(pedHandle, animDict, animName, 23000));
            parallelTasks.Add(new SequentialSetTask(bagAttachmentTasks));

            var parallelSetTask = new ParallelSetTask(parallelTasks);
            parallelSetTask.TaskDidEnd += SequenceTaskOnTaskDidEnd;
            parallelSetTask.Start();

            _sequenceTask = parallelSetTask;
        }

        protected override void ExecuteAborting()
        {
            _sequenceTask.Abort();
            
            _sequenceTask.TaskDidEnd -= SequenceTaskOnTaskDidEnd;
            _sequenceTask = null;
        }

        private static async Task LoadObject(uint modelHash)
        {
            RequestModel(modelHash);

            while (!HasModelLoaded(modelHash))
                await BaseScript.Delay(100);
        }

        private void SequenceTaskOnTaskDidEnd(object sender, EventArgs e)
        {
            _sequenceTask.TaskDidEnd -= SequenceTaskOnTaskDidEnd;
            _sequenceTask = null;
            
            NotifyTaskDidEnd();
        }
    }
}