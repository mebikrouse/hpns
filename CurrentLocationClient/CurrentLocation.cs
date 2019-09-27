using System;
using System.Collections.Generic;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace CurrentLocationClient
{
    public class CurrentLocation : BaseScript
    {
        public CurrentLocation()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private static void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
            
            RegisterCommand("pos", new Action<int, List<object>, string>((source, args, raw) =>
            {
                var playerPedPosition = Game.PlayerPed.Position;
                Debug.WriteLine($"Current position: {playerPedPosition.X}f, {playerPedPosition.Y}f, {playerPedPosition.Z}f");
                var playerPedRotation = Game.PlayerPed.Rotation;
                Debug.WriteLine($"Current rotation: {playerPedRotation.X}f, {playerPedRotation.Y}f, {playerPedRotation.Z}f");
                var playerHeading = Game.PlayerPed.Heading;
                Debug.WriteLine($"Current heading: {playerHeading}f");
            }), false);
        }
    }
}