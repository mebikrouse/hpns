using System;
using HPNS.Interactivity.Core;

namespace HPNS.Interactivity.Support
{
    public class DeferredCreationTask : TaskBase
    {
        private Func<ITask> _taskProvider;
        private ITask _currentTask;
        
        public DeferredCreationTask(Func<ITask> taskProvider)
        {
            _taskProvider = taskProvider;
        }
        
        protected override void ExecuteStarting()
        {
            var task = _taskProvider();
            task.TaskDidEnd += CurrentTaskTaskDidEnd;
            task.Start();

            _currentTask = task;
        }

        protected override void ExecuteAborting()
        {
            _currentTask.TaskDidEnd -= CurrentTaskTaskDidEnd;
            _currentTask.Abort();
        }

        private void CurrentTaskTaskDidEnd(object sender, EventArgs e)
        {
            _currentTask.TaskDidEnd -= CurrentTaskTaskDidEnd;
            NotifyTaskDidEnd();
        }
    }
}