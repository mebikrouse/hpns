namespace HPNS.Tasks.Core
{
    public interface IState
    {
        IStateDelegate Delegate { get; set; }
        bool IsValid { get; }
        void Start();
        void Stop();
    }
}