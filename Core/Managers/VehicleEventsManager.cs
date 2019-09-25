using System;
using CitizenFX.Core;

namespace HPNS.Core.Managers
{
    public class VehicleEventsManager : IUpdateObject
    {
        private bool _isInVehicle;
        private bool _isEnteringVehicle;

        private Vehicle _prevVehicle;

        public event EventHandler<Vehicle> PlayerEntering;
        public event EventHandler<Vehicle> PlayerAbortedEntering;
        public event EventHandler<Vehicle> PlayerEntered;
        public event EventHandler<Vehicle> PlayerLeft;
        
        public void Update(float deltaTime)
        {
            var currentVehicle = Game.PlayerPed.CurrentVehicle;
            if (!_isInVehicle && !Game.Player.IsDead)
            {
                var vehicleTryingToEnter = Game.PlayerPed.VehicleTryingToEnter;
                if (vehicleTryingToEnter != null && !_isEnteringVehicle)
                {
                    _prevVehicle = vehicleTryingToEnter;
                    _isEnteringVehicle = true;
                    PlayerEntering?.Invoke(this, vehicleTryingToEnter);
                } 
                else if (vehicleTryingToEnter == null && currentVehicle == null && _isEnteringVehicle)
                {
                    _isEnteringVehicle = false;
                    PlayerAbortedEntering?.Invoke(this, _prevVehicle);
                } 
                else if (currentVehicle != null)
                {
                    _prevVehicle = currentVehicle;
                    _isEnteringVehicle = false;
                    _isInVehicle = true;
                    PlayerEntered?.Invoke(this, currentVehicle);
                }
            } 
            else if (_isInVehicle)
            {
                if (currentVehicle == null || Game.Player.IsDead)
                {
                    _isInVehicle = false;
                    PlayerLeft?.Invoke(this, _prevVehicle);
                } 
                else if (_prevVehicle != currentVehicle)
                {
                    PlayerLeft?.Invoke(this, _prevVehicle);
                    PlayerEntered?.Invoke(this, currentVehicle);
                    _prevVehicle = currentVehicle;
                }
            }
        }
    }
}