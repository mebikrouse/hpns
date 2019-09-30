using HPNS.Core;
using HPNS.Interactivity.Core;

namespace HPNS.Interactivity.States
{
    public class AimingAtEntityState : StateBase
    {
        private int _entityHandle;

        public override bool IsValid => World.Current.AimingManager.IsPlayerAimingAtEntity(_entityHandle);

        public AimingAtEntityState(int entityHandle)
        {
            _entityHandle = entityHandle;
        }

        protected override void ExecuteStarting()
        {
            World.Current.AimingManager.PlayerDidStartAimingAtEntity += AimingManagerOnPlayerDidStartAimingAtEntity;
            World.Current.AimingManager.PlayerDidStopAimingAtEntity += AimingManagerOnPlayerDidStopAimingAtEntity;
        }

        protected override void ExecuteStopping()
        {
            World.Current.AimingManager.PlayerDidStartAimingAtEntity -= AimingManagerOnPlayerDidStartAimingAtEntity;
            World.Current.AimingManager.PlayerDidStopAimingAtEntity -= AimingManagerOnPlayerDidStopAimingAtEntity;
        }

        private void AimingManagerOnPlayerDidStartAimingAtEntity(object sender, int e)
        {
            if (e != _entityHandle) return;
            NotifyStateDidRecover();
        }

        private void AimingManagerOnPlayerDidStopAimingAtEntity(object sender, int e)
        {
            if (e != _entityHandle) return;
            NotifyStateDidBreak();
        }
    }
}