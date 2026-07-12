// MIT License - Copyright (c) odvancee
// Source: https://github.com/odvancee/unity-tools

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ResetStaticsSystem
{
    private static List<Action> _methods;
    private const int CACHE_SIZE = 64;

    static ResetStaticsSystem()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.ExitingPlayMode)
        {
            CacheMethods();
            ExecuteResets();
        }
    }

    private static void CacheMethods()
    {
        _methods = new List<Action>(CACHE_SIZE);
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (Assembly assembly in assemblies)
        {
            string assemblyName = assembly.FullName;

            if (assemblyName.StartsWith("System") ||
                assemblyName.StartsWith("mscorlib") ||
                assemblyName.StartsWith("Mono") ||
                assemblyName.StartsWith("UnityEditor") ||
                assemblyName.StartsWith("UnityEngine"))
            {
                continue;
            }

            IEnumerable<Type> types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }

            foreach (Type type in types)
            {
                CacheMethod(type);
            }
        }
    }

    private static void CacheMethod(Type type)
    {
        const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        MethodInfo[] methods = type.GetMethods(flags);

        foreach (MethodInfo method in methods)
        {
            if (!method.IsDefined(typeof(ResetStaticsAttribute), false)) continue;
            if (!method.IsStatic) continue;
            if (method.ReturnType != typeof(void)) continue;
            if (method.GetParameters().Length != 0) continue;

            _methods.Add((Action)Delegate.CreateDelegate(typeof(Action), method));
        }
    }

    private static void ExecuteResets()
    {
        if (_methods == null || _methods.Count == 0) return;

        for (int i = 0; i < _methods.Count; i++)
        {
            try
            {
                _methods[i]?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
#endif