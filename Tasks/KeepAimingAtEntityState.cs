using System;
using HPNS.Core;
using HPNS.Tasks.Core;

namespace HPNS.Tasks
{
    public class KeepAimingAtEntityState : IState
    {
        private int _entityHandle;

        private bool _isAimingAtEntity;

        public bool IsValid => _isAimingAtEntity;
        
        public event EventHandler StateDidBreak;
        public event EventHandler StateDidRecover;

        public KeepAimingAtEntityState(int entityHandle)
        {
            _entityHandle = entityHandle;
        }
        
        public void Start()
        {
            World.Current.AimingManager.PlayerDidStartAimingAtEntity += AimingManagerOnPlayerDidStartAimingAtEntity;
            World.Current.AimingManager.PlayerDidStopAimingAtEntity += AimingManagerOnPlayerDidStopAimingAtEntity;
        }

        public void Stop()
        {
            World.Current.AimingManager.PlayerDidStartAimingAtEntity -= AimingManagerOnPlayerDidStartAimingAtEntity;
            World.Current.AimingManager.PlayerDidStopAimingAtEntity -= AimingManagerOnPlayerDidStopAimingAtEntity;
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