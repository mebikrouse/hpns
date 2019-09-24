using CitizenFX.Core;
using HPNS.Tasks.Core;

using World = HPNS.Core.World;

using static CitizenFX.Core.Native.API;

namespace HPNS.Tasks
{
    public class StayInVehicleState : IState
    {
        private int _vehicleHandle;

        private int _blipHandle;
        
        public IStateDelegate Delegate { get; set; }

        public bool IsValid => Game.PlayerPed.CurrentVehicle != null &&
                               Game.PlayerPed.CurrentVehicle.Handle == _vehicleHandle;

        public StayInVehicleState(int vehicleHandle)
        {
            _vehicleHandle = vehicleHandle;
        }

        public void Start()
        {
            World.Current.VehicleEventsManager.PlayerEntered += VehicleEventsManagerOnPlayerEntered;
            World.Current.VehicleEventsManager.PlayerLeft += VehicleEventsManagerOnPlayerLeft;
            
            if (!IsValid) AddMarkerAndBlip();
        }

        public void Stop()
        {
            World.Current.VehicleEventsManager.PlayerEntered -= VehicleEventsManagerOnPlayerEntered;
            World.Current.VehicleEventsManager.PlayerLeft -= VehicleEventsManagerOnPlayerLeft;
            
            if (!IsValid) RemoveMarkerAndBlip();
        }

        private void VehicleEventsManagerOnPlayerEntered(object sender, Vehicle e)
        {
            if (e.Handle != _vehicleHandle) return;
            
            RemoveMarkerAndBlip();
            Delegate?.StateDidRecover(this);
        }

        private void VehicleEventsManagerOnPlayerLeft(object sender, Vehicle e)
        {
            if (e.Handle != _vehicleHandle) return;
            
            AddMarkerAndBlip();
            Delegate?.StateDidBreak(this);
        }

        private void AddMarkerAndBlip()
        {
            _blipHandle = AddBlipForEntity(_vehicleHandle);
            SetBlipSprite(_blipHandle, 146);
            SetBlipColour(_blipHandle, 5);
        }

        private void RemoveMarkerAndBlip()
        {
            RemoveBlip(ref _blipHandle);
        }
    }
}