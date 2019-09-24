using System;
using HPNS.Tasks.Core;

namespace HPNS.Tasks.Support
{
    public class StateConjunction : ITask
    {
        private Func<ITask> _taskProvider;
        private IState _state;
        
        private ITask _currentTask;

        public TaskState CurrentState { get; private set; } = TaskState.Waiting;
        
        public event EventHandler TaskDidEnd;

        public StateConjunction(Func<ITask> taskProvider, IState state)
        {
            _taskProvider = taskProvider;
            _state = state;
        }
        
        public void Start()
        {
            if (CurrentState != TaskState.Waiting)
                throw new Exception("Cannot start task that is not in Waiting state!");
            
            _state.StateDidBreak += OnStateDidBreak;
            _state.StateDidRecover += OnStateDidRecover;
            _state.Start();

            if (_state.IsValid)
            {
                _currentTask = _taskProvider();
                _currentTask.TaskDidEnd += OnTaskDidEnd;
                _currentTask.Start();
            }

            CurrentState = TaskState.Running;
        }

        public void Abort()
        {
            if (CurrentState != TaskState.Running)
                throw new Exception("Cannot abort task that is not in Running state!");
            
            _state.StateDidBreak -= OnStateDidBreak;
            _state.StateDidRecover -= OnStateDidRecover;
            _state.Stop();

            if (_currentTask != null)
            {
                _currentTask.TaskDidEnd -= OnTaskDidEnd;
                _currentTask.Abort();
                _currentTask = null;
            }

            CurrentState = TaskState.Aborted;
        }

        private void OnStateDidRecover(object sender, EventArgs e)
        {
            if (_currentTask != null)
                throw new Exception("Cannot resume task because it is already running!");
            
            _currentTask = _taskProvider();
            _currentTask.TaskDidEnd += OnTaskDidEnd;
            _currentTask.Start();
        }

        private void OnStateDidBreak(object sender, EventArgs e)
        {
            if (_currentTask == null)
                throw new Exception("Cannot abort task because it is already aborted!");
            
            _currentTask.TaskDidEnd -= OnTaskDidEnd;
            _currentTask.Abort();
            _currentTask = null;
        }

        private void OnTaskDidEnd(object sender, EventArgs e)
        {
            _state.StateDidBreak -= OnStateDidBreak;
            _state.StateDidRecover -= OnStateDidRecover;
            _state.Stop();

            _currentTask.TaskDidEnd -= OnTaskDidEnd;
            _currentTask = null;

            CurrentState = TaskState.Ended;
            TaskDidEnd?.Invoke(this, EventArgs.Empty);
        }
    }
}