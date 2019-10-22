using System;
using CitizenFX.Core;
using HPNS.Interactivity.Core.Activity;

namespace HPNS.Interactivity.Core.Task
{
    public abstract class TaskBase : ActivityBase, ITask
    {
        public event EventHandler DidEnd;

        protected void NotifyTaskDidEnd()
        {
            TranslateToConsumedState();
            DidEnd?.Invoke(this, EventArgs.Empty);
        }
    }
}