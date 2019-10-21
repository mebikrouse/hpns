using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using Newtonsoft.Json;
using static CitizenFX.Core.Native.API;

namespace HPNS.UI
{
    public static class UI
    {
        private static RootResponder _rootResponder;
        
        public static DialogueController Dialogues;
        
        static UI()
        {
            Dialogues = new DialogueController();
            
            _rootResponder = new RootResponder();
            _rootResponder.AddChild(Dialogues);
        }

        public static async Task Init(string resourceName, EventHandlerDictionary eventHandlers)
        {
            await _rootResponder.Init(resourceName);
            
            RegisterNuiCallbackType("message");
            eventHandlers["__cfx_nui:message"] += new Action<dynamic>(data =>
            {
                Debug.WriteLine(JsonConvert.SerializeObject(data));
                _rootResponder.Handle(data);
            });
        }
    }
}