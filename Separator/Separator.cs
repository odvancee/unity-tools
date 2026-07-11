// MIT License - Copyright (c) odvancee
// Source: https://github.com/odvancee/unity-tools

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad, ExecuteAlways, DisallowMultipleComponent, AddComponentMenu("")]
public class Separator : MonoBehaviour
{
    private static readonly string DEFAULT_NAME = "SEPARATOR";
    private static readonly string LEFT_DECORATOR = "~~~~~~~~";
    private static readonly string RIGHT_DECORATOR = "~~~~~~~~";
    private static readonly string FILLER = " ";
    private static readonly HashSet<Separator> _allSeparators = new();
    private static readonly char[] _trimmedChars = new char[] { '=', '~', '-', '_', '@', '%', '#', '*', '+', '$', '<', '>', '`', ' ', '\t' };

    static Separator() => EditorApplication.hierarchyChanged += EnforceRestrictionsOnAll;
    private void OnValidate() => EnforceRestrictions();
    private void Awake() => _allSeparators.Add(this);
    private void OnEnable() => _allSeparators.Add(this);
    private void OnDestroy() => _allSeparators.Remove(this);

    private static void EnforceRestrictionsOnAll()
    {
        foreach (Separator separator in _allSeparators)
        {
            if (separator != null && separator.gameObject != null)
            {
                separator.EnforceRestrictions();
            }
        }
    }

    // Destroys added components, keeps readonly Transform and Separator
    private void EnforceRestrictions()
    {
        if (gameObject == null) return;

        Component[] components = GetComponents<Component>();

        for (int i = components.Length - 1; i >= 0; i--)
        {
            Component comp = components[i];
            comp.hideFlags = HideFlags.NotEditable;
            
            if (comp is Transform || comp is Separator) continue;

            DestroyImmediate(comp);
            Debug.LogWarning($"Cannot add {comp.GetType().Name} to an {nameof(Separator)} object. It has been destroyed.", gameObject);
        }
    }

    // Detaches children of a Separator
    private void OnTransformChildrenChanged()
    {
        if (transform.childCount == 0) return;

        EditorApplication.delayCall += () =>
        {
            int baseIndex = transform.GetSiblingIndex();
            int childCount = transform.childCount;
            Transform[] children = new Transform[childCount];

            for (int i = 0; i < childCount; i++)
            {
                children[i] = transform.GetChild(i);
            }

            for (int i = 0; i < childCount; i++)
            {
                children[i].SetParent(null, true);
                children[i].SetSiblingIndex(baseIndex + 1 + i);
            }
        };
    }

    // Clears a parent of a Separator
    private void OnTransformParentChanged()
    {
        Transform parent = transform.parent;
        if (parent == null || parent.TryGetComponent<Separator>(out _)) return;

        EditorApplication.delayCall += () =>
        {
            if (transform.parent != parent) return; // Execute once for several reparented separators

            int baseIndex = parent.GetSiblingIndex();
            List<Transform> separatorsToDetach = new List<Transform>();

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.TryGetComponent<Separator>(out _))
                {
                    separatorsToDetach.Add(child);
                }
            }

            for (int i = 0; i < separatorsToDetach.Count; i++)
            {
                separatorsToDetach[i].SetParent(null, true);
                separatorsToDetach[i].SetSiblingIndex(baseIndex + 1 + i);
            }
        };
    }

    [MenuItem("GameObject/Separator", priority = 13)]
    public static GameObject Create()
    {
        return CreateSeparator(GetDecoratedName(DEFAULT_NAME));
    }

    public static GameObject Create(string name, bool decorateName = true)
    {
        string finalName = decorateName ? GetDecoratedName(name) : name;
        return CreateSeparator(finalName);
    }

    private static GameObject CreateSeparator(string finalName)
    {
        GameObject separator = new GameObject();
        separator.tag = "EditorOnly";
        separator.name = finalName;
        separator.AddComponent<Separator>().hideFlags = HideFlags.NotEditable;
        separator.transform.hideFlags = HideFlags.NotEditable;
        return separator;
    }

    [ContextMenu(nameof(UpdateName))]
    private void UpdateName()
    {
        Undo.RecordObject(gameObject, "Update Separator name");
        string trimmedName = gameObject.name.Trim(_trimmedChars);
        trimmedName = string.IsNullOrWhiteSpace(trimmedName) ? DEFAULT_NAME : trimmedName;
        gameObject.name = GetDecoratedName(trimmedName);
    }

    private static string GetDecoratedName(string name) => $"{LEFT_DECORATOR}{FILLER}{name}{FILLER}{RIGHT_DECORATOR}";
}
#endif