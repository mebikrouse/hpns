using System;
using System.Collections.Generic;
using CitizenFX.Core;
using HPNS.Core;
using Hubs;
using Hubs.Data;
using static CitizenFX.Core.Native.API;

namespace HubsTest
{
    public class Bootstrap : BaseScript
    {
        public Bootstrap()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;

            var configuration = new HubConfiguration
            {
                HubCenter = new Vector3(2049.88f, 3183.32f, 45.16f),
                HubRadius = 200f,
                SpawnConfigurations = new List<SpawnConfiguration>
                {
                    new SpawnConfiguration
                    {
                        Position = new Vector3(2043f, 2198f, 45f),
                        Heading = 234f,
                        PedConfiguration = new PedConfiguration
                        {
                            Model = "a_m_o_acult_01"
                        }
                    },
                    new SpawnConfiguration
                    {
                        Position = new Vector3(2057.64f, 3195.24f, 45.19f),
                        Heading = 114f,
                        PedConfiguration = new PedConfiguration
                        {
                            Model = "a_m_o_acult_02"
                        }
                    },
                    new SpawnConfiguration
                    {
                        Position = new Vector3(2060.47f, 3174.21f, 45.17f),
                        Heading = 57f,
                        PedConfiguration = new PedConfiguration
                        {
                            Model = "a_m_y_acult_01"
                        }
                    },
                    new SpawnConfiguration
                    {
                        Position = new Vector3(2053.91f, 3175.48f, 45.17f),
                        Heading = 356.26f,
                        PedConfiguration = new PedConfiguration
                        {
                            Model = "a_m_y_methhead_01"
                        }
                    },
                    new SpawnConfiguration
                    {
                        Position = new Vector3(2039.88f, 3183.31f, 45.22f),
                        Heading = 253.27f,
                        PedConfiguration = new PedConfiguration
                        {
                            Model = "a_f_m_fatcult_01"
                        }
                    }
                }
            };

            var updateObjectPool = new UpdateObjectPool(1000);
            updateObjectPool.Start();

            var hub = new Hub(configuration);
            updateObjectPool.AddUpdateObject(hub);
            
            RegisterCommand("location", new Action<int, List<object>, string>((source, args, raw) =>
            {
                var playerPosition = Game.PlayerPed.Position;
                Debug.WriteLine($"Position: {playerPosition.X}f, {playerPosition.Y}f, {playerPosition.Z}f");
                
                var playerHeading = Game.PlayerPed.Heading;
                Debug.WriteLine($"Heading: {playerHeading}f");
            }), false);
        }
    }
}