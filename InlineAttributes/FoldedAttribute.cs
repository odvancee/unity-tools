// MIT License - Copyright (c) odvancee
// Source: https://github.com/odvancee/unity-tools

using System;
using UnityEngine;

/// <summary>
/// Draws the referenced ScriptableObject or MonoBehaviour inline with a foldout.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class FoldedAttribute : PropertyAttribute { }