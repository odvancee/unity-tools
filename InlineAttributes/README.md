## InlineAttributes

### Concept

Two attributes to draw ScriptableObject and MonoBehaviour fields inline in the Inspector.

- Use `[Expanded]` for always-expanded fields.
- Use `[Folded]` for fields with foldouts.

<img width="1280" height="509" alt="InlineAttributesDemo" src="https://github.com/user-attachments/assets/99f4a661-7b08-461e-8439-37d79a096270" />

### Usage

```csharp
[Expanded] public CustomSO Config;
[Folded] public CustomMB Behaviour;

[Expanded] [SerializeField] private CustomSO Config;
[Folded] [SerializeField] private CustomMB Behaviour;
```
