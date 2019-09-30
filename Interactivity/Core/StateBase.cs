using System;
using HPNS.Interactivity.Exceptions;

namespace HPNS.Interactivity.Core
{
    public abstract class StateBase : IState
    {
        public StateState CurrentState { get; protected set; } = StateState.Waiting;
        
        public abstract bool IsValid { get; }
        
        public event EventHandler StateDidBreak;
        public event EventHandler StateDidRecover;
        
        public void Start()
        {
            if (CurrentState != StateState.Waiting)
                throw new StateStartException();

            CurrentState = StateState.Waiting;
            
            ExecuteStarting();
        }

        public void Stop()
        {
            if (CurrentState != StateState.Running)
                throw new StateStopException();

            CurrentState = StateState.Running;
            
            ExecuteStopping();
        }

        protected abstract void ExecuteStarting();

        protected abstract void ExecuteStopping();

        protected void NotifyStateDidBreak()
        {
            StateDidBreak?.Invoke(this, EventArgs.Empty);
        }

        protected void NotifyStateDidRecover()
        {
            StateDidRecover?.Invoke(this, EventArgs.Empty);
        }
    }
}