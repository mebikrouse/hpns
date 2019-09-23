namespace HPNS.Tasks.Core
{
    public interface ITask
    {
        ITaskDelegate Delegate { get; set; }
        void Start();
        void Abort();
        void Suspend();
        void Resume();
    }
}