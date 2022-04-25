using System;

internal class RegistrationOptions
{
    public readonly Type AbstractType;
    public readonly Type ConcreteType;
    public readonly Lifecycle Lifecycle;
    public readonly Func<object> FactoryFunc;

    public RegistrationOptions (Type concreteType, Lifecycle lifecycle)
        : this(concreteType, concreteType, lifecycle, null) { }

    public RegistrationOptions (Type abstractType, Type concreteType, Lifecycle lifecycle)
        : this(abstractType, concreteType, lifecycle, null) { }

    public RegistrationOptions (Type concreteType, Lifecycle lifecycle, Func<object> factoryFunc)
        : this(concreteType, concreteType, lifecycle, factoryFunc) { }

    public RegistrationOptions (Type abstractType, Type concreteType, Lifecycle lifecycle, Func<object> factoryFunc)
    {
        AbstractType = abstractType;
        ConcreteType = concreteType;
        Lifecycle = lifecycle;
        FactoryFunc = factoryFunc;
    }
}