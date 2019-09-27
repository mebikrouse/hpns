using System;
using HPNS.Interactivity.Core;

namespace HPNS.Interactivity.Support
{
    public class StateWaitTask : TaskBase
    {
        private IState _state;

        public StateWaitTask(IState state)
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

        private void StateOnStateDidRecover(object sender, EventArgs e)
        {
            UnsubscribeFromStateNotifications();
            NotifyTaskDidEnd();
        }

        private void SubscribeToStateNotifications()
        {
            _state.StateDidRecover += StateOnStateDidRecover;
            _state.Start();
        }

        private void UnsubscribeFromStateNotifications()
        {
            _state.StateDidRecover -= StateOnStateDidRecover;
            _state.Stop();
        }
    }
}