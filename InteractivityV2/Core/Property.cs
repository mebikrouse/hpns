namespace HPNS.InteractivityV2.Core
{
    public class Property<T>
    {
        public T Value { get; set; }

        public Property() { }
        
        public Property(T value)
        {
            Value = value;
        }
    }
}