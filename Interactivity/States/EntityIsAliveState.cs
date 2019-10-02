using HPNS.Core;
using HPNS.Interactivity.Core;

namespace HPNS.Interactivity.States
{
    public class EntityIsAliveState : StateBase
    {
        private int _entityHandle;

        public override bool IsValid => World.Current.EntityDeathTracker.IsEntityAlive(_entityHandle);

        public EntityIsAliveState(int entityHandle)
        {
            _entityHandle = entityHandle;
        }
        
        protected override void ExecuteStarting()
        {
            World.Current.EntityDeathTracker.AddEntity(_entityHandle);
            World.Current.EntityDeathTracker.EntityDidDie += EntityDeathTrackerOnEntityDidDie;
        }

        protected override void ExecuteStopping()
        {
            World.Current.EntityDeathTracker.RemoveEntity(_entityHandle);
            World.Current.EntityDeathTracker.EntityDidDie -= EntityDeathTrackerOnEntityDidDie;
        }

        private void EntityDeathTrackerOnEntityDidDie(object sender, int e)
        {
            NotifyStateDidBreak();
        }
    }
}