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

        public AimingAtEntityCondition() : base(nameof(AimingAtEntityCondition)) { }

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
            Debug.WriteLine($"Subscribing {Name} to AimingManager's notifications.");
            
            World.Current.AimingManager.PlayerDidStartAimingAtEntity += AimingManagerOnPlayerDidStartAimingAtEntity;
            World.Current.AimingManager.PlayerDidStopAimingAtEntity += AimingManagerOnPlayerDidStopAimingAtEntity;
        }

        private void UnsubscribeFromAimingManagerNotifications()
        {
            Debug.WriteLine($"Unsubscribing {Name} from AimingManager's notifications.");
            
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