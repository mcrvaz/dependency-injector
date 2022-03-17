using System;
using System.Collections.Generic;

public class DependencyNode
{
    public readonly Type Type;
    public readonly Lifecycle Lifecycle;
    public readonly List<DependencyNode> Dependencies;

    public bool HasDependencies => Dependencies?.Count > 0;

    public DependencyNode (Type type, Lifecycle lifecycle, List<DependencyNode> dependencies)
    {
        Type = type;
        Lifecycle = lifecycle;
        Dependencies = dependencies;
    }
}