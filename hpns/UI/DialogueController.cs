using System;
using CitizenFX.Core;
using HPNS.UI.Core;

namespace HPNS.UI
{
    public class DialogueController : Controller
    {
        public event Action DidPrint;
        
        public DialogueController() : base("dialogue")
        {
            RegisterHandler("printed", OnPrintedHandler);
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

        private void OnPrintedHandler(dynamic data)
        {
            Debug.WriteLine("printed");
            DidPrint?.Invoke();
        }
    }
}