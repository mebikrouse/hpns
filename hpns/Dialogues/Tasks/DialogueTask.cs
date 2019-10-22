using System.Collections.Generic;
using System.Threading.Tasks;
using Dialogues.Data;
using HPNS.Interactivity.Core.Data;
using HPNS.Interactivity.Core.Task;
using HPNS.Interactivity.Support;

namespace Dialogues.Tasks
{
    public class DialogueTask : TaskBase
    {
        private Dialogue _dialogue;
        private Configuration _configuration;
        
        private ITask _dialogueSequence;

        public Dictionary<string, IParameter<int>> PedHandles;

        public DialogueTask(Dialogue dialogue, Configuration configuration)
        {
            _dialogue = dialogue;
            _configuration = configuration;
        }
        
        protected override async Task ExecutePrepare()
        {
            var tasks = new List<ITask>();
            if (_dialogue.Responses.Count == 1)
            {
                var response = _dialogue.Responses[0];
                tasks.Add(new SingleResponseTask(response, _configuration.GetRandomCameraConfiguration(), 750)
                {
                    FromPedHandle = PedHandles[response.From.Identifier],
                    ToPedHandle = PedHandles[response.To.Identifier]
                });
            }
            else
            {
                var firstResponse = _dialogue.Responses[0];
                tasks.Add(new FirstResponseTask(firstResponse, _configuration.GetRandomCameraConfiguration(), 750)
                {
                    FromPedHandle = PedHandles[firstResponse.From.Identifier],
                    ToPedHandle = PedHandles[firstResponse.To.Identifier]
                });

                for (var i = 1; i < _dialogue.Responses.Count - 1; i++)
                {
                    var response = _dialogue.Responses[i];
                    tasks.Add(new ResponseTask(response, _configuration.GetRandomCameraConfiguration())
                    {
                        FromPedHandle = PedHandles[response.From.Identifier],
                        ToPedHandle = PedHandles[response.To.Identifier]
                    });
                }

                var lastResponse = _dialogue.Responses[_dialogue.Responses.Count - 1];
                tasks.Add(new LastResponseTask(lastResponse, _configuration.GetRandomCameraConfiguration(), 750)
                {
                    FromPedHandle = PedHandles[lastResponse.From.Identifier],
                    ToPedHandle = PedHandles[lastResponse.To.Identifier]
                });
            }
            
            tasks.Add(new LambdaTask(NotifyTaskDidEnd));

            _dialogueSequence = new SequenceTask(tasks);

            await _dialogueSequence.Prepare();
        }

        protected override void ExecuteStart()
        {
            _dialogueSequence.Start();
        }

        protected override void ExecuteAbort()
        {
            _dialogueSequence.Abort();
        }

        protected override void ExecuteReset()
        {
            _dialogueSequence.Reset();
        }
    }
}