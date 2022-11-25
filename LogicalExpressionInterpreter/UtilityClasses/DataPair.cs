namespace LogicalExpressionInterpreter.UtilityClasses
{
    public class DataPair<TKey, TValue>
    {
        private TKey Key;
        private TValue Value;

        public DataPair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public TKey GetKey()
        {
            return Key;
        }

        public TValue GetValue()
        {
            return Value;
        }
    }
}
