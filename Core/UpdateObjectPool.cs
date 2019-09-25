using System.Collections.Generic;
using System.Threading;
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
            _updateObjects.Add(updateObject);
        }

        public void RemoveUpdateObject(IUpdateObject updateObject)
        {
            _updateObjects.Remove(updateObject);
        }

        public async void Start()
        {
            if (CurrentState != State.Idle) return;
            CurrentState = State.Running;
            
            await UpdateLoop();
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
                var updateObjects = new List<IUpdateObject>(_updateObjects);
                foreach (var updateObject in updateObjects)
                    updateObject.Update(_refreshRate);

                await BaseScript.Delay(_refreshRate);
            }

            CurrentState = State.Idle;
        }
    }
}