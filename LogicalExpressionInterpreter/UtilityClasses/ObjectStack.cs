namespace LogicalExpressionInterpreter.UtilityClasses
{
    public class ObjectStack<T>
    {
        private List<T> container;

        public ObjectStack()
        {
            container = new List<T>();
        }

        public void Push(T value)
        {
            container.Add(value);
        }

        public T Pop()
        {
            if (container.Count == 0)
            {
                throw new InvalidOperationException("The stack is empty");
            }

            var v = container[container.Count - 1];
            container.RemoveAt(container.Count - 1);

            return v;
        }

        public T Peek()
        {
            return container[container.Count - 1];
        }

        public int Count()
        {
            return container.Count;
        }
    }
}
