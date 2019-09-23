using CitizenFX.Core;

namespace HPNS.Core.Tools
{
    public class CheckpointManager
    {
        private const int DEFAULT_REFRESH_RATE = 1000;

        private static CheckpointManager _instance;
        
        public static CheckpointManager Default
        {
            get
            {
                if (_instance == null) 
                    _instance = new CheckpointManager(DEFAULT_REFRESH_RATE);
                
                return _instance;
            }
        }

        private UpdateObjectPool _updateObjectPool;
        
        public CheckpointManager(int refreshRate)
        {
            _updateObjectPool = new UpdateObjectPool(refreshRate);
            _updateObjectPool.Start();
        }

        public Checkpoint AddCheckpoint(Vector3 center, float radius)
        {
            var checkpoint = new Checkpoint(center, radius);
            _updateObjectPool.AddUpdateObject(checkpoint);
            
            return checkpoint;
        }

        public void RemoveCheckpoint(Checkpoint checkpoint)
        {
            _updateObjectPool.RemoveUpdateObject(checkpoint);
            checkpoint.Destroy();
        }
    }
}