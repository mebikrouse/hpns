using System;

namespace HPNS.InteractivityV2.Core.Activity
{
    public abstract class ActivityBase : IActivity
    {
        public ActivityState ActivityState { get; private set; } = ActivityState.NotReady;
        
        public async System.Threading.Tasks.Task Prepare()
        {
            if (ActivityState != ActivityState.NotReady)
                throw new Exception($"Cannot prepare activity that is not in NotReady state. Current state - {this.ActivityState}.");

            await ExecutePrepare();

            ActivityState = ActivityState.Ready;
        }

        public void Start()
        {
            if (ActivityState != ActivityState.Ready)
                throw new Exception($"Cannot start activity that is not in Ready state. Current state - {this.ActivityState}.");

            ActivityState = ActivityState.Running;
            
            ExecuteStart();
        }

        public void Abort()
        {
            if (ActivityState != ActivityState.Running)
                throw new Exception($"Cannot abort activity that is not in Running state. Current state - {this.ActivityState}.");

            ActivityState = ActivityState.Consumed;
            
            ExecuteAbort();
        }

        public void Reset()
        {
            if (ActivityState != ActivityState.Consumed)
                throw new Exception($"Cannot reset activity that is not in Consumed state. Current state - {this.ActivityState}.");

            ActivityState = ActivityState.Ready;
            
            ExecuteReset();
        }

        protected abstract System.Threading.Tasks.Task ExecutePrepare();

        protected abstract void ExecuteStart();

        protected abstract void ExecuteAbort();

        protected abstract void ExecuteReset();

        protected void TranslateToConsumedState()
        {
            if (ActivityState != ActivityState.Running)
                throw new Exception($"Cannot end activity that is not in Running state. Current state - {this.ActivityState}");

            ActivityState = ActivityState.Consumed;
        }
    }
}