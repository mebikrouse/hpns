namespace HPNS.Tasks.Core
{
    public interface IStateDelegate
    {
        void StateDidBreak(IState state);
        void StateDidRecover(IState state);
    }
}