using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Interactivity.Core;
using HPNS.Interactivity.Scenarios;
using HPNS.Interactivity.States;
using HPNS.Interactivity.Support;
using HPNS.Interactivity.Tasks;
using static CitizenFX.Core.Native.API;

namespace QuestTestClient
{
    public class QuestTest : BaseScript
    {
        private ITask _currentTask;
        
        public QuestTest()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
            
            RegisterCommand("quest1", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                _currentTask?.Abort();
                
                var model = "toros";
                var vehicle = await World.CreateVehicle(model, Game.PlayerPed.Position + Game.PlayerPed.RightVector * 5.0f, Game.PlayerPed.Heading);

                Func<ITask> goToRadiusAreaTaskProvider = () => new GoToRadiusAreaTask(new Vector3(55.84977f, -1572.498f, 28.95687f), 25f);
                var stayInVehicleState = new BeingInVehicleState(vehicle.Handle);
                
                var stateConjunction = new StateSuspendTask(goToRadiusAreaTaskProvider, stayInVehicleState);
                stateConjunction.TaskDidEnd += CurrentTaskTaskDidEnd;
                stateConjunction.Start();

                _currentTask = stateConjunction;
            }), false);
            
            RegisterCommand("quest2", new Action<int, List<object>, string>((source, args, raw) =>
            {
                _currentTask?.Abort();
                
                var task = new GoToRadiusAreaTask(new Vector3(55.84977f, -1572.498f, 28.95687f), 25f);
                task.TaskDidEnd += CurrentTaskTaskDidEnd;
                task.Start();

                _currentTask = task;
            }), false);
            
            RegisterCommand("quest3", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                _currentTask?.Abort();
                
                var pedPosition = Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 5;
                var randomPedHash = await CreateRandomPed(pedPosition);
                
                var keepAimingState = new AimingAtEntityState(randomPedHash);
                var stateWait = new StateWaitTask(keepAimingState);
                var goToRadiusAreaTask = new GoToRadiusAreaTask(new Vector3(55.84977f, -1572.498f, 28.95687f), 25f);
                var taskSequence = new SequentialSetTask(new List<ITask>() {stateWait, goToRadiusAreaTask});
                taskSequence.TaskDidEnd += CurrentTaskTaskDidEnd;
                taskSequence.Start();

                _currentTask = taskSequence;
            }), false);
            
            RegisterCommand("quest4", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                _currentTask?.Abort();
                
                var pedPosition = Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 3;
                var heading = Game.PlayerPed.Heading - 180f;
                
                var pedHandle = await CreatePedAtPosition(pedPosition, heading, (uint) GetHashKey("a_m_m_ktown_01"));
                SetBlockingOfNonTemporaryEvents(pedHandle, true);
                PlaceObjectOnGroundProperly(pedHandle);

                var tasks = new List<ITask>();
                
                tasks.Add(new StateWaitTask(new AimingAtEntityState(pedHandle)));
                tasks.Add(new ShopRobberyScenario(pedHandle));
                tasks.Add(new LambdaTask(() =>
                {
                    SetBlockingOfNonTemporaryEvents(pedHandle, false);
                    TaskSmartFleePed(pedHandle, Game.PlayerPed.Handle, 50f, -1, true, true);
                }));

                var taskSequence = new SequentialSetTask(tasks);
                taskSequence.TaskDidEnd += CurrentTaskTaskDidEnd;
                taskSequence.Start();

                _currentTask = taskSequence;
            }), false);
            
            RegisterCommand("abort", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (_currentTask == null) return;

                _currentTask.Abort();

                _currentTask.TaskDidEnd -= CurrentTaskTaskDidEnd;
                _currentTask = null;
            }), false);
        }

        private void CurrentTaskTaskDidEnd(object sender, EventArgs e)
        {
            PrintToChat("Task did end");
            
            _currentTask.TaskDidEnd -= CurrentTaskTaskDidEnd;
            _currentTask = null;
        }

        private static async Task<int> CreateRandomPed(Vector3 position)
        {
            var pedHash = GetRandomPedHash();
            return await CreatePedAtPosition(position, 0f, pedHash);
        }
        
        private static uint GetRandomPedHash()
        {
            var pedHashes = Enum.GetValues(typeof(PedHash)).Cast<PedHash>().ToList();
            return (uint) pedHashes[new Random().Next(0, pedHashes.Count)];
        }
        
        private static async Task<int> CreatePedAtPosition(Vector3 position, float heading, uint pedHash)
        {
            while (!HasModelLoaded(pedHash))
            {
                RequestModel(pedHash);
                await Delay(500);
            }
            
            var ped = CreatePed(0, pedHash, position.X, position.Y, position.Z, heading, true, false);
            return ped;
        }

        private static void PlayAnim(int pedHandle, string dict, string name)
        {
            TaskPlayAnim(pedHandle, dict, name, 8.0f, 8.0f, -1, 0, 0.0f, false, false, false);
        }

        private static async Task LoadAnimDict(string dict)
        {
            RequestAnimDict(dict);

            while (!HasAnimDictLoaded(dict))
                await Delay(500);
        }

        private static async Task LoadObject(uint modelHash)
        {
            RequestModel(modelHash);

            while (!HasModelLoaded(modelHash))
                await Delay(500);
        }

        private void PrintToChat(string message)
        {
            TriggerEvent("chat:addMessage", new 
            {
                color = new[] {255, 0, 0},
                args = new[] {"[QuestTest]", message}
            });
        }
    }
}