using System.Collections.Generic;

namespace Dialogues.Data
{
    public class Dialogue
    {
        public List<Response> Responses { get; }

        public Dialogue(IEnumerable<Response> responses)
        {
            Responses = new List<Response>(responses);
        }
    }
}