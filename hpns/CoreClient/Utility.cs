using System;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace HPNS.CoreClient
{
    public static class Utility
    {
        private static Random Random = new Random();
        
        public static async Task LoadObject(uint modelHash)
        {
            RequestModel(modelHash);

            while (!HasModelLoaded(modelHash))
                await BaseScript.Delay(100);
        }
        
        public static async Task LoadAnimDict(string dict)
        {
            RequestAnimDict(dict);

            while (!HasAnimDictLoaded(dict))
                await BaseScript.Delay(100);
        }
        
        public static async Task<int> CreatePedAtPositionAsync(Vector3 position, float heading, uint pedHash, bool network = true)
        {
            await LoadObject(pedHash);
            return CreatePed(0, pedHash, position.X, position.Y, position.Z, heading, network, false);
        }
        
        public static int CreatePedAtPosition(Vector3 position, float heading, uint pedHash, bool network = true)
        {
            return CreatePed(0, pedHash, position.X, position.Y, position.Z, heading, network, false);
        }
        
        public static async Task<int> CreatePedAtPositionAsync(Vector3 position, float heading, string pedModel, bool network = true)
        {
            var pedHash = (uint) GetHashKey(pedModel);
            await LoadObject(pedHash);
            return CreatePed(0, pedHash, position.X, position.Y, position.Z, heading, network, false);
        }
        
        public static int CreatePedAtPosition(Vector3 position, float heading, string pedModel, bool network = true)
        {
            var pedHash = (uint) GetHashKey(pedModel);
            return CreatePed(0, pedHash, position.X, position.Y, position.Z, heading, network, false);
        }
        
        public static async Task<int> CreateRandomPedAsync(Vector3 position, float heading, bool network = true)
        {
            var pedHash = GetRandomPedHash();
            return await CreatePedAtPositionAsync(position, heading, pedHash, network);
        }

        public static void RemovePed(int pedHandle)
        {
            DeletePed(ref pedHandle);
        }
        
        public static uint GetRandomPedHash()
        {
            var pedHashes = Enum.GetValues(typeof(PedHash)).Cast<PedHash>().ToList();
            return (uint) pedHashes[Random.Next(0, pedHashes.Count)];
        }
    }
}