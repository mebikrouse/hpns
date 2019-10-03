using System;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;

using static CitizenFX.Core.Native.API;

namespace HPNS.Core
{
    public static class Utility
    {
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
        
        public static async Task<int> CreatePedAtPosition(Vector3 position, float heading, uint pedHash)
        {
            await LoadObject(pedHash);
            
            var ped = CreatePed(0, pedHash, position.X, position.Y, position.Z, heading, true, false);
            return ped;
        }
        
        public static async Task<int> CreateRandomPed(Vector3 position, float heading)
        {
            var pedHash = GetRandomPedHash();
            return await CreatePedAtPosition(position, heading, pedHash);
        }
        
        public static uint GetRandomPedHash()
        {
            var pedHashes = Enum.GetValues(typeof(PedHash)).Cast<PedHash>().ToList();
            return (uint) pedHashes[new Random().Next(0, pedHashes.Count)];
        }
    }
}