namespace HPNS.InteractivityV2.Core
{
    public class Parameter<T> : IParameter<T>
    {
        private T _value;

        public Parameter(T value = default(T))
        {
            _value = value;
        }

        public void SetValue(T value)
        {
            _value = value;
        }
        
        public T GetValue()
        {
            return _value;
        }
    }
}