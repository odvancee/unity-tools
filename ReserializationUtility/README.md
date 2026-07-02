## ReserializationUtility

### Concept:

Forces reserialization of assets to update YAML data without manually marking objects dirty.

- Clean up `[FormerlySerializedAs]` attributes safely
- Reduce long-term VCS noise after Unity Editor upgrades
- Reduce the risk of breaking changes

### Features

- **Reserialize Selected**: Reserializes selected supported files or files within a selected folder recursively.
- **Reserialize Supported**: Reserializes all supported files in `Assets/`.
- **Reserialize Project**: Reserializes all files in `Assets/` and `Packages/`.
- Adjust the `SUPPORTED_TYPES` to filter specific asset extensions (does not affect the 'Reserialize Project' option).

### Explanation (Short)

The `[FormerlySerializedAs]` attribute preserves a variable's data when you rename it. Unity will not update the underlying YAML files of SOs, prefabs and scenes until you mark them 'dirty' and save them. The process of marking objects dirty is error-prone and time-consuming.

This tool allows you to manually reserialize your objects without the 'marking dirty' process. It explicitly actualizes the variable names in YAML, allowing you to safely remove the `[FormerlySerializedAs]`-related code and keep your codebase clean.

### Explanation (Long)

Imagine you have a Prefab with a MonoBehaviour script containing the serializable `int poorInt` field. This field's name and data will be written into the `.prefab` YAML file.

Now, you want to fix the name of that field, making it `int _coolInt`. To prevent data loss, you apply the `[FormerlySerializedAs("poorInt")]` attribute, so that Unity could map the old variable name to the new one.

Whenever you mark an object dirty (e.g. modify its components/tags/layers) and then save it, Unity serializes it. As for your `.prefab` file, that MonoBehaviour's YAML block will be modified in one of two ways:

- No FSA: `poorInt: 5` becomes `_coolInt: 0` (previous variable is removed, new variable is added, data is lost).
- W/ FSA: `poorInt: 5` becomes `_coolInt: 5` (previous variable is mapped to the new one, data is preserved).

Now, if you've used the FSA attribute, you can remove it and the associated `UnityEngine.Serialization` dependency.

BUT HERE'S THE GOTCHA:

- If you've used the FSA attribute but skipped the manual 'mark dirty and save' routine, removing the attribute becomes a breaking change that results in immediate data loss.
- The manual inspector 'mark dirty' process is error prone (e.g. saved a wrong value[^1] or overlooked other assets[^2]).
- For codebase and VCS clarity, you don't want to keep FSA, hoping someone someday will mark that object dirty during normal development.

[^1]: You can't just save an asset in Unity. You have to modify its tag, a layer or any of its fields - only then you can save it. You can forget to revert these temporary changes.
[^2]: Several prefabs (and plain scene game objects) can use that one MonoBehaviour. If you've 'dirtied and saved' only one of those prefabs and then removed the FSA attribute, the rest of the prefabs will lose their data, as they have never been reserialized.

This is where this tool shines. It allows you to explicitly tell Unity to reserialize certain assets, actualizing all the variable names. After that, you can safely remove the FSA attribute code, without worrying about data loss.

Imagine you are working on a legacy-project with thousands of poorly named variables in hundreds of different MonoBehaviour components you have to refactor. You're renaming them and using the FSA attribute to preserve data. After that, you have to manually set dirty and save ALL the objects using those hundreds of components. Imagine the amount of time it would take to do that and the chance of making a mistake. With this tool, you can use the 'Reserialize Supported' option, updating all of the supported assets' YAML files. After that, you can safely remove all the FSA code from your codebase.

### Instructions

- Use 'Reserialize' for granular reserialization, especially with single-instance components.
- **Use 'Reserialize Supported' 90% of the time, with widely-used MonoBehaviours and SOs.**
- Use 'Reserialize Project' immediately after Unity Editor upgrades to reduce VCS noise *in a long run*.

| Option                  | Types             | `Assets/` | `Packages/` |
| ----------------------- | ----------------- | --------- | ----------- |
| Reserialize             | `SUPPORTED_TYPES` | +         | +           |
| Reserialize Supported   | `SUPPORTED_TYPES` | +         | -           |
| Reserialize Project     | All               | +         | +           |
| `ReserializeAssets` API | `SUPPORTED_TYPES` | +         | +           |

### VCS noise

Running 'Reserialize Supported' or 'Reserialize Project' the first time or after Unity Editor upgrade will likely generate VCS noise from diverse domains. Ensure you have a clean VCS working directory before using these options.

In terms of Unity Editor upgrades, these commands cause the *short-term* VCS noise, but remove it *in a long run*: Unity updates YAML files lazily, when their assets are 'dirtied and saved', and `git status` gets cluttered with unrelated YAML changes as you work. Forcing a restructuring of the YAML files immediately after an upgrade ensures that all future commits contain only functional code changes.

### Usage:

- Menu: Assets > Reserialize > Reserialize Project
- Menu: Assets > Reserialize > Reserialize Supported
- Menu: Assets > Reserialize > Reserialize (Selected assets/folders)
- Menu: Edit > Clear Reserialization Prefs (Restores warning dialogs)