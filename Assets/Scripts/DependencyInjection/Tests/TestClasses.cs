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
        public IntConstructor (int i)
        {
            Value = i;
        }
    }

    public class NestedEmptyConstructor
    {
        public readonly EmptyConstructor Value;
        public NestedEmptyConstructor (EmptyConstructor empty)
        {
            Value = empty;
        }
    }
}