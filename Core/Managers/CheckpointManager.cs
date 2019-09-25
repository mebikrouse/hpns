using System.Collections.Generic;
using CitizenFX.Core;

using Checkpoint = HPNS.Core.Environment.Checkpoint;

namespace HPNS.Core.Managers
{
    public class CheckpointManager : IUpdateObject
    {
        private List<Checkpoint> _checkpoints = new List<Checkpoint>();
        
        public Checkpoint AddCheckpoint(Vector3 center, float radius)
        {
            var checkpoint = new Checkpoint(center, radius);
            _checkpoints.Add(checkpoint);
            
            return checkpoint;
        }

        public void RemoveCheckpoint(Checkpoint checkpoint)
        {
            _checkpoints.Remove(checkpoint);
            checkpoint.Destroy();
        }

        public void Update(float deltaTime)
        {
            var checkpoints = new List<Checkpoint>(_checkpoints);
            foreach (var checkpoint in checkpoints)
                checkpoint.Update(deltaTime);
        }
    }
}