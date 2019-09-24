using HPNS.Core.Managers;

namespace HPNS.Core
{
    public class World
    {
        private const int CHECKPOINT_MANAGER_REFRESH_RATE = 500;
        private const int VEHICLE_EVENTS_MANAGER_REFRESH_RATE = 500;
        
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
        
        public CheckpointManager CheckpointManager
        {
            get { return _checkpointManager; }
        }

        public VehicleEventsManager VehicleEventsManager
        {
            get { return _vehicleEventsManager; }
        }
        
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
        }
    }
}