using System.Threading.Tasks;
using HPNS.Core;
using HPNS.InteractivityV2.Core.Condition;
using HPNS.InteractivityV2.Core.Data;

namespace HPNS.InteractivityV2.Conditions
{
    public class AimingAtEntityCondition : ConditionBase
    {
        public IParameter<int> EntityHandle;

        protected override async Task ExecutePrepare() { }

        protected override void ExecuteStart()
        {
            base.ExecuteStart();
            SubscribeToAimingManagerNotifications();
        }

        protected override void ExecuteAbort()
        {
            UnsubscribeFromAimingManagerNotifications();
        }

        protected override void ExecuteReset() { }

        protected override bool IsConditionInValidState()
        {
            return World.Current.AimingManager.IsPlayerAimingAtEntity(EntityHandle.GetValue());
        }

        private void SubscribeToAimingManagerNotifications()
        {
            World.Current.AimingManager.PlayerDidStartAimingAtEntity += AimingManagerOnPlayerDidStartAimingAtEntity;
            World.Current.AimingManager.PlayerDidStopAimingAtEntity += AimingManagerOnPlayerDidStopAimingAtEntity;
        }

        private void UnsubscribeFromAimingManagerNotifications()
        {
            World.Current.AimingManager.PlayerDidStartAimingAtEntity -= AimingManagerOnPlayerDidStartAimingAtEntity;
            World.Current.AimingManager.PlayerDidStopAimingAtEntity -= AimingManagerOnPlayerDidStopAimingAtEntity;
        }

        private void AimingManagerOnPlayerDidStartAimingAtEntity(object sender, int e)
        {
            if (e == EntityHandle.GetValue()) NotifyConditionDidRecover();
        }

        private void AimingManagerOnPlayerDidStopAimingAtEntity(object sender, int e)
        {
            if (e == EntityHandle.GetValue()) NotifyConditionDidBreak();
        }
    }
}