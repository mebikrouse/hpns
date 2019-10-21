using System;
using CitizenFX.Core;
using HPNS.Interactivity.Core.Activity;

namespace HPNS.Interactivity.Core.Task
{
    public abstract class TaskBase : ActivityBase, ITask
    {
        public event EventHandler DidEnd;
        
        protected TaskBase(string name) : base(name) { }

        protected void NotifyTaskDidEnd()
        {
            Debug.WriteLine($"Notifying task {Name} did end.");
            
            TranslateToConsumedState();
            
            Debug.WriteLine($"Invoking {Name}'s DidEnd event.");
            DidEnd?.Invoke(this, EventArgs.Empty);
        }
    }
}