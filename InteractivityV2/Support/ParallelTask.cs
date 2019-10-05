using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HPNS.InteractivityV2.Core;

namespace HPNS.InteractivityV2.Support
{
    public class ParallelTask : TaskBase
    {
        private List<ITask> _tasks;
        private List<ITask> _runningTasks;
        
        public ParallelTask(IEnumerable<ITask> tasks)
        {
            _tasks = new List<ITask>(tasks);
        }

        protected override async Task ExecutePrepare()
        {
            var prepareTasks = new List<Task>();
            foreach (var task in _tasks)
                prepareTasks.Add(task.Prepare());

            await Task.WhenAll(prepareTasks);
        }

        protected override void ExecuteStart()
        {
            _runningTasks = new List<ITask>();
            foreach (var task in _tasks)
            {
                task.TaskDidEnd += TaskOnTaskDidEnd;
                task.Start();

                _runningTasks.Add(task);
            }
        }

        protected override void ExecuteAbort()
        {
            foreach (var task in _runningTasks)
            {
                task.TaskDidEnd -= TaskOnTaskDidEnd;
                task.Abort();
            }
        }

        protected override void ExecuteReset()
        {
            foreach (var task in _tasks)
                task.Reset();
        }

        private void TaskOnTaskDidEnd(object sender, EventArgs e)
        {
            var task = (ITask) sender;
            
            task.TaskDidEnd -= TaskOnTaskDidEnd;
            _runningTasks.Remove(task);
            
            if (_runningTasks.Count == 0) NotifyTaskDidEnd();
        }
    }
}