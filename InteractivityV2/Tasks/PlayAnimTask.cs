using System.Collections.Generic;
using System.Threading.Tasks;
using HPNS.Core;
using HPNS.InteractivityV2.Core;
using HPNS.InteractivityV2.Support;
using static CitizenFX.Core.Native.API;

namespace HPNS.InteractivityV2.Tasks
{
    public class PlayAnimTask : TaskBase
    {
        private int _pedHandle;
        private string _dict;
        private string _name;
        private int _duration;

        protected virtual int AnimFlag => 1;

        private ITask _animSequence;

        public PlayAnimTask(int pedHandle, string dict, string name, int duration)
        {
            _pedHandle = pedHandle;
            _dict = dict;
            _name = name;
            _duration = duration;
        }
        
        protected override async Task ExecutePrepare()
        {
            await Utility.LoadAnimDict(_dict);

            var tasks = new List<ITask>();
            tasks.Add(new LambdaTask(PlayAnim));
            tasks.Add(new WaitTask(_duration));
            tasks.Add(new LambdaTask(StopAnim));
            tasks.Add(new LambdaTask(NotifyTaskDidEnd));

            _animSequence = new SequenceTask(tasks);
            
            await _animSequence.Prepare();
        }

        protected override void ExecuteStart()
        {
            _animSequence.Start();
        }

        protected override void ExecuteAbort()
        {
            _animSequence.Abort();
            StopAnim();
        }

        protected override void ExecuteReset()
        {
            _animSequence.Reset();
        }

        private void PlayAnim()
        {
            TaskPlayAnim(_pedHandle, _dict, _name, 8f, 8f, -1, AnimFlag, 
                0f, false, false, false);
        }

        private void StopAnim()
        {
            StopAnimTask(_pedHandle, _dict, _name, 3f);
        }
    }
}