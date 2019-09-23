namespace HPNS.Tasks.Core
{
    public interface ITaskDelegate
    {
        void TaskDidStart(ITask task);
        void TaskDidEnd(ITask task);
        void TaskDidAbort(ITask task);
        void TaskDidSuspend(ITask task);
        void TaskDidResume(ITask task);
    }
}