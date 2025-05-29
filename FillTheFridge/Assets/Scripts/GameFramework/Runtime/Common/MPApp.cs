﻿using System;
using UnityEngine;
using System.IO;
// using LuaFramework;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace MPStudio
{
    /// <summary>
    /// 设备网络状态
    /// </summary>
    public enum ENetState
    {
        /// <summary>
        /// 无网络
        /// </summary>
        NONE,

        /// <summary>
        /// Wifi连接中
        /// </summary>
        WIFI,

        /// <summary>
        /// 移动网络3G/4G连接中
        /// </summary>
        MOBILE_3G_4G,
    }

    /// <summary>
    /// 应用程序类
    /// </summary>
    public class MPApp : MPSingletonMono<MPApp>
    {
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        private bool hasInit = false;

        /// <summary>
        /// 设计分辨率
        /// </summary>
        public Vector2 DesignResolution { get; private set; }

        /// <summary>
        /// http下载目录
        /// </summary>
        public string Download_Path => Application.persistentDataPath + "/download/";

        /// <summary>
        /// 应用程序是否获得焦点
        /// </summary>
        public bool IsApplecationFocus { get; private set; }

        /// <summary>
        /// 应用程序是否暂停
        /// </summary>
        public bool IsApplecationPause { get; private set; }

        /// <summary>
        /// 多点触摸状态
        /// </summary>
        public bool MutiTouchEnabled
        {
            get => Input.multiTouchEnabled;
            set => Input.multiTouchEnabled = value;
        }

        /// <summary>
        /// 当前网络环境，wifi，3G/4G ,还是网络未连接
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
        /// 目标帧率
        /// </summary>
        public int TargetFrameRate
        {
            get => Application.targetFrameRate;
            set => Application.targetFrameRate = value;
        }

        /// <summary>
        /// 图片下载目录
        /// </summary>
        public string Texture_Path => Application.persistentDataPath + "/image/";

#if UNITY_EDITOR

        ///// <summary>
        ///// Log目录
        ///// </summary>
        public string Log_Path => Application.dataPath + "/../Log/";

#else
        public string Log_Path => Application.persistentDataPath + "/Log/";
#endif

        /// <summary>
        /// 退出应用程序事件
        /// </summary>
        public event Action EventAppQuit = null;

        /// <summary>
        /// 切换到前台事件
        /// </summary>
        public event Action EventAppSwitchIn = null;

        /// <summary>
        /// 切换到后台事件
        /// </summary>
        public event Action EventAppSwitchOut = null;

        /// <summary>
        /// 主循环
        /// </summary>
        public event Action Looper;

        /// <summary>
        /// APP每帧更新
        /// </summary>
        private void Update()
        {
            Looper?.Invoke();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            if (hasInit)
            {
                return;
            }

            IsApplecationFocus = true;
            IsApplecationPause = false;

            // 初始化常用文件夹
            InitDirectory();

            hasInit = true;
        }

        /// <summary>
        /// 初始化常用目录
        /// 检查设备下是否具备上述目录
        /// 没有则创建
        /// </summary>
        private void InitDirectory()
        {
            MPFile.CheckCreateFolder(Log_Path);
            MPFile.CheckCreateFolder(Download_Path);
            MPFile.CheckCreateFolder(Texture_Path);
        }

        /// <summary>
        /// 设置设计分辨率
        /// </summary>
        /// <param name="designX"></param>
        /// <param name="designY"></param>
        public void SetDesignSize(float designX, float designY)
        {
            // 设计分辨率
            DesignResolution = new Vector2(designX, designY);
            // 设计宽高比
            float _DesignWHRatio = designX / designY;
            // 设计宽高比下的摄像机尺寸
            float _DesignSize = Mathf.Max(designX, designY) / 200f;
            // 真实宽高比
            float _RealWHRatio = (float) Screen.width / (float) Screen.height;
            // 真实宽高比下的摄像机尺寸
            MPSceneManager.CameraSize = _DesignSize / _RealWHRatio * _DesignWHRatio;
        }

        /// <summary>
        /// 应用程序焦点变化
        /// </summary>
        /// <param name="focus"></param>
        private void OnApplicationFocus(bool focus)
        {
            MPLOG.I("app", $"Application Focus state {focus}");
            IsApplecationFocus = focus;

            var now = (int) MPTime.GetNowTimeStamp(false);
            if (focus)
            {
                // 检查是否切换到前台
                // 移动端以是否丢失焦点作为游戏切换的依据
                EventAppSwitchIn?.Invoke();
                var outTime = PlayerPrefs.GetInt("outTime", 0);
                PlayerPrefs.SetInt("outTime", 0);

                if (outTime != 0 && now - outTime > 1800)
                {
                    // Util.RestartGame(); // lua重启游戏
                }
            }
            else
            {
                // 检查是否切换到后台
                // 移动端以是否丢失焦点作为游戏切换的依据
                EventAppSwitchOut?.Invoke();
                PlayerPrefs.SetInt("outTime", now);
            }
        }

        /// <summary>
        /// 应用程序暂停状态变化
        /// </summary>
        /// <param name="pause"></param>
        private void OnApplicationPause(bool pause)
        {
            MPLOG.I("app", $"Application Pause state {pause}");
            IsApplecationPause = pause;
        }

        /// <summary>
        /// 应用退出
        /// </summary>
        private void OnApplicationQuit()
        {
            MPLOG.I("app", "Application Quit");
            PlayerPrefs.SetInt("outTime", int.MaxValue);
            EventAppQuit?.Invoke();
        }

        /// <summary>
        /// 退出
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
        /// 退出游戏
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
}