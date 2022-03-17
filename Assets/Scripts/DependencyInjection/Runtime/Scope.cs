using System;
using System.Collections.Generic;
using System.Linq;

public class Scope
{
    readonly Dictionary<Type, RegistrationOptions> installations = new Dictionary<Type, RegistrationOptions>();
    readonly Dictionary<Type, object> singletons = new Dictionary<Type, object>();
    readonly Dictionary<Type, List<object>> instantiated = new Dictionary<Type, List<object>>();

    DependencyGraph dependencyGraph;

    public void Install<T> (Lifecycle lifecycle) => Install(typeof(T), lifecycle);

    public void InstallFromInstance<T> (T instance)
    {
        if (!singletons.TryAdd(typeof(T), instance))
            throw new InvalidOperationException($"Singleton instace of type {typeof(T)} already registered.");
        installations.Add(typeof(T), new RegistrationOptions(Lifecycle.Singleton));
        AddToInstantiated(instance);
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
        object ResolveAsTransientInternal (DependencyNode initialNode)
        {
            Type type = initialNode.Type;
            if (initialNode.Dependencies == null)
                return CreateFromEmptyConstructor(type);
            foreach (DependencyNode item in initialNode.Dependencies)
            {
                if (singletons.ContainsKey(item.Type))
                    continue;

                if (!item.HasDependencies)
                    CreateFromEmptyConstructor(type);
                else
                    ResolveAsTransientInternal(item);
            }
            return CreateFromNonEmptyConstructor(
                type,
                initialNode.Dependencies.Select(x => x.Type).ToArray()
            );
        }

        DependencyNode targetNode = dependencyGraph.GetNode(type);
        return ResolveAsTransientInternal(targetNode);
    }

    object CreateFromEmptyConstructor (Type type)
    {
        RegistrationOptions options = installations[type];
        object instance = Activator.CreateInstance(type);
        AddToInstantiated(instance);
        if (options.Lifecycle == Lifecycle.Singleton)
            singletons[type] = instance;
        return instance;
    }

    object CreateFromNonEmptyConstructor (Type type, object[] args)
    {
        RegistrationOptions options = installations[type];
        object instance = Activator.CreateInstance(type, args);
        AddToInstantiated(instance);
        if (options.Lifecycle == Lifecycle.Singleton)
            singletons[type] = instance;
        return instance;
    }

    void AddToInstantiated<T> (T instance)
    {
        if (instantiated.TryGetValue(typeof(T), out List<object> list))
            list.Add(instance);
        else
            instantiated.Add(typeof(T), new List<object> { instance });
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