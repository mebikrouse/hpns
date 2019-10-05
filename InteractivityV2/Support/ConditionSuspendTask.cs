using System;
using System.Threading.Tasks;
using HPNS.InteractivityV2.Core;

namespace HPNS.InteractivityV2.Support
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
            await _task.Prepare();
        }

        protected override void ExecuteStart()
        {
            SubscribeToConditionEvents();
        }

        protected override void ExecuteAbort()
        {
            UnsubscribeFromConditionEvents();
            if (_task.CurrentState == TaskState.Running) 
                AbortTask();
        }

        protected override void ExecuteReset()
        {
            _task.Reset();
        }

        private void SubscribeToConditionEvents()
        {
            _condition.ConditionDidBreak += ConditionOnConditionDidBreak;
            _condition.ConditionDidRecover += ConditionOnConditionDidRecover;
        }

        private void UnsubscribeFromConditionEvents()
        {
            _condition.ConditionDidBreak -= ConditionOnConditionDidBreak;
            _condition.ConditionDidRecover -= ConditionOnConditionDidRecover;
        }

        private void ConditionOnConditionDidBreak(object sender, EventArgs e)
        {
            AbortTask();
        }

        private void ConditionOnConditionDidRecover(object sender, EventArgs e)
        {
            StartTask();
        }

        private void StartTask()
        {
            if (_task.CurrentState == TaskState.Aborted) 
                _task.Reset();
            
            _task.TaskDidEnd += TaskOnTaskDidEnd;
            _task.Start();
        }

        private void AbortTask()
        {
            _task.TaskDidEnd -= TaskOnTaskDidEnd;
            _task.Abort();
        }

        private void TaskOnTaskDidEnd(object sender, EventArgs e)
        {
            _task.TaskDidEnd -= TaskOnTaskDidEnd;
            
            UnsubscribeFromConditionEvents();
            NotifyTaskDidEnd();
        }
    }
}