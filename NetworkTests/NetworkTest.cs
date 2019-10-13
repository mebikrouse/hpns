using System;
using System.Collections.Generic;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.CoreClient;
using static CitizenFX.Core.Native.API;

namespace NetworkTests
{
    public class NetworkTest : BaseScript
    {
        private class PedChecker : IUpdateObject
        {
            private int _pedHandle;
            private int _pedNetId;
            private bool _pedExists;
            private bool _pedAlive;
            
            public int PedHandle => _pedHandle;

            public PedChecker(int pedHandle)
            {
                _pedHandle = pedHandle;
                _pedNetId = NetworkGetNetworkIdFromEntity(pedHandle);
                _pedExists = true;
                _pedAlive = true;
            }
            
            public void Update(float deltaTime)
            {
                var pedExists = DoesEntityExist(_pedHandle);
                if (_pedExists != pedExists)
                {
                    _pedExists = pedExists;
                    PrintToChat($"Ped with handle {_pedHandle} changed existance to {pedExists}.");
                }

                var pedAlive = !IsEntityDead(_pedHandle);
                if (_pedAlive != pedAlive)
                {
                    _pedAlive = pedAlive;
                    PrintToChat($"Ped with handle {_pedHandle} changed alive to {pedAlive}.");
                }
            }

            public override string ToString()
            {
                return $"[Handle: {_pedHandle}; Network ID: {_pedNetId}; Exists: {_pedExists}]";
            }
        }
        
        private UpdateObjectPool _updateObjectPool = new UpdateObjectPool(50);
        private List<PedChecker> _pedCheckers = new List<PedChecker>();

        public NetworkTest()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;

            _updateObjectPool.Start();
            
            RegisterCommand("spawnped", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                if (args.Count != 0)
                {
                    PrintToChat("Usage: /spawnped");
                    return;
                }
                
                var pedPosition = Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 3f;
                var pedHeading = Game.PlayerPed.Heading + 180f;

                var pedHandle = await Utility.CreateRandomPedAsync(pedPosition, pedHeading);
                //SetEntityAsMissionEntity(pedHandle, false, false);

                var pedChecker = new PedChecker(pedHandle);
                _pedCheckers.Add(pedChecker);
                _updateObjectPool.AddUpdateObject(pedChecker);
                
                PrintToChat($"Spawned ped with handle of {pedHandle}. PedChecker - {pedChecker}.");
                
            }), false);
            
            RegisterCommand("peds", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (args.Count != 0)
                {
                    PrintToChat("Usage: /peds");
                    return;
                }
                
                if (_pedCheckers.Count == 0)
                {
                    PrintToChat("There are no spawned peds.");
                    return;
                }

                var i = 0;
                foreach (var pedChecker in _pedCheckers)
                    PrintToChat($"{i} - {pedChecker}");
                
            }), false);
            
            RegisterCommand("removeped", new Action<int, List<object>, string>((source, args, raw) =>
            {
                var pedIndex = 0;
                if (args.Count != 1 ||
                    !int.TryParse(args[0].ToString(), out pedIndex))
                {
                    PrintToChat("Usage: /removeped [ped_number]");
                    return;
                }

                if (pedIndex < 0 || pedIndex >= _pedCheckers.Count)
                {
                    PrintToChat($"Unable to find ped with number of {pedIndex}. Try /peds to see all peds.");
                    return;
                }

                var pedChecker = _pedCheckers[pedIndex];
                _pedCheckers.RemoveAt(pedIndex);
                _updateObjectPool.RemoveUpdateObject(pedChecker);

                var pedHandle = pedChecker.PedHandle;
                DeleteEntity(ref pedHandle);
                
                PrintToChat($"Removed ped with handle of {pedHandle}. PedChecker - {pedChecker}.");

            }), false);
            
            RegisterCommand("owner", new Action<int, List<object>, string>((source, args, raw) =>
            {
                var pedNetId = 0;
                if (args.Count != 1 ||
                    !int.TryParse(args[0].ToString(), out pedNetId))
                {
                    PrintToChat("Usage: /owner [net_id]");
                    return;
                }

                var pedHandle = NetworkGetEntityFromNetworkId(pedNetId);
                var owner = NetworkGetEntityOwner(pedHandle);
                
                PrintToChat($"Owner of ped with Net ID of {pedNetId} is {owner}.");

            }), false);
            
            RegisterCommand("handle", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (args.Count != 0)
                {
                    PrintToChat("Usage: /handle");
                    return;
                }

                var playerHandle = Game.Player.Handle;
                PrintToChat($"Your handle is {playerHandle}");

                var pedHandle = Game.PlayerPed.Handle;
                PrintToChat($"Your ped handle is {pedHandle}");

                var pedNetworkId = Game.PlayerPed.NetworkId;
                PrintToChat($"Your ped network id is {pedNetworkId}");

            }), false);
            
            RegisterCommand("id", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (args.Count != 0)
                {
                    PrintToChat("Usage: /id");
                    return;
                }

                var playerId = GetPlayerServerId(Game.Player.Handle);
                PrintToChat($"Your server id is {playerId}");
                
            }), false);
            
            RegisterCommand("anime", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                var handle = 0;
                if (args.Count != 1 ||
                    !int.TryParse(args[0].ToString(), out handle))
                {
                    PrintToChat("Usage: /anime [handle]");
                    return;
                }

                await Utility.LoadAnimDict("amb@world_human_cheering@male_a");

                TaskPlayAnim(handle, "amb@world_human_cheering@male_a", "base", 
                    8f, 8f, -1, 0, 0, 
                    false, false, false);

            }), false);
            
            RegisterCommand("hfn", new Action<int, List<object>, string>((source, args, raw) =>
            {
                var networkId = 0;
                if (args.Count != 1 ||
                    !int.TryParse(args[0].ToString(), out networkId))
                {
                    PrintToChat("Usage: /hfn [network_id]");
                    return;
                }

                var handle = NetworkGetEntityFromNetworkId(networkId);
                PrintToChat($"Handle of entity with Network ID of {networkId} is {handle}.");
                
            }), false);
        }

        private static void PrintToChat(string message)
        {
            TriggerEvent("chat:addMessage", new 
            {
                color = new[] {255, 0, 0},
                args = new[] {"[NetworkTests]", message}
            });
        }
    }
}