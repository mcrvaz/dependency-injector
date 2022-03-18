using System;

public readonly struct TypeMapping : IEquatable<TypeMapping>
{
    public readonly Type AbstractType;
    public readonly Type Concretetype;

    public TypeMapping (Type abstractType, Type concretetype)
    {
        AbstractType = abstractType;
        Concretetype = concretetype;
    }

    public bool Equals (TypeMapping other) => AbstractType == other.AbstractType && Concretetype == other.Concretetype;
}