using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Core;
using HPNS.Core.Managers;
using static CitizenFX.Core.Native.API;

namespace PedTest
{
    public class PedTest : BaseScript
    {
        private bool _isPedCreated;
        private int _currentPed;

        private string _faceAnimDict;
        private string _faceAnimName;
        
        public PedTest()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
            
            RegisterCommand("ped", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                if (_isPedCreated)
                    DeletePed(ref _currentPed);
                
                var pedPosition = Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 5f;
                var ped = await Utility.CreatePedAtPositionAsync(pedPosition, Game.PlayerPed.Heading - 180f, (uint) GetHashKey("a_m_m_ktown_01"));

                TaskSetBlockingOfNonTemporaryEvents(ped, true);

                _isPedCreated = true;
                _currentPed = ped;
            }), false);
            
            RegisterCommand("remove", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (!_isPedCreated) return;
                
                DeletePed(ref _currentPed);
            }), false);
            
            RegisterCommand("anim", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                if (args.Count != 2)
                {
                    PrintToChat("Usage: /anim [dict] [name]");
                    return;
                }

                if (!_isPedCreated)
                {
                    PrintToChat("You need to spawn ped at first!");
                    return;
                }
                
                var dict = args[0].ToString();
                var anim = args[1].ToString();

                await PlayAnim(_currentPed, dict, anim);
                
            }), false);
            
            RegisterCommand("face", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                if (args.Count != 2)
                {
                    StopAnimTask(_currentPed, _faceAnimDict, _faceAnimName, 3.0f);
                    return;
                }

                if (!_isPedCreated)
                {
                    PrintToChat("You need to spawn ped at first!");
                    return;
                }
                
                var dict = args[0].ToString();
                var anim = args[1].ToString();

                _faceAnimDict = dict;
                _faceAnimName = anim;

                await PlayFacialAnim(_currentPed, dict, anim);
            }), false);
        }

        private static async Task PlayAnim(int pedHandle, string dict, string name)
        {
            await Utility.LoadAnimDict(dict);

            PrintToChat("Stating to play animation...");
            TaskPlayAnim(pedHandle, dict, name, 8.0f, 8.0f, -1, 0, 0.0f, false, false, false);
        }

        private static async Task PlayFacialAnim(int pedHandle, string dict, string name)
        {
            await Utility.LoadAnimDict(dict);
            
            PrintToChat("Stating to play facial animation...");
            TaskPlayAnim(pedHandle, dict, name, 8.0f, 8.0f, -1, 33, 0.0f, false, false, false);
        }
        
        private static void PrintToChat(string message)
        {
            TriggerEvent("chat:addMessage", new 
            {
                color = new[] {255, 0, 0},
                args = new[] {"[PedTest]", message}
            });
        }
    }
}