namespace HPNS.InteractivityV2.Core
{
    public interface IResult<in T>
    {
        void SetValue(T value);
    }
}