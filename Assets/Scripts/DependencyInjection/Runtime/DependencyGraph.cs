using System;
using System.Collections.Generic;
using System.Reflection;

internal class DependencyGraph
{
    public IReadOnlyList<DependencyNode> Nodes => nodes;

    readonly List<DependencyNode> nodes;
    readonly IReadOnlyDictionary<Type, RegistrationOptions> installations;

    public DependencyGraph (IReadOnlyDictionary<Type, RegistrationOptions> installations)
    {
        this.installations = installations;
        nodes = new List<DependencyNode>(installations.Count);
        GenerateGraph();
    }

    public DependencyNode GetNode (Type type)
    {
        if (TryGetNode(type, out DependencyNode node))
            return node;
        throw new NodeNotFoundException(type);
    }

    public bool TryGetNode (Type type, out DependencyNode resultNode)
    {
        foreach (DependencyNode node in nodes)
        {
            if (node.Type == type)
            {
                resultNode = node;
                return true;
            }
        }
        resultNode = default;
        return false;
    }

    void GenerateGraph ()
    {
        foreach ((Type type, RegistrationOptions options) in installations)
            nodes.Add(GenerateNode(type, options.Lifecycle));
    }

    DependencyNode GenerateNode (Type type, Lifecycle lifecycle)
    {
        ConstructorInfo[] constructors = type.GetConstructors(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );
        if (constructors?.Length == 0)
            return new DependencyNode(type, lifecycle, DependencyNode.EmptyDependencies);

        ConstructorInfo constructorInfo = constructors[0];
        ParameterInfo[] parameters = constructorInfo.GetParameters();
        DependencyNode[] currentDeps = new DependencyNode[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            ParameterInfo info = parameters[i];
            Type parameterType = info.ParameterType;
            currentDeps[i] = GenerateNode(parameterType, installations[parameterType].Lifecycle);
        }
        return new DependencyNode(type, lifecycle, currentDeps);
    }
}

[Serializable]
public class NodeNotFoundException : Exception
{
    public NodeNotFoundException (Type type) : base($"Node not found for type {type}") { }
}