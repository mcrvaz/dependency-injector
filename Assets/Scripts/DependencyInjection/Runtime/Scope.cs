using System;
using System.Collections.Generic;

public partial class Scope
{
    readonly Dictionary<Type, RegistrationOptions> installations = new Dictionary<Type, RegistrationOptions>();
    readonly Dictionary<Type, Type> typeMappings = new Dictionary<Type, Type>();
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

    public void Install<T1, T2> (Lifecycle lifecycle) => Install(typeof(T1), typeof(T2), lifecycle);

    public void Install<T> (Lifecycle lifecycle) => Install(typeof(T), lifecycle);

    public void InstallFromInstance<T> (T instance)
    {
        Type type = typeof(T);
        Install(type, Lifecycle.Singleton);
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

    void Install (Type concreteType, Lifecycle lifecycle) => Install(concreteType, concreteType, lifecycle);
    void Install (Type abstractType, Type concreteType, Lifecycle lifecycle)
    {
        AddToTypeMapping(abstractType, concreteType);

        if (installations.ContainsKey(abstractType))
            throw new ArgumentException($"Type {abstractType} already installed as {lifecycle}.");
        installations.Add(abstractType, new RegistrationOptions(lifecycle));
    }

    object Resolve (Type type, Lifecycle lifecycle) => lifecycle switch
    {
        Lifecycle.Singleton => ResolveAsSingleton(type),
        Lifecycle.Transient => ResolveAsTransient(type),
        _ => throw new ArgumentException($"Invalid lifecycle type. Value: {lifecycle}"),
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
        throw new InvalidOperationException($"No registration found for type {type}.");
    }

    object ResolveAsSingleton (Type type)
    {
        if (dependencyResolver.TryResolveAsSingletonStrict(type, out object singleton))
            return singleton;
        return ResolveAsTransient(type);
    }

    object ResolveAsTransient (Type type) => ResolveAsTransient(GetDependencyNode(type));
    object ResolveAsTransient (DependencyNode targetNode)
    {
        RegistrationOptions options = GetInstallationOptions(targetNode.Type);
        if (options.Lifecycle == Lifecycle.Singleton
            && dependencyResolver.TryResolveAsSingletonStrict(targetNode.Type, out object instance)
        )
            return instance;
        if (!targetNode.HasDependencies)
            return CreateFromEmptyConstructor(targetNode);
        return CreateFromNonEmptyConstructor(
            targetNode,
            GetDependencies(targetNode)
        );
    }

    object[] GetDependencies (DependencyNode node)
    {
        object[] dependencies = new object[node.Dependencies.Count];
        for (int i = 0; i < node.Dependencies.Count; i++)
        {
            DependencyNode dependency = node.Dependencies[i];
            RegistrationOptions options = GetInstallationOptions(dependency.Type);
            if (options.Lifecycle == Lifecycle.Singleton
                && dependencyResolver.TryResolveAsSingletonStrict(dependency.Type, out object instance)
            )
                dependencies[i] = instance;
            else if (!node.HasDependencies)
                dependencies[i] = CreateFromEmptyConstructor(dependency);
            else
                dependencies[i] = ResolveAsTransient(dependency);
        }
        return dependencies;
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
        throw new InvalidOperationException($"No dependency node found for type {type}.");
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

    void AddToTypeMapping (Type abstractType, Type concreteType)
    {
        if (!typeMappings.TryAdd(abstractType, concreteType))
            throw new InvalidOperationException($"Type {abstractType} is already mapped to {typeMappings[abstractType].GetType()}.");
    }

    void AddToSingletons (Type type, object instance)
    {
        if (!singletons.TryAdd(type, instance))
            throw new InvalidOperationException($"Singleton instance of type {type} already registered.");
    }

    public void Dispose ()
    {
        foreach (IDisposable disposable in disposables)
            disposable.Dispose();

        foreach (Scope scope in childScopes)
            scope.Dispose();
    }
}