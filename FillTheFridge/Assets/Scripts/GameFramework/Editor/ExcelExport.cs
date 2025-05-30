using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading;
using System.Text;
using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

public class ExcelExport
{
    [MenuItem("Tools/--导表--", false, 102)]
    public static async void OnOutLuaCsProto()
    {
        Thread thread = new Thread(() =>
        {
#if UNITY_EDITOR_WIN
            OnLuaAndBinFile();
            //#elif UNITY_EDITOR_OSX
            //            OnMacLuaAndBinFile();
#endif
        });

        await Task.Run(() => { thread.Start(); });
        AssetDatabase.Refresh();
    }
    private static void UpdateConfigManager(string name)
    {
        StringBuilder fileContent = new StringBuilder();
        fileContent.Append(configContent);
        if (!fileContent.ToString().Contains("Init"+name))
        {
            fileContent = fileContent.Replace("//---InitConfig---", "//---InitConfig---" + "\n"+"        "+ $"configTasks.Add(Init{name}());");
            File.WriteAllText(configPath, fileContent.ToString());
        }
    }

    private static void CreatProtoCode(string name)
    {
        //写入内容
        StringBuilder fileContent = new StringBuilder();
        fileContent.Append(_viewTemplateContent);
        string upname = name.Substring(0, 1).ToUpper() + name.Substring(1);
        fileContent = fileContent.Replace("DTlanguage", "DT" + name).Replace("Dtlanguage", "Dt" + name).Replace("GetlanguageConfigById", "Get" + name + "ConfigById").Replace("Initlanguage", "Init" + name).Replace("LanguageDefine", upname + "Define").Replace("Language", upname);
        string path = "Assets/Res/Config/ProtoScript/Cs" + name + "_ex.cs";
        if (!Directory.Exists(path))
        {
            File.WriteAllText(path, fileContent.ToString());
        }
    }
    public static void OnLuaAndBinFile()
    {
        Console.OutputEncoding = Encoding.GetEncoding("gbk");
        Process p = new Process();

        p.StartInfo.FileName = "cmd.exe";

        p.StartInfo.UseShellExecute = false;

        p.StartInfo.RedirectStandardInput = true;

        p.StartInfo.RedirectStandardOutput = true;

        p.StartInfo.RedirectStandardError = true;

        p.StartInfo.CreateNoWindow = true;

        p.Start();

        string path = @"tools\\Config";
        string outPath = "Assets\\Res\\Config\\ConfigBin\\";
        DirectoryInfo root = new DirectoryInfo(path);
        FileInfo[] files = root.GetFiles();

        //   p.StandardInput.WriteLine("tools\\tabtoy\\tabtoy.exe --mode=v2 --lua_out=Assets\\Lua\\App\\design_config\\Globals.lua tools\\Config\\Globals.xlsx");
        //   p.StandardInput.WriteLine("tools\\tabtoy\\tabtoy.exe --mode=v2 --csharp_out=Assets\\Res\\Config\\ConfigBin\\Globals.cs --binary_out=Assets\\Res\\Config\\ConfigBin\\Globals.bytes tools\\Config\\Globals.xlsx");

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Name.Split('.')[1] != "xlsx" /*|| files[i].Name.Split('.')[0] == "Globals"*/ ||
                files[i].Name.Split('.')[0].Substring(0, 2) == "~$") continue;
            string file_out_name = files[i].Name.Split('.')[0];

            //p.StandardInput.WriteLine("tools\\tabtoy\\tabtoy.exe --mode=v2 --lua_out=Assets\\Lua\\App\\design_config\\" +
            //    file_out_name + ".lua tools\\Config\\" + files[i].Name);

            //if (files[i].Name.IndexOf("CS") >= 0 || files[i].Name.IndexOf("cs") >= 0)
            //{
            //    var package = files[i].Name.Split('.')[0].Split('_')[1];
            //    package = "DT" + package;

            //    p.StandardInput.WriteLine("tools\\tabtoy\\tabtoy.exe --mode=v2 --package=" + package + " --csharp_out=" + outPath + file_out_name + ".cs " +
            //        "--binary_out=" + outPath + file_out_name + ".bytes tools\\Config\\" + files[i].Name);
            //}
            var package = files[i].Name.Split('.')[0].Split('_')[1];
            package = "DT" + package;

            p.StandardInput.WriteLine("tools\\tabtoy\\tabtoy.exe --mode=v2 --package=" + package + " --csharp_out=" + outPath + file_out_name + ".cs " +
                "--binary_out=" + outPath + file_out_name + ".bytes tools\\Config\\" + files[i].Name);

            CreatProtoCode(files[i].Name.Split('.')[0].Split('_')[1]);
            UpdateConfigManager(files[i].Name.Split('.')[0].Split('_')[1]);
        }

        p.StandardInput.WriteLine("exit");

        string strOuput = p.StandardOutput.ReadToEnd();
        p.Close();
        UnityEngine.Debug.LogWarning(strOuput);
    }


    /// <summary>
    /// 获取文件内容
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    private static string GetFileContent(string filePath)
    {
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);

        }
        else
        {
            UnityEngine.Debug.Log("Error:[" + filePath + "]该文件不存在！");
            return null;
        }
    }
    /// <summary>
    /// 模板类脚本路径
    /// </summary>
    private static string _viewTemplatePath = "Assets/Res/Config/ProtoScript/CsLanguage_Ex.cs";
    /// <summary>
    /// 模板类脚本内容
    /// </summary>
    private static string _viewTemplateContent
    {
        get { return GetFileContent(_viewTemplatePath); }
    }
    /// <summary>
    /// configmanager脚本路径
    /// </summary>
    private static string configPath = "Assets/Res/Config/ConfigManager.cs";
    /// <summary>
    /// configmanager脚本内容
    /// </summary>
    private static string configContent
    {
        get { return GetFileContent(configPath); }
    }


}
