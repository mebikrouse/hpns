using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Interactivity.Core.Task;

namespace HPNS.Interactivity.Support
{
    public class ParallelAllTask : TaskBase
    {
        private List<ITask> _tasks;
        private List<ITask> _runningTasks;
        
        public ParallelAllTask(IEnumerable<ITask> tasks)
        {
            _tasks = new List<ITask>(tasks);
        }
        
        public ParallelAllTask(params ITask[] tasks)
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
                task.DidEnd += TaskOnDidEnd;
                task.Start();

                _runningTasks.Add(task);
            }
        }

        protected override void ExecuteAbort()
        {
            foreach (var task in _runningTasks)
            {
                task.DidEnd -= TaskOnDidEnd;
                task.Abort();
            }
        }

        protected override void ExecuteReset()
        {
            foreach (var task in _tasks)
                task.Reset();
        }

        private void TaskOnDidEnd(object sender, EventArgs e)
        {
            var task = (ITask) sender;
            
            Debug.WriteLine($"Task did end. Tasks left: {_runningTasks.Count - 1}.");
            
            task.DidEnd -= TaskOnDidEnd;
            _runningTasks.Remove(task);

            if (_runningTasks.Count == 0)
            {
                Debug.WriteLine("All tasks have been consumed.");
                NotifyTaskDidEnd();
            }
        }
    }
}