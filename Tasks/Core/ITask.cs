using System;

namespace HPNS.Tasks.Core
{
    public interface ITask
    {
        TaskState CurrentState { get; }
        event EventHandler TaskDidEnd;
        void Start();
        void Abort();
    }
}