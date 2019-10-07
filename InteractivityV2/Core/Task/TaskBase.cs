using System;
using HPNS.InteractivityV2.Core.Activity;

namespace HPNS.InteractivityV2.Core.Task
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