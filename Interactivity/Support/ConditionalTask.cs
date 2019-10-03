using System;
using HPNS.Interactivity.Core;

namespace HPNS.Interactivity.Support
{
    public class ConditionalTask : TaskBase
    {
        private Func<bool> _condition;
        private ITask _task;
        
        public ConditionalTask(Func<bool> condition, ITask task)
        {
            _condition = condition;
            _task = task;
        }
        
        protected override void ExecuteStarting()
        {
            if (!_condition())
            {
                NotifyTaskDidEnd();
                return;
            }
            
            _task.TaskDidEnd += TaskOnTaskDidEnd;
            _task.Start();
        }

        protected override void ExecuteAborting()
        {
            _task.TaskDidEnd -= TaskOnTaskDidEnd;
            _task.Abort();
        }

        private void TaskOnTaskDidEnd(object sender, EventArgs e)
        {
            _task.TaskDidEnd -= TaskOnTaskDidEnd;
            NotifyTaskDidEnd();
        }
    }
}