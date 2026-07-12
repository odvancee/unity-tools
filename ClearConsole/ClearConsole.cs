// MIT No Attribution License - odvancee
// Source: https://github.com/odvancee/unity-tools

#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public static class ClearConsole
{
    [Shortcut("Clear Console", KeyCode.C, ShortcutModifiers.Control | ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
    public static void Clear()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
        Type type = assembly.GetType("UnityEditor.LogEntries");
        MethodInfo method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
#endif