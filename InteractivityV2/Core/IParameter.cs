namespace HPNS.InteractivityV2.Core
{
    public interface IParameter<out T>
    {
        T GetValue();
    }
}