using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
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
                var stayInVehicleState = new BeingInVehicleState(vehicle.Handle);
                
                var stateConjunction = new StateSuspendTask(goToRadiusAreaTaskProvider, stayInVehicleState);
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
                
                var keepAimingState = new AimingAtEntityState(randomPedHash);
                var stateWait = new StateWaitTask(keepAimingState);
                var goToRadiusAreaTask = new GoToRadiusAreaTask(new Vector3(55.84977f, -1572.498f, 28.95687f), 25f);
                var taskSequence = new SequenceTask(new List<ITask>() {stateWait, goToRadiusAreaTask});
                taskSequence.TaskDidEnd += (sender, e) => PrintToChat("Task did end");
                taskSequence.Start();
            }), false);
            
            RegisterCommand("quest4", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                //var pedPosition = new Vector3(1166.155f, 2710.851f, 38.15769f);
                //var heading = 180.775f;

                var pedPosition = Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 3;
                var heading = Game.PlayerPed.Heading - 180f;
                
                var pedHandle = await CreatePedAtPosition(pedPosition, heading, (uint) GetHashKey("a_m_m_ktown_01"));
                SetBlockingOfNonTemporaryEvents(pedHandle, true);
                PlaceObjectOnGroundProperly(pedHandle);
                
                var dict = "mp_am_hold_up";
                var anim = "holdup_victim_20s";
                await LoadAnimDict(dict);

                var propModelHash = (uint) GetHashKey("prop_poly_bag_01");
                await LoadObject(propModelHash);

                var propHandle = 0;
                
                var tasks = new List<ITask>();
                tasks.Add(new StateWaitTask(new AimingAtEntityState(pedHandle)));
                tasks.Add(new LambdaTask(() => PlayAnim(pedHandle, dict, anim)));
                tasks.Add(new WaitTask(11250));
                tasks.Add(new LambdaTask(() =>
                {
                    propHandle = CreateObject((int) propModelHash, 0f, 0f, 0f, true, true, true);
                    
                    var boneIndex = GetPedBoneIndex(pedHandle, 4138);
                    AttachEntityToEntity(propHandle, pedHandle, boneIndex, 
                        -0.09999999f, -0.04f, -0.13f, 0, 0, 0, true, 
                        false, false, false, 0, true);
                }));
                tasks.Add(new WaitTask(9750));
                tasks.Add(new LambdaTask(() =>
                {
                    DetachEntity(propHandle, true, true);
                }));
                tasks.Add(new WaitTask(2000));
                tasks.Add(new LambdaTask(() =>
                {
                    SetBlockingOfNonTemporaryEvents(pedHandle, false);
                    TaskSmartFleePed(pedHandle, Game.PlayerPed.Handle, 50f, -1, true, true);
                }));

                var taskSequence = new SequenceTask(tasks);
                taskSequence.TaskDidEnd += (sender, e) => PrintToChat("Task did end");
                taskSequence.Start();
            }), false);
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