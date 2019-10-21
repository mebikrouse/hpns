using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dialogues.Data.Serialization
{
    public class DialogueConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var dialogue = serializer.Deserialize<SerializableDialogue>(reader);

            var participants = new Dictionary<string, Participant>();
            foreach (var participant in dialogue.Participants)
                participants[participant.Name] = new Participant(participant.Identifier, participant.Name);
            
            var responses = new List<Response>();
            foreach (var response in dialogue.Responses)
                responses.Add(new Response(participants[response.FromIdentifier], participants[response.ToIdentifier],
                    response.Content));
            
            return new Dialogue(responses);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Dialogue).IsAssignableFrom(objectType);
        }
    }
}