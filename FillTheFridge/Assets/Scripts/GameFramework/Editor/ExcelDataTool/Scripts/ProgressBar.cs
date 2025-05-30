using UnityEditor;

namespace ExcelDataManager
{
    public class ProgressBar : EditorWindow
    {
        void OnInspectorUpdate() //更新
        {
            Repaint();  //重新绘制
        }

        public static void UpdataBar(string info, float progress)
        {
            //使用这句代码，在进度条后边会有一个 关闭按钮，但是用这句话会直接卡死，切记不要用
            //EditorUtility.DisplayCancelableProgressBar("Loading Excel Data", info + "...", progress);
            //使用这句创建一个进度条，  参数1 为标题，参数2为提示，参数3为 进度百分比 0~1 之间
            EditorUtility.DisplayProgressBar("导表工具", info + "...", progress);
            if (progress >= 1)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("导表工具", info, "确定");
            }
        }
        public static void HideBarWithFailInfo(string failinfo)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("注意！！！", failinfo, "确定");
        }
    }
}
