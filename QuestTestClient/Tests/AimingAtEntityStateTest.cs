using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Interactivity.Core;
using HPNS.Interactivity.States;
using HPNS.Interactivity.Support;
using HPNS.Interactivity.Tasks;
using static CitizenFX.Core.Native.API;

namespace QuestTestClient.Tests
{
    public class AimingAtEntityStateTest : TaskBase
    {
        private const float PED_DISTANCE = 5f;
        private const float AREA_RADIUS = 25f;

        private static readonly Vector3 AREA_CENTER = new Vector3(55.84977f, -1572.498f, 28.95687f);

        private ITask _currentTask;
        
        protected override async void ExecuteStarting()
        {
            var pedPosition = Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * PED_DISTANCE;
            var pedHandle = await CreateRandomPed(pedPosition, Game.PlayerPed.Heading - 180f);
                
            var aimingAtEntityState = new AimingAtEntityState(pedHandle);
            var stateWaitTask = new StateRecoverWaitTask(aimingAtEntityState);
            
            var goToRadiusAreaTask = new GoToRadiusAreaTask(AREA_CENTER, AREA_RADIUS);
            
            var sequentialSetTask = new SequentialSetTask(new List<ITask>() {stateWaitTask, goToRadiusAreaTask});
            sequentialSetTask.TaskDidEnd += CurrentTaskTaskDidEnd;
            sequentialSetTask.Start();

            _currentTask = sequentialSetTask;
        }

        protected override void ExecuteAborting()
        {
            _currentTask.TaskDidEnd -= CurrentTaskTaskDidEnd;
            _currentTask.Abort();
        }

        private void CurrentTaskTaskDidEnd(object sender, EventArgs e)
        {
            _currentTask.TaskDidEnd -= CurrentTaskTaskDidEnd;
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