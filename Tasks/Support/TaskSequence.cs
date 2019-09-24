using System.Collections.Generic;
using HPNS.Tasks.Core;

namespace HPNS.Tasks.Support
{
    public class TaskSequence : ITask, ITaskDelegate
    {
        private Queue<ITask> _tasks;
        private ITask _currentTask;
        
        public ITaskDelegate Delegate { get; set; }

        public TaskState CurrentState { get; private set; } = TaskState.Waiting;

        public TaskSequence(IEnumerable<ITask> tasks)
        {
            _tasks = new Queue<ITask>(tasks);
        }
        
        public void Start()
        {
            if (CurrentState != TaskState.Waiting) return;

            if (_tasks.Count == 0)
            {
                CurrentState = TaskState.Ended;
                Delegate?.TaskDidEnd(this);
                
                return;
            }
            
            StartNextTask();
            
            CurrentState = TaskState.Running;
            Delegate?.TaskDidStart(this);
        }

        private void StartNextTask()
        {
            if (_tasks.Count == 0)
            {
                CurrentState = TaskState.Ended;
                Delegate?.TaskDidEnd(this);

                return;
            }

            var nextTask = _tasks.Dequeue();
            _currentTask = nextTask;

            nextTask.Delegate = this;
            nextTask.Start();
        }

        public void Abort()
        {
            if (CurrentState != TaskState.Running &&
                CurrentState != TaskState.Suspended) return;
            
            _currentTask.Abort();

            CurrentState = TaskState.Aborted;
            Delegate?.TaskDidAbort(this);
        }

        public void Suspend()
        {
            if (CurrentState != TaskState.Running) return;
            
            _currentTask.Suspend();

            CurrentState = TaskState.Suspended;
            Delegate?.TaskDidSuspend(this);
        }

        public void Resume()
        {
            if (CurrentState != TaskState.Suspended) return;
            
            _currentTask.Resume();

            CurrentState = TaskState.Running;
            Delegate?.TaskDidResume(this);
        }

        public void TaskDidStart(ITask task) { }

        public void TaskDidEnd(ITask task)
        {
            StartNextTask();
        }

        public void TaskDidAbort(ITask task) { }

        public void TaskDidSuspend(ITask task) { }

        public void TaskDidResume(ITask task) { }
    }
}