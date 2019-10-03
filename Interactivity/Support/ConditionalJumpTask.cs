using System;
using HPNS.Interactivity.Core;

namespace HPNS.Interactivity.Support
{
    public class ConditionalJumpTask : TaskBase
    {
        private Func<bool> _condition;
        private ITask _taskOnTrue;
        private ITask _taskOnFalse;

        private ITask _currentTask;
        
        public ConditionalJumpTask(Func<bool> condition, ITask taskOnTrue, ITask taskOnFalse)
        {
            _condition = condition;
            _taskOnTrue = taskOnTrue;
            _taskOnFalse = taskOnFalse;
        }
        
        protected override void ExecuteStarting()
        {
            var task = _condition() ? _taskOnTrue : _taskOnFalse;
            task.TaskDidEnd += CurrentTaskOnTaskDidEnd;
            task.Start();

            _currentTask = task;
        }

        protected override void ExecuteAborting()
        {
            _currentTask.TaskDidEnd -= CurrentTaskOnTaskDidEnd;
            _currentTask.Abort();
        }

        private void CurrentTaskOnTaskDidEnd(object sender, EventArgs e)
        {
            _currentTask.TaskDidEnd -= CurrentTaskOnTaskDidEnd;
            NotifyTaskDidEnd();
        }
    }
}