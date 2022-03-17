using System;

public partial class Scope
{
    public class SelfDependencyResolver
    {
        readonly Scope scope;

        public SelfDependencyResolver (Scope scope)
        {
            this.scope = scope;
        }

        public bool TryResolveAsSingletonStrict (Type type, out object instance)
        {
            if (scope.singletons.TryGetValue(type, out instance))
                return true;
            instance = default;
            return false;
        }

        // public DependencyNode GetDependencyNode (Type type)
        // {
        //     if (scope.dependencyGraph.TryGetNode(type, out DependencyNode node))
        //         return node;
        //     throw new InvalidOperationException($"No dependency node found for type {type}.");
        // }
    }
}