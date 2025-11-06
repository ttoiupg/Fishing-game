// Copyright (c) 2025 PinePie. All rights reserved.

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;

public static class PathUtility
{
    private static string GetAnchorFilePath()
    {
        var tempInstance = ScriptableObject.CreateInstance<AnchorSO>();
        string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(tempInstance));

        Object.DestroyImmediate(tempInstance);

        return string.IsNullOrEmpty(scriptPath)
            ? string.Empty
            : Path.GetDirectoryName(scriptPath);
    }

    public static string GetPieTabsPath()
    {
        string originalPath = GetAnchorFilePath();

        originalPath = originalPath.Replace("\\", "/");

        string[] parts = originalPath.Split('/');

        // last no. to know how much anchor file is deep in folders
        return string.Join("/", parts, 0, parts.Length - 5);
    }

    public static string GetFullPathFromRelative(string relativePath)
    {
        return GetPieTabsPath() + "/" + relativePath;
    }
}

#endif