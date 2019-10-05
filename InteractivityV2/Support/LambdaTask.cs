using System;
using System.Threading.Tasks;
using HPNS.InteractivityV2.Core;

namespace HPNS.InteractivityV2.Support
{
    public class LambdaTask : TaskBase
    {
        private Action _lambda;
        
        public LambdaTask(Action lambda)
        {
            _lambda = lambda;
        }

        protected override async Task ExecutePrepare() { }

        protected override void ExecuteStart()
        {
            _lambda();
            NotifyTaskDidEnd();
        }

        protected override void ExecuteAbort() { }

        protected override void ExecuteReset() { }
    }
}