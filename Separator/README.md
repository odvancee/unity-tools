## Separator

### Concept

A customizable editor-only hierarchy separator for organizing scenes without nesting game objects.

```
~~~~~~~~ UI ~~~~~~~~
Canvas
~~~~~~~~ ENVIRONMENT ~~~~~~~~
Directional Light
Walls
~~~~~~~~ DYNAMIC ~~~~~~~~
Projectile
```

Name Template: `{LEFT_DECORATOR}{FILLER}{name}{FILLER}{RIGHT_DECORATOR}`

### Features

- Destroys any components added to it
- Detaches itself from any parent
- Detaches children and places them below itself
- Uses the `EditorOnly` tag, which removes it from the build
- Customize `LEFT_DECORATOR`, `RIGHT_DECORATOR`, and `FILLER`
- Use `UpdateName` from a Separator's context menu to update its name after modifying decorative fields

### Usage

1. Create a separator (Menu: Assets > Create > Separator)
2. Rename it
3. Use it