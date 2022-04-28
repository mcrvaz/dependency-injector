using System;
using System.Collections.Generic;
using System.Reflection;

namespace DependencyInjectionFramework
{
    internal class DependencyGraph
    {
        public IReadOnlyList<DependencyNode> Nodes => nodes;

        readonly List<DependencyNode> nodes;
        readonly InstallationsContainer installations;
        readonly TypeMapping typeMappings;

        public DependencyGraph (
            InstallationsContainer installations,
            TypeMapping typeMappings
        )
        {
            this.installations = installations;
            this.typeMappings = typeMappings;
            nodes = new List<DependencyNode>(installations.Count);
            GenerateGraph();
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
            foreach ((Type type, RegistrationOptions options) in installations.Installations)
                nodes.Add(GenerateNode(type, options.Lifecycle));
        }

        DependencyNode GenerateNode (Type type, Lifecycle lifecycle)
        {
            Type mappedType = typeMappings.GetMappedType(type);
            ConstructorInfo[] constructors = mappedType.GetConstructors(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );
            if (constructors?.Length == 0)
                return new DependencyNode(
                    type,
                    mappedType,
                    lifecycle,
                    DependencyNode.EmptyDependencies
                );

            ConstructorInfo constructorInfo = constructors[0];
            ParameterInfo[] parameters = constructorInfo.GetParameters();
            DependencyNode[] currentDeps = new DependencyNode[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo info = parameters[i];
                Type parameterType = info.ParameterType;
                currentDeps[i] = GenerateNode(
                    parameterType,
                    installations.Get(parameterType).Lifecycle
                );
            }
            return new DependencyNode(type, mappedType, lifecycle, currentDeps);
        }
    }
}
