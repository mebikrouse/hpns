namespace HPNS.InteractivityV2.Core.Data
{
    public interface IParameter<out T>
    {
        T GetValue();
    }
}