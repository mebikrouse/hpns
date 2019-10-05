using System;
using System.Threading.Tasks;

namespace HPNS.InteractivityV2.Core
{
    public abstract class TaskBase : ITask
    {
        public TaskState CurrentState { get; private set; } = TaskState.NotReady;

        public event EventHandler TaskDidEnd;

        public async Task Prepare()
        {
            if (CurrentState != TaskState.NotReady)
                throw new Exception();

            await ExecutePrepare();

            CurrentState = TaskState.Ready;
        }

        public void Start()
        {
            if (CurrentState != TaskState.Ready)
                throw new Exception();

            CurrentState = TaskState.Running;
            
            ExecuteStart();
        }

        public void Abort()
        {
            if (CurrentState != TaskState.Running)
                throw new Exception();

            CurrentState = TaskState.Aborted;
            
            ExecuteAbort();
        }

        public void Reset()
        {
            if (CurrentState == TaskState.NotReady ||
                CurrentState == TaskState.Running)
                throw new Exception();
            
            if (CurrentState == TaskState.Ready) 
                return;

            CurrentState = TaskState.Ready;
            
            ExecuteReset();
        }

        protected abstract Task ExecutePrepare();
        
        protected abstract void ExecuteStart();

        protected abstract void ExecuteAbort();

        protected abstract void ExecuteReset();
        
        protected void NotifyTaskDidEnd()
        {
            if (CurrentState != TaskState.Running)
                throw new Exception();
            
            CurrentState = TaskState.Ended;
            
            TaskDidEnd?.Invoke(this, EventArgs.Empty);
        }
    }
}