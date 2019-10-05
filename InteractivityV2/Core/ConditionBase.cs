using System;

namespace HPNS.InteractivityV2.Core
{
    public abstract class ConditionBase : ICondition
    {
        public ConditionState CurrentState { get; private set; } = ConditionState.Waiting;
        
        public event EventHandler ConditionDidBreak;
        public event EventHandler ConditionDidRecover;
        
        public void Abort()
        {
            if (CurrentState == ConditionState.Aborted)
                throw new Exception();

            CurrentState = ConditionState.Aborted;
            
            ExecuteAbort();
        }

        protected abstract void ExecuteAbort();

        protected void NotifyConditionDidBreak()
        {
            if (CurrentState != ConditionState.Recovered)
                throw new Exception();

            CurrentState = ConditionState.Broken;
            
            ConditionDidBreak?.Invoke(this, EventArgs.Empty);
        }

        protected void NotifyConditionDidRecover()
        {
            if (CurrentState != ConditionState.Broken)
                throw new Exception();

            CurrentState = ConditionState.Recovered;
            
            ConditionDidRecover?.Invoke(this, EventArgs.Empty);
        }
    }
}