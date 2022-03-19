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

        public Type GetMappedType (Type type)
        {
            Scope currentParent = scope;
            do
            {
                if (currentParent.typeMappings.TryGetValue(type, out Type mappedType))
                    return mappedType;
                currentParent = currentParent.parent;
            } while (currentParent != null);
            throw new InvalidOperationException($"Type mapping not found for {type}");
        }
    }
}