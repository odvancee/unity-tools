## InlineAttributes

### Concept

Two attributes to draw ScriptableObject and MonoBehaviour fields inline in the Inspector.

- Use `[Expanded]` for always-expanded fields.
- Use `[Folded]` for fields with foldouts.

### Usage

```csharp
[Expanded] public CustomSO Config;
[Folded] public CustomMB Behaviour;

[Expanded] [SerializeField] private CustomSO Config;
[Folded] [SerializeField] private CustomMB Behaviour;
```