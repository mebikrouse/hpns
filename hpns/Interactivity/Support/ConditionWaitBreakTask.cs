using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Interactivity.Core.Condition;
using HPNS.Interactivity.Core.Task;

namespace HPNS.Interactivity.Support
{
    public class ConditionWaitBreakTask : TaskBase
    {
        private ICondition _condition;
        
        public ConditionWaitBreakTask(ICondition condition)
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

            if (_condition.ConditionState == ConditionState.Broken)
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

        private void ConditionOnDidBreak(object sender, EventArgs e)
        {
            AbortConditionNotifications();
            NotifyTaskDidEnd();
        }

        private void StartConditionNotifications()
        {
            Debug.WriteLine($"Subscribing to condition's notifications.");
            
            _condition.DidBreak += ConditionOnDidBreak;
            _condition.Start();
        }

        private void AbortConditionNotifications()
        {
            Debug.WriteLine($"Unsubscribing from condition's notifications.");
            
            _condition.DidBreak -= ConditionOnDidBreak;
            _condition.Abort();
        }
    }
}