// MIT License - Copyright (c) odvancee
// Source: https://github.com/odvancee/unity-tools

using System;
using UnityEngine;

/// <summary>
/// Draws the referenced ScriptableObject or MonoBehaviour inline fully expanded.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class ExpandedAttribute : PropertyAttribute { }