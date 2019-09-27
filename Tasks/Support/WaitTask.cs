using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Tasks.Core;
using HPNS.Tasks.Core.Exceptions;

namespace HPNS.Tasks.Support
{
    public class WaitTask : ITask
    {
        private int _delay;
        private bool _notifyAboutDelayEnding;
        
        public TaskState CurrentState { get; private set; } = TaskState.Waiting;
        
        public event EventHandler TaskDidEnd;

        public WaitTask(int delay)
        {
            _delay = delay;
        }
        
        public void Start()
        {
            if (CurrentState != TaskState.Waiting)
                throw new StartException();

            _notifyAboutDelayEnding = true;
            _ = WaitForDelay(_delay);

            CurrentState = TaskState.Running;
        }

        public void Abort()
        {
            if (CurrentState != TaskState.Running)
                throw new AbortException();

            _notifyAboutDelayEnding = false;

            CurrentState = TaskState.Aborted;
        }

        private async Task WaitForDelay(int delay)
        {
            await BaseScript.Delay(delay);

            if (_notifyAboutDelayEnding)
            {
                CurrentState = TaskState.Ended;
                TaskDidEnd?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}