using CitizenFX.Core;

namespace HPNS.Core.Tools
{
    public interface ICheckpointDecorator
    {
        void AddDecoration(Vector3 center, float radius);
        void RemoveDecoration();
    }
}