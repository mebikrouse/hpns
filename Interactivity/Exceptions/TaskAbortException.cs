using System;

namespace HPNS.Interactivity.Exceptions
{
    public class TaskAbortException : Exception
    {
        public override string Message => "Cannot abort task that is not in Running state.";
    }
}