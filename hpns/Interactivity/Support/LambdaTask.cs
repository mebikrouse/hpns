using System;
using System.Threading.Tasks;
using HPNS.Core;
using HPNS.Interactivity.Core.Task;

namespace HPNS.Interactivity.Support
{
    public class LambdaTask : TaskBase
    {
        private Action _lambda;
        
        public LambdaTask(Action lambda)
        {
            _lambda = lambda;
        }

        protected override Task ExecutePrepare()
        {
            return TaskHelper.CompletedTask;
        }

        protected override void ExecuteStart()
        {
            _lambda();
            NotifyTaskDidEnd();
        }

        protected override void ExecuteAbort() { }

        protected override void ExecuteReset() { }
    }
}