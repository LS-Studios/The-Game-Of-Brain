using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

public class LSUtils : MonoBehaviour
{
    //Returns an ValueTurple with the classDataTyp, the Fieldinfo, as well as the value of the field 
    //You have to pass a list of classes the field could be in as well as the field name it self
    public static ValueTuple<object, FieldInfo, object> GetClassFieldProperties(List<object> classDataTypes, string variableName)
    {
        FieldInfo variableField = null;
        object variableValue = null;

        foreach (object itemDataTyp in classDataTypes)
        {
            variableField = itemDataTyp.GetType().GetField(variableName);

            if (variableField != null)
            {
                variableValue = variableField.GetValue(itemDataTyp);
                return new(itemDataTyp, variableField, variableValue);
            }
        }

        return new(null, variableField, variableValue);
    }

    public static Vector3 AddVector(Vector3 baseVector, Vector3 vectorToAddOnTop)
    {
        return new Vector3(baseVector.x + vectorToAddOnTop.x, baseVector.y + vectorToAddOnTop.y, baseVector.z + vectorToAddOnTop.z);
    }

    public static string SeperateCompundString(string compundSting)
    {
        string text = compundSting;
        text = Regex.Replace(text, "([a-z])([A-Z])", "$1 $2").ToLower();
        text = text[0].ToString().ToUpper() + text[1..text.Length];
        return text;
    }

    public static IEnumerator ScaleToNull(GameObject objectToScale)
    {
        while (objectToScale.transform.localScale.x > 0)
        {
            float factor = -Time.deltaTime * 0.0035f;
            objectToScale.transform.localScale = AddVector(objectToScale.transform.localScale, new Vector3(factor, factor));
            yield return null;
        }

        Destroy(objectToScale);
    }

    public static void RotateObjectTowards(GameObject objectToRotate, GameObject target)
    {
        var offset = -90f;
        Vector2 direction = (Vector2) target.transform.position - (Vector2)objectToRotate.transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        var rotation = Quaternion.Euler(Vector3.forward * (angle + offset));

        objectToRotate.transform.rotation = rotation;
    }

    public static Quaternion GetRotationTowardsObject(GameObject objectToRotate, GameObject target)
    {
        var offset = -90f;
        Vector2 direction = (Vector2) target.transform.position - (Vector2)objectToRotate.transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        var rotation = Quaternion.Euler(Vector3.forward * (angle + offset));

        return rotation;
    }
}
