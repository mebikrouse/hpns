using System;

namespace HPNS.Interactivity.Exceptions
{
    public class StateStopException : Exception
    {
        public override string Message => "Cannot stop state that is not in Running state.";
    }
}