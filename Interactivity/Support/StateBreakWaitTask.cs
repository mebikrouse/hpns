using System;
using HPNS.Interactivity.Core;

namespace HPNS.Interactivity.Support
{
    public class StateBreakWaitTask : TaskBase
    {
        private IState _state;

        public StateBreakWaitTask(IState state)
        {
            _state = state;
        }
        
        protected override void ExecuteStarting()
        {
            SubscribeToStateNotifications();
        }

        protected override void ExecuteAborting()
        {
            UnsubscribeFromStateNotifications();
        }

        private void SubscribeToStateNotifications()
        {
            _state.StateDidBreak += StateOnStateDidBreak;
            _state.Start();
        }

        private void UnsubscribeFromStateNotifications()
        {
            _state.StateDidBreak -= StateOnStateDidBreak;
            _state.Stop();
        }

        private void StateOnStateDidBreak(object sender, EventArgs e)
        {
            UnsubscribeFromStateNotifications();
            NotifyTaskDidEnd();
        }
    }
}