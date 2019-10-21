using System.Threading.Tasks;
using HPNS.CoreClient;
using HPNS.Interactivity.Core.Activity;
using HPNS.Interactivity.Core.Data;
using static CitizenFX.Core.Native.API;

namespace HPNS.Interactivity.Activities
{
    public class PlayAnimActivity : ActivityBase
    {
        private string _dict;
        private string _name;
        
        public IParameter<int> PedHandle;

        protected virtual int AnimFlag => 1;

        public PlayAnimActivity(string dict, string name, string typeName = nameof(PlayAnimActivity)) : base(typeName)
        {
            _dict = dict;
            _name = name;
        }
        
        protected override async Task ExecutePrepare()
        {
            await Utility.LoadAnimDict(_dict);
        }

        protected override void ExecuteStart()
        {
            PlayAnim();
        }

        protected override void ExecuteAbort()
        {
            StopAnim();
        }

        protected override void ExecuteReset() { }

        private void PlayAnim()
        {
            var pedHandle = PedHandle.GetValue();
            TaskPlayAnim(pedHandle, _dict, _name, 8f, 8f, -1, AnimFlag, 
                0f, false, false, false);
        }

        private void StopAnim()
        {
            var pedHandle = PedHandle.GetValue();
            StopAnimTask(pedHandle, _dict, _name, 3f);
        }
    }
}