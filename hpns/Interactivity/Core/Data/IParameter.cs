namespace HPNS.Interactivity.Core.Data
{
    public interface IParameter<out T>
    {
        T GetValue();
    }
}