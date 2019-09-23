using System.Collections.Generic;
using HPNS.Tasks.Core;

namespace HPNS.Tasks.Support
{
    public class TaskSequence : ITask, ITaskDelegate
    {
        public ITaskDelegate Delegate { get; set; }

        private Queue<ITask> _tasks;
        private ITask _currentTask;
        
        public TaskSequence(IEnumerable<ITask> tasks)
        {
            _tasks = new Queue<ITask>(tasks);
        }
        
        public void Start()
        {
            Delegate?.TaskDidStart(this);
            StartNextTask();
        }

        public void Abort()
        {
            _currentTask?.Abort();
        }

        public void Suspend()
        {
            _currentTask?.Suspend();
        }

        public void Resume()
        {
            _currentTask?.Resume();
        }

        private void StartNextTask()
        {
            if (_tasks.Count == 0)
            {
                Delegate?.TaskDidEnd(this);
                return;
            }

            var nextTask = _tasks.Dequeue();
            nextTask.Delegate = this;
            nextTask.Start();
        }

        public void TaskDidStart(ITask task)
        {
            _currentTask = task;
        }

        public void TaskDidEnd(ITask task)
        {
            StartNextTask();
        }

        public void TaskDidAbort(ITask task)
        {
            Delegate?.TaskDidAbort(this);
        }

        public void TaskDidSuspend(ITask task)
        {
            Delegate?.TaskDidSuspend(this);
        }

        public void TaskDidResume(ITask task)
        {
            Delegate?.TaskDidResume(this);
        }
    }
}