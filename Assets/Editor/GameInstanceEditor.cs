using UnityEditor;

//[CustomEditor(typeof(GameInstance))]
public class GameInstanceEditor : Editor
{
    // The various categories the editor will display the variables in 
    public enum DisplayCategory
    {
        Inventory, Mission, Character, Defaults, Debug
    }

    // The enum field that will determine what variables to display in the Inspector
    public DisplayCategory categoryToDisplay;

    // The function that makes the custom editor work
    public override void OnInspectorGUI()
    {
        // Display the enum popup in the inspector
        categoryToDisplay = (DisplayCategory)EditorGUILayout.EnumPopup("Display", categoryToDisplay);

        // Create a space to separate this enum popup from other variables 
        EditorGUILayout.Space();

        // Switch statement to handle what happens for each category
        switch (categoryToDisplay)
        {
            case DisplayCategory.Inventory:
                DisplayInventoryInfo();
                break;

            case DisplayCategory.Mission:
                DisplayMissionInfo();
                break;

            case DisplayCategory.Character:
                DisplayCharacterInfo();
                break;
            case DisplayCategory.Defaults:
                DisplayDefaultInfo();
                break;
            case DisplayCategory.Debug:
                DisplayDebugInfo();
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }

    // When the categoryToDisplay enum is at "Basic"
    void DisplayInventoryInfo()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("gameInventory"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("gameInventoryBackUp")); 
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxInventorySlots"));
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerInventory"));
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("equipmentSets"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("currentEquipmentSet")); 
    }

    void DisplayMissionInfo()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("missions"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("missionsSceneName"));
    }

    void DisplayCharacterInfo()
    {
        #region Something with ticking Bool

        //// Store the hasMagic bool as a serializedProperty so we can access it
        //SerializedProperty hasMagicProperty = serializedObject.FindProperty("hasMagic");

        //// Draw a property for the hasMagic bool
        //EditorGUILayout.PropertyField(hasMagicProperty);

        //// Check if hasMagic is true
        //if (hasMagicProperty.boolValue)
        //{
        //    EditorGUILayout.PropertyField(serializedObject.FindProperty("mana"));
        //    EditorGUILayout.PropertyField(serializedObject.FindProperty("magicType"));
        //    EditorGUILayout.PropertyField(serializedObject.FindProperty("magicDamage"));
        //} 
        #endregion
    }

    void DisplayDefaultInfo()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cash"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("brainCells"));
    }

    void DisplayDebugInfo()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isMobile"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("activateWaveSpawner"));
    }
}