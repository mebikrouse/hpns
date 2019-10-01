using System;
using CitizenFX.Core;

namespace HPNS.Core.Environment
{
    public class Checkpoint : IObject
    {
        private Vector3 _center;
        private float _radius;
        
        private bool _playerWasInside;

        public event EventHandler PlayerEntered;
        public event EventHandler PlayerLeft;
        
        public Checkpoint(Vector3 center, float radius)
        {
            _center = center;
            _radius = radius;
        }

        public void OnCreate() { }

        public void OnDestroy() { }
        
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

        private bool IsPlayerInside()
        {
            return Vector3.Distance(Game.PlayerPed.Position, _center) <= _radius;
        }
    }
}