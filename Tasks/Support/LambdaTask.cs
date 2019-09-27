using System;
using HPNS.Tasks.Core;
using HPNS.Tasks.Core.Exceptions;

namespace HPNS.Tasks.Support
{
    public class LambdaTask : ITask
    {
        private Action _lambda;
        
        public TaskState CurrentState { get; private set; } = TaskState.Waiting;
        
        public event EventHandler TaskDidEnd;

        public LambdaTask(Action lambda)
        {
            _lambda = lambda;
        }
        
        public void Start()
        {
            if (CurrentState != TaskState.Waiting)
                throw new StartException();
            
            _lambda();

            CurrentState = TaskState.Ended;
            TaskDidEnd?.Invoke(this, EventArgs.Empty);
        }

        public void Abort()
        {
            if (CurrentState != TaskState.Running)
                throw new AbortException();

            CurrentState = TaskState.Aborted;
        }
    }
}