#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class EditorWindowUtility
{
    [Shortcut("Window/Close Focused Window", KeyCode.W, ShortcutModifiers.Control)]
    private static void CloseTab(ShortcutArguments args)
    {
        var window = EditorWindow.focusedWindow;

        if (window != null)
        {
            EditorWindowHistory.RegisterClosed(window);
            EditorWindow.focusedWindow.Close();
        }
    }

    [Shortcut("Window/Reopen Closed Window", KeyCode.T, ShortcutModifiers.Control | ShortcutModifiers.Shift)]
    private static void Reopen(ShortcutArguments args)
    {
        EditorWindowHistory.ReopenLast();
    }
}

[InitializeOnLoad]
public static class EditorWindowHistory
{
    private static Stack<Type> closedWindows = new Stack<Type>();

    public static void RegisterClosed(EditorWindow window)
    {
        if (window == null) return;
        closedWindows.Push(window.GetType());
    }

    public static void ReopenLast()
    {
        if (closedWindows.Count == 0) return;
        var type = closedWindows.Pop();
        EditorWindow.GetWindow(type);
    }
}
#endif