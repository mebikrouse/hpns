using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using Dialogues.Data;
using Dialogues.Tasks;
using HPNS.CoreClient;
using HPNS.Interactivity.Core.Data;
using HPNS.Interactivity.Core.Task;
using HPNS.Interactivity.Support;
using HPNS.Interactivity.Tasks;
using HPNS.UI;
using InteractivityTests.Core;

namespace InteractivityTests.Tests
{
    public class DialogueTaskTest : TaskBase, ITest
    {
        private ITask _testSequence;
        
        private Configuration _configuration = new Configuration
        {
            Configurations = new List<CameraConfiguration>
            {
                new CameraConfiguration(
                    new Vector3(-0.0009017102f, 0.8136478f, 0.03338432f),
                    new Vector3(-0.420009f, 0.0001329383f, -180.1577f),
                    30f),
                new CameraConfiguration(
                    new Vector3(0.3481081f, 0.6787802f, -0.147724f),
                    new Vector3(11.97f, 0.0001331912f, 151.7988f),
                    30f),
                new CameraConfiguration(
                    new Vector3(-0.5304984f, 0.7575822f, 0.1575111f),
                    new Vector3(-9.449986f, 0.0001370825f, -143.8104f),
                    30f),
                new CameraConfiguration(
                    new Vector3(-0.4194367f, 0.5977129f, -0.1750073f),
                    new Vector3(12.54f, 0.0001261951f, -143.4225f),
                    30f),
                new CameraConfiguration(
                    new Vector3(0.557281f, 0.8663054f, -0.4690321f),
                    new Vector3(22.86f, 0.000126269f, 145.7156f),
                    30f)
            }
        };

        private Dialogue _dialogue;
        
        public string TestName => nameof(DialogueTaskTest);

        public DialogueTaskTest() : base(nameof(DialogueTaskTest))
        {
            var participants = new List<Participant>
            {
                new Participant("player", "Вы"),
                new Participant("ped_a", "Левый хуй"),
                new Participant("ped_b", "Правый хуй")
            };
            
            var responses = new List<Response>
            {
                new Response(participants[1], participants[0], "Добрый день!"),
                new Response(participants[0], participants[1], "Здравствуйте."),
                new Response(participants[2], participants[0], "Ага..."),
                
                new Response(participants[1], participants[0], "Как у вас дела? Наверное, отлично, ведь на улице такая замечательная погода!"),
                new Response(participants[0], participants[1], "Да, у меня все хорошо. А погода действительно великолепная!"),
                new Response(participants[2], participants[0], "Что за глупый вопрос?! Ты можешь нормальную дискуссию вести?"),
                
                new Response(participants[1], participants[2], "Простите, что с моим вопросом не так?"),
                new Response(participants[2], participants[1], "Ну ты и омега, я тебя оскорбляю, а ты извиняешься. Ебанулся что ли?"),
                
                new Response(participants[0], participants[2], "Ты можешь успокоиться и не трогать его?!"),
                new Response(participants[2], participants[0], "Заткнись нахуй, тебя не спрашивали."),
                new Response(participants[0], participants[2], "Чего?!"),
                
                new Response(participants[2], participants[1], "Аривидерчи, я сваливаю от вас"),
                new Response(participants[0], participants[2], "Ну и скатертью дорога!"),
                new Response(participants[1], participants[2], "Да, вали отсюда!")
            };
            
            _dialogue = new Dialogue(responses);
        }

        protected override async Task ExecutePrepare()
        {
            var playerPedHandle = new Parameter<int>();
            
            var pedAPosition = new Parameter<Vector3>();
            var pedAHeading = new Parameter<float>();
            
            var pedBPosition = new Parameter<Vector3>();
            var pedBHeading = new Parameter<float>();
            
            var pedAHandle = new ResultCapturer<int>();
            var pedBHandle = new ResultCapturer<int>();
            
            var tasks = new List<ITask>();
            tasks.Add(new LambdaTask(() =>
            {
                playerPedHandle.SetValue(Game.PlayerPed.Handle);
                
                pedAPosition.SetValue(Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 1.5f + Game.PlayerPed.RightVector);
                pedAHeading.SetValue(Game.PlayerPed.Heading - 180f);
                
                pedBPosition.SetValue(Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 1.5f - Game.PlayerPed.RightVector);
                pedBHeading.SetValue(Game.PlayerPed.Heading - 180f);
            }));
            tasks.Add(new CreatePedTask(Utility.GetRandomPedHash())
            {
                Position = pedAPosition,
                Heading = pedAHeading,
                PedHandle = pedAHandle
            });
            tasks.Add(new CreatePedTask(Utility.GetRandomPedHash())
            {
                Position = pedBPosition,
                Heading = pedBHeading,
                PedHandle = pedBHandle
            });
            tasks.Add(new WaitTask {Duration = new Parameter<int>(2000)});
            tasks.Add(new DialogueTask(_dialogue, _configuration)
            {
                PedHandles = new Dictionary<string, IParameter<int>>
                {
                    {"player", playerPedHandle},
                    {"ped_a", pedAHandle},
                    {"ped_b", pedBHandle}
                }
            });
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
            UI.Dialogues.Stop();
        }

        protected override void ExecuteReset()
        {
            _testSequence.Reset();
        }
    }
}