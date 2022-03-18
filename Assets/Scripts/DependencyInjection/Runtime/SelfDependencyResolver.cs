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
    }
}