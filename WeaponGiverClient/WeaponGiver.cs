using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;

using static CitizenFX.Core.Native.API;

namespace WeaponGiverClient
{
    public class WeaponGiver : BaseScript
    {
        public WeaponGiver()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private static void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
            
            RegisterCommand("weapons", new Action<int, List<object>, string>((source, args, raw) =>
            {
                var playerPedHandle = Game.PlayerPed.Handle;

                var weaponHashes = Enum.GetValues(typeof(WeaponHash)).Cast<uint>().ToList();
                foreach (var weaponHash in weaponHashes)
                    GiveWeaponToPed(playerPedHandle, weaponHash, 9999, false, false);
            }), false);
        }
    }
}