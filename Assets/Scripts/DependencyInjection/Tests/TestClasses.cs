public class TestClasses
{
    public class NoConstructor { }

    public class EmptyConstructor
    {
        public EmptyConstructor () { }
    }

    public class IntConstructor
    {
        public readonly int Value;
        public IntConstructor (int value)
        {
            Value = value;
        }
    }

    public class NestedEmptyConstructor
    {
        public readonly EmptyConstructor Value;
        public NestedEmptyConstructor (EmptyConstructor value)
        {
            Value = value;
        }
    }

    public class NestedEmptyConstructorMultipleParameters
    {
        public readonly EmptyConstructor Value1;
        public readonly int Value2;
        public NestedEmptyConstructorMultipleParameters (EmptyConstructor value1, int value2)
        {
            Value1 = value1;
            Value2 = value2;
        }
    }
}