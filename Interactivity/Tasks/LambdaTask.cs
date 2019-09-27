using System;
using HPNS.Interactivity.Core;
using HPNS.Interactivity.Exceptions;

namespace HPNS.Interactivity.Tasks
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