using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("globalData"));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemName"));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("gamePointPrice"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cashPrice"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("braincellPrice"));

        EditorGUILayout.Space();

        var dataTyp = serializedObject.FindProperty("dataTyp").intValue;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("dataTyp"));

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("itemCategory"));

        EditorGUILayout.Space();

        if (dataTyp != 3 && dataTyp != 4)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("damage"));

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentAmmount"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxAmmount"));

            EditorGUILayout.Space();

            var canGetLeveled = serializedObject.FindProperty("canGetLeveled").boolValue;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("canGetLeveled"));

            if (canGetLeveled)
            {
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("currentLevel"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxLevel"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("upgradeImprovements"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("levelImprovements"));
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("generalData"));
        }

        switch (dataTyp)
        {
            case 0:
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponData"));
                break;

            case 1:
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("grenadeData"));
                break;

            case 2:
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponData"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("lMGData"));
                break;

            case 3:
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("perkData"));
                break;

            case 4:
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("extraData"));
                break;
        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("slotData"));

        serializedObject.ApplyModifiedProperties();
    }
}

[CustomPropertyDrawer(typeof(ItemData.SlotData.Stet.StetValues))]
public class StetValueDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var var1Rect = new Rect(position.x, position.y, 130, position.height);
        var appending = new Rect(position.x + 134, position.y, 15, position.height);
        var var2Rect = new Rect(position.x + 153, position.y, 130, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(var1Rect, property.FindPropertyRelative("variable1Name"), GUIContent.none);
        EditorGUI.PropertyField(appending, property.FindPropertyRelative("appending"), GUIContent.none);
        EditorGUI.PropertyField(var2Rect, property.FindPropertyRelative("variable2Name"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }
}

[CustomPropertyDrawer(typeof(ItemData.ChangeVariable.ChangeValues))]
public class UpgradeValueDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var val1Rect = new Rect(position.x, position.y, 130, position.height);
        var val2Rect = new Rect(position.x + 153, position.y, 130, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(val1Rect, property.FindPropertyRelative("valToUpgradeVar1"), GUIContent.none);
        EditorGUI.PropertyField(val2Rect, property.FindPropertyRelative("valToUpgradeVar2"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
