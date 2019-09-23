using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.Core.Tools;
using static CitizenFX.Core.Native.API;

namespace PedTest
{
    public class PedTest : BaseScript
    {
        private const int DEFAULT_REFRESH_RATE = 500;
        
        private List<int> _peds = new List<int>();
        private UpdateObjectPool _updateObjectPool;
        private VehicleEventsManager _vehicleEventsManager;

        private uint _friendlyFollowerGroupHash;
        private uint _playerGroupHash;
        
        public PedTest()
        {
            _vehicleEventsManager = new VehicleEventsManager();
            
            _updateObjectPool = new UpdateObjectPool(DEFAULT_REFRESH_RATE);
            _updateObjectPool.AddUpdateObject(_vehicleEventsManager);
            _updateObjectPool.Start();
            
            _vehicleEventsManager.PlayerEntered += VehicleEventsManagerOnPlayerEntered;
            _vehicleEventsManager.PlayerLeft += VehicleEventsManagerOnPlayerLeft;
            
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            EventHandlers["onClientResourceStop"] += new Action<string>(OnClientResourceStop);

            AddRelationshipGroup("FRIENDLY_FOLLOWER", ref _friendlyFollowerGroupHash);
            AddRelationshipGroup("PLAYER", ref _playerGroupHash);
            
            SetRelationshipBetweenGroups(0, _friendlyFollowerGroupHash, _playerGroupHash);
            SetRelationshipBetweenGroups(0, _playerGroupHash, _friendlyFollowerGroupHash);
            
            SetPedRelationshipGroupHash(Game.PlayerPed.Handle, _playerGroupHash);
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
            
            RegisterCommand("ped", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                var pedPosition = Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 5f;
                var ped = await CreateRandomPed(pedPosition);
                _peds.Add(ped);
                
                SetPedRelationshipGroupHash(ped, _friendlyFollowerGroupHash);
                
                FollowEntity(ped, Game.PlayerPed.Handle);

            }), false);
            
            RegisterCommand("remove", new Action<int, List<object>, string>((source, args, raw) =>
            {
                foreach (var ped in _peds)
                {
                    var pedHandle = ped;
                    DeletePed(ref pedHandle);
                }
                
                _peds.Clear();
            }), false);
        }

        private void OnClientResourceStop(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
            _updateObjectPool.Stop();
        }

        private static uint GetRandomPedHash()
        {
            var pedHashes = Enum.GetValues(typeof(PedHash)).Cast<PedHash>().ToList();
            return (uint) pedHashes[new Random().Next(0, pedHashes.Count)];
        }

        private static async Task<int> CreateRandomPed(Vector3 position)
        {
            var pedHash = GetRandomPedHash();

            while (!HasModelLoaded(pedHash))
            {
                RequestModel(pedHash);
                await Delay(500);
            }
            
            var ped = CreatePed(0, pedHash, position.X, position.Y, position.Z, 0f, true, false);
            return ped;
        }

        private void VehicleEventsManagerOnPlayerLeft(object sender, EventArgs e)
        {
            foreach (var ped in _peds)
            {
                if (IsPedInAnyVehicle(ped, true))
                {
                    TaskLeaveAnyVehicle(ped, 0, 0);
                }

                FollowEntity(ped, Game.PlayerPed.Handle);
            }
        }

        private void VehicleEventsManagerOnPlayerEntered(object sender, Vehicle e)
        {
            var freeSeats = GetFreeSeats(e);
            var random = new Random();
            foreach (var ped in _peds)
            {
                if (freeSeats.Count > 0)
                {
                    var seatIndex = random.Next(0, freeSeats.Count);
                    var seat = freeSeats[seatIndex];
                    freeSeats.RemoveAt(seatIndex);
                    TaskEnterVehicle(ped, e.Handle, -1, (int) seat, 10f, 1, 0);
                }
                else
                {
                    FollowEntity(ped, e.Handle);
                }
            }
        }

        private List<VehicleSeat> GetFreeSeats(Vehicle vehicle)
        {
            var seats = Enum.GetValues(typeof(VehicleSeat)).Cast<VehicleSeat>().Distinct().ToList();
            var freeSeats = new List<VehicleSeat>();
            foreach (var seat in seats)
                if (vehicle.IsSeatFree(seat) &&
                    seat != VehicleSeat.Any)
                    freeSeats.Add(seat);

            return freeSeats;
        }

        private void FollowEntity(int ped, int entity)
        {
            TaskFollowToOffsetOfEntity(ped, entity, 2.0f, 0.0f, 0.0f, 2.0f, -1, 10f, true);
        }
    }
}