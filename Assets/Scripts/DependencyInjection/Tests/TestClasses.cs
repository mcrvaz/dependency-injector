public class TestClasses
{
    public class NoConstructor { }

    public interface IEmptyConstructor { }
    public class EmptyConstructor : IEmptyConstructor
    {
        public EmptyConstructor () { }
    }

    public interface IIntConstructor { }
    public class IntConstructor : IIntConstructor
    {
        public readonly int Value;
        public IntConstructor (int value)
        {
            Value = value;
        }
    }

    public interface IFloatConstructor { }
    public class FloatConstructor : IFloatConstructor
    {
        public readonly float Value;
        public FloatConstructor (float value)
        {
            Value = value;
        }
    }

    public interface INestedEmptyConstructor { }
    public class NestedEmptyConstructor : INestedEmptyConstructor
    {
        public readonly EmptyConstructor Value;
        public NestedEmptyConstructor (EmptyConstructor value)
        {
            Value = value;
        }
    }

    public class NestedInterfaceEmptyConstructor : INestedEmptyConstructor
    {
        public readonly IEmptyConstructor Value;
        public NestedInterfaceEmptyConstructor (IEmptyConstructor value)
        {
            Value = value;
        }
    }
    public interface IDoubleNestedInterfaceEmptyConstructor { }
    public class DoubleNestedInterfaceEmptyConstructor : IDoubleNestedInterfaceEmptyConstructor
    {
        public readonly INestedEmptyConstructor Value;
        public DoubleNestedInterfaceEmptyConstructor (INestedEmptyConstructor value)
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

    public interface INestedEmptyConstructorMultipleNestedParameters { }
    public class NestedEmptyConstructorMultipleNestedParameters : INestedEmptyConstructorMultipleNestedParameters
    {
        public readonly INestedEmptyConstructor Value1;
        public readonly IDoubleNestedInterfaceEmptyConstructor Value2;
        public NestedEmptyConstructorMultipleNestedParameters (INestedEmptyConstructor value1, IDoubleNestedInterfaceEmptyConstructor value2)
        {
            Value1 = value1;
            Value2 = value2;
        }
    }
}