using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ClearData
{
    [MenuItem("Tools/--����û�����--")]
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
    [MenuItem("Tools/--������·��--")]
    public static void OpenData()
    {
        string path = Application.dataPath.Replace("Assets", "Data");
        path = path.Replace("/", "\\");
        if (Directory.Exists(path))
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

    }
    [MenuItem("Tools/--�򿪵ڶ�������·��--")]
    public static void OpenasData()
    {
        string path = Application.persistentDataPath;
        path = path.Replace("/", "\\");
        if (Directory.Exists(path))
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

    }
    [MenuItem("Tools/--����ڶ�������--")]
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
