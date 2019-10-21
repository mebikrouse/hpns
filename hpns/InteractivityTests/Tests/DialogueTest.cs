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
using HPNS.UI;
using InteractivityTests.Core;

namespace InteractivityTests.Tests
{
    public class DialogueTest : TaskBase, ITest
    {
        private class Response
        {
            public string Title { get; }

            public string Content { get; }

            public Response(string title, string content)
            {
                Title = title;
                Content = content;
            }
        }
        
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

        private List<Response> _responses = new List<Response>
        {
            new Response("Случайный прохожий", "Добрый день!"),
            new Response("Вы", "Здравствуйте."),
            new Response("Случайный прохожий", "Как у вас дела? Наверное, отлично, ведь на улице такая замечательная погода!"),
            new Response("Вы", "Да, у меня все хорошо. А погода действительно великолепная!"),
            new Response("Случайный прохожий", "Чем вы занимаетесь в свое свободное время?"),
            new Response("Вы", "Ой, по-разному бывает. Сегодня я планировал сходить в бар!"),
            new Response("Случайный прохожий", "Ого, как круто! А можно с вами?"),
            new Response("Вы", "Хмм... Да, вы можете пойти со мной! Я познакомлю вас со своими друзьями. Мы весело проведем время!"),
            new Response("Случайный прохожий", "Класс! Отправляемся?"),
            new Response("Вы", "Отправляемся!"),
        };

        private ITask _testSequence;
        
        public string TestName => nameof(DialogueTest);
        
        public DialogueTest() : base(nameof(DialogueTest)) { }

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
            
            tasks.Add(new LambdaTask(UI.Dialogues.Start));

            var currentIteration = 0;
            var dialogueTasks = new List<ITask>();
            foreach (var response in _responses)
            {
                var targetPedHandle = currentIteration % 2 == 0 ? (IParameter<int>) createdPedHandle : playerPedHandle;
                currentIteration++;
                
                dialogueTasks.Add(new LambdaTask(() => { UI.Dialogues.Print(response.Title, response.Content); }));
                dialogueTasks.Add(new ParallelActivityTask(
                    new SequenceTask(new List<ITask>{
                        new ParallelActivityTask(
                            new DialogueDidPrintWaitTask(),
                            new PlayFacialAnimActivity("mp_facial", "mic_chatter") {PedHandle = targetPedHandle}
                        ),
                        new WaitTask {Duration = new Parameter<int>(1500)}
                    }),
                    new PedCameraActivity(GetRandomCameraConfiguration()) {PedHandle = targetPedHandle})
                );
            }

            tasks.Add(new ParallelActivityTask(
                new SequenceTask(dialogueTasks),
                new PedLookAtEntityActivity {PedHandle = playerPedHandle, EntityHandle = createdPedHandle},
                new PedLookAtEntityActivity {PedHandle = createdPedHandle, EntityHandle = playerPedHandle}
            ));

            tasks.Add(new LambdaTask(UI.Dialogues.Stop));
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

        private Random _random = new Random();
        private PedCameraActivity.Configuration GetRandomCameraConfiguration()
        {
            return _configurations[_random.Next(0, _configurations.Count)];
        }
    }
}