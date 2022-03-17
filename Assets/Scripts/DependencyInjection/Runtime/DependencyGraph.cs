using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class DependencyGraph
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
        foreach (DependencyNode node in nodes)
        {
            if (node.Type == type)
                return node;

            foreach (DependencyNode nodeDependency in node.Dependencies)
            {
                if (nodeDependency.Type == type)
                    return node;
            }
        }
        throw new NodeNotFoundException(type);
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
            return new DependencyNode(type, lifecycle, new List<DependencyNode>());

        ConstructorInfo constructorInfo = constructors[0];
        ParameterInfo[] parameters = constructorInfo.GetParameters();
        List<DependencyNode> currentDeps = new List<DependencyNode>();
        foreach (Type item in parameters.Select(x => x.ParameterType))
            currentDeps.Add(GenerateNode(item, installations[item].Lifecycle));
        return new DependencyNode(type, lifecycle, currentDeps);
    }
}

[Serializable]
public class NodeNotFoundException : Exception
{
    public NodeNotFoundException (Type type) : base($"Node not found for type {type}") { }
}