using System;
using System.Collections.Generic;
using DependencyInjector.Exceptions;

namespace DependencyInjector
{
    internal class TypeMapping
    {
        public IReadOnlyDictionary<Type, Type> TypeMappings => typeMappings;

        readonly Dictionary<Type, Type> typeMappings = new Dictionary<Type, Type>();

        public void AddTypeMapping (Type abstractType, Type concreteType)
        {
            if (!typeMappings.TryAdd(abstractType, concreteType))
                throw new RegistrationException($"Type {abstractType} is already mapped to {typeMappings[abstractType].GetType()}.");
        }

        public bool TryGetMappedType (Type type, out Type mappedType) =>
            typeMappings.TryGetValue(type, out mappedType);

        public Type GetMappedType (Type type)
        {
            if (TryGetMappedType(type, out Type mappedType))
                return mappedType;
            throw new RegistrationException($"Type mapping not found for {type}");
        }
    }
}