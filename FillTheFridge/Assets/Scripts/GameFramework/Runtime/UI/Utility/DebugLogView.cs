using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class DebugLogView : MonoBehaviour {
    public bool Log = true;
    private bool isShowLog;//是否显示日志,
    public KeyCode keyOpenLog = KeyCode.O;//按键打开/关闭日志
    private Vector2 m_scroll;
    public GUIStyle labelStyle;//日志的OnGUI样式设定
 
    internal void OnEnable()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
        isShowLog = true;//当脚本打开，是否可以显示日志， Log = True;//这个变量也必须为True
        Application.logMessageReceived += HandleLog;//注册Unity的日志回调
    }
 
    internal void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;//去掉Unity的日志回调
    }
 
    private string m_logs;
    /// <summary>
    /// /// </summary>    
    /// /// <param name="logString">错误信息</param>    /// 
    /// <param name="stackTrace">跟踪堆栈</param>    /// 
    /// <param name="type">错误类型</param>    
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string[] splitStr = stackTrace.Split('\n');
        string strType = "";
        switch (type)
        {//给日志类型加颜色
            case LogType.Error:
                strType = "<color=red>" + type.ToString() + "</color>";
                break;
            case LogType.Assert:
                break;
            case LogType.Warning:
                strType = "<color=yellow>" + type.ToString() + "</color>";
                break;
            case LogType.Log:
                strType = "<color=white>" + type.ToString() + "</color>";
                break;
            case LogType.Exception:
                break;
            default:
                break;
        }
        strType = strType.Length == 0 ? type.ToString() : strType;//如果没有日志类型，那么就赋值一个类型
        string strLog = "【—" + strType + "—】: \n" + logString + "\n" + splitStr[0] + "\t\n" + splitStr[1] + "\t\n\t\t<——————分割线——————>\n";
        m_logs = strLog + m_logs;
        if (m_logs.Length>1024*8)
        {//如果字符超出长度是会报错的，所以超出限制一下长度
            m_logs = "";
            m_logs = strLog + m_logs;
        }
    }
    
    void OnGUI()
    {
        if (!Log)
            return;
        
        GUILayout.BeginArea(new Rect(Screen.width-400, Screen.height - 600, 400, 400));
        if(GUILayout.Button("查看日志"))
        {
            isShowLog = !isShowLog;
        }
        GUILayout.EndArea();


        if (isShowLog)
        {
            m_scroll = GUILayout.BeginScrollView(m_scroll);
            if (GUILayout.Button("清空日志"))
            {
                m_logs = "";
            }
            GUILayout.Label(m_logs, labelStyle);
            GUILayout.EndScrollView();
        }
    }
 
    void Update()
    {
        if (Input.GetKeyUp(keyOpenLog))
        {
            isShowLog = !isShowLog;
        }
    }
}