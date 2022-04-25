using System;
using System.Collections.Generic;

namespace DependencyInjector
{
    public partial class Scope
    {
        readonly Dictionary<Type, RegistrationOptions> installations = new Dictionary<Type, RegistrationOptions>();
        readonly TypeMapping typeMappings = new TypeMapping();
        readonly Dictionary<Type, object> singletons = new Dictionary<Type, object>();
        readonly List<IDisposable> disposables = new List<IDisposable>();

        readonly Scope parent;
        readonly SelfAndParentDependencyResolver dependencyResolver;
        readonly List<Scope> childScopes = new List<Scope>();

        DependencyGraph dependencyGraph;

        public Scope ()
        {
            dependencyResolver = new SelfAndParentDependencyResolver(this);
        }

        Scope (Scope parent) : this()
        {
            this.parent = parent;
        }

        public Scope CreateChildScope ()
        {
            Scope child = new Scope(this);
            childScopes.Add(child);
            return child;
        }

        public void Register<T1, T2> (Lifecycle lifecycle) =>
            Register(new RegistrationOptions(typeof(T1), typeof(T2), lifecycle));

        public void Register<T> (Lifecycle lifecycle) =>
            Register(new RegistrationOptions(typeof(T), lifecycle));

        public void RegisterFromFactory<T> (Func<object> factory, Lifecycle lifecycle) =>
            Register(new RegistrationOptions(typeof(T), lifecycle, factory));

        public void RegisterFromFactory<T1, T2> (Func<object> factory, Lifecycle lifecycle) =>
            Register(new RegistrationOptions(typeof(T1), typeof(T2), lifecycle, factory));

        public void RegisterFromInstance<T> (T instance)
        {
            Type type = typeof(T);
            Register(new RegistrationOptions(type, Lifecycle.Singleton));
            FinishInstantiation(type, instance, Lifecycle.Singleton);
        }

        public void ResolveAll ()
        {
            GenerateDependencyGraph();
            foreach ((Type type, RegistrationOptions options) in installations)
                Resolve(type, options.Lifecycle);
        }

        public T Resolve<T> ()
        {
            GenerateDependencyGraph();
            Type type = typeof(T);
            RegistrationOptions options = GetInstallationOptions(type);
            return (T)Resolve(type, options.Lifecycle);
        }

        void Register (RegistrationOptions options)
        {
            (Type abstractType, Type concreteType) = (options.AbstractType, options.ConcreteType);
            typeMappings.AddTypeMapping(abstractType, concreteType);

            if (installations.ContainsKey(abstractType))
                throw new RegistrationException($"Type {abstractType} already installed as {options.Lifecycle}.");
            installations.Add(abstractType, options);
        }

        object Resolve (Type type, Lifecycle lifecycle) => lifecycle switch
        {
            Lifecycle.Singleton => ResolveAsSingleton(type),
            Lifecycle.Transient => ResolveAsTransient(type),
            _ => throw new RegistrationException($"Invalid lifecycle type. Value: {lifecycle}"),
        };

        RegistrationOptions GetInstallationOptions (Type type)
        {
            Scope currentParent = this;
            do
            {
                if (currentParent.installations.TryGetValue(type, out RegistrationOptions options))
                    return options;
                currentParent = currentParent.parent;
            } while (currentParent != null);
            throw new RegistrationException($"No registration found for type {type}.");
        }

        object ResolveAsSingleton (Type type)
        {
            if (dependencyResolver.TryResolveAsSingletonStrict(type, out object singleton))
                return singleton;
            return ResolveAsTransient(type);
        }

        object ResolveAsTransient (Type type) => ResolveFromNode(GetDependencyNode(type));

        object[] GetDependencies (DependencyNode node)
        {
            object[] dependencies = new object[node.Dependencies.Count];
            for (int i = 0; i < node.Dependencies.Count; i++)
                dependencies[i] = ResolveFromNode(node.Dependencies[i]);
            return dependencies;
        }

        object ResolveFromNode (DependencyNode node)
        {
            RegistrationOptions options = GetInstallationOptions(node.Type);

            if (options.FactoryFunc != null)
                return options.FactoryFunc();

            if (options.Lifecycle == Lifecycle.Singleton
                && dependencyResolver.TryResolveAsSingletonStrict(node.Type, out object instance)
            )
                return instance;

            if (!node.HasDependencies)
                return CreateFromEmptyConstructor(node);

            return CreateFromNonEmptyConstructor(
                node,
                GetDependencies(node)
            );
        }

        void GenerateDependencyGraph ()
        {
            Scope currentParent = this;
            do
            {
                if (currentParent.dependencyGraph == null)
                    currentParent.dependencyGraph = new DependencyGraph(
                        currentParent.installations,
                        currentParent.typeMappings
                    );
                currentParent = currentParent.parent;
            } while (currentParent != null);
        }

        DependencyNode GetDependencyNode (Type type)
        {
            Scope currentParent = this;
            do
            {
                if (currentParent.dependencyGraph.TryGetNode(type, out DependencyNode node))
                    return node;
                currentParent = currentParent.parent;
            } while (currentParent != null);
            throw new ResolutionException($"No dependency node found for type {type}.");
        }

        object CreateFromEmptyConstructor (DependencyNode dependencyNode)
        {
            RegistrationOptions options = GetInstallationOptions(dependencyNode.Type);
            object instance = Activator.CreateInstance(dependencyNode.MappedType);
            FinishInstantiation(dependencyNode.Type, instance, options.Lifecycle);
            return instance;
        }

        object CreateFromNonEmptyConstructor (DependencyNode dependencyNode, object[] args)
        {
            RegistrationOptions options = GetInstallationOptions(dependencyNode.Type);
            object instance = Activator.CreateInstance(dependencyNode.MappedType, args);
            FinishInstantiation(dependencyNode.Type, instance, options.Lifecycle);
            return instance;
        }

        void FinishInstantiation (Type type, object instance, Lifecycle lifecycle)
        {
            if (type is IDisposable disposable)
                disposables.Add(disposable);
            if (lifecycle == Lifecycle.Singleton)
                AddToSingletons(type, instance);
        }

        void AddToSingletons (Type type, object instance)
        {
            if (!singletons.TryAdd(type, instance))
                throw new RegistrationException($"Singleton instance of type {type} already registered.");
        }

        public void Dispose ()
        {
            foreach (IDisposable disposable in disposables)
                disposable.Dispose();

            foreach (Scope scope in childScopes)
                scope.Dispose();
        }
    }
}