using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Interactivity.Core;

namespace HPNS.Interactivity.Tasks
{
    public class WaitTask : TaskBase
    {
        private int _delay;
        private bool _notifyAboutDelayEnding;

        public WaitTask(int delay)
        {
            _delay = delay;
        }

        protected override void ExecuteStarting()
        {
            _notifyAboutDelayEnding = true;
            _ = WaitForDelay(_delay);
        }

        protected override void ExecuteAborting()
        {
            _notifyAboutDelayEnding = false;
        }

        private async Task WaitForDelay(int delay)
        {
            await BaseScript.Delay(delay);
            if (_notifyAboutDelayEnding) NotifyTaskDidEnd();
        }
    }
}