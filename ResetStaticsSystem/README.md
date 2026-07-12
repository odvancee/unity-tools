## ResetStaticsSystem

### Concept

An editor-only centralized utility that resets static fields on both entering and exiting Play Mode (especially useful with domain reload disabled).

For [Unity 6.5 and above](https://discussions.unity.com/t/path-to-coreclr-2026-upgrade-guide/1714279#p-7279285-code-reload-12), consider using the built-in [\[AutoStaticsCleanup\]](https://docs.unity3d.com/6000.5/Documentation/ScriptReference/Unity.Scripting.LifecycleManagement.AutoStaticsCleanupAttribute.html) attribute instead to reset static fields.

### Features

- Reset occurs on entering and exiting Play Mode, preventing static state from leaking in both directions[^1].
- An explicit[^2] custom `[ResetStatics]` attribute.

[^1]: [`SubsystemRegistration`](https://docs.unity3d.com/6000.5/Documentation/ScriptReference/RuntimeInitializeLoadType.SubsystemRegistration.html) methods run only on entering Play Mode, leaking runtime static state into Editor.
[^2]: [`SubsystemRegistration`](https://docs.unity3d.com/6000.5/Documentation/ScriptReference/RuntimeInitializeLoadType.SubsystemRegistration.html) methods are too general to indicate static reset intent.

### Usage

1. Mark a `static void` parameterless method with the `[ResetStatics]` attribute.
2. Assign your static fields the same value they have in their initializers.

```csharp
public static int Counter = 0;
public static event Action StaticEvent;

[ResetStatics]
private static void ResetStatics()
{
    Counter = 0;
    StaticEvent = null;
}
```

### Limitations

- You must manually reset fields to their initial values inside the reset method.
- You must assign the same value both in the field initializer and inside the reset method, because the reset method's assignment overrides the initializer at runtime.

```csharp
public static int Counter = 0; // Lost

[ResetStatics]
private static void ResetStatics() => Counter = 1; // Runtime: 1
```