using System;
using HPNS.Interactivity.Core;

namespace HPNS.Interactivity.Support
{
    public class StateSuspendTask : TaskBase
    {
        private Func<ITask> _taskProvider;
        private IState _state;
        
        private ITask _currentTask;

        public StateSuspendTask(Func<ITask> taskProvider, IState state)
        {
            _taskProvider = taskProvider;
            _state = state;
        }

        protected override void ExecuteStarting()
        {
            SubscribeToStateNotifications();
        }

        protected override void ExecuteAborting()
        {
            UnsubscribeFromStateNotifications();
            if (_currentTask != null) AbortCurrentTask();
        }

        private void OnStateDidRecover(object sender, EventArgs e)
        {
            if (_currentTask != null)
                throw new Exception("Cannot resume task because it is already running!");

            StartNewTask();
        }

        private void OnStateDidBreak(object sender, EventArgs e)
        {
            if (_currentTask == null)
                throw new Exception("Cannot abort task because it is already aborted!");

            AbortCurrentTask();
        }

        private void OnTaskDidEnd(object sender, EventArgs e)
        {
            UnsubscribeFromStateNotifications();

            _currentTask.TaskDidEnd -= OnTaskDidEnd;
            _currentTask = null;

            NotifyTaskDidEnd();
        }

        private void SubscribeToStateNotifications()
        {
            _state.StateDidBreak += OnStateDidBreak;
            _state.StateDidRecover += OnStateDidRecover;
            _state.Start();
        }

        private void UnsubscribeFromStateNotifications()
        {
            _state.StateDidBreak -= OnStateDidBreak;
            _state.StateDidRecover -= OnStateDidRecover;
            _state.Stop();
        }

        private void StartNewTask()
        {
            _currentTask = _taskProvider();
            
            _currentTask.TaskDidEnd += OnTaskDidEnd;
            _currentTask.Start();
        }

        private void AbortCurrentTask()
        {
            _currentTask.Abort();
            
            _currentTask.TaskDidEnd -= OnTaskDidEnd;
            _currentTask = null;
        }
    }
}