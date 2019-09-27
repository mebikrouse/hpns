using System;
using HPNS.Interactivity.Core;

namespace HPNS.Interactivity.Tasks
{
    public class LambdaTask : TaskBase
    {
        private Action _lambda;

        public LambdaTask(Action lambda)
        {
            _lambda = lambda;
        }

        protected override void ExecuteStarting()
        {
            _lambda();
            NotifyTaskDidEnd();
        }

        protected override void ExecuteAborting() { }
    }
}