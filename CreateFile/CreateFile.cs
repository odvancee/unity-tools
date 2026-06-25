#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public static class CreateFile
{
    [MenuItem("Assets/Create/File/Markdown", priority = 11)]
    private static void CreateMarkdown()
    {
        Create("NewMarkdownFile.md", string.Empty);
    }

    [MenuItem("Assets/Create/File/Txt", priority = 12)]
    private static void CreateTxt()
    {
        Create("NewTextFile.txt", string.Empty);
    }

    [MenuItem("Assets/Create/File/Json", priority = 13)]
    private static void CreateJson()
    {
        Create("NewJsonFile.json", string.Empty);
    }

    private static void Create(string fileName, string content)
    {
        string folderPath = GetSelectedPath();
        string filePath = Path.Combine(folderPath, fileName);
        CreateFileAction.Content = content;
        
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            ScriptableObject.CreateInstance<CreateFileAction>(),
            filePath,
            null,
            null
        );
    }

    private static string GetSelectedPath()
    {
        Object selectedObject = Selection.activeObject;

        if (selectedObject == null) return "Assets";

        string path = AssetDatabase.GetAssetPath(selectedObject);

        if (string.IsNullOrEmpty(path)) return "Assets";
            
        if (Directory.Exists(path)) return path;
            
        return Path.GetDirectoryName(path);
    }

    private sealed class CreateFileAction : EndNameEditAction
    {
        public static string Content;
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            File.WriteAllText(pathName, Content);
            AssetDatabase.ImportAsset(pathName);
            Object createdAsset = AssetDatabase.LoadAssetAtPath<Object>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(createdAsset);
        }
    }
}
#endif