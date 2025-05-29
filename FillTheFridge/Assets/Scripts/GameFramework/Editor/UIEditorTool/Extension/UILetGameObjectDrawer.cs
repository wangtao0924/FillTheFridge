using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(UILetGameObject))]
public class UILetGameObjectDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            EditorGUIUtility.labelWidth = 60;
            position.height = EditorGUIUtility.singleLineHeight;
            var uiGameNameRect = new Rect(position)
            {
                y = position.y + EditorGUIUtility.singleLineHeight + 5,
            };
            var uiGameObjectRect = new Rect(uiGameNameRect)
            {
                y = uiGameNameRect.y + EditorGUIUtility.singleLineHeight + 5,

            };

            SerializedProperty uiGameNameProperty = property.FindPropertyRelative("uiName");
            SerializedProperty uiGameObjectProperty = property.FindPropertyRelative("uiGameObj");

            uiGameNameProperty.stringValue = EditorGUI.TextField(uiGameNameRect, uiGameNameProperty.displayName, uiGameNameProperty.stringValue);
            uiGameObjectProperty.objectReferenceValue = EditorGUI.ObjectField(uiGameObjectRect, uiGameObjectProperty.objectReferenceValue,typeof(GameObject),true);
            uiGameNameProperty.stringValue = uiGameObjectProperty.objectReferenceValue.name;
        }
    }
}
