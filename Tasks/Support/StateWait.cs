using System;
using HPNS.Tasks.Core;

namespace HPNS.Tasks.Support
{
    public class StateWait : ITask
    {
        private IState _state;
        
        public TaskState CurrentState { get; private set; } = TaskState.Waiting;
        
        public event EventHandler TaskDidEnd;

        public StateWait(IState state)
        {
            _state = state;
        }
        
        public void Start()
        {
            if (CurrentState != TaskState.Waiting)
                throw new Exception("Cannot start task that is not in Waiting state!");
            
            _state.StateDidRecover += StateOnStateDidRecover;
            _state.Start();
            
            CurrentState = TaskState.Running;
        }

        public void Abort()
        {
            if (CurrentState != TaskState.Running)
                throw new Exception("Cannot abort task that is not in Running state!");

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