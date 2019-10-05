namespace HPNS.InteractivityV2.Tasks
{
    public class PlayFacialAnimTask : PlayAnimTask
    {
        protected override int AnimFlag => 33;

        public PlayFacialAnimTask(string dict, string name) : base(dict, name) { }
    }
}