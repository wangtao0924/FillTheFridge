using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MPStudio
{
    /// <summary>
    /// 场景管理类
    /// </summary>
    public static class MPSceneManager
    {
        /// <summary>
        /// 准备加载的场景绑定类
        /// </summary>
        public static Type ReadyToLoadSceneType;

        /// <summary>
        /// 是否已经初始化
        /// </summary>
        private static bool m_HasInit = false;

        /// <summary>
        /// 是否正在加载场景
        /// </summary>
        private static bool m_IsLoadingScene = false;

        /// <summary>
        /// 异步加载操作对象
        /// </summary>
        public static AsyncOperation AsyncOperator { get; private set; }

        /// <summary>
        /// 摄像机尺寸
        /// </summary>
        public static float CameraSize { get; set; } = 6.4f;

        /// <summary>
        /// 结束加载
        /// </summary>
        private static bool m_FinishLoading = false;

        /// <summary>
        /// 是否正在Lua加载，lua加载不实例化场景类
        /// </summary>
        private static bool m_IsLuaLoading = false;

        /// <summary>
        /// 当前运行的场景名
        /// </summary>
        public static string CurrentSceneName { get;set; }

        /// <summary>
        /// 是否正在加载场景
        /// </summary>
        public static bool IsLoadingScene
        {
            get => m_IsLoadingScene;
            private set
            {
                if (!m_IsLoadingScene && value)
                {
                    MPLOG.I("scene", "Start Loading Scene");
                }
                else if (m_IsLoadingScene && !value)
                {
                    MPLOG.I("scene", "Loading Scene Complete");
                }

                m_IsLoadingScene = value;
            }
        }

        /// <summary>
        /// 当前场景
        /// </summary>
        public static MPSceneBase RunningScene { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            if (m_HasInit)
            {
                return;
            }

            // 提高加载频度，获取更好的加载体验
            Application.backgroundLoadingPriority = ThreadPriority.Low;

            //注册回调
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnLoaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;

            m_HasInit = true;
        }

        /// <summary>
        /// Lua加载Addressable场景  加载Addressable场景通过Addressable路径
        /// </summary>
        /// <param name="scenePath"></param>
        /// <param name="loadingCallback"></param>
        /// <param name="loadedCallback"></param>
        public static async void LuaLoadAddressableScene(string scenePath, Action<float> loadingCallback, Action loadedCallback)
        {
            MPLOG.I("scene", $"lua ready to load addressable scene {scenePath}");

            // 设置为正在加载场景
            IsLoadingScene = true;
            m_FinishLoading = false;
            m_IsLuaLoading = true;

            var ao = Addressables.LoadSceneAsync(scenePath);
            if (loadingCallback != null)
            {
                while (!ao.IsDone)
                {
                    loadingCallback.Invoke(ao.PercentComplete);
                    // await Awaiters.NextFrame;
                }
            }

            loadedCallback?.Invoke();
        }

        /// <summary>
        /// lua加载内置场景  加载内置场景通过场景名加载
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="loadingCallback"></param>
        /// <param name="loadedCallback"></param>
        public static async void LuaLoadScene(string sceneName, Action<float> loadingCallback, Action loadedCallback)
        {
            MPLOG.I("scene", $"lua ready to load scene {sceneName}");

            // 设置为正在加载场景
            IsLoadingScene = true;
            m_FinishLoading = false;
            m_IsLuaLoading = true;

            var ao = SceneManager.LoadSceneAsync(sceneName);
            if (loadingCallback != null)
            {
                while (!ao.isDone)
                {
                    loadingCallback.Invoke(ao.progress);
                    // await Awaiters.NextFrame;
                }
            }

            loadedCallback?.Invoke();
        }

        /// <summary>
        /// 异步加载场景
        /// 加载完成立刻激活
        /// </summary>
        public static async Task LoadScene<T>(string scenePath, Action<float> LoadingCallback = null) where T : MPSceneBase
        {
            MPLOG.I("scene", $"ready to load scene {scenePath}");
            // 切换场景时把当前场景设置为不可用
            if (RunningScene != null)
            {
                RunningScene.IsDirty = true;
            }

            // 设置为正在加载场景
            IsLoadingScene = true;
            m_FinishLoading = false;
            m_IsLuaLoading = false;

            // 记录准备加载的场景绑定类
            ReadyToLoadSceneType = typeof(T);

            try
            {
                var ao = Addressables.LoadSceneAsync(scenePath);
                if (LoadingCallback != null)
                {
                    while (!ao.IsDone)
                    {
                        LoadingCallback.Invoke(ao.PercentComplete);
                        // await Awaiters.NextFrame;
                    }
                }
                IsLoadingScene = false;
                new WaitUntil(() => m_FinishLoading); // await
            }
            catch (Exception ex)
            {
                MPLOG.E("scene", ex.ToString());

                // 遇到异常，切换失败
                if (RunningScene != null)
                {
                    RunningScene.IsDirty = false;
                }
            }
        }

        /// <summary>
        /// 激活场景改变的回调
        /// </summary>
        private static void OnActiveSceneChanged(Scene OldScene, Scene NewScene)
        {
            MPLOG.I("scene", $"active scene changed:{OldScene.name} ==> {NewScene.name}");
        }

        /// <summary>
        /// 场景加载完毕回调
        /// </summary>
        /// <param name="TargetScene"></param>
        /// <param name="LoadMode"></param>
        private static void OnSceneLoaded(Scene TargetScene, LoadSceneMode LoadMode)
        {
            CurrentSceneName = TargetScene.name;
            MPLOG.I("scene", $"Scene:{CurrentSceneName} Loaded!");

            m_FinishLoading = true;

            if (!m_IsLuaLoading && ReadyToLoadSceneType != null)
            {
                RunningScene = Activator.CreateInstance(ReadyToLoadSceneType) as MPSceneBase;
                RunningScene.BindUnityScene(TargetScene);

                // 初始化场景
                SceneInit(RunningScene);
                // 执行场景进入完毕事件
                RunningScene.AfterEnterScene(TargetScene);
            }
        }

        /// <summary>
        /// 场景卸载回调
        /// </summary>
        /// <param name="TargetScene"></param>
        /// <param name="LoadMode"></param>
        private static void OnSceneUnLoaded(Scene TargetScene)
        {
            MPLOG.I("scene", $"Scene:{TargetScene.name} unLoad!");

            // 当前场景不为空,且当前场景名相同,则执行退出逻辑
            if (RunningScene != null && RunningScene.SceneName == TargetScene.name)
            {
                // 执行离开前的处理
                RunningScene.BeforeLeftScene(TargetScene);
                RunningScene = null;
            }
        }

        /// <summary>
        /// 初始化场景
        /// </summary>
        /// <param name="runningScene"></param>
        private static void SceneInit(MPSceneBase runningScene)
        {
            runningScene.UICanvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
            runningScene.TopCanvas = GameObject.Find("TopCanvas")?.GetComponent<Canvas>();
            runningScene.MainCamera = GameObject.Find("Main Camera")?.GetComponent<Camera>();
        }
    }
}