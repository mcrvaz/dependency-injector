using System;
using System.Collections.Generic;

public partial class Scope
{
    readonly Dictionary<Type, RegistrationOptions> installations = new Dictionary<Type, RegistrationOptions>();
    readonly Dictionary<Type, object> singletons = new Dictionary<Type, object>();
    readonly Dictionary<Type, List<object>> instantiated = new Dictionary<Type, List<object>>();
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

    public void Install<T> (Lifecycle lifecycle) => Install(typeof(T), lifecycle);

    public void InstallFromInstance<T> (T instance)
    {
        Type type = typeof(T);
        Install(type, Lifecycle.Singleton);
        AddToInstantiated(type, instance, Lifecycle.Singleton);
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
        RegistrationOptions options = GetInstallationOptions(typeof(T));
        return (T)Resolve(typeof(T), options.Lifecycle);
    }

    void Install (Type type, Lifecycle lifecycle)
    {
        if (installations.ContainsKey(type))
            throw new ArgumentException($"Type {type} already installed as {lifecycle}.");
        installations.Add(type, new RegistrationOptions(lifecycle));
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

    object ResolveAsTransient (Type type)
    {
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
                    dependencies[i] = CreateFromEmptyConstructor(dependency.Type);
                else
                    dependencies[i] = ResolveAsTransient(dependency.Type);
            }
            return dependencies;
        }

        DependencyNode targetNode = GetDependencyNode(type);
        RegistrationOptions options = GetInstallationOptions(type);
        if (options.Lifecycle == Lifecycle.Singleton
            && dependencyResolver.TryResolveAsSingletonStrict(targetNode.Type, out object instance)
        )
            return instance;
        if (!targetNode.HasDependencies)
            return CreateFromEmptyConstructor(targetNode.Type);
        return CreateFromNonEmptyConstructor(
            targetNode.Type,
            GetDependencies(targetNode)
        );
    }

    void GenerateDependencyGraph ()
    {
        Scope currentParent = this;
        do
        {
            if (currentParent.dependencyGraph == null)
                currentParent.dependencyGraph = new DependencyGraph(currentParent.installations);
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

    object CreateFromEmptyConstructor (Type type)
    {
        RegistrationOptions options = GetInstallationOptions(type);
        object instance = Activator.CreateInstance(type);
        AddToInstantiated(type, instance, options.Lifecycle);
        return instance;
    }

    object CreateFromNonEmptyConstructor (Type type, object[] args)
    {
        RegistrationOptions options = GetInstallationOptions(type);
        object instance = Activator.CreateInstance(type, args);
        AddToInstantiated(type, instance, options.Lifecycle);
        return instance;
    }

    void AddToInstantiated (Type type, object instance, Lifecycle lifecycle)
    {
        if (instantiated.TryGetValue(type, out List<object> list))
            list.Add(instance);
        else
            instantiated.Add(type, new List<object> { instance });

        if (lifecycle == Lifecycle.Singleton)
            AddToSingletons(type, instance);
    }

    void AddToSingletons (Type type, object instance)
    {
        if (!singletons.TryAdd(type, instance))
            throw new InvalidOperationException($"Singleton instance of type {type} already registered.");
    }

    public void Dispose ()
    {
        foreach (KeyValuePair<Type, List<object>> pair in instantiated)
        {
            foreach (object instance in pair.Value)
            {
                if (instance is IDisposable disposable)
                    disposable.Dispose();
            }
        }

        foreach (Scope scope in childScopes)
            scope.Dispose();
    }
}