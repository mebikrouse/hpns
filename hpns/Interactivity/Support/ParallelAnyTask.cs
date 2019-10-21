using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Interactivity.Core.Task;

namespace HPNS.Interactivity.Support
{
    public class ParallelAnyTask : TaskBase
    {
        private List<ITask> _tasks;
        private List<ITask> _runningTasks;
        
        public ParallelAnyTask(IEnumerable<ITask> tasks) : base(nameof(ParallelAnyTask))
        {
            _tasks = new List<ITask>(tasks);
        }
        
        public ParallelAnyTask(params ITask[] tasks) : base(nameof(ParallelAnyTask))
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
            AbortRunningTasks();
        }

        protected override void ExecuteReset()
        {
            foreach (var task in _tasks)
                task.Reset();
        }

        private void TaskOnDidEnd(object sender, EventArgs e)
        {
            var task = (ITask) sender;
            
            Debug.WriteLine($"Task did end. Aborting all other tasks.");
            
            task.DidEnd -= TaskOnDidEnd;
            _runningTasks.Remove(task);
            
            AbortRunningTasks();
            NotifyTaskDidEnd();
        }

        private void AbortRunningTasks()
        {
            foreach (var task in _runningTasks)
            {
                task.DidEnd -= TaskOnDidEnd;
                task.Abort();
            }
        }
    }
}