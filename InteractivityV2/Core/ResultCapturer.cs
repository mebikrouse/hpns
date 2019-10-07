namespace HPNS.InteractivityV2.Core
{
    public class ResultCapturer<T> : IResult<T>, IParameter<T>
    {
        private T _value;
        private IResult<T> _result;

        public ResultCapturer(IResult<T> result = null)
        {
            _result = result;
        }
        
        public void SetValue(T value)
        {
            _value = value;
            _result?.SetValue(value);
        }

        public T GetValue()
        {
            return _value;
        }
    }
}