/********************************************************************
	All Right Reserved By Leo
	Created:	2019/01/09 22:59
	File base:	CLOG.cs
	author:		Leo

	purpose:	LOG������
                �ṩLOG����ͼ�¼�����ļ��Ĺ���

                δ�������Զ��LOG�ռ�
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

using UnityEngine;

/// <summary>
/// LOG��
/// </summary>
public static class CLOG
{
    // �Ƿ���������
    private static bool m_IsRunning = false;

    // ��
    private static object m_Locker;

    // ��д��Ϣ����
    private static Queue<string> m_LogQueue;

    // ��Ϣ�߳�
    private static Thread m_LogThread;

    // д����
    private static StreamWriter m_LogWriter;

    /// <summary>
    /// ���õ�Tag�嵥
    /// </summary>
    private static Dictionary<string, bool> m_DisableTagDic;

    /// <summary>
    /// δ�ܹ����TagĬ�����
    /// </summary>
    private static bool m_DefaultTagAllow = true;

    /// <summary>
    /// LOGϵͳ�Ƿ����ڹ���
    /// </summary>
    public static bool IsRunning => m_IsRunning;

    #region LOG ���

    /// <summary>
    /// ��Ӧ��׿�ϵ� LogInfo ���
    /// </summary>
    [Conditional("NEED_LOG")]
    public static void I(string logStr)
    {
        UnityEngine.Debug.Log(GetLogString("", logStr, "INFO"));
    }

    /// <summary>
    /// ��Ӧ��׿�ϵ� LogInfo ���
    /// </summary>
    [Conditional("NEED_LOG")]
    public static void I(string tag, string logStr)
    {
        if (CanLog(tag))
        {
            UnityEngine.Debug.Log(GetLogString(tag, logStr, "INFO"));
        }
    }

    /// <summary>
    /// ��Ӧ��׿�ϵ� LogWarning ���
    /// </summary>
    public static void W(string logStr)
    {
        UnityEngine.Debug.LogWarning(GetLogString("", logStr, "WARN"));
    }

    /// <summary>
    /// ��Ӧ��׿�ϵ� LogWarning ���
    /// </summary>
    public static void W(string tag, string logStr)
    {
        UnityEngine.Debug.LogWarning(GetLogString(tag, logStr, "WARN"));
    }

    /// <summary>
    /// ��Ӧ��׿�ϵ� LogError ���
    /// </summary>
    public static void E(string logStr)
    {
        UnityEngine.Debug.LogError(GetLogString("", logStr, "ERROR"));
    }

    /// <summary>
    /// ��Ӧ��׿�ϵ� LogError ���
    /// </summary>
    public static void E(string tag, string logStr)
    {
        UnityEngine.Debug.LogError(GetLogString(tag, logStr, "ERROR"));
    }

    #endregion LOG ���

    /// <summary>
    /// ��ʼDebug��Logger����
    /// </summary>
    [Conditional("NEED_LOG")]
    public static void Init()
    {
        if (m_IsRunning)
        {
            return;
        }
        m_DisableTagDic = new Dictionary<string, bool>();
        m_LogQueue = new Queue<string>();
        m_Locker = new object();

        // �ļ�ʱ��
        DateTime now = DateTime.Now;
        string logName = $"Log_{now.Year}_{now.Month:D2}_{now.Day:D2}_{now.Hour:D2}_{now.Minute:D2}_{now.Second:D2}.txt";
        CFile.CheckCreateFolder(CApp.Inst.Log_Path);

        string logPath = $"{CApp.Inst.Log_Path}{logName}";

        // �ļ�������ɾ��
        if (File.Exists(logPath))
        {
            File.Delete(logPath);
        }

        UnityEngine.Debug.Log($"log system actived! write to {logPath}");

        // д�ļ�����
        m_LogWriter = new StreamWriter(logPath);
        m_LogWriter.AutoFlush = true;
        m_IsRunning = true;

        // д�߳�
        m_LogThread = new Thread(Process);

        // ����Ϊ��̨�̣߳������̹߳رն��ر�
        m_LogThread.IsBackground = true;
        m_LogThread.Start();

        // ���Log����
        Application.logMessageReceived += OnLogHandler;
    }

    /// <summary>
    /// ���ñ�ǩ�Ƿ��������
    /// </summary>
    /// <param name="tag">tag</param>
    /// <param name="enable">�Ƿ��������</param>
    [Conditional("NEED_LOG")]
    public static void SetTag(string tag, bool enable)
    {
        if (m_DisableTagDic.ContainsKey(tag))
        {
            m_DisableTagDic[tag] = enable;
        }
        else
        {
            m_DisableTagDic.Add(tag, enable);
        }
    }

    /// <summary>
    /// ����/��������Tag���
    /// </summary>
    /// <param name="enable">�Ƿ��������</param>
    [Conditional("NEED_LOG")]
    public static void SetAllTag(bool enable)
    {
        m_DefaultTagAllow = enable;
        var keys = m_DisableTagDic.Keys;
        foreach (var key in keys)
        {
            m_DisableTagDic[key] = enable;
        }
    }

    /// <summary>
    /// ���һ��tag�Ƿ�����LOG
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    private static bool CanLog(string tag)
    {
        if (m_DisableTagDic.ContainsKey(tag))
        {
            return m_DisableTagDic[tag];
        }
        else
        {
            return m_DefaultTagAllow;
        }
    }

    /// <summary>
    /// ֹͣ��log
    /// </summary>
    [Conditional("NEED_LOG")]
    public static void Stop()
    {
        lock (m_Locker)
        {
            m_IsRunning = false;
            m_LogQueue.Clear();
            m_LogWriter.Close();
        }
    }

    /// <summary>
    /// �õ�Log�ַ���
    /// </summary>
    /// <param name="tag">��ǩ</param>
    /// <param name="str">LOG�ַ���</param>
    /// <param name="typeString">���ʹ�</param>
    /// <returns></returns>
    private static string GetLogString(string tag, string str, string typeString)
    {
        DateTime now = DateTime.Now;
        typeString = typeString.PadRight(5, ' ');
        if (tag.Length > 0)
        {
            return $"[{now}.{now.Millisecond:D3} | F:{Time.frameCount,-6} | {typeString}] <{tag}> {str}";
        }
        else
        {
            return $"[{now}.{now.Millisecond:D3} | F:{Time.frameCount,-6} | {typeString}] {str}";
        }
    }

    /// <summary>
    /// Log����
    /// </summary>
    /// <param name="log">log����</param>
    /// <param name="stackTrace">��ջ��Ϣ</param>
    /// <param name="type">log����</param>
    private static void OnLogHandler(string log, string stackTrace, LogType type)
    {
        if (!m_IsRunning)
        {
            return;
        }

        lock (m_Locker)
        {
            switch (type)
            {
                case LogType.Assert:
                case LogType.Error:
                case LogType.Exception:
                    PushLog(log);
                    PushLog("---------------[ Stack ]---------------");
                    PushLog(stackTrace);
                    break;

                case LogType.Log:
                case LogType.Warning:
                    PushLog(log);
                    break;
            }
        }
    }

    /// <summary>
    /// �Ӷ�����ȡһ��log��������ӡ
    /// </summary>
    /// <returns>�����е�����</returns>
    private static string PopLog()
    {
        if (m_LogQueue.Count == 0)
        {
            return "";
        }

        lock (m_Locker)
        {
            return m_LogQueue.Dequeue();
        }
    }

    /// <summary>
    /// ��ӡlog
    /// </summary>
    private static void Process()
    {
        while (m_IsRunning)
        {
            string logStr;

            while ((logStr = PopLog()).Length > 0)
            {
                m_LogWriter.WriteLine(logStr);
            }

            Thread.Sleep(1);
        }
    }

    /// <summary>
    /// ��Ҫ��ӡ��logѹ�����
    /// </summary>
    /// <param name="msg"></param>
    [Conditional("NEED_LOG")]
    private static void PushLog(string msg)
    {
        m_LogQueue.Enqueue(msg);
    }
}
