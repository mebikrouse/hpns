namespace HPNS.Interactivity.Activities
{
    public class PlayFacialAnimActivity : PlayAnimActivity
    {
        protected override int AnimFlag => 33;

        public PlayFacialAnimActivity(string dict, string name) : base(dict, name, nameof(PlayFacialAnimActivity)) { }
    }
}