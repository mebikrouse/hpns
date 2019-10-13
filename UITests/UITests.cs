using System;
using System.Collections.Generic;
using CitizenFX.Core;
using HPNS.UI;
using static CitizenFX.Core.Native.API;

namespace UITests
{
    public class UITests : BaseScript
    {
        public UITests()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private static void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;

            RegisterCommand("hint", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (args.Count != 1)
                {
                    PrintToChat("Usage: /hint [content]");
                    return;
                }
                
                Hints.ShowNextHint(args[0].ToString());
            }), false);

            RegisterCommand("hide", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (args.Count != 0)
                {
                    PrintToChat("Usage: /hide");
                    return;
                }
                
                Hints.HideCurrentHint();
            }), false);
        }

        private static void PrintToChat(string message)
        {
            TriggerEvent("chat:addMessage", new 
            {
                color = new[] {255, 0, 0},
                args = new[] {"[UITests]", message}
            });
        }
    }
}