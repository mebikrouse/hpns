using HPNS.UI.Core;
using Newtonsoft.Json;

using static CitizenFX.Core.Native.API;

namespace HPNS.UI
{
    public class RootResponder : Responder
    {
        public RootResponder() : base("root") { }

        public void Init(string resourceName)
        {
            Reply(new
            {
                name = "init",
                data = new {resourceName = resourceName}
            });
        }

        protected override void Reply(object command)
        {
            SendNuiMessage(JsonConvert.SerializeObject(command));
        }
    }
}