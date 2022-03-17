using System;

public class RegistrationOptions
{
    public readonly Lifecycle Lifecycle;
    public readonly bool Lazy;
    public readonly Func<object> FactoryFunc;

    public RegistrationOptions (Lifecycle lifecycle)
    {
        Lifecycle = lifecycle;
        FactoryFunc = null;
    }

    public RegistrationOptions (Lifecycle lifecycle, Func<object> factoryFunc)
    {
        Lifecycle = lifecycle;
        FactoryFunc = factoryFunc;
    }
}