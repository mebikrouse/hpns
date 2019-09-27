using System;
using System.Collections.Generic;
using HPNS.Interactivity.Core;
using HPNS.Interactivity.Exceptions;

namespace HPNS.Interactivity.Support
{
    public class SequenceTask : ITask
    {
        private Queue<ITask> _tasks;
        private ITask _currentTask;
        
        public TaskState CurrentState { get; private set; } = TaskState.Waiting;
        
        public event EventHandler TaskDidEnd;

        public SequenceTask(IEnumerable<ITask> tasks)
        {
            _tasks = new Queue<ITask>(tasks);
        }
        
        public void Start()
        {
            if (CurrentState != TaskState.Waiting)
                throw new StartException();

            if (_tasks.Count == 0)
            {
                CurrentState = TaskState.Ended;
                TaskDidEnd?.Invoke(this, EventArgs.Empty);
                
                return;
            }
            
            StartNextTask();
            
            CurrentState = TaskState.Running;
        }

        public void Abort()
        {
            if (CurrentState != TaskState.Running)
                throw new AbortException();
            
            _currentTask.Abort();
            _currentTask = null;

            CurrentState = TaskState.Aborted;
        }

        private void StartNextTask()
        {
            if (_tasks.Count == 0)
            {
                CurrentState = TaskState.Ended;
                TaskDidEnd?.Invoke(this, EventArgs.Empty);

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