namespace HPNS.UI.Core
{
    public class Controller : Responder
    {
        public Controller(string name) : base(name) { }
        
        public void Start()
        {
            Reply(new {name = "start"});
        }

        public void Stop()
        {
            Reply(new {name = "stop"});
        }
    }
}