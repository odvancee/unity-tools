// MIT License - Copyright (c) odvancee
// Source: https://github.com/odvancee/unity-tools

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FoldedAttribute))]
public class FoldedDrawer : PropertyDrawer
{
    private Editor _cachedEditor;

    private static bool IsValidReference(SerializedProperty property)
    {
        if (property.propertyType != SerializedPropertyType.ObjectReference)
        {
            return false;
        }
        Object target = property.objectReferenceValue;
        return target is ScriptableObject || target is MonoBehaviour;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!IsValidReference(property))
        {
            EditorGUI.PropertyField(position, property, label, true);
            return;
        }

        EditorGUI.BeginProperty(position, label, property);

        Rect foldoutRect = new Rect(position.x - 2f, position.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
        Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        EditorGUI.PropertyField(labelRect, property, label, false);

        if (property.objectReferenceValue != null)
        {
            Rect labelClickRect = labelRect;
            labelClickRect.xMin += EditorGUIUtility.singleLineHeight;

            bool foldoutClicked = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none, true) != property.isExpanded;
            bool labelClicked = Event.current.type == EventType.MouseDown && labelClickRect.Contains(Event.current.mousePosition);

            if (foldoutClicked || labelClicked)
            {
                property.isExpanded = !property.isExpanded;
                Event.current.Use();
            }
        }

        if (!property.isExpanded || property.objectReferenceValue == null)
        {
            EditorGUI.EndProperty();
            return;
        }

        if (_cachedEditor == null || _cachedEditor.target != property.objectReferenceValue)
        {
            Editor.CreateCachedEditor(property.objectReferenceValue, null, ref _cachedEditor);
        }

        SerializedObject so = _cachedEditor.serializedObject;
        so.Update();

        Rect contentRect = EditorGUI.IndentedRect(position);
        contentRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        contentRect.height = EditorGUIUtility.singleLineHeight;

        int originalIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel++;

        SerializedProperty prop = so.GetIterator();
        prop.NextVisible(true);

        while (prop.NextVisible(false))
        {
            float propHeight = EditorGUI.GetPropertyHeight(prop, true);
            contentRect.height = propHeight;
            EditorGUI.PropertyField(contentRect, prop, true);
            contentRect.y += propHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        EditorGUI.indentLevel = originalIndent;
        so.ApplyModifiedProperties();
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!IsValidReference(property))
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        if (!property.isExpanded || property.objectReferenceValue == null)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        if (_cachedEditor == null || _cachedEditor.target != property.objectReferenceValue)
        {
            Editor.CreateCachedEditor(property.objectReferenceValue, null, ref _cachedEditor);
        }

        SerializedObject so = _cachedEditor.serializedObject;
        so.Update();

        SerializedProperty prop = so.GetIterator();
        prop.NextVisible(true);

        float totalHeight = EditorGUIUtility.singleLineHeight;

        while (prop.NextVisible(false))
        {
            totalHeight += EditorGUI.GetPropertyHeight(prop, true) + EditorGUIUtility.standardVerticalSpacing;
        }

        return totalHeight;
    }
}
#endif