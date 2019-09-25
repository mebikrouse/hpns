using System;
using System.Collections.Generic;
using System.Text;
using CitizenFX.Core;

namespace GameEventsTestClient
{
    public class GameEventsTest : BaseScript
    {
        public GameEventsTest()
        {
            EventHandlers.Add("gameEventTriggered", new Action<string, List<object>>(GameEventTriggered));
        }

        private void GameEventTriggered(string arg1, List<object> arg2)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"{arg1}: ");
            foreach (var obj in arg2)
                stringBuilder.Append(obj).Append(", ");
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            
            Debug.WriteLine(stringBuilder.ToString());
        }
    }
}