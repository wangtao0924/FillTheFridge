using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(UILet), true)]
[CanEditMultipleObjects]
public class UILetEditor : SelectableEditor
{
    private ReorderableList uiGameObjectLst;
    protected override void OnEnable()
    {
        uiGameObjectLst = new ReorderableList(serializedObject,serializedObject.FindProperty("uiGameObject"),true,true,true,true);
        uiGameObjectLst.drawHeaderCallback = (Rect rect) => {
            GUI.Label(rect,"UIÔªËØ");
        };
        uiGameObjectLst.elementHeight = 80;

        uiGameObjectLst.drawElementCallback = (Rect rect, int index, bool selected, bool focused) => {
            SerializedProperty item = uiGameObjectLst.serializedProperty.GetArrayElementAtIndex(index);
            rect.height -= 4;
            rect.y += 2;
            EditorGUI.PropertyField(rect,item,new GUIContent("Index"+index));
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        uiGameObjectLst.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
