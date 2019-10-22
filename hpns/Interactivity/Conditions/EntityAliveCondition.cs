using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.Interactivity.Core.Condition;
using HPNS.Interactivity.Core.Data;
using World = HPNS.CoreClient.World;

namespace HPNS.Interactivity.Conditions
{
    public class EntityAliveCondition : ConditionBase
    {
        public IParameter<int> EntityHandle;

        protected override Task ExecutePrepare()
        {
            return TaskHelper.CompletedTask;
        }

        protected override void ExecuteStart()
        {
            base.ExecuteStart();
            SubscribeToEntityDeathTrackerNotifications();
        }

        protected override void ExecuteAbort()
        {
            UnsubscribeFromEntityDeathTrackerNotifications();
        }

        protected override void ExecuteReset() { }

        protected override bool IsConditionInValidState()
        {
            return World.Current.EntityDeathTracker.IsEntityAlive(EntityHandle.GetValue());
        }

        private void SubscribeToEntityDeathTrackerNotifications()
        {
            World.Current.EntityDeathTracker.EntityDidDie += EntityDeathTrackerOnEntityDidDie;
            World.Current.EntityDeathTracker.AddEntity(EntityHandle.GetValue());
        }

        private void UnsubscribeFromEntityDeathTrackerNotifications()
        {
            World.Current.EntityDeathTracker.EntityDidDie -= EntityDeathTrackerOnEntityDidDie;
            World.Current.EntityDeathTracker.RemoveEntity(EntityHandle.GetValue());
        }

        private void EntityDeathTrackerOnEntityDidDie(int e)
        {
            if (e != EntityHandle.GetValue()) return;
            
            UnsubscribeFromEntityDeathTrackerNotifications();
            NotifyConditionDidBreak();
        }
    }
}