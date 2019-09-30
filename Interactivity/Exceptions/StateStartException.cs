using System;

namespace HPNS.Interactivity.Exceptions
{
    public class StateStartException : Exception
    {
        public override string Message => "Cannot start state that is not in Waiting state.";
    }
}