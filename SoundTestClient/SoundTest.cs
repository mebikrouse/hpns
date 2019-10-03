using System;
using System.Collections.Generic;
using CitizenFX.Core;

using static CitizenFX.Core.Native.API;

namespace SoundTestClient
{
    public class SoundTest : BaseScript
    {
        public SoundTest()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private static void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
            
            RegisterCommand("sound", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (args.Count != 1 && args.Count != 2) return; 
                
                //StopSound(64);

                var sound = args[0].ToString();
                var soundRef = args.Count == 2 ? args[1].ToString() : null;
                PlaySound(-1, sound, soundRef, false, 0, true);
            }), false);
            
            RegisterCommand("entsound", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (args.Count != 1 && args.Count != 2) return; 
                
                //StopSound(64);

                var sound = args[0].ToString();
                var soundRef = args.Count == 2 ? args[1].ToString() : null;
                PlaySoundFromEntity(-1, sound, Game.PlayerPed.Handle, soundRef, false, 0);
            }), false);
        }
    }
}