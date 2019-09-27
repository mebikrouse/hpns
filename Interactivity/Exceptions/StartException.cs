using System;

namespace HPNS.Interactivity.Exceptions
{
    public class StartException : Exception
    {
        public override string Message => "Cannot start task that is not in Waiting state.";
    }
}