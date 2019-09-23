using System;
using CitizenFX.Core;

namespace HPNS.Core.Tools
{
    public class VehicleEventsManager : IUpdateObject
    {
        private bool _isInVehicle;
        private bool _isEnteringVehicle;

        public event EventHandler<Vehicle> PlayerEntering;
        public event EventHandler PlayerAbortedEntering;
        public event EventHandler<Vehicle> PlayerEntered;
        public event EventHandler PlayerLeft;
        
        public void Update(float deltaTime)
        {
            var currentVehicle = Game.PlayerPed.CurrentVehicle;
            if (!_isInVehicle && !Game.Player.IsDead)
            {
                var vehicleTryingToEnter = Game.PlayerPed.VehicleTryingToEnter;
                if (vehicleTryingToEnter != null && !_isEnteringVehicle)
                {
                    _isEnteringVehicle = true;
                    PlayerEntering?.Invoke(this, vehicleTryingToEnter);
                } else if (vehicleTryingToEnter == null && currentVehicle == null && _isEnteringVehicle)
                {
                    _isEnteringVehicle = false;
                    PlayerAbortedEntering?.Invoke(this, EventArgs.Empty);
                } else if (currentVehicle != null)
                {
                    _isEnteringVehicle = false;
                    _isInVehicle = true;
                    PlayerEntered?.Invoke(this, currentVehicle);
                }
            } else if (_isInVehicle)
            {
                if (currentVehicle == null || Game.Player.IsDead)
                {
                    _isInVehicle = false;
                    PlayerLeft?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}