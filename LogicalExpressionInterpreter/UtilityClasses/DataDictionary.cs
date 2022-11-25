namespace LogicalExpressionInterpreter.UtilityClasses
{
    public class DataDictionary<TKey,TValue>
    {
        private List<TKey> keys;
        private List<TValue> values;
        private List<DataPair<TKey, TValue>> items;

        public DataDictionary() 
        {
            keys = new List<TKey>();
            values = new List<TValue>();
            items = new List<DataPair<TKey,TValue>>();
        }

        public void Add(TKey key, TValue value)
        {
            items.Add(new DataPair<TKey,TValue>(key, value));     
            keys.Add(key);
            values.Add(value);
        }

        public TValue? GetValue(TKey key)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].GetKey().Equals(key))
                {
                    return items[i].GetValue();
                }
            }

            throw new Exception("Dictionary doesn't contain key");
        }

        public bool ContainsKey(TKey key)
        {
            return keys.Contains(key);
        }

        public bool ContainsValue(TValue value)
        {
            return values.Contains(value);
        }
    }
}
