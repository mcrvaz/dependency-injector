using System;
using System.Collections.Generic;

public readonly struct DependencyNode
{
    public static readonly DependencyNode[] EmptyDependencies = new DependencyNode[0];

    public IReadOnlyList<DependencyNode> Dependencies => dependencies;
    public readonly Type Type;
    public readonly Lifecycle Lifecycle;
    readonly DependencyNode[] dependencies;

    public bool HasDependencies => dependencies?.Length > 0;

    public DependencyNode (Type type, Lifecycle lifecycle, DependencyNode[] dependencies)
    {
        Type = type;
        Lifecycle = lifecycle;
        this.dependencies = dependencies;
    }
}