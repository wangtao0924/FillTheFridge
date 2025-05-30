using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ClearData
{
    [MenuItem("Tools/--清除用户数据--")]
    public static void Clear()
    {
        string path = Application.dataPath.Replace("Assets", "Data");
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
            File.Delete(path + ".meta");
        }
        AssetDatabase.Refresh();
    }
    [MenuItem("Tools/--打开数据路径--")]
    public static void OpenData()
    {
        string path = Application.dataPath.Replace("Assets", "Data");
        path = path.Replace("/", "\\");
        if (Directory.Exists(path))
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

    }
    [MenuItem("Tools/--打开第二个数据路径--")]
    public static void OpenasData()
    {
        string path = Application.persistentDataPath;
        path = path.Replace("/", "\\");
        if (Directory.Exists(path))
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

    }
    [MenuItem("Tools/--清除第二个数据--")]
    public static void Clesadar()
    {
        string path = Application.persistentDataPath;
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
            File.Delete(path + ".meta");
        }
        AssetDatabase.Refresh();
    }
}
