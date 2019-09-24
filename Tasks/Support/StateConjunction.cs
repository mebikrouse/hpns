using HPNS.Tasks.Core;

namespace HPNS.Tasks.Support
{
    public class StateConjunction : ITask, ITaskDelegate, IStateDelegate
    {
        private ITask _task;
        private IState _state;
        
        public ITaskDelegate Delegate { get; set; }
        
        public TaskState CurrentState { get; private set; }

        public StateConjunction(ITask task, IState state)
        {
            _task = task;
            _task.Delegate = this;
            
            _state = state;
            _state.Delegate = this;
        }
        
        public void Start()
        {
            if (CurrentState != TaskState.Waiting) return;
            
            _state.Start();
            if (_state.IsValid) _task.Start();

            CurrentState = TaskState.Running;
            Delegate?.TaskDidStart(this);
        }

        public void Abort()
        {
            if (CurrentState != TaskState.Running &&
                CurrentState != TaskState.Suspended) return;
            
            _state.Stop();
            _task.Abort();

            CurrentState = TaskState.Aborted;
            Delegate?.TaskDidAbort(this);
        }

        public void Suspend()
        {
            if (CurrentState != TaskState.Running) return;
            
            _state.Stop();
            _task.Suspend();

            CurrentState = TaskState.Suspended;
            Delegate?.TaskDidSuspend(this);
        }

        public void Resume()
        {
            if (CurrentState != TaskState.Suspended) return;
            
            _state.Start();
            if (_state.IsValid) _task.Resume();

            CurrentState = TaskState.Running;
            Delegate?.TaskDidResume(this);
        }

        public void TaskDidStart(ITask task) { }

        public void TaskDidEnd(ITask task)
        {
            _state.Stop();
            
            CurrentState = TaskState.Ended;
            Delegate?.TaskDidEnd(this);
        }

        public void TaskDidAbort(ITask task) { }

        public void TaskDidSuspend(ITask task) { }

        public void TaskDidResume(ITask task) { }

        public void StateDidBreak(IState state)
        {
            _task.Suspend();
        }

        public void StateDidRecover(IState state)
        {
            if (_task.CurrentState == TaskState.Waiting) _task.Start();
            else _task.Resume();
        }
    }
}