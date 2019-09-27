using System;

namespace HPNS.Interactivity.Core
{
    public interface ITask
    {
        TaskState CurrentState { get; }
        event EventHandler TaskDidEnd;
        void Start();
        void Abort();
    }
}