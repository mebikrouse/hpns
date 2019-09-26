using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Core.Managers;
using HPNS.Tasks;
using HPNS.Tasks.Core;
using HPNS.Tasks.Support;
using static CitizenFX.Core.Native.API;

namespace QuestTestClient
{
    public class QuestTest : BaseScript
    {
        public QuestTest()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
            
            RegisterCommand("quest1", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                var model = "toros";
                var vehicle = await World.CreateVehicle(model, Game.PlayerPed.Position + Game.PlayerPed.RightVector * 5.0f, Game.PlayerPed.Heading);

                Func<ITask> goToRadiusAreaTaskProvider = () => new GoToRadiusAreaTask(new Vector3(55.84977f, -1572.498f, 28.95687f), 25f);
                var stayInVehicleState = new StayInVehicleState(vehicle.Handle);
                
                var stateConjunction = new StateConjunction(goToRadiusAreaTaskProvider, stayInVehicleState);
                stateConjunction.TaskDidEnd += (sender, e) => Debug.WriteLine("Task did end");
                stateConjunction.Start();
            }), false);
            
            RegisterCommand("quest2", new Action<int, List<object>, string>((source, args, raw) =>
            {
                var task = new GoToRadiusAreaTask(new Vector3(55.84977f, -1572.498f, 28.95687f), 25f);
                task.TaskDidEnd += (sender, e) => Debug.WriteLine("Task did end");
                task.Start();
            }), false);

            //HPNS.Core.World.Current.AimingManager.PlayerDidStartAimingAtEntity += (sender, e) =>
            //    PrintToChat($"Started aiming at entity with handle {e}");
            //HPNS.Core.World.Current.AimingManager.PlayerDidStopAimingAtEntity += (sender, e) =>
            //    PrintToChat($"Stopped aiming at entity with handle {e}");
            
            RegisterCommand("quest3", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                var pedPosition = Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 5;
                var randomPedHash = await CreateRandomPed(pedPosition);
                
                var keepAimingState = new KeepAimingAtEntityState(randomPedHash);
                keepAimingState.StateDidRecover += (sender, e) => PrintToChat($"Started aiming at {randomPedHash}");
                keepAimingState.StateDidBreak += (sender, e) => PrintToChat($"Stopped aiming at {randomPedHash}");
                keepAimingState.Start();
            }), false);
        }

        private void PrintToChat(string message)
        {
            TriggerEvent("chat:addMessage", new 
            {
                color = new[] {255, 0, 0},
                args = new[] {"[QuestTest]", message}
            });
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
        
        private static uint GetRandomPedHash()
        {
            var pedHashes = Enum.GetValues(typeof(PedHash)).Cast<PedHash>().ToList();
            return (uint) pedHashes[new Random().Next(0, pedHashes.Count)];
        }
    }
}