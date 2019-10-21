using System;
using CitizenFX.Core;
using HPNS.UI;

using static CitizenFX.Core.Native.API;

namespace UITests
{
    public class Bootstrap : BaseScript
    {
        public Bootstrap()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        public void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;

            var dialogueController = new DialogueController();
            
            var rootResponder = new RootResponder();
            rootResponder.AddChild(dialogueController);

            RegisterNuiCallbackType("message");
            EventHandlers["__cfx_nui:message"] += new Action<dynamic>(data =>
            {
                rootResponder.Handle(data);
            });
            
            rootResponder.Init(resourceName);
            
            
        }
    }
}