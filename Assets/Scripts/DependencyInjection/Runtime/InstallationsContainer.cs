using System;
using System.Collections.Generic;
using DependencyInjector.Exceptions;

namespace DependencyInjector
{
    internal class InstallationsContainer
    {
        public IReadOnlyDictionary<Type, RegistrationOptions> Installations => installations;
        public int Count => installations.Count;
        readonly Dictionary<Type, RegistrationOptions> installations =
            new Dictionary<Type, RegistrationOptions>();

        public bool IsInstalled (Type type) => installations.ContainsKey(type);

        public void Add (Type type, RegistrationOptions options) =>
            installations.Add(type, options);

        public RegistrationOptions Get (Type type)
        {
            if (TryGet(type, out RegistrationOptions options))
                return options;
            throw new RegistrationException($"Type {type} is not registered.");
        }

        public bool TryGet (Type type, out RegistrationOptions options) =>
            installations.TryGetValue(type, out options);
    }
}