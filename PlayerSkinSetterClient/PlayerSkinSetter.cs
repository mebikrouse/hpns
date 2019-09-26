using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;

using static CitizenFX.Core.Native.API;

namespace PlayerSkinSetterClient
{
    public class PlayerSkinSetter : BaseScript
    {
        public PlayerSkinSetter()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }
        
        private static void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
            
            RegisterCommand("setskin", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                if (args.Count != 1) 
                    PrintToChat("Usage: /setskin [model]");

                var modelHash = (uint) GetHashKey(args[0].ToString());

                if (!IsModelInCdimage(modelHash) || !IsModelAPed(modelHash)) 
                    PrintToChat($"{args[0]} is not a ped model!");
                
                await SetPlayerModel(modelHash);
            }), false);
        }

        private static async Task SetPlayerModel(uint modelHash)
        {
            RequestModel(modelHash);

            while (!HasModelLoaded(modelHash))
                await Delay(500);

            CitizenFX.Core.Native.API.SetPlayerModel(Game.Player.Handle, modelHash);
        }

        private static void PrintToChat(string message)
        {
            TriggerEvent("chat:addMessage", new 
            {
                color = new[] {255, 0, 0},
                args = new[] {"[SkinSetter]", message}
            });
        }
    }
}