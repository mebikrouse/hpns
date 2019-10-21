using System.Threading.Tasks;
using HPNS.Core;
using HPNS.Interactivity.Core.Task;

namespace HPNS.Interactivity.Tasks
{
    public class DialogueDidPrintWaitTask : TaskBase
    {
        public DialogueDidPrintWaitTask() : base(nameof(DialogueDidPrintWaitTask)) { }

        protected override Task ExecutePrepare()
        {
            return TaskHelper.CompletedTask;
        }

        protected override void ExecuteStart()
        {
            UI.UI.Dialogues.DidPrint += DialoguesOnDidPrint;
        }

        protected override void ExecuteAbort()
        {
            UI.UI.Dialogues.DidPrint -= DialoguesOnDidPrint;
        }

        protected override void ExecuteReset() { }

        private void DialoguesOnDidPrint()
        {
            UI.UI.Dialogues.DidPrint -= DialoguesOnDidPrint;
            NotifyTaskDidEnd();
        }
    }
}