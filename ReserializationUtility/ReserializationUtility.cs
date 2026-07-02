#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public class ReserializationUtility
{
    private const string NEVER_ASK_PROJECT_KEY = "ReserializationUtility_SkipReserializeProjectDialog";
    private const string NEVER_ASK_SUPPORTED_KEY = "ReserializationUtility_SkipReserializeSupportedDialog";
    private static readonly HashSet<string> SUPPORTED_TYPES = new(StringComparer.OrdinalIgnoreCase)
    {
        ".prefab", ".asset", ".unity"
    };
    private const string ITEM_PATH = "Assets/Reserialize/";

    /// <summary>
    /// Forces reserialization of the entire project with AssetDatabase.ForceReserializeAssets().
    /// </summary>
    [MenuItem(ITEM_PATH + "Reserialize Project", priority = 42)]
    public static void ReserializeProject()
    {
        if (!GetConfirmation("Reserialize Project?",
            "Rewrites all files in Assets/ and Packages/. May take a while and cause massive VCS noise.",
            NEVER_ASK_PROJECT_KEY)) return;

        Stopwatch sw = Stopwatch.StartNew();
        EditorUtility.DisplayProgressBar("Reserializing Project", "Processing...", 0.5f);

        try
        {
            AssetDatabase.ForceReserializeAssets();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{nameof(ReserializationUtility)}] Error during reserialization: {ex.Message}");
        }
        finally
        {
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }

        sw.Stop();
        Debug.Log($"[{nameof(ReserializationUtility)}] Project reserialized in {sw.Elapsed.TotalSeconds:F2}s.");
    }

    /// <summary>
    /// Forces reserialization of supported types in Assets/.
    /// </summary>
    [MenuItem(ITEM_PATH + "Reserialize Supported", priority = 41)]
    public static void ReserializeSupported()
    {
        if (!GetConfirmation("Reserialize Supported Assets?",
            $"Rewrites supported files in Assets/. May take a while and cause massive VCS noise.\nSupported: {string.Join(", ", SUPPORTED_TYPES)}",
            NEVER_ASK_SUPPORTED_KEY)) return;

        string[] guids = AssetDatabase.FindAssets("", new[] { "Assets" });
        IEnumerable<string> paths = guids
            .Distinct()
            .Select(AssetDatabase.GUIDToAssetPath);
            
        ReserializeAssets(paths);
    }

    [MenuItem(ITEM_PATH + "Reserialize", priority = 40)]
    public static void ReserializeSelected()
    {
        Object[] assets = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
        ReserializeAssets(assets);
    }

    [MenuItem(ITEM_PATH + "Reserialize", true)]
    private static bool ValidateReserializeSelected()
    {
        return Selection.GetFiltered<Object>(SelectionMode.DeepAssets)
            .Select(AssetDatabase.GetAssetPath)
            .Any(IsSupportedPath);
    }

    /// <summary>
    /// Forces reserialization of the specified supported assets.
    /// </summary>
    /// <param name="assets">UnityEngine.Object items to be reserialized.</param>
    public static void ReserializeAssets(IEnumerable<Object> assets)
    {
        if (assets == null) return;

        ReserializeAssets(assets.Select(AssetDatabase.GetAssetPath));
    }

    /// <summary>
    /// Forces reserialization of the supported assets at the specified paths.
    /// </summary>
    /// <param name="assetPaths">Paths to items to be reserialized.</param>
    public static void ReserializeAssets(IEnumerable<string> assetPaths)
    {
        if (assetPaths == null) return;

        string[] paths = assetPaths
            .Distinct()
            .Where(IsSupportedPath)
            .ToArray();

        if (paths.Length == 0)
        {
            Debug.Log($"[{nameof(ReserializationUtility)}] No supported assets found to reserialize.");
            return;
        }

        Stopwatch sw = Stopwatch.StartNew();
        ReserializeWithProgress("Reserializing Selected", paths);
        sw.Stop();

        Debug.Log($"[{nameof(ReserializationUtility)}] Reserialized {paths.Length} assets in {sw.Elapsed.TotalSeconds:F2}s.");
    }

    private static void ReserializeWithProgress(string title, string[] paths)
    {
        try
        {
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                string fileName = System.IO.Path.GetFileName(path);

                EditorUtility.DisplayProgressBar(title, $"{i + 1}/{paths.Length} - {fileName}", (float)i / paths.Length);
                AssetDatabase.ForceReserializeAssets(new[] { path });
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{nameof(ReserializationUtility)}] Error during reserialization: {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
        }
        finally
        {
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }
    }

    private static bool IsSupportedPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return false;

        string extension = System.IO.Path.GetExtension(path);
        return SUPPORTED_TYPES.Contains(extension);
    }

    private static bool GetConfirmation(string title, string message, string neverAskKey)
    {
        if (EditorPrefs.GetBool(neverAskKey, false)) return true;

        int option = EditorUtility.DisplayDialogComplex(title, message, "Proceed", "Cancel", "Never Ask");
        if (option == 1) return false;
        if (option == 2) EditorPrefs.SetBool(neverAskKey, true);
        return true;
    }

    [MenuItem("Edit/Clear Reserialization Prefs", priority = 15001)]
    private static void ClearPrefs()
    {
        EditorPrefs.DeleteKey(NEVER_ASK_PROJECT_KEY);
        EditorPrefs.DeleteKey(NEVER_ASK_SUPPORTED_KEY);
    }
}
#endif