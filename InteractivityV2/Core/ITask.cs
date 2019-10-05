using System;
using System.Threading.Tasks;

namespace HPNS.InteractivityV2.Core
{
    public interface ITask
    {
        TaskState CurrentState { get; }
        event EventHandler TaskDidEnd;
        Task Prepare();
        void Start();
        void Abort();
        void Reset();
    }
}