using System;
using System.Collections.Generic;
using System.Reflection;
using DependencyInjector.Exceptions;

namespace DependencyInjector
{
    internal class DependencyGraph
    {
        public IReadOnlyList<DependencyNode> Nodes => nodes;

        static readonly ParameterInfo[] emptyParams = new ParameterInfo[0];

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
            HashSet<Type> parentTypes = new HashSet<Type>();
            foreach ((Type type, ref RegistrationOptions options) in installations.Installations)
            {
                nodes.Add(GenerateNode(type, options.Lifecycle, parentTypes));
                parentTypes.Clear();
            }
        }

        DependencyNode GenerateNode (
            Type type,
            Lifecycle lifecycle,
            HashSet<Type> parentTypes
        )
        {
            Type mappedType = typeMappings.GetMappedType(type);
            ParameterInfo[] parameters = GetConstructorParameters(mappedType);
            if (parameters.Length == 0)
            {
                return new DependencyNode(
                    type,
                    mappedType,
                    lifecycle,
                    DependencyNode.EmptyDependencies
                );
            }

            if (parentTypes.Contains(type))
                throw new CircularDependencyException($"Circular dependency detected when trying to resolve type {type} as {mappedType}.");
            parentTypes.Add(type);

            DependencyNode[] currentDeps = new DependencyNode[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo info = parameters[i];
                Type parameterType = info.ParameterType;
                currentDeps[i] = GenerateNode(
                    parameterType,
                    installations.Get(parameterType).Lifecycle,
                    parentTypes
                );
                parentTypes.Clear();
            }
            return new DependencyNode(type, mappedType, lifecycle, currentDeps);
        }

        ParameterInfo[] GetConstructorParameters (Type mappedType)
        {
            ConstructorInfo[] constructors = mappedType.GetConstructors(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );
            if (constructors?.Length == 0)
                return emptyParams;

            foreach (ConstructorInfo ctor in constructors)
            {
                if (ctor.GetCustomAttribute<InjectAttribute>() != null)
                    return ctor.GetParameters();
            }
            return constructors[0].GetParameters();
        }
    }
}