using System.Collections.Generic;
using System.Threading.Tasks;
using HPNS.Core;
using HPNS.InteractivityV2.Core.Data;
using HPNS.InteractivityV2.Core.Task;
using HPNS.InteractivityV2.Support;
using static CitizenFX.Core.Native.API;

namespace HPNS.InteractivityV2.Tasks
{
    public class PlayAnimTask : TaskBase
    {
        private string _dict;
        private string _name;
        
        public IParameter<int> PedHandle;
        public IParameter<int> Duration;

        protected virtual int AnimFlag => 1;

        private ITask _animSequence;

        public PlayAnimTask(string dict, string name)
        {
            _dict = dict;
            _name = name;
        }
        
        protected override async Task ExecutePrepare()
        {
            await Utility.LoadAnimDict(_dict);

            var tasks = new List<ITask>();
            tasks.Add(new LambdaTask(PlayAnim));
            tasks.Add(new WaitTask {Duration = Duration});
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
            var pedHandle = PedHandle.GetValue();
            TaskPlayAnim(pedHandle, _dict, _name, 8f, 8f, -1, AnimFlag, 
                0f, false, false, false);
        }

        private void StopAnim()
        {
            var pedHandle = PedHandle.GetValue();
            StopAnimTask(pedHandle, _dict, _name, 3f);
        }
    }
}