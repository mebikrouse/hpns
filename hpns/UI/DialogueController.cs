using System;
using HPNS.UI.Core;

namespace HPNS.UI
{
    public class DialogueController : Controller
    {
        public event Action DidPrint;
        
        public DialogueController() : base("dialogue")
        {
            RegisterHandler("didPrint", DidPrintHandler);
        }

        public void Print(string title, string content)
        {
            Reply(new
            {
                name = "print",
                data = new
                {
                    title = title,
                    content = content
                }
            });
        }

        public void Skip()
        {
            Reply(new {name = "skip"});
        }

        private void DidPrintHandler(dynamic data)
        {
            DidPrint?.Invoke();
        }
    }
}