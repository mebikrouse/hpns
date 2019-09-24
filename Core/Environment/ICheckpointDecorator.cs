using CitizenFX.Core;

namespace HPNS.Core.Environment
{
    public interface ICheckpointDecorator
    {
        void AddDecoration(Vector3 center, float radius);
        void RemoveDecoration();
    }
}