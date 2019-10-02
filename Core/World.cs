using HPNS.Core.Managers;

namespace HPNS.Core
{
    public class World
    {
        private const int REFRESH_RATE = 100;
        
        private static World _current;
        
        public static World Current
        {
            get
            {
                if (_current == null)
                    _current = new World();

                return _current;
            }
        }

        private UpdateObjectPool _updateObjectPool;
        
        public VehicleEventsManager VehicleEventsManager { get; }
        public AimingManager AimingManager { get; }
        public ObjectManager ObjectManager { get; }
        
        public EntityDeathTracker EntityDeathTracker { get; }

        public World()
        {
            _updateObjectPool = new UpdateObjectPool(REFRESH_RATE);
            _updateObjectPool.Start();

            VehicleEventsManager = new VehicleEventsManager();
            _updateObjectPool.AddUpdateObject(VehicleEventsManager);
            
            AimingManager = new AimingManager();
            _updateObjectPool.AddUpdateObject(AimingManager);
            
            EntityDeathTracker = new EntityDeathTracker();
            _updateObjectPool.AddUpdateObject(EntityDeathTracker);
            
            ObjectManager = new ObjectManager(REFRESH_RATE);
        }
    }
}