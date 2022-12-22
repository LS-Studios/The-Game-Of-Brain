using UnityEngine;
using UnityEditor;
using System.Reflection;

//Original version of the ConditionalEnumHideAttribute created by Brecht Lecluyse (www.brechtos.com)
//Edited by Michal on Brecht Lecluyse (www.brechtos.com)

[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
        if (!condHAtt.HideInInspector || enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        GUI.enabled = wasEnabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        if (!condHAtt.HideInInspector || enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }

    private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty propertyA)
    {
        bool enabled = true;

        foreach (string sourceField in condHAtt.SourceFields)
        {

            SerializedProperty sourcePropertyValue = FindSerializableProperty(sourceField, propertyA);

            if (sourcePropertyValue != null)
            {
                var fieldValue = GetPropertyValue(sourcePropertyValue);

                var comparingValue = condHAtt.CompareValue.ToString();

                var fieldValueString = "null";

                if (fieldValue != null)
                    fieldValueString = fieldValue.ToString();

                enabled = comparingValue == fieldValueString;
            }
            else
            {
                Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + sourceField);
            }

            if (condHAtt.Inverse) enabled = !enabled;

            if (!enabled)
                break;
        }

        return enabled;
    }

    private SerializedProperty FindSerializableProperty(string sourceField, SerializedProperty property)
    {
        string propertyPath = property.propertyPath;
        int idx = propertyPath.LastIndexOf('.');

        if (idx == -1)
        {
            return property.serializedObject.FindProperty(sourceField);
        }
        else
        {
            propertyPath = propertyPath.Substring(0, idx);
            return property.serializedObject.FindProperty(propertyPath).FindPropertyRelative(sourceField);
        }
    }

    public static object GetTargetObjectOfProperty(SerializedProperty prop)
    {
        if (prop == null) return null;

        var path = prop.propertyPath.Replace(".Array.data[", "[");
        Debug.Log(path);
        object obj = prop.serializedObject.targetObject;
        var elements = path.Split('.');
        foreach (var element in elements)
        {
            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue_Imp(obj, elementName, index);
            }
            else
            {
                obj = GetValue_Imp(obj, element);
            }
        }
        return obj;
    }
    private static object GetValue_Imp(object source, string name)
    {
        if (source == null)
            return null;
        var type = source.GetType();

        while (type != null)
        {
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
                return f.GetValue(source);

            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p != null)
                return p.GetValue(source, null);

            type = type.BaseType;
        }
        return null;
    }

    private static object GetValue_Imp(object source, string name, int index)
    {
        var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
        if (enumerable == null) return null;
        var enm = enumerable.GetEnumerator();
        //while (index-- >= 0)
        //    enm.MoveNext();
        //return enm.Current;

        for (int i = 0; i <= index; i++)
        {
            if (!enm.MoveNext()) return null;
        }
        return enm.Current;
    }

    public static object GetPropertyValue(SerializedProperty prop)
    {
        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer:
                return prop.intValue;
            case SerializedPropertyType.Boolean:
                return prop.boolValue;
            case SerializedPropertyType.Float:
                return prop.floatValue;
            case SerializedPropertyType.String:
                return prop.stringValue;
            case SerializedPropertyType.Color:
                return prop.colorValue;
            case SerializedPropertyType.ObjectReference:
                return prop.objectReferenceValue;
            case SerializedPropertyType.LayerMask:
                return (LayerMask)prop.intValue;
            case SerializedPropertyType.Enum:
                if (prop.enumNames.Length == 0) { return "undefined"; }
                return prop.enumNames[prop.enumValueIndex];
            case SerializedPropertyType.Vector2:
                return prop.vector2Value;
            case SerializedPropertyType.Vector3:
                return prop.vector3Value;
            case SerializedPropertyType.Vector4:
                return prop.vector4Value;
            case SerializedPropertyType.Rect:
                return prop.rectValue;
            case SerializedPropertyType.ArraySize:
                return prop.arraySize;
            case SerializedPropertyType.Character:
                return (char)prop.intValue;
            case SerializedPropertyType.AnimationCurve:
                return prop.animationCurveValue;
            case SerializedPropertyType.Bounds:
                return prop.boundsValue;
            case SerializedPropertyType.Gradient:
                return null;
                //throw new System.InvalidOperationException("Can not handle Gradient types.");
        }

        return null;
    }
}
