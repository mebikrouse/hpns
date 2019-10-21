namespace Dialogues.Data
{
    public class Response
    {
        public Participant Participant { get; }

        public string Content { get; }

        public Response(Participant participant, string content)
        {
            Participant = participant;
            Content = content;
        }
    }
}