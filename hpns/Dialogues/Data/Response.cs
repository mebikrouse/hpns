namespace Dialogues.Data
{
    public class Response
    {
        public Participant From { get; }

        public Participant To { get; }
        
        public string Content { get; }

        public Response(Participant from, Participant to, string content)
        {
            From = from;
            To = to;
            Content = content;
        }
    }
}