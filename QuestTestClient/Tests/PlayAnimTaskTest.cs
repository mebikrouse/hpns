using System;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Interactivity.Core;
using HPNS.Interactivity.Tasks;
using static CitizenFX.Core.Native.API;

namespace QuestTestClient.Tests
{
    public class PlayAnimTaskTest : TaskBase
    {
        private const float PED_DISTANCE = 5f;

        private ITask _currentTask;
        
        protected override async void ExecuteStarting()
        {
            var animDict = "mp_am_hold_up";
            var animName = "holdup_victim_20s";
            var duration = 23000;
            
            var pedPosition = Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * PED_DISTANCE;
            var pedHandle = await CreateRandomPed(pedPosition, Game.PlayerPed.Heading - 180f);

            var playAnimTask = new PlayAnimTask(pedHandle, animDict, animName, duration);
            playAnimTask.TaskDidEnd += CurrentTaskTaskOnTaskDidEnd;
            playAnimTask.Start();
            
            _currentTask = playAnimTask;
        }

        protected override void ExecuteAborting()
        {
            _currentTask.TaskDidEnd -= CurrentTaskTaskOnTaskDidEnd;
            _currentTask.Abort();
        }

        private void CurrentTaskTaskOnTaskDidEnd(object sender, EventArgs e)
        {
            _currentTask.TaskDidEnd -= CurrentTaskTaskOnTaskDidEnd;
            NotifyTaskDidEnd();
        }
        
        private static async Task<int> CreateRandomPed(Vector3 position, float heading)
        {
            var pedHash = GetRandomPedHash();
            return await CreatePedAtPosition(position, heading, pedHash);
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
                await BaseScript.Delay(100);
            }
            
            var ped = CreatePed(0, pedHash, position.X, position.Y, position.Z, heading, true, false);
            return ped;
        }
    }
}