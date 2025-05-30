//using LuaInterface;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using UnityEditor;

using UnityEngine;
using UnityEngine.Networking;

#if UNITY_EDITOR

public class Tabtoy
{
    [MenuItem("Tools/Tabtoy/Lua", false, 102)]
    public static void OnTabtoy_lua()
    {
        OnOutFile(FileType.lua);
    }

    [MenuItem("Tools/Tabtoy/Json", false, 102)]
    public static void OnTabtoy_json()
    {
        OnOutFile(FileType.json);
    }

    [MenuItem("Tools/Tabtoy/cs", false, 102)]
    public static void OnTabtoy_cs()
    {
        OnOutFile(FileType.cs);
    }

    [MenuItem("Tools/Tabtoy/Protobuf", false, 102)]
    public static void OnTabtoy_protobuf()
    {
        OnOutFile(FileType.Protobuf);
    }

    [MenuItem("Tools/Tabtoy/ProtobufBin", false, 102)]
    public static void OnTabtoy_ProtobufBin()
    {
        OnOutFile(FileType.ProtobufBin);
    }

    [MenuItem("Tools/Tabtoy/Protobuf 文本格式", false, 102)]
    public static void OnTabtoy_protobf_pbt()
    {
        OnOutFile(FileType.Protobuf_pbt);
    }

    [MenuItem("Tools/Tabtoy/bin", false, 102)]
    public static void OnTabtoy_bin()
    {
        OnOutFile(FileType.bin);
    }

    [MenuItem("Tools/Tabtoy/go", false, 102)]
    public static void OnTabtoy_go()
    {
        OnOutFile(FileType.go);
    }

    public enum FileType
    {
        lua,
        json,
        cs,
        Protobuf,
        ProtobufBin,
        Protobuf_pbt,
        bin,
        go,
    }

    public static void OnOutFile(FileType type) {
        #if UNITY_EDITOR_WIN 
        OnWindowOutFile(FileType.json);
        #elif UNITY_EDITOR_OSX 
        OnMacOutFile(FileType.json);
        #endif
    }
    /// <summary>
    /// 在Mac上转化数据
    /// </summary>
    /// <param name="type">转化的数据类型</param>
    public static void OnMacOutFile(FileType type)
    {
        Console.OutputEncoding = Encoding.GetEncoding("gbk");
        Process p = new Process();
        //设置要启动的应用程序
        p.StartInfo.FileName = "/bin/bash";
        //是否使用操作系统shell启动
        p.StartInfo.UseShellExecute = false;
        // 接受来自调用程序的输入信息
        p.StartInfo.RedirectStandardInput = true;
        //输出信息
        p.StartInfo.RedirectStandardOutput = true;
        // 输出错误
        p.StartInfo.RedirectStandardError = true;
        //显示程序窗口
        p.StartInfo.CreateNoWindow = true;
        p.Start();
        //启动程序
        string path = @"tools/Config";
        string outPath = "/Lua/App/design_config/";
        DirectoryInfo root = new DirectoryInfo(path);
        FileInfo[] files = root.GetFiles();
        for (int i = 0; i < files.Length; i++)
        {
            string file_out_name = files[i].Name.Split('.')[0];

            if (type == FileType.lua)
            {
                p.StandardInput.WriteLine("tools/tabtoy/./tabtoy --mode=v2 --binary_out=Assets" + 
                outPath + file_out_name + ".lua tools/Config/" + files[i].Name);
            }
            else if (type == FileType.json)
            {
                p.StandardInput.WriteLine("tools/tabtoy/./tabtoy --mode=v2 --json_out=Assets" +
                   outPath + file_out_name + ".json tools/Config/" + files[i].Name);
            }
            else if (type == FileType.bin)
            {
                p.StandardInput.WriteLine("tools/tabtoy/./tabtoy --mode=v2 --binary_out=Assets" +
                    outPath + file_out_name + ".bin tools/Config/" + files[i].Name);
            }
            else if (type == FileType.cs)
            {
                p.StandardInput.WriteLine("tools/tabtoy/./tabtoy --mode=v2 --csharp_out=Assets" +
                    outPath + file_out_name + ".cs tools/Config/" + files[i].Name);
            }
            else if (type == FileType.go)
            {
                p.StandardInput.WriteLine("tools/tabtoy/./tabtoy --mode=v2 --go_out=Assets" +
                    outPath + file_out_name + ".go tools/Config/" + files[i].Name);
            }
            else if (type == FileType.Protobuf)
            {
                p.StandardInput.WriteLine("tools/tabtoy/./tabtoy --mode=v2 -proto_out=Assets" +
                    outPath + file_out_name + ".proto tools/Config/" + files[i].Name);
            }
            else if (type == FileType.ProtobufBin)
            {
                p.StandardInput.WriteLine("tools/tabtoy/./tabtoy --mode=v2 -index=tools/Config/" + files[i].Name +
                    " -pbbin_out=Assets" + outPath + file_out_name + ".pbb");
            }
            else if (type == FileType.Protobuf_pbt)
            {
                p.StandardInput.WriteLine("tools/tabtoy/./tabtoy --mode=v2 --pbt_out=Assets" +
                    outPath + file_out_name + ".pbt tools/Config/" + files[i].Name);
            }
        }
        //结束执行
        p.StandardInput.WriteLine("exit");
        //向cmd窗口发送输入信息
        p.StandardInput.AutoFlush = true;
        p.WaitForExit();
        

        string strOuput = p.StandardOutput.ReadToEnd();
        UnityEngine.Debug.Log("p.Close();");
        p.Close();
        UnityEngine.Debug.Log(strOuput);
    }

    public static void OnWindowOutFile(FileType type)
    {
        Console.OutputEncoding = Encoding.GetEncoding("gbk");
        Process p = new Process();
        //设置要启动的应用程序
        p.StartInfo.FileName = "cmd.exe";
        //是否使用操作系统shell启动
        p.StartInfo.UseShellExecute = false;
        // 接受来自调用程序的输入信息
        p.StartInfo.RedirectStandardInput = true;
        //输出信息
        p.StartInfo.RedirectStandardOutput = true;
        // 输出错误
        p.StartInfo.RedirectStandardError = true;
        //不显示程序窗口
        p.StartInfo.CreateNoWindow = true;
        //启动程序
        p.Start();

        string path = @"tools\\Config";
        string outPath = "\\Lua\\App\\design_config\\";
        DirectoryInfo root = new DirectoryInfo(path);
        FileInfo[] files = root.GetFiles();
        for (int i = 0; i < files.Length; i++)
        {
            string file_out_name = files[i].Name.Split('.')[0];

            if (type == FileType.lua)
            {
                p.StandardInput.WriteLine("tools\\tabtoy\\tabtoy.exe --mode=v2 --lua_out=Assets" +
                    outPath + file_out_name + ".lua tools\\Config\\" + files[i].Name);
            }
            else if (type == FileType.json)
            {
                p.StandardInput.WriteLine("tools\\tabtoy\\tabtoy.exe --mode=v2 --json_out=Assets" +
                   outPath + file_out_name + ".json tools\\Config\\" + files[i].Name);
            }
            else if (type == FileType.bin)
            {
                p.StandardInput.WriteLine("tools\\tabtoy\\tabtoy.exe --mode=v2 --binary_out=Assets" +
                    outPath + file_out_name + ".bin tools\\Config\\" + files[i].Name);
            }
            else if (type == FileType.cs)
            {
                p.StandardInput.WriteLine("tools\\tabtoytabtoy.exe --mode=v2 --csharp_out=Assets" +
                    outPath + file_out_name + ".cs tools\\Config\\" + files[i].Name);
            }
            else if (type == FileType.go)
            {
                p.StandardInput.WriteLine("tools\\tabtoy\\tabtoy.exe --mode=v2 --go_out=Assets" +
                    outPath + file_out_name + ".go tools\\Config\\" + files[i].Name);
            }
            else if (type == FileType.Protobuf)
            {
                p.StandardInput.WriteLine("tools\\tabtoy\\tabtoy.exe --mode=v2 -proto_out=Assets" +
                    outPath + file_out_name + ".proto tools\\Config\\" + files[i].Name);
            }
            else if (type == FileType.ProtobufBin)
            {
                //p.StandardInput.WriteLine("tools\\tabtoy\\tabtoy.exe --mode=v2 -pbbin_out=Assets" +
                //    outPath + file_out_name + ".pbb tools\\Config\\" + files[i].Name);

                p.StandardInput.WriteLine("tools\\tabtoy\\tabtoy.exe --mode=v2 -index=tools\\Config\\" + files[i].Name +
                    " -pbbin_out=Assets" + outPath + file_out_name + ".pbb");
            }
            else if (type == FileType.Protobuf_pbt)
            {
                p.StandardInput.WriteLine("tools\\tabtoy\\tabtoy.exe --mode=v2 --pbt_out=Assets" +
                    outPath + file_out_name + ".pbt tools\\Config\\" + files[i].Name);
            }
        }

        //结束执行
        p.StandardInput.WriteLine("exit");
        //向cmd窗口发送输入信息
        p.StandardInput.AutoFlush = true;
        p.WaitForExit();

        string strOuput = p.StandardOutput.ReadToEnd();
        p.Close();
        UnityEngine.Debug.Log(strOuput);
    }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR

    /// <summary>
    /// 清理玩家数据  位置随便放了...
    ///
    /// 付正邦 2021-02-05  这个接口不要使用， 会清除玩家的所有数据（包含玩家的所有服务器数据）
    /// </summary>
    [MenuItem("Tools/ClearUserData", false, 103)]
    public static void ClearUserData()
    {
        if (Application.isPlaying)
        {
            UnityEngine.Debug.Log("不能在运行环境下运行.");
            return;
        }

        if (PlayerPrefs.HasKey("TDUserId"))
        {
            UnityEngine.Debug.Log("https://st1.sagigame.net:8010/v1/manage/user/clear?inviteCode=" + PlayerPrefs.GetInt("TDUserId"));
            UnityWebRequest ww = new UnityWebRequest("https://st1.sagigame.net:8010/v1/manage/user/clear?inviteCode=" + PlayerPrefs.GetInt("TDUserId"), UnityWebRequest.kHttpVerbPOST);
            ww.SendWebRequest();

            while (true)
            {
                if (ww.isDone)
                    break;
            }
            if (ww.error != null)
            {
                UnityEngine.Debug.Log(ww.error + "  error");
            }
            else
            {
                UnityEngine.Debug.Log(ww.responseCode);
                //if (ww.downloadHandler != null)
                //    UnityEngine.Debug.Log(ww.downloadHandler.text);
            }

            var path = "Assets/StreamingAssets/" + ".db_cache";
            DirectoryInfo direction = new DirectoryInfo(path);
            if (direction.Exists)
            {
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    string FilePath = path + "\\" + files[i].Name;
                    File.Delete(FilePath);
                }
            }
        }
    }

#endif

    public static void WriteFile(byte[] bytes, string path)
    {
        using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
        {
            using (BinaryReader br = new BinaryReader(fs))
            {
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
                UnityEngine.Debug.Log("写入文件完成" + path + " length:" + bytes.Length);
            }
        }
    }
}

// public class CsToLua
// {
//     protected LuaState luaState = null;
//     private LuaFunction levelLoaded = null;

//     public CsToLua()
//     {
//         luaState = new LuaState();
//         luaState.Start();
//         luaState.DoFile("OnConfigTools.lua");
//         levelLoaded = luaState.GetFunction("OnGetPototByte");

//         //levelLoaded.BeginPCall();
//         //levelLoaded.Push(level);
//         //levelLoaded.PCall();
//         //levelLoaded.EndPCall();
//     }

//     public static void Test()
//     {
//     }

//     public void Start(string str, string name, string path)
//     {
//         levelLoaded.BeginPCall();
//         levelLoaded.Push(str);
//         levelLoaded.Push(name);
//         levelLoaded.PCall();
//         byte[] ret1 = levelLoaded.CheckValue<byte[]>();
//         levelLoaded.EndPCall();

//         Tabtoy.WriteFile(ret1, path);
//     }
// }

#endif