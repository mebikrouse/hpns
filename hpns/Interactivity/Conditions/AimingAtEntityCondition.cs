using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.Interactivity.Core.Condition;
using HPNS.Interactivity.Core.Data;
using World = HPNS.CoreClient.World;

namespace HPNS.Interactivity.Conditions
{
    public class AimingAtEntityCondition : ConditionBase
    {
        public IParameter<int> EntityHandle;

        protected override Task ExecutePrepare()
        {
            return TaskHelper.CompletedTask;
        }

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