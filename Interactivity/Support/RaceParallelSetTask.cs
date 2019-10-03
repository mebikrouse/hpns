using System;
using System.Collections.Generic;
using CitizenFX.Core;
using HPNS.Interactivity.Core;

namespace HPNS.Interactivity.Support
{
    public class RaceParallelSetTask : TaskBase
    {
        private List<ITask> _tasks;
        
        public RaceParallelSetTask(IEnumerable<ITask> tasks)
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
            AbortAllRunningTasks();
        }

        private void TaskOnTaskDidEnd(object sender, EventArgs e)
        {
            var task = (ITask) sender;

            task.TaskDidEnd -= TaskOnTaskDidEnd;
            _tasks.Remove((ITask) sender);

            AbortAllRunningTasks();
            NotifyTaskDidEnd();
        }

        private void AbortAllRunningTasks()
        {
            foreach (var task in _tasks)
            {
                task.TaskDidEnd -= TaskOnTaskDidEnd;
                task.Abort();
            }
        }
    }
}