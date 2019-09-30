using CitizenFX.Core;
using HPNS.Interactivity.Core;

using static CitizenFX.Core.Native.API;

using World = HPNS.Core.World;

namespace HPNS.Interactivity.States
{
    public class BeingInVehicleState : StateBase
    {
        private int _vehicleHandle;
        private int _blipHandle;

        public override bool IsValid => Game.PlayerPed.CurrentVehicle != null && 
                                        Game.PlayerPed.CurrentVehicle.Handle == _vehicleHandle;

        public BeingInVehicleState(int vehicleHandle)
        {
            _vehicleHandle = vehicleHandle;
        }

        protected override void ExecuteStarting()
        {
            World.Current.VehicleEventsManager.PlayerEntered += VehicleEventsManagerOnPlayerEntered;
            World.Current.VehicleEventsManager.PlayerLeft += VehicleEventsManagerOnPlayerLeft;
        }

        protected override void ExecuteStopping()
        {
            World.Current.VehicleEventsManager.PlayerEntered -= VehicleEventsManagerOnPlayerEntered;
            World.Current.VehicleEventsManager.PlayerLeft -= VehicleEventsManagerOnPlayerLeft;
        }

        private void VehicleEventsManagerOnPlayerEntered(object sender, Vehicle e)
        {
            if (e.Handle != _vehicleHandle) return;
            
            RemoveMarkerAndBlip();
            NotifyStateDidRecover();
        }

        private void VehicleEventsManagerOnPlayerLeft(object sender, Vehicle e)
        {
            if (e.Handle != _vehicleHandle) return;
            
            AddMarkerAndBlip();
            NotifyStateDidBreak();
        }

        private void AddMarkerAndBlip()
        {
            _blipHandle = AddBlipForEntity(_vehicleHandle);
            SetBlipSprite(_blipHandle, 143);
            SetBlipColour(_blipHandle, 3);
            SetBlipScale(_blipHandle, 0.75f);
        }

        private void RemoveMarkerAndBlip()
        {
            RemoveBlip(ref _blipHandle);
        }
    }
}