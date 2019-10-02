namespace HPNS.Core.Managers
{
    public class ObjectManager
    {
        private UpdateObjectPool _updateObjectPool;
        
        public ObjectManager(int refreshRate)
        {
            _updateObjectPool = new UpdateObjectPool(refreshRate);
            _updateObjectPool.Start();
        }

        public T AddObject<T>(T obj) where T : IObject
        {
            obj.OnCreate();
            _updateObjectPool.AddUpdateObject(obj);

            return obj;
        }

        public void DestroyObject(IObject obj)
        {
            obj.OnDestroy();
            _updateObjectPool.RemoveUpdateObject(obj);
        }
    }
}