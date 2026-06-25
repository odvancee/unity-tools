#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public static class ClearConsole
{
    [Shortcut("Clear Console", KeyCode.C, ShortcutModifiers.Control | ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
    public static void Clear()
    {
        var assembly = Assembly.GetAssembly(typeof(SceneView));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
#endif