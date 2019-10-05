using System;
using System.Collections.Generic;
using CitizenFX.Core;
using HPNS.InteractivityV2.Core;
using InteractivityTests.Core;
using InteractivityTests.Tests;
using static CitizenFX.Core.Native.API;

namespace InteractivityTests
{
    public class TestsRunner : BaseScript
    {
        private List<ITest> _tests = new List<ITest>
        {
            new AttachEntityTaskTest()
        };
        
        private ITest _currentTest;
        
        public TestsRunner()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;
            
            RegisterCommand("test", new Action<int, List<object>, string>(async (source, args, raw) =>
            {
                var testIndex = 0;
                if (args.Count != 1 || !int.TryParse(args[0].ToString(), out testIndex))
                {
                    PrintToChat("Usage: /test [test_number]");
                    return;
                }

                if (testIndex >= _tests.Count || testIndex < 0)
                {
                    PrintToChat($"Unable to start test with number of {testIndex}. Type /tests to show all possible tests.");
                    return;
                }

                if (_currentTest != null)
                {
                    _currentTest.TaskDidEnd -= CurrentTestOnTaskDidEnd;
                    _currentTest.Abort();
                    
                    PrintToChat($"Currently running test {_currentTest.Name} has been aborted.");
                }

                _currentTest = _tests[testIndex];

                if (_currentTest.CurrentState == TaskState.NotReady)
                {
                    PrintToChat("Loading test...");
                    await _currentTest.Prepare();
                    PrintToChat("Test has been loaded.");
                }

                _currentTest.Reset();
                
                _currentTest.TaskDidEnd += CurrentTestOnTaskDidEnd;
                _currentTest.Start();
                
                PrintToChat($"Test {_currentTest.Name} started.");
                
            }), false);
            
            RegisterCommand("abort", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (args.Count != 0)
                {
                    PrintToChat("Usage: /abort");
                    return;
                }
                
                if (_currentTest == null)
                {
                    PrintToChat("There is nothing to abort. Type /current to check if any test is running.");
                    return;
                }

                var abortedTest = _currentTest;

                _currentTest.TaskDidEnd -= CurrentTestOnTaskDidEnd;
                _currentTest.Abort();
                _currentTest = null;
                
                PrintToChat($"Test {abortedTest.Name} has been aborted.");
                
            }), false);
            
            RegisterCommand("current", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (args.Count != 0)
                {
                    PrintToChat("Usage: /current");
                    return;
                }

                PrintToChat(_currentTest == null
                    ? "No tests are running right now."
                    : $"Current test is {_currentTest.Name}");
                
            }), false);
            
            RegisterCommand("tests", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (args.Count != 0)
                {
                    PrintToChat("Usage: /help");
                    return;
                }

                if (_tests.Count == 0)
                {
                    PrintToChat("There are no tests to check out.");
                    return;
                }

                var testIndex = 0;
                foreach (var test in _tests)
                    PrintToChat($"{testIndex++} - {test.Name}");
                
            }), false);
        }

        private void CurrentTestOnTaskDidEnd(object sender, EventArgs e)
        {
            if (_currentTest == null)
            {
                PrintToChat("Current test ended and is now somehow null.");
            }
            
            PrintToChat($"Current test {_currentTest.Name} ended.");
            
            _currentTest.TaskDidEnd -= CurrentTestOnTaskDidEnd;
            _currentTest = null;
        }

        private static void PrintToChat(string message)
        {
            TriggerEvent("chat:addMessage", new 
            {
                color = new[] {255, 0, 0},
                args = new[] {"[TestsRunner]", message}
            });
        }
    }
}