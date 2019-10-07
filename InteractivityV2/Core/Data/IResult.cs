namespace HPNS.InteractivityV2.Core.Data
{
    public interface IResult<in T>
    {
        void SetValue(T value);
    }
}