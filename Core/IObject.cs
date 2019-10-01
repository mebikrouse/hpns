namespace HPNS.Core
{
    public interface IObject : IUpdateObject
    {
        void OnCreate();
        void OnDestroy();
    }
}