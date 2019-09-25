using System;
using CitizenFX.Core;

namespace HPNS.Core.Environment
{
    public class Checkpoint : IUpdateObject
    {
        private Vector3 _center;
        private float _radius;
        private ICheckpointDecorator _decorator;
        
        private bool _playerWasInside;

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
            if (IsPlayerInside())
            {
                if (_playerWasInside) return;
                
                _playerWasInside = true;
                PlayerEntered?.Invoke(this, EventArgs.Empty);
                
            }
            else
            {
                if (!_playerWasInside) return;

                _playerWasInside = false;
                PlayerLeft?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Destroy()
        {
            PlayerLeft?.Invoke(this, EventArgs.Empty);
            _decorator?.RemoveDecoration();
        }

        private bool IsPlayerInside()
        {
            return Vector3.Distance(Game.PlayerPed.Position, _center) <= _radius;
        }
    }
}