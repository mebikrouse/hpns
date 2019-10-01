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
        public CheckpointManager CheckpointManager { get; }
        public ObjectManager ObjectManager { get; }

        public World()
        {
            _updateObjectPool = new UpdateObjectPool(REFRESH_RATE);

            VehicleEventsManager = new VehicleEventsManager();
            _updateObjectPool.AddUpdateObject(VehicleEventsManager);
            
            AimingManager = new AimingManager();
            _updateObjectPool.AddUpdateObject(AimingManager);
            
            CheckpointManager = new CheckpointManager();
            ObjectManager = new ObjectManager(REFRESH_RATE);
        }
    }
}