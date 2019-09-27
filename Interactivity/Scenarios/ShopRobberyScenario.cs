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
            Debug.WriteLine("Loading animation dict");
            var dict = "mp_am_hold_up";
            var anim = "holdup_victim_20s";
            await LoadAnimDict(dict);

            Debug.WriteLine("Loading prop model");
            var propModelHash = (uint) GetHashKey("prop_poly_bag_01");
            await LoadObject(propModelHash);

            var propHandle = 0;
            var pedHandle = _pedHandle;

            var tasks = new List<ITask>();

            tasks.Add(new LambdaTask(() =>
            {
                Debug.WriteLine("Playing animation");
                TaskPlayAnim(pedHandle, dict, anim, 8.0f, 8.0f, -1, 0, 0.0f, false, false, false);
            }));
            tasks.Add(new WaitTask(11250));
            tasks.Add(new LambdaTask(() =>
            {
                Debug.WriteLine("Creating bag");
                propHandle = CreateObject((int) propModelHash, 0f, 0f, 0f, true, true, true);

                Debug.WriteLine("Attaching bag");
                var boneIndex = GetPedBoneIndex(pedHandle, 4138);
                AttachEntityToEntity(propHandle, pedHandle, boneIndex,
                    -0.09999999f, -0.04f, -0.13f, 0, 0, 0, true,
                    false, false, false, 0, true);
            }));
            tasks.Add(new WaitTask(9750));
            tasks.Add(new LambdaTask(() =>
            {
                Debug.WriteLine("Detaching bag");
                DetachEntity(propHandle, true, true);
            }));
            tasks.Add(new WaitTask(2000));

            var sequenceTask = new SequenceTask(tasks);

            Debug.WriteLine("Starting sequence");
            sequenceTask.TaskDidEnd += SequenceTaskOnTaskDidEnd;
            sequenceTask.Start();

            _sequenceTask = sequenceTask;
        }

        protected override void ExecuteAborting()
        {
            Debug.WriteLine("Aborting");
            
            _sequenceTask.Abort();
            
            _sequenceTask.TaskDidEnd -= SequenceTaskOnTaskDidEnd;
            _sequenceTask = null;
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
            Debug.WriteLine("Shop robbery scenario did end");
            
            _sequenceTask.TaskDidEnd -= SequenceTaskOnTaskDidEnd;
            _sequenceTask = null;
            
            NotifyTaskDidEnd();
        }
    }
}