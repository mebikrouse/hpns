using System;
using System.Collections.Generic;
using HPNS.Core;

using static CitizenFX.Core.Native.API;

namespace HPNS.CoreClient.Managers
{
    public class EntityDeathTracker : IUpdateObject
    {
        private class Counter<T>
        {
            public T Value { get; }
            public int Count { get; set; } = 1;

            public Counter(T value)
            {
                Value = value;
            }
        }
        
        private class EntityTracker
        {
            private int _entityHandle;
            private bool _entityPrevState;

            public event Action<int> EntityDidDie;
            public event Action<int> EntityDidResurrect;

            public EntityTracker(int entityHandle)
            {
                _entityHandle = entityHandle;
                _entityPrevState = IsEntityDead(entityHandle);
            }
            
            public void Update()
            {
                var currentEntityState = IsEntityDead(_entityHandle);
                if (currentEntityState == _entityPrevState) return;
                
                if (currentEntityState) EntityDidDie?.Invoke(_entityHandle);
                else EntityDidResurrect?.Invoke(_entityHandle);

                _entityPrevState = currentEntityState;
            }
        }
        
        private Dictionary<int, Counter<EntityTracker>> _entities = new Dictionary<int, Counter<EntityTracker>>();

        public event Action<int> EntityDidDie;
        public event Action<int> EntityDidResurrect;
        
        public bool IsEntityAlive(int entityHandle)
        {
            return !IsEntityDead(entityHandle);
        }
        
        public void AddEntity(int entityHandle)
        {
           if (!_entities.TryGetValue(entityHandle, out var counter))
           {
               var entityTracker = new EntityTracker(entityHandle);
               entityTracker.EntityDidDie += EntityTrackerOnEntityDidDie;
               entityTracker.EntityDidResurrect += EntityTrackerOnEntityDidResurrect;
               
               _entities[entityHandle] = new Counter<EntityTracker>(entityTracker);
           }
           else
           {
               counter.Count++;
           }
        }

        public void RemoveEntity(int entityHandle)
        {
            if (!_entities.TryGetValue(entityHandle, out var counter)) return;

            if (counter.Count == 1)
            {
                var entityTracker = counter.Value;
                entityTracker.EntityDidDie -= EntityTrackerOnEntityDidDie;
                entityTracker.EntityDidResurrect -= EntityTrackerOnEntityDidResurrect;
                
                _entities.Remove(entityHandle);
            }
            else
            {
                counter.Count--;
            }
        }

        public void Update(float deltaTime)
        {
            var entities = new List<Counter<EntityTracker>>(_entities.Values);
            foreach (var entity in entities)
                entity.Value.Update();
        }

        private void EntityTrackerOnEntityDidDie(int entityHandle)
        {
            EntityDidDie?.Invoke(entityHandle);
        }

        private void EntityTrackerOnEntityDidResurrect(int entityHandle)
        {
            EntityDidResurrect?.Invoke(entityHandle);
        }
    }
}