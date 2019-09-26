using System;

namespace HPNS.Tasks.Core.Exceptions
{
    public class AbortException : Exception
    {
        public override string Message => "Cannot abort task that is not in Running state.";
    }
}