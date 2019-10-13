using System;
using System.Collections.Generic;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace CarSpawnerClient
{
    public class CarSpawner : BaseScript
    {
        private int _prevVehicleHandle;
        
        public CarSpawner()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
            
            RegisterCommand("spawncar", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                if (args.Count != 1)
                {
                    PrintToChat("Usage: /spawncar [model]");
                    return;
                }

                var model = args[0].ToString();
            
                var hash = (uint) GetHashKey(model);
                if (!IsModelInCdimage(hash) || !IsModelAVehicle(hash))
                {
                    PrintToChat($"Unable to spawn car with model {model}.");
                    return;
                }
            
                var vehicle = await World.CreateVehicle(model, Game.PlayerPed.Position, Game.PlayerPed.Heading);
                Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);

                SetModelAsNoLongerNeeded(hash);
                if (_prevVehicleHandle != 0)
                    SetEntityAsNoLongerNeeded(ref _prevVehicleHandle);

                _prevVehicleHandle = vehicle.Handle;

                PrintToChat($"Vehicle {model} has been spawned.");
            }), false);
            
            RegisterCommand("fix", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (args.Count != 0)
                {
                    PrintToChat("Usage: /fix");
                    return;
                }
                
                var playerVehicle = Game.PlayerPed.CurrentVehicle;
                if (playerVehicle == null)
                {
                    PrintToChat("You have to be in a vehicle to fix it.");
                    return;
                }
                
                playerVehicle.Repair();

                PrintToChat("Your car has been fixed.");
            }), false);
        }

        private static void PrintToChat(string message)
        {
            TriggerEvent("chat:addMessage", new 
            {
                color = new[] {255, 0, 0},
                args = new[] {"[CarSpawner]", message}
            });
        }
    }
}