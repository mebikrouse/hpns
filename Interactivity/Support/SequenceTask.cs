using System;
using System.Collections.Generic;
using HPNS.Interactivity.Core;

namespace HPNS.Interactivity.Support
{
    public class SequenceTask : TaskBase
    {
        private Queue<ITask> _tasks;
        private ITask _currentTask;

        public SequenceTask(IEnumerable<ITask> tasks)
        {
            _tasks = new Queue<ITask>(tasks);
        }

        protected override void ExecuteStarting()
        {
            StartNextTask();
        }

        protected override void ExecuteAborting()
        {
            _currentTask.Abort();
            
            _currentTask.TaskDidEnd -= CurrentTaskOnTaskDidEnd;
            _currentTask = null;
        }

        private void StartNextTask()
        {
            if (_tasks.Count == 0)
            {
                NotifyTaskDidEnd();
                return;
            }

            _currentTask = _tasks.Dequeue();
            
            _currentTask.TaskDidEnd += CurrentTaskOnTaskDidEnd;
            _currentTask.Start();
        }

        private void CurrentTaskOnTaskDidEnd(object sender, EventArgs e)
        {
            _currentTask.TaskDidEnd -= CurrentTaskOnTaskDidEnd;
            _currentTask = null;
            
            StartNextTask();
        }
    }
}