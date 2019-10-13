namespace HPNS.Interactivity.Core.Activity
{
    public interface IActivity
    {
        ActivityState ActivityState { get; }
        System.Threading.Tasks.Task Prepare();
        void Start();
        void Abort();
        void Reset();
    }
}