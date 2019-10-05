using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.InteractivityV2.Core;

namespace HPNS.InteractivityV2.Support
{
    public class WaitTask : TaskBase
    {
        private class CancellationToken
        {
            public bool Cancelled { get; set; }
        }
        
        private CancellationToken _currentCancellationToken;

        public Property<int> Duration;

        protected override async Task ExecutePrepare() { }

        protected override void ExecuteStart()
        {
            var duration = Duration.Value;
            
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