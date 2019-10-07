using System;
using System.Threading.Tasks;
using HPNS.InteractivityV2.Core.Task;

namespace HPNS.InteractivityV2.Support
{
    public class ConditionalTask : TaskBase
    {
        private ITask _task;
        private Func<bool> _condition;
        
        public ConditionalTask(ITask task, Func<bool> condition)
        {
            _task = task;
            _condition = condition;
        }
        
        protected override async Task ExecutePrepare()
        {
            await _task.Prepare();
        }

        protected override void ExecuteStart()
        {
            if (!_condition())
            {
                NotifyTaskDidEnd();
                return;
            }
            
            _task.DidEnd += TaskOnDidEnd;
            _task.Start();
        }

        protected override void ExecuteAbort()
        {
            _task.DidEnd -= TaskOnDidEnd;
            _task.Abort();
        }

        protected override void ExecuteReset()
        {
            _task.Reset();
        }

        private void TaskOnDidEnd(object sender, EventArgs e)
        {
            _task.DidEnd -= TaskOnDidEnd;
            NotifyTaskDidEnd();
        }
    }
}