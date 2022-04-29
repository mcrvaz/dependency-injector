using System;
using System.Collections.Generic;

namespace DependencyInjectionFramework
{
    public partial class Scope : IScope
    {
        readonly InstallationsContainer installations = new InstallationsContainer();
        readonly TypeMapping typeMappings = new TypeMapping();
        readonly Dictionary<Type, object> singletons = new Dictionary<Type, object>();
        readonly List<IDisposable> disposables = new List<IDisposable>();

        readonly Scope parent;
        readonly SelfAndParentDependencyResolver dependencyResolver;
        readonly List<Scope> childScopes = new List<Scope>();

        DependencyGraph dependencyGraph;

        public Scope (IInstaller installer = null)
        {
            dependencyResolver = new SelfAndParentDependencyResolver(this);
            installer?.Install(this);
        }

        protected Scope (Scope parent, IInstaller installer) : this(installer)
        {
            this.parent = parent;
        }

        public Scope CreateChildScope (IInstaller installer = null)
        {
            Scope child = new Scope(this, installer);
            childScopes.Add(child);
            return child;
        }

        public void Register<AbstractT, ConcreteT> (Lifecycle lifecycle) =>
            Register(new RegistrationOptions(typeof(AbstractT), typeof(ConcreteT), lifecycle));

        public void Register<T> (Lifecycle lifecycle) =>
            Register(new RegistrationOptions(typeof(T), lifecycle));

        public void RegisterFromFactory<T> (Func<object> factory, Lifecycle lifecycle) =>
            Register(new RegistrationOptions(typeof(T), lifecycle, factory));

        public void RegisterFromFactory<AbstractT, ConcreteT> (
            Func<object> factory,
            Lifecycle lifecycle
        ) => Register(
            new RegistrationOptions(typeof(AbstractT), typeof(ConcreteT), lifecycle, factory)
        );

        public void RegisterFromInstance<T> (T instance)
        {
            Type type = typeof(T);
            Register(new RegistrationOptions(type, Lifecycle.Singleton));
            FinishInstantiation(type, instance, Lifecycle.Singleton);
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

            if (installations.IsInstalled(abstractType))
                throw new RegistrationException($"Type {abstractType} already installed as {options.Lifecycle}.");
            installations.Add(abstractType, options);
        }

        object Resolve (Type type, Lifecycle lifecycle) => lifecycle switch
        {
            Lifecycle.Singleton => ResolveAsSingleton(type),
            Lifecycle.Transient => ResolveAsTransient(type),
            _ => throw new ResolutionException($"Invalid lifecycle type. Value: {lifecycle}"),
        };

        RegistrationOptions GetInstallationOptions (Type type)
        {
            Scope currentParent = this;
            do
            {
                if (currentParent.installations.TryGet(type, out RegistrationOptions options))
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

            if (options.Lifecycle == Lifecycle.Singleton
                && dependencyResolver.TryResolveAsSingletonStrict(node.Type, out object instance)
            )
                return instance;

            if (options.FactoryFunc != null)
                return ResolveFromFunc(options);

            if (!node.HasDependencies)
                return CreateFromEmptyConstructor(node);

            return CreateFromNonEmptyConstructor(
                node,
                GetDependencies(node)
            );
        }

        object ResolveFromFunc (RegistrationOptions options)
        {
            object instance = options.FactoryFunc();
            FinishInstantiation(
                options.AbstractType,
                instance,
                options.Lifecycle
            );
            return instance;
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
            if (instance is IDisposable disposable)
                disposables.Add(disposable);
            if (lifecycle == Lifecycle.Singleton)
                AddToSingletons(type, instance);
        }

        void AddToSingletons (Type type, object instance)
        {
            if (!singletons.TryAdd(type, instance))
                throw new RegistrationException($"Singleton instance of type {type} already registered.");
        }

        public virtual void Dispose ()
        {
            foreach (IDisposable disposable in disposables)
                disposable.Dispose();

            foreach (Scope scope in childScopes)
                scope.Dispose();
        }
    }
}
