using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
                    AttributeTargets.Class | AttributeTargets.Struct)]
public class ConditionalHideAttribute : PropertyAttribute
{
    public string[] SourceFields;
    public bool HideInInspector;
    public bool Inverse;
    public object CompareValue;

    public ConditionalHideAttribute(string sourceField, object compareObject, bool inverse = false, bool hideInInspector = true)
    {
        this.SourceFields = new string[] { sourceField };
        this.HideInInspector = hideInInspector;
        this.Inverse = inverse;
        this.CompareValue = compareObject == null ? true : compareObject;
    }

    public ConditionalHideAttribute(string sourceField, bool compareValue = true, bool inverse = false, bool hideInInspector = true)
    {
        this.SourceFields = new string[] { sourceField };
        this.HideInInspector = hideInInspector;
        this.Inverse = inverse;
        this.CompareValue = compareValue;
    }

    public ConditionalHideAttribute(string[] sourceFields, object compareObject, bool inverse = false, bool hideInInspector = true)
    {
        this.SourceFields = sourceFields;
        this.HideInInspector = hideInInspector;
        this.Inverse = inverse;
        this.CompareValue = compareObject == null ? true : compareObject;
    }

    public ConditionalHideAttribute(string[] sourceFields, bool compareValue = true, bool inverse = false, bool hideInInspector = true)
    {
        this.SourceFields = sourceFields;
        this.HideInInspector = hideInInspector;
        this.Inverse = inverse;
        this.CompareValue = compareValue;
    }
}