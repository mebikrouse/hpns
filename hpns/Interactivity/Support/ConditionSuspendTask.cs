using System;
using System.Threading.Tasks;
using HPNS.Interactivity.Core.Condition;
using HPNS.Interactivity.Core.Task;

namespace HPNS.Interactivity.Support
{
    public class ConditionSuspendTask : TaskBase
    {
        private ITask _task;
        private ICondition _condition;
        
        public ConditionSuspendTask(ITask task, ICondition condition)
        {
            _task = task;
            _condition = condition;
        }
        
        protected override async Task ExecutePrepare()
        {
            var taskPrepare = _task.Prepare();
            var conditionPrepare = _condition.Prepare();

            await Task.WhenAll(taskPrepare, conditionPrepare);
        }

        protected override void ExecuteStart()
        {
            StartConditionNotifications();
            if (_condition.ConditionState == ConditionState.Recovered)
                StartTask();
        }

        protected override void ExecuteAbort()
        {
            AbortConditionNotifications();
            if (_condition.ConditionState == ConditionState.Recovered)
                AbortTask();
        }

        protected override void ExecuteReset()
        {
            _task.Reset();
            _condition.Reset();
        }

        private void ConditionOnDidRecover(object sender, EventArgs e)
        {
            StartTask();
        }

        private void ConditionOnDidBreak(object sender, EventArgs e)
        {
            AbortTask();
            _task.Reset();
        }

        private void TaskOnDidEnd(object sender, EventArgs e)
        {
            _task.DidEnd -= TaskOnDidEnd;
            
            AbortConditionNotifications();
            NotifyTaskDidEnd();
        }

        private void StartConditionNotifications()
        {
            _condition.DidBreak += ConditionOnDidBreak;
            _condition.DidRecover += ConditionOnDidRecover;
            _condition.Start();
        }

        private void AbortConditionNotifications()
        {
            _condition.DidBreak -= ConditionOnDidBreak;
            _condition.DidRecover -= ConditionOnDidRecover;
            _condition.Abort();
        }

        private void StartTask()
        {
            _task.DidEnd += TaskOnDidEnd;
            _task.Start();
        }

        private void AbortTask()
        {
            _task.DidEnd -= TaskOnDidEnd;
            _task.Abort();   
        }
    }
}