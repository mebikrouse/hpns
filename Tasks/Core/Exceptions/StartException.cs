using System;

namespace HPNS.Tasks.Core.Exceptions
{
    public class StartException : Exception
    {
        public override string Message => "Cannot start task that is not in Waiting state.";
    }
}