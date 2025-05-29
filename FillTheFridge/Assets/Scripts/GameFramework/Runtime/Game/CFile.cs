/********************************************************************
   All Right Reserved By Leo
   Created:    2020/6/6 19:09:35
   File: 	   CFile.cs
   Author:     Leo

   Purpose:    �ļ�������
*********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CFile
{
    /// <summary>
    /// <para>���ַ��������ļ�·��ʱ</para>
    /// <para>��ȡ�ļ���</para>
    /// <para>����</para>
    /// <para>var str = "Assets/Resources/Prefab/Player/Player_1.prefab"</para>
    /// <para>str = str.GetFileName()</para>
    /// <para>Debug.Log(str)</para>
    /// <para></para>
    /// <para>�����Player_1</para>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFileName(string path)
    {
        int start = path.LastIndexOf('/');
        int end = path.LastIndexOf('.');

        if (end == -1)
        { end = path.Length; }

        return path.Substring(start + 1, end - start - 1);
    }

    /// <summary>
    /// <para>���ַ��������ļ�·��ʱ</para>
    /// <para>��ȡ������׺�����ļ���</para>
    /// <para>����</para>
    /// <para>var str = "Assets/Resources/Prefab/Player/Player_1.prefab"</para>
    /// <para>str = str.GetFileFullName()</para>
    /// <para>Debug.Log(str)</para>
    /// <para></para>
    /// <para>�����Player_1.prefab</para>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFileNameWithType(string path)
    {
        int last = path.LastIndexOf('/');
        return path.Substring(last + 1);
    }

    /// <summary>
    /// <para>���ַ��������ļ�·��ʱ</para>
    /// <para>��ȡ��׺��</para>
    /// <para>����</para>
    /// <para>var str = "Assets/Resources/Prefab/Player/Player_1.prefab"</para>
    /// <para>str = str.GetFileType()</para>
    /// <para>Debug.Log(str)</para>
    /// <para></para>
    /// <para>�����prefab</para>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFileType(string path)
    {
        int lp = path.LastIndexOf('.');
        if (lp != -1)
            return path.Substring(lp + 1);
        else
            return "";
    }

    /// <summary>
    /// ��ȡ�ļ����������ļ�
    /// </summary>
    /// <param name="FolderPath">�ļ���·��</param>
    /// <param name="SearchPattern">�ļ������� Ĭ��*.*�����ļ�</param>
    /// <param name="SO">����ģʽ �ݹ� �� �ǵݹ� Ĭ�ϵݹ�����ļ�</param>
    public static List<string> GetFolderFiles(string FolderPath, string SearchPattern = "*.*", SearchOption SO = SearchOption.AllDirectories)
    {
        List<string> allPaths = new List<string>();
        DirectoryInfo DI = new DirectoryInfo(FolderPath);

        if (!DI.Exists)
        {
            return allPaths;
        }

        var files = DI.GetFiles(SearchPattern, SO);

        for (int i = 0; i < files.Length; i++)
        {
            var path = files[i].FullName.Replace("\\", "/");
            allPaths.Add(path);
        }

        return allPaths;
    }

    /// <summary>
    /// <para>���ַ��������ļ�·��ʱ</para>
    /// <para>��ȡ�ļ�����</para>
    /// <para>����</para>
    /// <para>var str = "Assets/Resources/Prefab/Player/Player_1.prefab"</para>
    /// <para>str = str.GetFolderName()</para>
    /// <para>Debug.Log(str)</para>
    /// <para></para>
    /// <para>�����Player</para>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFolderName(string path)
    {
        var fi = new FileInfo(path);
        var dname = fi.DirectoryName.Replace("\\", "/");
        var lastgang = dname.LastIndexOf("/") + 1;

        return dname.Substring(lastgang);
    }

    /// <summary>
    /// <para>��ȡ�ļ�·���е��ļ�����</para>
    /// <para>����</para>
    /// <para>var str = "Assets/Resources/Prefab/Player/Player_1.prefab"</para>
    /// <para>str = str.GetFolder()</para>
    /// <para>Debug.Log(str)</para>
    /// <para></para>
    /// <para>�����Assets/Resources/Prefab/Player</para>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFolderPath(string path)
    {
        int last = path.LastIndexOf('/');
        if (last < 0)
            return "";
        else
            return path.Substring(0, last);
    }

    /// <summary>
    /// ����ļ��Ƿ����
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsFileExist(string path)
    {
        return File.Exists(path);
    }

    /// <summary>
    /// ����ļ����Ƿ����
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsFolderExist(string path)
    {
        return Directory.Exists(path);
    }

    /// <summary>
    /// �����ߴ���ָ���ļ���
    /// </summary>
    /// <param name="path">Ҫ�����ļ���·��</param>
    public static void CheckCreateFolder(string path)
    {
        if (!IsFolderExist(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// ɾ��һ���ļ��������е����ļ�
    /// </summary>
    /// <param name="path">·��</param>
    public static void DeleteAllFileInFolder(string path)
    {
        var files = Directory.GetFiles(path);
        foreach (var file in files)
        {
            File.Delete(file);
        }
    }
}
