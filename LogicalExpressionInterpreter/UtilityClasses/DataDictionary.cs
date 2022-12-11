namespace LogicalExpressionInterpreter.UtilityClasses
{
    public class DataDictionary<TKey,TValue>
    {
        private List<DataPair<TKey, TValue>> _dataEntries;

        public DataDictionary() 
        {
            _dataEntries = new List<DataPair<TKey,TValue>>();
        }

        public void Add(TKey key, TValue value)
        {
            _dataEntries.Add(new DataPair<TKey,TValue>(key, value));     
        }

        public TValue? GetValue(TKey key)
        {
            for (int i = 0; i < _dataEntries.Count; i++)
            {
                if (_dataEntries[i].GetKey().Equals(key))
                {
                    return _dataEntries[i].GetValue();
                }
            }

            throw new Exception("Dictionary doesn't contain key");
        }

        public bool ContainsKey(TKey key)
        {
            for (int i = 0; i < _dataEntries.Count; i++)
            {
                if (_dataEntries[i].GetKey().Equals(key))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
