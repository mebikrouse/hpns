using System;
using System.Threading.Tasks;
using HPNS.InteractivityV2.Core;

namespace HPNS.InteractivityV2.Support
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

        protected override async Task ExecutePrepare()
        {
            await _task.Prepare();
        }

        protected override void ExecuteStart()
        {
            if (_condition()) StartTask();
            else NotifyTaskDidEnd();
        }

        protected override void ExecuteAbort()
        {
            _task.TaskDidEnd -= TaskOnTaskDidEnd;
            _task.Abort();
        }

        protected override void ExecuteReset()
        {
            _task.Reset();
        }

        private void StartTask()
        {
            _task.TaskDidEnd += TaskOnTaskDidEnd;
            _task.Start();
        }

        private void TaskOnTaskDidEnd(object sender, EventArgs e)
        {
            _task.TaskDidEnd -= TaskOnTaskDidEnd;
            NotifyTaskDidEnd();
        }
    }
}