using System;
using System.Collections.Generic;
using HPNS.Interactivity.Core;

namespace HPNS.Interactivity.Support
{
    public class ParallelSetTask : TaskBase
    {
        private List<ITask> _tasks;
        
        public ParallelSetTask(IEnumerable<ITask> tasks)
        {
            _tasks = new List<ITask>(tasks);
        }
        
        protected override void ExecuteStarting()
        {
            if (_tasks.Count == 0)
            {
                NotifyTaskDidEnd();
                return;
            }
            
            foreach (var task in _tasks)
            {
                task.TaskDidEnd += TaskOnTaskDidEnd;
                task.Start();
            }
        }

        protected override void ExecuteAborting()
        {
            foreach (var task in _tasks)
            {
                task.TaskDidEnd -= TaskOnTaskDidEnd;
                task.Abort();
            }
        }

        private void TaskOnTaskDidEnd(object sender, EventArgs e)
        {
            _tasks.Remove((ITask) sender);
            if (_tasks.Count == 0) NotifyTaskDidEnd();
        }
    }
}