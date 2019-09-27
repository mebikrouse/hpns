using System;
using HPNS.Interactivity.Exceptions;

namespace HPNS.Interactivity.Core
{
    public abstract class TaskBase : ITask
    {
        public TaskState CurrentState { get; protected set; } = TaskState.Waiting;
        
        public event EventHandler TaskDidEnd;
        
        public void Start()
        {
            if (CurrentState != TaskState.Waiting)
                throw new StartException();

            CurrentState = TaskState.Running;
            
            ExecuteStarting();
        }

        public void Abort()
        {
            if (CurrentState != TaskState.Running)
                throw new AbortException();

            CurrentState = TaskState.Aborted;
            
            ExecuteAborting();
        }

        protected abstract void ExecuteStarting();

        protected abstract void ExecuteAborting();

        protected void NotifyTaskDidEnd()
        {
            CurrentState = TaskState.Ended;
            TaskDidEnd?.Invoke(this, EventArgs.Empty);
        }
    }
}