namespace HPNS.Tasks.Core
{
    public interface ITask
    {
        ITaskDelegate Delegate { get; set; }
        TaskState CurrentState { get; }
        void Start();
        void Abort();
        void Suspend();
        void Resume();
    }
}