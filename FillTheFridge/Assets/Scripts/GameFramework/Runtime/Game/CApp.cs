/********************************************************************
   All Right Reserved By Leo
   Created:	    2020/6/5 20:35:15
   File: 	    CApp.cs
   Author:      Leo

   Purpose:	    APP��
                ��װ��һЩ�����¼�
*********************************************************************/

using System;

using UnityEngine;
using System.IO;

#if UNITY_EDITOR

using UnityEditor;

#endif

/// <summary>
/// �豸����״̬
/// </summary>
public enum ENetState
{
    /// <summary>
    /// ������
    /// </summary>
    NONE,

    /// <summary>
    /// Wifi������
    /// </summary>
    WIFI,

    /// <summary>
    /// �ƶ�����3G/4G������
    /// </summary>
    MOBILE_3G_4G,
}

/// <summary>
/// Ӧ�ó�����
/// </summary>
public class CApp : CSingletonMono<CApp>
{
    /// <summary>
    /// �Ƿ��Ѿ���ʼ��
    /// </summary>
    private bool hasInit = false;

    /// <summary>
    /// ��Ʒֱ���
    /// </summary>
    public Vector2 DesignResolution { get; private set; }

    /// <summary>
    /// http����Ŀ¼
    /// </summary>
    public string Download_Path => Application.persistentDataPath + "/download/";

    /// <summary>
    /// Ӧ�ó����Ƿ��ý���
    /// </summary>
    public bool IsApplecationFocus { get; private set; }

    /// <summary>
    /// Ӧ�ó����Ƿ���ͣ
    /// </summary>
    public bool IsApplecationPause { get; private set; }

    /// <summary>
    /// ��㴥��״̬
    /// </summary>
    public bool MutiTouchEnabled { get => Input.multiTouchEnabled; set => Input.multiTouchEnabled = value; }

    /// <summary>
    /// ��ǰ���绷����wifi��3G/4G ,��������δ����
    /// </summary>
    public ENetState NetState
    {
        get
        {
            switch (Application.internetReachability)
            {
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    return ENetState.WIFI;

                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    return ENetState.MOBILE_3G_4G;

                default:
                    return ENetState.NONE;
            }
        }
    }

    /// <summary>
    /// Ŀ��֡��
    /// </summary>
    public int TargetFrameRate { get => Application.targetFrameRate; set => Application.targetFrameRate = value; }

    /// <summary>
    /// ͼƬ����Ŀ¼
    /// </summary>
    public string Texture_Path => Application.persistentDataPath + "/image/";

#if UNITY_EDITOR

    ///// <summary>
    ///// LogĿ¼
    ///// </summary>
    public string Log_Path => Application.dataPath + "/../Log/";

#else
        public string Log_Path => Application.persistentDataPath + "/Log/";
#endif

    /// <summary>
    /// �˳�Ӧ�ó����¼�
    /// </summary>
    public event Action EventAppQuit = null;

    /// <summary>
    /// �л���ǰ̨�¼�
    /// </summary>
    public event Action EventAppSwitchIn = null;

    /// <summary>
    /// �л�����̨�¼�
    /// </summary>
    public event Action EventAppSwitchOut = null;

    /// <summary>
    /// ��ѭ��
    /// </summary>
    public event Action Looper;

    /// <summary>
    /// APPÿ֡����
    /// </summary>
    private void Update()
    {
        Looper?.Invoke();
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    public void Init()
    {
        if (hasInit)
        {
            return;
        }

        IsApplecationFocus = true;
        IsApplecationPause = false;

        // ��ʼ�������ļ���
        InitDirectory();

        hasInit = true;
    }

    /// <summary>
    /// ��ʼ������Ŀ¼
    /// ����豸���Ƿ�߱�����Ŀ¼
    /// û���򴴽�
    /// </summary>
    private void InitDirectory()
    {
        CFile.CheckCreateFolder(Log_Path);
        CFile.CheckCreateFolder(Download_Path);
        CFile.CheckCreateFolder(Texture_Path);
    }

    /// <summary>
    /// ������Ʒֱ���
    /// </summary>
    /// <param name="designX"></param>
    /// <param name="designY"></param>
    //public void SetDesignSize(float designX, float designY)
    //{
    //    // ��Ʒֱ���
    //    DesignResolution = new Vector2(designX, designY);
    //    // ��ƿ�߱�
    //    float _DesignWHRatio = designX / designY;
    //    // ��ƿ�߱��µ�������ߴ�
    //    float _DesignSize = Mathf.Max(designX, designY) / 200f;
    //    // ��ʵ��߱�
    //    float _RealWHRatio = (float)Screen.width / (float)Screen.height;
    //    // ��ʵ��߱��µ�������ߴ�
    //    CSceneManager.CameraSize = _DesignSize / _RealWHRatio * _DesignWHRatio;
    //}

    /// <summary>
    /// Ӧ�ó��򽹵�仯
    /// </summary>
    /// <param name="focus"></param>
    //private void OnApplicationFocus(bool focus)
    //{
    //    CLOG.I("app", $"Application Focus state {focus}");
    //    IsApplecationFocus = focus;

    //    var now = (int)CTime.GetNowTimeStamp(false);
    //    if (focus)
    //    {
    //        // ����Ƿ��л���ǰ̨
    //        // �ƶ������Ƿ�ʧ������Ϊ��Ϸ�л�������
    //        EventAppSwitchIn?.Invoke();
    //        var outTime = PlayerPrefs.GetInt("outTime", 0);
    //        PlayerPrefs.SetInt("outTime", 0);

    //        if (outTime != 0 && now - outTime > 1800)
    //        {
    //            Util.RestartGame();
    //        }
    //    }
    //    else
    //    {
    //        // ����Ƿ��л�����̨
    //        // �ƶ������Ƿ�ʧ������Ϊ��Ϸ�л�������
    //        EventAppSwitchOut?.Invoke();
    //        PlayerPrefs.SetInt("outTime", now);
    //    }
    //}

    /// <summary>
    /// Ӧ�ó�����ͣ״̬�仯
    /// </summary>
    /// <param name="pause"></param>
    private void OnApplicationPause(bool pause)
    {
        CLOG.I("app", $"Application Pause state {pause}");
        IsApplecationPause = pause;
    }

    /// <summary>
    /// Ӧ���˳�
    /// </summary>
    private void OnApplicationQuit()
    {
        CLOG.I("app", "Application Quit");
        PlayerPrefs.SetInt("outTime", int.MaxValue);
        EventAppQuit?.Invoke();
    }

    /// <summary>
    /// �˳�
    /// </summary>
    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    /// <summary>
    /// �˳���Ϸ
    /// </summary>
    public static void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
