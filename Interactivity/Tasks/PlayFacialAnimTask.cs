using System.Collections.Generic;
using HPNS.Core;
using HPNS.Interactivity.Core;
using HPNS.Interactivity.Support;

using static CitizenFX.Core.Native.API;

namespace HPNS.Interactivity.Tasks
{
    public class PlayFacialAnimTask : TaskBase
    {
        private int _pedHandle;
        private string _dict;
        private string _name;
        private int _duration;

        private ITask _sequenceTask;
        
        public PlayFacialAnimTask(int pedHandle, string dict, string name, int duration)
        {
            _pedHandle = pedHandle;
            _dict = dict;
            _name = name;
            _duration = duration;
        }
        
        protected override async void ExecuteStarting()
        {
            var loadingTask = Utility.LoadAnimDict(_dict);
            
            var tasks = new List<ITask>();
            
            tasks.Add(new LambdaTask(StartAnimation));
            tasks.Add(new WaitTask(_duration));
            tasks.Add(new LambdaTask(StopAnimation));
            tasks.Add(new LambdaTask(NotifyTaskDidEnd));
            
            _sequenceTask = new SequentialSetTask(tasks);

            await loadingTask;
            _sequenceTask.Start();
        }

        protected override void ExecuteAborting()
        {
            _sequenceTask.Abort();
            StopAnimation();
        }

        private void StartAnimation()
        {
            TaskPlayAnim(_pedHandle, _dict, _name, 8.0f, 8.0f, -1, 33, 0.0f, false, false, false);
        }

        private void StopAnimation()
        {
            StopAnimTask(_pedHandle, _dict, _name, 3.0f);
        }
    }
}