using System;

public partial class Scope
{
    public class ParentDependencyResolver
    {
        readonly Scope scope;

        public ParentDependencyResolver (Scope scope)
        {
            this.scope = scope;
        }

        public bool TryResolveAsSingletonStrict (Type type, out object instance)
        {
            Scope currentParent = scope.parent;
            while (currentParent != null)
            {
                if (currentParent.singletons.TryGetValue(type, out instance))
                    return true;
                currentParent = currentParent.parent;
            }

            instance = default;
            return false;
        }
    }
}