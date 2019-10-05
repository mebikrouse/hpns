using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HPNS.InteractivityV2.Core;

namespace HPNS.InteractivityV2.Support
{
    public class SequenceTask : TaskBase
    {
        private List<ITask> _tasks;
        private ITask _currentTask;
        private int _nextTaskIndex;
        
        public SequenceTask(IEnumerable<ITask> tasks)
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
            StartNextTask();
        }

        protected override void ExecuteAbort()
        {
            _currentTask.TaskDidEnd -= TaskOnTaskDidEnd;
            _currentTask.Abort();
        }

        protected override void ExecuteReset()
        {
            for (var i = 0; i < _nextTaskIndex; i++)
                _tasks[i].Reset();

            _nextTaskIndex = 0;
        }

        private void StartNextTask()
        {
            if (_nextTaskIndex >= _tasks.Count)
            {
                NotifyTaskDidEnd();
                return;
            }

            var task = _tasks[_nextTaskIndex];
            task.TaskDidEnd += TaskOnTaskDidEnd;
            task.Start();

            _currentTask = task;
            _nextTaskIndex++;
        }

        private void TaskOnTaskDidEnd(object sender, EventArgs e)
        {
            _currentTask.TaskDidEnd -= TaskOnTaskDidEnd;
            StartNextTask();
        }
    }
}