using CitizenFX.Core;

using Checkpoint = HPNS.Core.Environment.Checkpoint;

namespace HPNS.Core.Managers
{
    public class CheckpointManager
    {
        public Checkpoint AddCheckpoint(Vector3 center, float radius)
        {
            return World.Current.ObjectManager.AddObject(new Checkpoint(center, radius));
        }

        public void RemoveCheckpoint(Checkpoint checkpoint)
        {
            World.Current.ObjectManager.DestroyObject(checkpoint);
        }
    }
}