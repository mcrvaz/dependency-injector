using System;
using System.Collections.Generic;

public class Scope
{
    readonly Dictionary<Type, RegistrationOptions> installations = new Dictionary<Type, RegistrationOptions>();
    readonly Dictionary<Type, object> singletons = new Dictionary<Type, object>();
    readonly Dictionary<Type, List<object>> instantiated = new Dictionary<Type, List<object>>();

    DependencyGraph dependencyGraph;

    public void Install<T> (Lifecycle lifecycle) => Install(typeof(T), lifecycle);

    public void InstallFromInstance<T> (T instance)
    {
        Type type = typeof(T);
        Install(type, Lifecycle.Singleton);
        AddToInstantiated(type, instance, Lifecycle.Singleton);
    }

    public void ResolveAll ()
    {
        if (dependencyGraph == null)
            dependencyGraph = new DependencyGraph(installations);

        foreach ((Type type, RegistrationOptions options) in installations)
            Resolve(type, options.Lifecycle);
    }

    public T Resolve<T> ()
    {
        if (dependencyGraph == null)
            dependencyGraph = new DependencyGraph(installations);

        return (T)Resolve(typeof(T), installations[typeof(T)].Lifecycle);
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

    object ResolveAsSingleton (Type type)
    {
        if (singletons.TryGetValue(type, out object singleton))
            return singleton;
        return ResolveAsTransient(type);
    }

    object ResolveAsTransient (Type type)
    {
        object[] GetDependencies (DependencyNode node)
        {
            object[] dependencies = new object[node.Dependencies.Length];
            for (int i = 0; i < node.Dependencies.Length; i++)
            {
                DependencyNode dependency = node.Dependencies[i];
                if (singletons.TryGetValue(dependency.Type, out object instance))
                    dependencies[i] = instance;
                else if (!node.HasDependencies)
                    dependencies[i] = CreateFromEmptyConstructor(dependency.Type);
                else
                    dependencies[i] = ResolveAsTransient(dependency.Type);
            }
            return dependencies;
        }

        DependencyNode targetNode = dependencyGraph.GetNode(type);
        if (singletons.TryGetValue(targetNode.Type, out object instance))
            return instance;
        if (!targetNode.HasDependencies)
            return CreateFromEmptyConstructor(targetNode.Type);
        return CreateFromNonEmptyConstructor(
            targetNode.Type,
            GetDependencies(targetNode)
        );
    }

    object CreateFromEmptyConstructor (Type type)
    {
        RegistrationOptions options = installations[type];
        object instance = Activator.CreateInstance(type);
        AddToInstantiated(type, instance, options.Lifecycle);
        return instance;
    }

    object CreateFromNonEmptyConstructor (Type type, object[] args)
    {
        RegistrationOptions options = installations[type];
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
            throw new InvalidOperationException($"Singleton instace of type {type} already registered.");
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
    }
}