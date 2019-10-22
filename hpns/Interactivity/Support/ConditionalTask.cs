using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Interactivity.Core.Task;

namespace HPNS.Interactivity.Support
{
    public class ConditionalTask : TaskBase
    {
        private ITask _task;
        private Func<bool> _condition;

        private bool _taskDidStart;
        
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
                Debug.WriteLine("Condition is false.");
                
                NotifyTaskDidEnd();
                return;
            }
            
            Debug.WriteLine("Condition is true.");

            _taskDidStart = true;
            
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
            if (!_taskDidStart)
            {
                Debug.WriteLine("Subtask never started. No need to reset.");
                return;
            }
            
            Debug.WriteLine("Resetting subtask.");
            
            _taskDidStart = false;
            _task.Reset();
        }

        private void TaskOnDidEnd(object sender, EventArgs e)
        {
            _task.DidEnd -= TaskOnDidEnd;
            NotifyTaskDidEnd();
        }
    }
}