using System;
using CitizenFX.Core;

namespace HPNS.Core.Tools
{
    public class Checkpoint : IUpdateObject
    {
        private Vector3 _center;
        private float _radius;
        private ICheckpointDecorator _decorator;
        
        private bool _isPlayerInside;

        public event EventHandler PlayerEntered;
        public event EventHandler PlayerLeft;
        
        public Checkpoint(Vector3 center, float radius, ICheckpointDecorator decorator = null)
        {
            _center = center;
            _radius = radius;
            
            _decorator = decorator;
            _decorator?.AddDecoration(center, radius);
        }
        
        public void Update(float deltaTime)
        {
            var distance = Vector3.Distance(Game.PlayerPed.Position, _center);
            if (distance <= _radius)
            {
                if (_isPlayerInside) return;

                _isPlayerInside = true;
                PlayerEntered?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                if (!_isPlayerInside) return;

                _isPlayerInside = false;
                PlayerLeft?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Destroy()
        {
            PlayerLeft?.Invoke(this, EventArgs.Empty);
            _decorator?.RemoveDecoration();
        }
    }
}