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

        private ITask _currentTask;

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

            var bagEntityHandle = 0;

            var offset = new Vector3(-0.09999999f, -0.04f, -0.13f);
            var rotation = Vector3.Zero;

            var bagAttachmentTasks = new List<ITask>();
            bagAttachmentTasks.Add(new WaitTask(11250));
            bagAttachmentTasks.Add(new LambdaTask(() =>
            {
                bagEntityHandle = CreateObject((int) propModelHash, 0f, 0f, 0f, true, true, true);
            }));
            bagAttachmentTasks.Add(new DeferredCreationTask(() =>
                new AttachEntityToPedTask(bagEntityHandle, _pedHandle, 4138, offset, rotation, 9750)));

            var wholeScenarioTasks = new List<ITask>();
            wholeScenarioTasks.Add(new SequentialSetTask(bagAttachmentTasks));
            wholeScenarioTasks.Add(new PlayAnimTask(_pedHandle, animDict, animName, 23000));
            
            var scenarioTask = new ParallelSetTask(wholeScenarioTasks);
            scenarioTask.TaskDidEnd += CurrentTaskTaskDidEnd;
            scenarioTask.Start();

            _currentTask = scenarioTask;
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

        private static async Task LoadObject(uint modelHash)
        {
            RequestModel(modelHash);

            while (!HasModelLoaded(modelHash))
                await BaseScript.Delay(100);
        }
    }
}