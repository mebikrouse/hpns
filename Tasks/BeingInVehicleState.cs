using System;
using CitizenFX.Core;
using HPNS.Tasks.Core;

using World = HPNS.Core.World;

using static CitizenFX.Core.Native.API;

namespace HPNS.Tasks
{
    public class BeingInVehicleState : IState
    {
        private int _vehicleHandle;
        
        private int _blipHandle;

        public StateState CurrentState { get; private set; } = StateState.Waiting;

        public bool IsValid => Game.PlayerPed.CurrentVehicle != null &&
                               Game.PlayerPed.CurrentVehicle.Handle == _vehicleHandle;

        public event EventHandler StateDidBreak;
        public event EventHandler StateDidRecover;

        public BeingInVehicleState(int vehicleHandle)
        {
            _vehicleHandle = vehicleHandle;
        }

        public void Start()
        {
            if (CurrentState != StateState.Waiting)
                throw new Exception("Cannot start state that is not in Waiting state.");
            
            World.Current.VehicleEventsManager.PlayerEntered += VehicleEventsManagerOnPlayerEntered;
            World.Current.VehicleEventsManager.PlayerLeft += VehicleEventsManagerOnPlayerLeft;
            
            if (!IsValid) AddMarkerAndBlip();

            CurrentState = StateState.Running;
        }

        public void Stop()
        {
            if (CurrentState != StateState.Running)
                throw new Exception("Cannot stop state that is not in Running state.");
            
            World.Current.VehicleEventsManager.PlayerEntered -= VehicleEventsManagerOnPlayerEntered;
            World.Current.VehicleEventsManager.PlayerLeft -= VehicleEventsManagerOnPlayerLeft;
            
            if (!IsValid) RemoveMarkerAndBlip();

            CurrentState = StateState.Waiting;
        }

        private void VehicleEventsManagerOnPlayerEntered(object sender, Vehicle e)
        {
            if (e.Handle != _vehicleHandle) return;
            
            RemoveMarkerAndBlip();
            StateDidRecover?.Invoke(this, EventArgs.Empty);
        }

        private void VehicleEventsManagerOnPlayerLeft(object sender, Vehicle e)
        {
            if (e.Handle != _vehicleHandle) return;
            
            AddMarkerAndBlip();
            StateDidBreak?.Invoke(this, EventArgs.Empty);
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