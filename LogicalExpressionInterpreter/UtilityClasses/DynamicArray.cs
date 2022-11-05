namespace LogicalExpressionInterpreter.UtilityClasses
{
    public class DynamicArray<T>
    {
        private T[] container;

        public DynamicArray()
        {
            container = Array.Empty<T>();
        }

        public int Count { get; private set; }
        public int Capacity
        {
            get { return container.Length; }
            set
            {
                if (Capacity < value)
                {
                    Resize(value);
                }
            }
        }

        public void Add(T value)
        {
            if (Capacity <= Count)
            {
                Resize(Capacity == 0 ? 2 : Capacity * 2);
            }

            container[Count++] = value;
        }

        public void RemoveAt(int index)
        {
            int counter = 0;
            for (int i = 0; i < container.Length; i++)
            {
                if (container[i] == null)
                {
                    counter++;
                }
            }

            T[] tempContainer = new T[container.Length - counter - 1];

            int position = 0;
            for (int i = 0; i < container.Length - counter; i++)
            {
                if (i == index)
                {
                    continue;
                }
                tempContainer[position] = container[i];
                position++;
            }
            container = tempContainer;

            Count--;
            Resize(Capacity - 1);
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index > Count)
                {
                    throw new IndexOutOfRangeException();
                }
                return container[index];
            }

            set
            {
                if (index < 0 || index > Count)
                {
                    throw new IndexOutOfRangeException();
                }
                container[index] = value;
            }
        }

        private void Resize(int newCapacity)
        {
            if (Capacity >= newCapacity)
            {
                return;
            }

            var newContainer = new T[newCapacity];

            for (int i = 0; i < container.Length; i++)
            {
                newContainer[i] = container[i];
            }

            container = newContainer;
        }
    }
}
