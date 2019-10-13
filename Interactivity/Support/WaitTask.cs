using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.Interactivity.Core.Data;
using HPNS.Interactivity.Core.Task;

namespace HPNS.Interactivity.Support
{
    public class WaitTask : TaskBase
    {
        private class CancellationToken
        {
            public bool Cancelled { get; set; }
        }
        
        private CancellationToken _currentCancellationToken;

        public IParameter<int> Duration;
        
        public WaitTask() : base(nameof(WaitTask)) { }

        protected override Task ExecutePrepare()
        {
            return TaskHelper.CompletedTask;
        }

        protected override void ExecuteStart()
        {
            var duration = Duration.GetValue();
            
            var cancellationToken = new CancellationToken();
            _ = WaitForDelay(duration, cancellationToken);

            _currentCancellationToken = cancellationToken;
        }

        protected override void ExecuteAbort()
        {
            _currentCancellationToken.Cancelled = true;
        }

        protected override void ExecuteReset() { }

        private async Task WaitForDelay(int delay, CancellationToken cancellationToken)
        {
            await BaseScript.Delay(delay);
            if (!cancellationToken.Cancelled) NotifyTaskDidEnd();
        }
    }
}