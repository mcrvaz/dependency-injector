using System;

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor,
    AllowMultiple = false
)]
public class InjectAttribute : Attribute { }