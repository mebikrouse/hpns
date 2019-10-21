using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.CoreClient;
using HPNS.Interactivity.Activities;
using HPNS.Interactivity.Core.Data;
using HPNS.Interactivity.Core.Task;
using HPNS.Interactivity.Support;
using HPNS.Interactivity.Tasks;
using InteractivityTests.Core;

namespace InteractivityTests.Tests
{
    public class FocusFromBehindActivityTest : TaskBase, ITest
    {
        /*
        [  12889969] Camera offset: -0.0009017102f, 0.6136478f, 0.03338432f
        [  12889984] Camera rotation: -0.420009f, 0.0001329383f, -180.1577f
        [  12889984] Camera fov: 43.66001f
        
        [  12919016] Camera offset: 0.3481081f, 0.6787802f, -0.147724f
        [  12919016] Camera rotation: 11.97f, 0.0001331912f, 151.7988f
        [  12919031] Camera fov: 41.72006f
        
        [  12992203] Camera offset: -0.5304984f, 0.7575822f, 0.1575111f
        [  12992219] Camera rotation: -9.449986f, 0.0001370825f, -143.8104f
        [  12992234] Camera fov: 29.69007f
        
        [  13032469] Camera offset: -0.4194367f, 0.5977129f, -0.1750073f
        [  13032469] Camera rotation: 12.54f, 0.0001261951f, -143.4225f
        [  13032484] Camera fov: 36.68006f
        
        [  13074266] Camera offset: 0.557281f, 0.8663054f, -0.4690321f
        [  13074281] Camera rotation: 22.86f, 0.000126269f, 145.7156f
        [  13074281] Camera fov: 27.97008f
        */
        
        private List<PedCameraActivity.Configuration> _configurations = new List<PedCameraActivity.Configuration>
        {
            new PedCameraActivity.Configuration(
                new Vector3(-0.0009017102f, 0.8136478f, 0.03338432f), 
                new Vector3(-0.420009f, 0.0001329383f, -180.1577f), 
                30f),
            new PedCameraActivity.Configuration(
                new Vector3(0.3481081f, 0.6787802f, -0.147724f), 
                new Vector3(11.97f, 0.0001331912f, 151.7988f), 
                30f),
            new PedCameraActivity.Configuration(
                new Vector3(-0.5304984f, 0.7575822f, 0.1575111f), 
                new Vector3(-9.449986f, 0.0001370825f, -143.8104f), 
                30f),
            new PedCameraActivity.Configuration(
                new Vector3(-0.4194367f, 0.5977129f, -0.1750073f), 
                new Vector3(12.54f, 0.0001261951f, -143.4225f), 
                30f),
            new PedCameraActivity.Configuration(
                new Vector3(0.557281f, 0.8663054f, -0.4690321f), 
                new Vector3(22.86f, 0.000126269f, 145.7156f), 
                30f)
        };
        
        private ITask _testSequence;

        public string TestName => nameof(FocusFromBehindActivityTest);
        
        public FocusFromBehindActivityTest() : base(nameof(FocusFromBehindActivityTest)) { }

        protected override async Task ExecutePrepare()
        {
            var playerPedHandle = new Parameter<int>(Game.PlayerPed.Handle);
            
            var pedPosition = new Parameter<Vector3>();
            var pedHeading = new Parameter<float>();
            var createdPedHandle = new ResultCapturer<int>();
            
            var tasks = new List<ITask>();
            tasks.Add(new LambdaTask(() =>
            {
                pedPosition.SetValue(Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 1.5f);
                pedHeading.SetValue(Game.PlayerPed.Heading + 180f);
            }));
            tasks.Add(new CreatePedTask(Utility.GetRandomPedHash())
            {
                Position = pedPosition,
                Heading = pedHeading,
                PedHandle = createdPedHandle
            });
            tasks.Add(new WaitTask {Duration = new Parameter<int>(2000)});

            var chattingTasks = new List<ITask>();
            for (var i = 0; i < 10; i++)
            {
                var targetPed = (i % 2 == 0) ? (IParameter<int>) playerPedHandle : createdPedHandle;
                chattingTasks.Add(new ParallelActivityTask(
                    new WaitTask {Duration = new Parameter<int>(4000)},
                    new PedCameraActivity(GetRandomCameraConfiguration()) {PedHandle = targetPed},
                    new PlayFacialAnimActivity("mp_facial", "mic_chatter") {PedHandle = targetPed}
                ));
            }

            tasks.Add(new ParallelActivityTask(
                new SequenceTask(chattingTasks),
                new PedLookAtEntityActivity
                {
                    PedHandle = playerPedHandle,
                    EntityHandle = createdPedHandle
                },
                new PedLookAtEntityActivity
                {
                    PedHandle = createdPedHandle,
                    EntityHandle = playerPedHandle
                }
            ));

            tasks.Add(new LambdaTask(NotifyTaskDidEnd));

            _testSequence = new SequenceTask(tasks);

            await _testSequence.Prepare();
        }

        protected override void ExecuteStart()
        {
            _testSequence.Start();
        }

        protected override void ExecuteAbort()
        {
            _testSequence.Abort();
        }

        protected override void ExecuteReset()
        {
            _testSequence.Reset();
        }

        private Random _random = new Random();
        private PedCameraActivity.Configuration GetRandomCameraConfiguration()
        {
            return _configurations[_random.Next(0, _configurations.Count)];
        }
    }
}