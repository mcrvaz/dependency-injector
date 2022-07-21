using System;

namespace DependencyInjector
{
    public static class RegistrationExtensions
    {
        public static void Register<T1, T2> (this Scope scope, Lifecycle lifecycle) where T2 : T1
            => scope.Register(new RegistrationOptions(typeof(T1), typeof(T2), lifecycle));

        public static void Register<T> (this Scope scope, Lifecycle lifecycle) =>
            scope.Register(new RegistrationOptions(typeof(T), lifecycle));

        public static void RegisterFromFactory<T> (
            this Scope scope,
            Func<object> factory,
            Lifecycle lifecycle
        ) => scope.Register(new RegistrationOptions(typeof(T), lifecycle, factory));

        public static void RegisterFromFactory<T1, T2> (
            this Scope scope,
            Func<object> factory,
            Lifecycle lifecycle
        ) where T2 : T1
            => scope.Register(new RegistrationOptions(typeof(T1), typeof(T2), lifecycle, factory));
    }
}