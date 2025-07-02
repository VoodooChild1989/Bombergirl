using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameRestartButton : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw the script field explicitly
        SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(scriptProperty);
        EditorGUI.EndDisabledGroup();

        // Draw all serialized properties except the script field
        SerializedProperty property = serializedObject.GetIterator();
        property.NextVisible(true); // Skip the script field
        while (property.NextVisible(false))
        {
            EditorGUILayout.PropertyField(property, true);
        }

        // Add spacing before the button
        GUILayout.Space(20);

        // Add the restart button at the bottom
        GameManager myComponent = (GameManager)target;
        if (GUILayout.Button("RESTART THE GAME"))
        {
            myComponent.RestartTheGame();
        }

        // Apply any modified properties
        serializedObject.ApplyModifiedProperties();
    }
}
