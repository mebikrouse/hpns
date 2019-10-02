using System;
using System.Collections.Generic;

using static CitizenFX.Core.Native.API;

namespace HPNS.Core.Managers
{
    public class EntityDeathTracker : IUpdateObject
    {
        private Dictionary<int, int> _entities = new Dictionary<int, int>();

        public event EventHandler<int> EntityDidDie;

        public void AddEntity(int entityHandle)
        {
            _entities.TryGetValue(entityHandle, out var currentCount); 
            _entities[entityHandle] = currentCount + 1;
        }

        public void RemoveEntity(int entityHandle)
        {
            if (!_entities.TryGetValue(entityHandle, out var currentCount)) return;
            
            if (currentCount > 1) _entities[entityHandle] = currentCount - 1;
            else _entities.Remove(entityHandle);
        }

        public bool IsEntityAlive(int entityHandle)
        {
            return !IsEntityDead(entityHandle);
        }
        
        public void Update(float deltaTime)
        {
            var entities = new List<int>(_entities.Keys);
            foreach (var entityHandle in entities)
            {
                if (!IsEntityDead(entityHandle)) continue;

                RemoveEntity(entityHandle);
                EntityDidDie?.Invoke(this, entityHandle);
            }
        }
    }
}