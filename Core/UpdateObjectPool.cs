using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace HPNS.Core
{
    public class UpdateObjectPool
    {
        public enum State
        {
            Idle,
            Running,
            Stopping
        }
        
        public State CurrentState { get; private set; }
        
        private int _refreshRate;
        private List<IUpdateObject> _updateObjects;
        
        public UpdateObjectPool(int refreshRate)
        {
            _refreshRate = refreshRate;
            _updateObjects = new List<IUpdateObject>();
        }

        public void AddUpdateObject(IUpdateObject updateObject)
        {
            lock (_updateObjects) _updateObjects.Add(updateObject);
        }

        public void RemoveUpdateObject(IUpdateObject updateObject)
        {
            lock(_updateObjects) _updateObjects.Remove(updateObject);
        }

        public void Start()
        {
            if (CurrentState != State.Idle) return;
            CurrentState = State.Running;
            
            _ = UpdateLoop();
        }

        public void Stop()
        {
            if (CurrentState != State.Running) return;
            CurrentState = State.Stopping;
        }

        private async Task UpdateLoop()
        {
            while (CurrentState == State.Running)
            {
                lock (_updateObjects)
                {
                    foreach (var updateObject in _updateObjects)
                        updateObject.Update(_refreshRate);
                }

                await BaseScript.Delay(_refreshRate);
            }

            CurrentState = State.Idle;
        }
    }
}