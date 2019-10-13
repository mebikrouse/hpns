using System;
using CitizenFX.Core;
using HPNS.Interactivity.Core.Activity;

namespace HPNS.Interactivity.Core.Condition
{
    public abstract class ConditionBase : ActivityBase, ICondition
    {
        public ConditionState ConditionState { get; private set; }
        
        public event EventHandler DidBreak;
        public event EventHandler DidRecover;

        protected ConditionBase(string name) : base(name) { }
        
        protected override void ExecuteStart()
        {
            ConditionState = IsConditionInValidState() ? ConditionState.Recovered : ConditionState.Broken;
        }

        protected abstract bool IsConditionInValidState();

        protected void NotifyConditionDidBreak()
        {
            Debug.WriteLine($"Notifying condition {Name} did break.");
            
            if (ConditionState == ConditionState.Broken)
                throw new Exception("Cannot notify about break in condition that is in Broken state.");

            ConditionState = ConditionState.Broken;
            
            Debug.WriteLine($"Invoking {Name}'s DidBreak event.");
            DidBreak?.Invoke(this, EventArgs.Empty);
        }

        protected void NotifyConditionDidRecover()
        {
            Debug.WriteLine($"Notifying condition {Name} did recover.");
            
            if (ConditionState == ConditionState.Recovered)
                throw new Exception("Cannot notify about recovering in condition that is in Recovered state.");

            ConditionState = ConditionState.Recovered;
            
            Debug.WriteLine($"Invoking {Name}'s DidRecover event.");
            DidRecover?.Invoke(this, EventArgs.Empty);
        }
    }
}