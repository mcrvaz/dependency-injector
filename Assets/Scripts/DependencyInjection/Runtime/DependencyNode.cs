using System;

public class DependencyNode
{
    public static readonly DependencyNode[] EmptyDependencies = new DependencyNode[0];

    public readonly Type Type;
    public readonly Lifecycle Lifecycle;
    public readonly DependencyNode[] Dependencies;

    public bool HasDependencies => Dependencies?.Length > 0;

    public DependencyNode (Type type, Lifecycle lifecycle, DependencyNode[] dependencies)
    {
        Type = type;
        Lifecycle = lifecycle;
        Dependencies = dependencies;
    }
}