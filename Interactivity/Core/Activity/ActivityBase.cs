using System;
using CitizenFX.Core;

namespace HPNS.Interactivity.Core.Activity
{
    public abstract class ActivityBase : IActivity
    {
        public ActivityState ActivityState { get; private set; } = ActivityState.NotReady;
        
        public string Name;

        protected ActivityBase(string name)
        {
            Name = name;
        }
        
        public async System.Threading.Tasks.Task Prepare()
        {
            Debug.WriteLine($"Preparing activity {Name}.");
            
            if (ActivityState != ActivityState.NotReady)
                throw new Exception($"Cannot prepare activity that is not in NotReady state. Current state - {this.ActivityState}.");

            await ExecutePrepare();

            ActivityState = ActivityState.Ready;
        }

        public void Start()
        {
            Debug.WriteLine($"Starting activity {Name}.");
            
            if (ActivityState != ActivityState.Ready)
                throw new Exception($"Cannot start activity that is not in Ready state. Current state - {this.ActivityState}.");

            ActivityState = ActivityState.Running;
            
            ExecuteStart();
        }

        public void Abort()
        {
            Debug.WriteLine($"Aborting activity {Name}.");
            
            if (ActivityState != ActivityState.Running)
                throw new Exception($"Cannot abort activity that is not in Running state. Current state - {this.ActivityState}.");

            ActivityState = ActivityState.Consumed;
            
            ExecuteAbort();
        }

        public void Reset()
        {
            Debug.WriteLine($"Resetting activity {Name}.");
            
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
            Debug.WriteLine($"Translating activity {Name} to Consumed state.");
            
            if (ActivityState != ActivityState.Running)
                throw new Exception($"Cannot end activity that is not in Running state. Current state - {this.ActivityState}");

            ActivityState = ActivityState.Consumed;
        }
    }
}