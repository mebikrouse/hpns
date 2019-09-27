using System;
using HPNS.Core;
using HPNS.Interactivity.Core;

namespace HPNS.Interactivity.States
{
    public class AimingAtEntityState : IState
    {
        private int _entityHandle;

        private bool _isAimingAtEntity;

        public StateState CurrentState { get; private set; } = StateState.Waiting;
        
        public bool IsValid => _isAimingAtEntity;
        
        public event EventHandler StateDidBreak;
        public event EventHandler StateDidRecover;

        public AimingAtEntityState(int entityHandle)
        {
            _entityHandle = entityHandle;
        }
        
        public void Start()
        {
            if (CurrentState != StateState.Waiting)
                throw new Exception("Cannot start state that is not in Waiting state.");
            
            World.Current.AimingManager.PlayerDidStartAimingAtEntity += AimingManagerOnPlayerDidStartAimingAtEntity;
            World.Current.AimingManager.PlayerDidStopAimingAtEntity += AimingManagerOnPlayerDidStopAimingAtEntity;

            CurrentState = StateState.Running;
        }

        public void Stop()
        {
            if (CurrentState != StateState.Running)
                throw new Exception("Cannot stop state that is not in Running state.");
            
            World.Current.AimingManager.PlayerDidStartAimingAtEntity -= AimingManagerOnPlayerDidStartAimingAtEntity;
            World.Current.AimingManager.PlayerDidStopAimingAtEntity -= AimingManagerOnPlayerDidStopAimingAtEntity;

            CurrentState = StateState.Waiting;
        }

        private void AimingManagerOnPlayerDidStartAimingAtEntity(object sender, int e)
        {
            if (e != _entityHandle) return;

            _isAimingAtEntity = true;
            StateDidRecover?.Invoke(this, EventArgs.Empty);
        }

        private void AimingManagerOnPlayerDidStopAimingAtEntity(object sender, int e)
        {
            if (e != _entityHandle) return;

            _isAimingAtEntity = false;
            StateDidBreak?.Invoke(this, EventArgs.Empty);
        }
    }
}