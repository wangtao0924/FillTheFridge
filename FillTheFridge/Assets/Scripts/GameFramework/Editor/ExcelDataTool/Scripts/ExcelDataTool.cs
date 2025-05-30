using System.Collections.Generic;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine;
using UnityEditor;
using Excel;

namespace ExcelDataManager
{
    public class ExcelDataTool
    {
        private static string INPUT_PATH;
        public const string CODE_NAMESPACE = "ExcelDataClass";//由表生成的数据类型均在此命名空间内
        public const string BinDataFolder = "BinConfigData";//序列化的数据文件都会放在此文件夹内,此文件夹位于Resources文件夹下用于读取数据

        private static List<string> codeList;//存放所有生成的类的代码
        private static Dictionary<string, List<ConfigData[]>> dataDict;//存放所有数据表内的数据，key：类名  value：数据

        [UnityEditor.MenuItem("开发工具/导表工具/导入全部数据")]
        public static void LoadAllExcelData()
        {
            //if (!EditorUtility.DisplayDialog("注意！！！", "导表前要关闭打开的数据表，否则会失败，是否继续？", "继续", "取消")) return;

            INPUT_PATH = PlayerPrefs.GetString(System.Environment.CurrentDirectory + "ExcelDataInputPath", "");

            if (string.IsNullOrEmpty(INPUT_PATH))
            {
                ProgressBar.HideBarWithFailInfo("\n请先设置数据表路径！");
                throw new Exception("请先设置数据表路径！");
            }

            string[] files = Directory.GetFiles(INPUT_PATH, "*.xlsx");
            if (files == null || files.Length == 0)
            {
                EditorUtility.DisplayDialog("注意！！！", "\n暂无可以导入的数据表！", "确定");
                EditorUtility.ClearProgressBar();
                throw new Exception("暂无可以导入的数据表！");
            }
            if (codeList == null)
            {
                codeList = new List<string>();
            }
            else
            {
                codeList.Clear();
            }
            if (dataDict == null)
            {
                dataDict = new Dictionary<string, List<ConfigData[]>>();
            }
            else
            {
                dataDict.Clear();
            }
            float step = 1f;
            foreach (string item in files)
            {
                ProgressBar.UpdataBar("正在加载 " + item, step / files.Length * 0.4f);
                step++;
                GetExcelData(item);
            }
            if (codeList.Count == 0)
            {
                EditorUtility.DisplayDialog("注意！！！", "\n暂无可以导入的数据表！", "确定");
                EditorUtility.ClearProgressBar();
                throw new Exception("暂无可以导入的数据表！");
            }
            //编译代码,生成包含所有数据表内数据类型的dll
            Assembly assembly = CompileCode(codeList.ToArray());
            //准备序列化数据
            string BinDataPath = System.Environment.CurrentDirectory + "/Assets/Resources/" + BinDataFolder;//序列化后的数据存放路径
            if (Directory.Exists(BinDataPath)) Directory.Delete(BinDataPath, true);//删除旧的数据文件
            Directory.CreateDirectory(BinDataPath);
            step = 1;
            foreach (KeyValuePair<string, List<ConfigData[]>> each in dataDict)
            {
                ProgressBar.UpdataBar("序列化数据: " + each.Key, step / dataDict.Count * 0.6f + 0.399f);//0.399是为了进度条在生成所有代码以前不会走完显示完成弹窗
                step++;
                //Assembly.CreateInstance 方法 (String) 使用区分大小写的搜索，从此程序集中查找指定的类型，然后使用系统激活器创建它的实例化对象
                object container = assembly.CreateInstance(CODE_NAMESPACE + "." + each.Key);
                Type temp = assembly.GetType(CODE_NAMESPACE + "." + each.Key + "Item");
                //序列化数据
                Serialize(container, temp, each.Value, BinDataPath);
            }
            ProgressBar.UpdataBar("创建数据管理类: DataManager", 0.999f);
            ScriptGenerator.CreateDataManager(assembly);
            ProgressBar.UpdataBar("\n导表成功!", 1);
            AssetDatabase.Refresh();
            Debug.Log("<color=yellow>导表成功!</color>");
        }

        //数据表内每一格数据
        class ConfigData
        {
            public string Type;//数据类型
            public string Name;//字段名
            public string Data;//数据值
        }

        private static void GetExcelData(string inputPath)
        {
            FileStream stream = null;
            try
            {
                stream = File.Open(inputPath, FileMode.Open, FileAccess.Read,FileShare.Read);
            }
            catch
            {
                EditorUtility.DisplayDialog("注意！！！", "\n请关闭 " + inputPath + " 后再导表！", "确定");
                EditorUtility.ClearProgressBar();
                throw new Exception("请关闭 " + inputPath + " 后再导表！");
            }
            IExcelDataReader excelReader = null;
            if (inputPath.EndsWith(".xls")) excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            else if (inputPath.EndsWith(".xlsx")) excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            if (!excelReader.IsValid)
            {
                ProgressBar.HideBarWithFailInfo("\n无法读取的文件:  " + inputPath);
                EditorUtility.ClearProgressBar();
                throw new Exception("无法读取的文件:  " + inputPath);
            }
            else
            {
                do
                {
                    // sheet name
                    string className = excelReader.Name;
                    string[] types = null;//数据类型
                    string[] names = null;//字段名
                    List<ConfigData[]> dataList = new List<ConfigData[]>();
                    int index = 1;

                    //开始读取
                    while (excelReader.Read())
                    {
                        //这里读取的是每一行的数据
                        string[] datas = new string[excelReader.FieldCount];
                        for (int j = 0; j < excelReader.FieldCount; ++j)
                        {
                            datas[j] = excelReader.GetString(j);
                        }

                        //空行不处理
                        if (datas.Length == 0 || string.IsNullOrEmpty(datas[0]))
                        {
                            ++index;
                            continue;
                        }
                        //第4行表示类型
                        if (index == 4) types = datas;
                        //第5行表示变量名
                        else if (index == 5) names = datas;
                        //后面的表示数据
                        else if (index > 5)
                        {
                            //把读取的数据和数据类型,名称保存起来,后面用来动态生成类
                            List<ConfigData> configDataList = new List<ConfigData>();
                            for (int j = 0; j < datas.Length; ++j)
                            {
                                ConfigData data = new ConfigData();
                                data.Type = types[j];
                                data.Name = names[j];
                                data.Data = datas[j];
                                if (string.IsNullOrEmpty(data.Type) || string.IsNullOrEmpty(data.Data)) continue;//空的数据不处理
                                configDataList.Add(data);
                            }
                            dataList.Add(configDataList.ToArray());
                        }
                        ++index;
                    }
                    if (string.IsNullOrEmpty(className))
                    {
                        ProgressBar.HideBarWithFailInfo("\n空的类名（excel页签名）, 路径:  " + inputPath);
                        throw new Exception("空的类名（excel页签名）, 路径:  " + inputPath);
                    }
                    if (names != null && types != null)
                    {
                        //根据刚才的数据来生成C#脚本
                        ScriptGenerator generator = new ScriptGenerator(inputPath, className, names, types);
                        //所有生成的类的代码最终保存在这个链表中
                        codeList.Add(generator.Generate());
                        if (dataDict.ContainsKey(className))
                        {
                            ProgressBar.HideBarWithFailInfo("\n类名重复:" + className + " ,路径: " + inputPath);
                            throw new Exception("类名重复: " + className + " ,路径:  " + inputPath);
                        }
                        else dataDict.Add(className, dataList);
                    }
                }
                while (excelReader.NextResult());//excelReader.NextResult() Excel表下一个sheet页有没有数据
            }
            stream.Dispose();
            stream.Close();
        }

        //编译代码
        private static Assembly CompileCode(string[] scripts)
        {
            string path = System.Environment.CurrentDirectory + "/Assets/Plugins/" + CODE_NAMESPACE;
            if (Directory.Exists(path)) Directory.Delete(path, true);//删除旧dll
            Directory.CreateDirectory(path);
            //编译器实例对象
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            //编译器参数实例对象
            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.AddRange(new string[] { "System.dll" });//添加程序集引用
            objCompilerParameters.OutputAssembly = path + "/" + CODE_NAMESPACE + ".dll";//设置输出的程序集名
            objCompilerParameters.GenerateExecutable = false;
            objCompilerParameters.GenerateInMemory = true;

            //开始编译脚本
            CompilerResults cr = codeProvider.CompileAssemblyFromSource(objCompilerParameters, scripts);
            if (cr.Errors.HasErrors)
            {
                ProgressBar.HideBarWithFailInfo("\n编译dll出错（详情见控制台）！");
                foreach (CompilerError err in cr.Errors)
                {
                    Debug.LogError(err.ErrorText);
                }
                throw new Exception("编译dll出错！");
            }
            Debug.Log("已生成 " + path + "/<color=#FFFF00>" + CODE_NAMESPACE + ".dll</color>");
            return cr.CompiledAssembly;
        }

        //序列化对象
        private static void Serialize(object container, Type temp, List<ConfigData[]> dataList,string BinDataPath)
        {
            //设置数据
            foreach (ConfigData[] datas in dataList)
            {
                //Type.FullName 获取该类型的完全限定名称，包括其命名空间，但不包括程序集。
                object t = temp.Assembly.CreateInstance(temp.FullName);
                foreach (ConfigData data in datas)
                {
                    //Type.GetField(String) 搜索Type内指定名称的公共字段。
                    FieldInfo info = temp.GetField(data.Name);
                    // FieldInfo.SetValue 设置对象内指定名称的字段的值
                    info.SetValue(t, ParseValue(data.Type, data.Data, temp.Name));
                }
                // FieldInfo.GetValue 获取对象内指定名称的字段的值
                object id = temp.GetField("id").GetValue(t);//获取id
                FieldInfo dictInfo = container.GetType().GetField("Dict");
                object dict = dictInfo.GetValue(container);

                bool isExist = (bool)dict.GetType().GetMethod("ContainsKey").Invoke(dict, new System.Object[] { id });
                if (isExist)
                {
                    EditorUtility.DisplayDialog("注意！！！", "ID重复：" + id + "，类型： " + container.GetType().Name, "确定");
                    throw new Exception("ID重复：" + id + "，类型： " + container.GetType().Name);
                }
                dict.GetType().GetMethod("Add").Invoke(dict, new System.Object[] { id, t });
            }
            IFormatter f = new BinaryFormatter();
            Stream s = new FileStream(BinDataPath + "/" + container.GetType().Name + ".bytes", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            f.Serialize(s, container);
            Debug.Log("已生成 " + BinDataPath + "/<color=#FFFF00>" + container.GetType().Name + ".bytes</color>");
            s.Close();
        }

        public static object ParseValue(string type, string data, string classname)
        {
            object o = null;
            try
            {
                switch (type)
                {
                    case SupportType.INT:
                        o = int.Parse(data);
                        break;
                    case SupportType.LONG:
                        o = long.Parse(data);
                        break;
                    case SupportType.FLOAT:
                        o = float.Parse(data);
                        break;
                    case SupportType.STRING:
                        o = data;
                        break;
                    case SupportType.LIST_INT:
                        string[] ints = data.Split('|');
                        List<int> list = new List<int>();
                        foreach (var item in ints)
                        {
                            list.Add(int.Parse(item));
                        }
                        o = list;
                        break;
                    case SupportType.LIST_FLOAT:
                        string[] floats = data.Split('|');
                        List<float> list2 = new List<float>();
                        foreach (var item in floats)
                        {
                            list2.Add(float.Parse(item));
                        }
                        o = list2;
                        break;
                    case SupportType.LIST_STRING:
                        string[] strs = data.Split(';');//逗号分隔
                        o = strs;
                        break;
                    case SupportType.LIST_LIST_INT:
                        break;
                    case SupportType.LIST_LIST_FLOAT:
                        break;
                    case SupportType.LIST_LIST_STRING:
                        break;
                    case SupportType.DICTIONARY_INT_INT:
                        break;
                    case SupportType.DICTIONARY_INT_FLOAT:
                        break;
                    case SupportType.DICTIONARY_INT_STRING:
                        break;
                    case SupportType.DICTIONARY_INT_LIST_INT:
                        break;
                    case SupportType.DICTIONARY_INT_LIST_FLOAT:
                        break;
                }
            }
            catch (Exception ex)
            {
                ProgressBar.HideBarWithFailInfo("\n" + "错误的数据值:" + data + "\n位于:" + classname);
                throw new Exception("\n错误的数据值:" + data + "\n位于:" + classname, ex);
            }
            return o;
        }
    }
}
