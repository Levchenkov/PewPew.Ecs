using System;
using System.Collections.Generic;

namespace PewPew.Ecs.Hybrid.SourceGenerators;

public class ArchetypeInfo
{
    public string ClassName { get; set; } = "";
    public string Namespace { get; set; } = "";
    public string MaskType { get; set; } = "";
    public List<ComponentField> Fields { get; set; } = new();
    public string ArchetypeName { get; set; } = "";
    public string ValueTypeName { get; set; } = "";
}

public class ComponentField
{
    public string TypeName { get; set; } = "";
    public string FieldName { get; set; } = "";
    public int Index { get; set; }

    public string GetComponentN => $"GetComponent{Index + 1}";
    public string ComponentN => $"Component{Index + 1}";
}


