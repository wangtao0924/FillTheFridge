using UnityEngine;
using UnityEditor;

namespace ExcelDataManager
{
    public class PathWindow : EditorWindow
    {
        static EditorWindow window;
        [MenuItem("开发工具/导表工具/设置导表路径")]
        public static void Init()
        {
            window = EditorWindow.GetWindow(typeof(PathWindow));
            window.titleContent.text = "设置路径";
            window.ShowTab();
            window.Show();
        }
        private string m_Path;
        void OnGUI()
        {
            EditorGUILayout.LabelField("", GUILayout.MaxHeight(5f));
            EditorGUILayout.LabelField("当前路径", GUILayout.MinHeight(20f));
            EditorGUILayout.LabelField(PlayerPrefs.GetString(System.Environment.CurrentDirectory + "ExcelDataInputPath", ""), GUILayout.MinHeight(20f));
            if (GUILayout.Button("选择路径"))
            {
                string oldpath = PlayerPrefs.GetString(System.Environment.CurrentDirectory + "ExcelDataInputPath", "");
                string path = EditorUtility.OpenFolderPanel("设置导表路径", oldpath, "");//打开文件夹选择面板
                if (!string.IsNullOrEmpty(path))
                {
                    PlayerPrefs.SetString(System.Environment.CurrentDirectory + "ExcelDataInputPath", path);
                    Debug.Log("新的excel文件路径: " + "<color=#FFFF00>" + path + "</color>");
                    EditorUtility.DisplayDialog("Set Excel Data Input Path", "\n新的excel文件路径:  " + path, "OK");
                    window.Repaint();
                }
            }
        }
    }
}

