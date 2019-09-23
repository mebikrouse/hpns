using HPNS.Tasks.Core;

namespace HPNS.Tasks.Support
{
    public class StateConjunction : ITask, ITaskDelegate, IStateDelegate
    {
        public ITaskDelegate Delegate { get; set; }

        private ITask _task;
        private IState _state;
        
        public StateConjunction(ITask task, IState state)
        {
            _task = task;
            _state = state;
        }
        
        public void Start()
        {
            Delegate?.TaskDidStart(this);
            _task.Start();
        }

        public void Abort()
        {
            _task.Abort();
        }

        public void Suspend()
        {
            _task.Suspend();
        }

        public void Resume()
        {
            _task.Resume();
        }

        public void TaskDidStart(ITask task)
        {
            _state.Start();
        }

        public void TaskDidEnd(ITask task)
        {
            _state.Stop();
            Delegate?.TaskDidEnd(this);
        }

        public void TaskDidAbort(ITask task)
        {
            _state.Stop();
            Delegate?.TaskDidAbort(this);
        }

        public void TaskDidSuspend(ITask task)
        {
            Delegate?.TaskDidSuspend(this);
        }

        public void TaskDidResume(ITask task)
        {
            Delegate?.TaskDidResume(this);
        }

        public void StateDidBreak(IState state)
        {
            _task.Suspend();
        }

        public void StateDidRecover(IState state)
        {
            _task.Resume();
        }
    }
}