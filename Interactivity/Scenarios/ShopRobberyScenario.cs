using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Interactivity.Core;
using HPNS.Interactivity.Exceptions;
using HPNS.Interactivity.Support;
using HPNS.Interactivity.Tasks;

using static CitizenFX.Core.Native.API;

namespace HPNS.Interactivity.Scenarios
{
    public class ShopRobberyScenario : ITask
    {
        private int _pedHandle;
        private ITask _sequenceTask;

        public TaskState CurrentState { get; private set; } = TaskState.Waiting;
        
        public event EventHandler TaskDidEnd;

        public ShopRobberyScenario(int pedHandle)
        {
            _pedHandle = pedHandle;
        }
        
        public async void Start()
        {
            if (CurrentState != TaskState.Waiting)
                throw new StartException();
            
            var dict = "mp_am_hold_up";
            var anim = "holdup_victim_20s";
            await LoadAnimDict(dict);

            var propModelHash = (uint) GetHashKey("prop_poly_bag_01");
            await LoadObject(propModelHash);

            var tasks = new List<ITask>();
            var propHandle = 0;
            var pedHandle = _pedHandle;

            tasks.Add(new LambdaTask(() =>
                TaskPlayAnim(_pedHandle, dict, anim, 8.0f, 8.0f, -1, 0, 0.0f, false, false, false)));
            tasks.Add(new WaitTask(11250));
            tasks.Add(new LambdaTask(() =>
            {
                propHandle = CreateObject((int) propModelHash, 0f, 0f, 0f, true, true, true);

                var boneIndex = GetPedBoneIndex(pedHandle, 4138);
                AttachEntityToEntity(propHandle, pedHandle, boneIndex,
                    -0.09999999f, -0.04f, -0.13f, 0, 0, 0, true,
                    false, false, false, 0, true);
            }));
            tasks.Add(new WaitTask(9750));
            tasks.Add(new LambdaTask(() =>
            {
                DetachEntity(propHandle, true, true);
            }));
            tasks.Add(new WaitTask(2000));

            var sequenceTask = new SequenceTask(tasks);
            sequenceTask.TaskDidEnd += SequenceTaskOnTaskDidEnd;
            sequenceTask.Start();

            _sequenceTask = sequenceTask;

            CurrentState = TaskState.Running;
        }

        public void Abort()
        {
            if (CurrentState != TaskState.Running)
                throw new AbortException();

            _sequenceTask.TaskDidEnd -= SequenceTaskOnTaskDidEnd;
            _sequenceTask.Abort();
            _sequenceTask = null;

            CurrentState = TaskState.Aborted;
        }
        
        private static async Task LoadAnimDict(string dict)
        {
            RequestAnimDict(dict);

            while (!HasAnimDictLoaded(dict))
                await BaseScript.Delay(100);
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
            
            CurrentState = TaskState.Ended;
            TaskDidEnd?.Invoke(this, EventArgs.Empty);
        }
    }
}