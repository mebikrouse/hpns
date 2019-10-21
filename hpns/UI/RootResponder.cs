using System.Threading.Tasks;
using CitizenFX.Core;
using HPNS.UI.Core;
using Newtonsoft.Json;

using static CitizenFX.Core.Native.API;

namespace HPNS.UI
{
    public class RootResponder : Responder
    {
        public RootResponder() : base("root") { }

        public async Task Init(string resourceName)
        {
            var replyResult = false;
            while (!replyResult)
            {
                await BaseScript.Delay(50);
                
                replyResult = Reply(new
                {
                    name = "init",
                    data = new {resourceName = resourceName}
                });
            }
        }

        protected override bool Reply(object command)
        {
            return SendNuiMessage(JsonConvert.SerializeObject(command));
        }
    }
}