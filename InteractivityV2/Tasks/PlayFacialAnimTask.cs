namespace HPNS.InteractivityV2.Tasks
{
    public class PlayFacialAnimTask : PlayAnimTask
    {
        protected override int AnimFlag => 33;

        public PlayFacialAnimTask(int pedHandle, string dict, string name, int duration) : base(pedHandle, dict, name, duration) { }
    }
}