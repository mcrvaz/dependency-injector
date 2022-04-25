using System;
using System.Collections.Generic;

namespace DependencyInjector
{
    internal readonly struct DependencyNode
    {
        public static readonly DependencyNode[] EmptyDependencies = new DependencyNode[0];

        public IReadOnlyList<DependencyNode> Dependencies => dependencies;
        public bool HasDependencies => dependencies?.Length > 0;

        public readonly Type Type;
        public readonly Type MappedType;
        public readonly Lifecycle Lifecycle;

        readonly DependencyNode[] dependencies;

        public DependencyNode (Type type, Type mappedType, Lifecycle lifecycle, DependencyNode[] dependencies)
        {
            Type = type;
            MappedType = mappedType;
            Lifecycle = lifecycle;
            this.dependencies = dependencies;
        }
    }
}