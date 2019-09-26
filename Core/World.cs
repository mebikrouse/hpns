using HPNS.Core.Managers;

namespace HPNS.Core
{
    public class World
    {
        private const int CHECKPOINT_MANAGER_REFRESH_RATE = 500;
        private const int VEHICLE_EVENTS_MANAGER_REFRESH_RATE = 500;
        private const int AIMING_MANAGER_REFRESH_RATE = 100;
        
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

        private UpdateObjectPool _checkpointManagerUpdate;
        private CheckpointManager _checkpointManager;

        private UpdateObjectPool _vehicleEventsManagerUpdate;
        private VehicleEventsManager _vehicleEventsManager;

        private UpdateObjectPool _aimingManagerUpdate;
        private AimingManager _aimingManager;
        
        public CheckpointManager CheckpointManager => _checkpointManager;

        public VehicleEventsManager VehicleEventsManager => _vehicleEventsManager;

        public AimingManager AimingManager => _aimingManager;

        public World()
        {
            _checkpointManagerUpdate = new UpdateObjectPool(CHECKPOINT_MANAGER_REFRESH_RATE);
            _checkpointManager = new CheckpointManager();
            _checkpointManagerUpdate.AddUpdateObject(_checkpointManager);
            _checkpointManagerUpdate.Start();

            _vehicleEventsManagerUpdate = new UpdateObjectPool(VEHICLE_EVENTS_MANAGER_REFRESH_RATE);
            _vehicleEventsManager = new VehicleEventsManager();
            _vehicleEventsManagerUpdate.AddUpdateObject(_vehicleEventsManager);
            _vehicleEventsManagerUpdate.Start();
            
            _aimingManagerUpdate = new UpdateObjectPool(AIMING_MANAGER_REFRESH_RATE);
            _aimingManager = new AimingManager();
            _aimingManagerUpdate.AddUpdateObject(_aimingManager);
            _aimingManagerUpdate.Start();
        }
    }
}