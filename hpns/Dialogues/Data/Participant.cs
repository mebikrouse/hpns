namespace Dialogues.Data
{
    public class Participant
    {
        public string Identifier { get; }

        public string Name { get; }

        public Participant(string identifier, string name)
        {
            Identifier = identifier;
            Name = name;
        }
    }
}