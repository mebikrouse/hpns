using System.Threading.Tasks;
using HPNS.Core;
using HPNS.Interactivity.Core.Task;

namespace Dialogues.Tasks
{
    public class DialogueDidPrintWaitTask : TaskBase
    {
        protected override Task ExecutePrepare()
        {
            return TaskHelper.CompletedTask;
        }

        protected override void ExecuteStart()
        {
            HPNS.UI.UI.Dialogues.DidPrint += DialoguesOnDidPrint;
        }

        protected override void ExecuteAbort()
        {
            HPNS.UI.UI.Dialogues.DidPrint -= DialoguesOnDidPrint;
        }

        protected override void ExecuteReset() { }

        private void DialoguesOnDidPrint()
        {
            HPNS.UI.UI.Dialogues.DidPrint -= DialoguesOnDidPrint;
            NotifyTaskDidEnd();
        }
    }
}