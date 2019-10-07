using System;
using System.Threading.Tasks;
using HPNS.InteractivityV2.Core.Condition;
using HPNS.InteractivityV2.Core.Task;

namespace HPNS.InteractivityV2.Support
{
    public class ConditionWaitRecoverTask : TaskBase
    {
        private ICondition _condition;
        
        public ConditionWaitRecoverTask(ICondition condition)
        {
            _condition = condition;
        }
        
        protected override async Task ExecutePrepare()
        {
            await _condition.Prepare();
        }

        protected override void ExecuteStart()
        {
            StartConditionNotifications();

            if (_condition.ConditionState == ConditionState.Recovered)
            {
                AbortConditionNotifications();
                NotifyTaskDidEnd();
            }
        }

        protected override void ExecuteAbort()
        {
            AbortConditionNotifications();
        }

        protected override void ExecuteReset()
        {
            _condition.Reset();
        }

        private void ConditionOnDidRecover(object sender, EventArgs e)
        {
            AbortConditionNotifications();
            NotifyTaskDidEnd();
        }

        private void StartConditionNotifications()
        {
            _condition.DidRecover += ConditionOnDidRecover;
            _condition.Start();
        }

        private void AbortConditionNotifications()
        {
            _condition.DidRecover -= ConditionOnDidRecover;
            _condition.Abort();
        }
    }
}