using System;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.Interactivity.Core;
using HPNS.Interactivity.States;
using HPNS.Interactivity.Support;
using static CitizenFX.Core.Native.API;

namespace QuestTestClient.Tests
{
    public class ParallelSetTaskTest : TaskBase
    {
        private const float PED_DISTANCE = 3f;
        
        private ITask _currentTask;
        
        protected override async void ExecuteStarting()
        {
            var pedsHeading = Game.PlayerPed.Heading - 180f;

            var playerPosition = Game.PlayerPed.Position;
            var forwardVector = Game.PlayerPed.ForwardVector;
            var lineDirection = Game.PlayerPed.RightVector;
            
            var pedAPosition = playerPosition + (-lineDirection + forwardVector) * PED_DISTANCE;
            var pedATask = CreateRandomPed(pedAPosition, pedsHeading);

            var pedBPosition = playerPosition + forwardVector * PED_DISTANCE;
            var pedBTask = CreateRandomPed(pedBPosition, pedsHeading);

            var pedCPosition = playerPosition + (lineDirection + forwardVector) * PED_DISTANCE;
            var pedCTask = CreateRandomPed(pedCPosition, pedsHeading);

            var pedAHandle = await pedATask;
            var pedBHandle = await pedBTask;
            var pedCHandle = await pedCTask;
            
            var aimingAState = new AimingAtEntityState(pedAHandle);
            var stateWaitATask = new StateRecoverWaitTask(aimingAState);
            
            var aimingBState = new AimingAtEntityState(pedBHandle);
            var stateWaitBTask = new StateRecoverWaitTask(aimingBState);
            
            var aimingCState = new AimingAtEntityState(pedCHandle);
            var stateWaitCTask = new StateRecoverWaitTask(aimingCState);
            
            var parallelSetTask = new ParallelSetTask(new [] {stateWaitATask, stateWaitBTask, stateWaitCTask});
            parallelSetTask.TaskDidEnd += CurrentTaskOnTaskDidEnd;
            parallelSetTask.Start();

            _currentTask = parallelSetTask;
        }

        protected override void ExecuteAborting()
        {
            _currentTask.TaskDidEnd -= CurrentTaskOnTaskDidEnd;
            _currentTask.Abort();
        }

        private void CurrentTaskOnTaskDidEnd(object sender, EventArgs e)
        {
            _currentTask.TaskDidEnd -= CurrentTaskOnTaskDidEnd;
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