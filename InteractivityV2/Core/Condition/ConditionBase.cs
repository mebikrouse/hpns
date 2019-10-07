using System;
using HPNS.InteractivityV2.Core.Activity;

namespace HPNS.InteractivityV2.Core.Condition
{
    public abstract class ConditionBase : ActivityBase, ICondition
    {
        public ConditionState ConditionState { get; private set; }
        
        public event EventHandler DidBreak;
        public event EventHandler DidRecover;

        protected override void ExecuteStart()
        {
            ConditionState = IsConditionInValidState() ? ConditionState.Recovered : ConditionState.Broken;
        }

        protected abstract bool IsConditionInValidState();

        protected void NotifyConditionDidBreak()
        {
            if (ConditionState == ConditionState.Broken)
                throw new Exception("Cannot notify about break in condition that is in Broken state.");

            ConditionState = ConditionState.Broken;
            
            DidBreak?.Invoke(this, EventArgs.Empty);
        }

        protected void NotifyConditionDidRecover()
        {
            if (ConditionState == ConditionState.Recovered)
                throw new Exception("Cannot notify about recovering in condition that is in Recovered state.");

            ConditionState = ConditionState.Recovered;
            
            DidRecover?.Invoke(this, EventArgs.Empty);
        }
    }
}