using System;

public partial class Scope
{
    public class SelfAndParentDependencyResolver
    {
        readonly Scope scope;

        public SelfAndParentDependencyResolver (Scope scope)
        {
            this.scope = scope;
        }

        public bool TryResolveAsSingletonStrict (Type type, out object instance)
        {
            Scope currentParent = scope;
            do
            {
                if (currentParent.singletons.TryGetValue(type, out instance))
                    return true;
                currentParent = currentParent.parent;
            } while (currentParent != null);

            instance = default;
            return false;
        }
    }
}