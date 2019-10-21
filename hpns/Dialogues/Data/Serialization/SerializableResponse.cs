namespace Dialogues.Data.Serialization
{
    public class SerializableResponse
    {
        public string FromIdentifier { get; set; }
        
        public string ToIdentifier { get; set; }

        public string Content { get; set; }
    }
}