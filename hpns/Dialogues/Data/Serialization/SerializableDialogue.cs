using System.Collections.Generic;

namespace Dialogues.Data.Serialization
{
    public class SerializableDialogue
    {
        public IEnumerable<SerializableParticipant> Participants { get; set; }

        public IEnumerable<SerializableResponse> Responses { get; set; }
    }
}