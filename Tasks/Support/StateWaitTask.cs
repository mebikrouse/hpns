using System;
using HPNS.Tasks.Core;
using HPNS.Tasks.Core.Exceptions;

namespace HPNS.Tasks.Support
{
    public class StateWaitTask : ITask
    {
        private IState _state;
        
        public TaskState CurrentState { get; private set; } = TaskState.Waiting;
        
        public event EventHandler TaskDidEnd;

        public StateWaitTask(IState state)
        {
            _state = state;
        }
        
        public void Start()
        {
            if (CurrentState != TaskState.Waiting)
                throw new StartException();
            
            _state.StateDidRecover += StateOnStateDidRecover;
            _state.Start();
            
            CurrentState = TaskState.Running;
        }

        public void Abort()
        {
            if (CurrentState != TaskState.Running)
                throw new AbortException();

            _state.StateDidRecover -= StateOnStateDidRecover;
            _state.Stop();

            CurrentState = TaskState.Aborted;
        }

        private void StateOnStateDidRecover(object sender, EventArgs e)
        {
            _state.StateDidRecover -= StateOnStateDidRecover;
            _state.Stop();
            
            CurrentState = TaskState.Ended;
            TaskDidEnd?.Invoke(this, EventArgs.Empty);
        }
    }
}