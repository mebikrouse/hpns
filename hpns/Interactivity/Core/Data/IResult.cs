namespace HPNS.Interactivity.Core.Data
{
    public interface IResult<in T>
    {
        void SetValue(T value);
    }
}