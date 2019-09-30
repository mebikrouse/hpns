using System;
using System.Collections.Generic;
using CitizenFX.Core;
using HPNS.Interactivity.Core;
using QuestTestClient.Tests;

using static CitizenFX.Core.Native.API;

namespace QuestTestClient
{
    public class QuestTest : BaseScript
    {
        private List<Func<ITask>> _testProviders = new List<Func<ITask>>()
        {
            () => new BeingInVehicleStateTest(),
            () => new GoToRadiusAreaTaskTest(),
            () => new AimingAtEntityStateTest(),
            () => new ShopRobberyScenarioTest()
        };
        
        private ITask _currentTest;
        
        public QuestTest()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
            
            RegisterCommand("test", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (args.Count != 1 ||
                    !int.TryParse(args[0].ToString(), out var testNum))
                {
                    PrintToChat("Usage: /test [test_num] - starts test with test_num number.");
                    return;
                }

                if (_currentTest != null) AbortCurrentTest();

                var test = _testProviders[testNum]();
                test.TaskDidEnd += CurrentTestTaskDidEnd;
                test.Start();
                
                PrintToChat("Started new test.");

                _currentTest = test;

            }), false);

            RegisterCommand("abort", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (_currentTest == null)
                {
                    PrintToChat("There is nothing to abort!");
                    return;
                }
                
                AbortCurrentTest();
            }), false);
        }

        private void CurrentTestTaskDidEnd(object sender, EventArgs e)
        {
            PrintToChat("Current test did end.");
            
            _currentTest.TaskDidEnd -= CurrentTestTaskDidEnd;
            _currentTest = null;
        }

        private void AbortCurrentTest()
        {
            _currentTest.TaskDidEnd -= CurrentTestTaskDidEnd;
            _currentTest.Abort();
            _currentTest = null;

            PrintToChat("Aborted currently executing test.");
        }

        private void PrintToChat(string message)
        {
            TriggerEvent("chat:addMessage", new 
            {
                color = new[] {255, 0, 0},
                args = new[] {"[QuestTest]", message}
            });
        }
    }
}