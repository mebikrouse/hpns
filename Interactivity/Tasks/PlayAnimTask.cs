using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Interactivity.Core;
using HPNS.Interactivity.Support;

using static CitizenFX.Core.Native.API;

namespace HPNS.Interactivity.Tasks
{
    public class PlayAnimTask : TaskBase
    {
        private const float BLEND_IN_SPEED = 8.0f;
        private const float BLEND_OUT_SPEED = 8.0f;
        
        private int _pedHandle;
        private string _dict;
        private string _name;
        private int _duration;

        private ITask _sequenceTask;
        
        public PlayAnimTask(int pedHandle, string dict, string name, int duration)
        {
            _pedHandle = pedHandle;
            _dict = dict;
            _name = name;
            _duration = duration;
        }
        
        protected override async void ExecuteStarting()
        {
            var loadingTask = LoadAnimDict(_dict);
            
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
            TaskPlayAnim(_pedHandle, _dict, _name, BLEND_IN_SPEED, BLEND_OUT_SPEED, _duration, 0, 
                0.0f, false, false, false);
        }

        private void StopAnimation()
        {
            StopAnimTask(_pedHandle, _dict, _name, 0f);
        }
        
        private static async Task LoadAnimDict(string dict)
        {
            RequestAnimDict(dict);

            while (!HasAnimDictLoaded(dict))
                await BaseScript.Delay(100);
        }
    }
}