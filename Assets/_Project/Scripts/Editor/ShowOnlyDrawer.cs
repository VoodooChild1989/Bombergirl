using UnityEditor;
using UnityEngine;

// Place This Script In "Editor" (Assets->Editor) folder (If There Is No Such Folders Create It)
[CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
public class ShowOnlyDrawer : PropertyDrawer
{
    /// <summary>
    /// Hides a field
    /// For Example, [ShowOnly] public int health;
    /// </summary>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false; // Disable editing in the Inspector
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true; // Re-enable editing
    }
}