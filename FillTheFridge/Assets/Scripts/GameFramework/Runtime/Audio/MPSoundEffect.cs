using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MPStudio
{
    /// <summary>
    /// 背景音效管理器
    /// </summary>
    public static class MPSoundEffect
    {
        /// <summary>
        ///固定数量的 音效对象池
        /// </summary>
        private static MPPool<GameObject> FixEffectPool;

        /// <summary>
        /// 不固定数量的 音效对象池
        /// </summary>
        private static MPPool<GameObject> EffectPool;

        /// <summary>
        /// 音效节点
        /// </summary>
        private static Transform mEffectRoot;

        private static List<AudioSource> allAS;

        /// <summary>
        /// 得到音效节点
        /// </summary>
        /// <returns></returns>
        public static Transform GetEffectRoot()
        {
            if (mEffectRoot == null)
            {
                var go = new GameObject("SoundEffectNode");
                GameObject.DontDestroyOnLoad(go);
                mEffectRoot = go.transform;
            }

            return mEffectRoot;
        }

        private static float _volume = 1.0f;

        static MPSoundEffect()
        {
            // int MaxEffectSize = ConfigManager.GetGlobalDataByName("MaxSound").param_a;
            int MaxEffectSize = 50; // todo 读取配置
            // 不固定数量的 音效对象池
            EffectPool = MPPool<GameObject>.CreatePoolWithCreateFunc(CreateSoundEffect);
            // 固定数量的 音效对象池
            FixEffectPool = MPPool<GameObject>.CreatePoolWithCreateFunc(CreateSoundEffect, MaxEffectSize);

            allAS=new List<AudioSource>();
        }

        ///// <summary>
        ///// 创建音效组件
        ///// </summary>
        private static GameObject CreateSoundEffect()
        {
            GameObject ob = new GameObject("SoundEffect");
            //记录音源组件
            AudioSource AS = ob.AddComponent<AudioSource>();
            AS.loop = false;
            GameObject.DontDestroyOnLoad(ob);

            // 音效节点
            ob.transform.SetParent(GetEffectRoot());
            return ob;
        }

        /// <summary>
        /// 设置当前所有音效音量
        /// </summary>
        /// <param name="Volume">音量 0.0f-1.0f </param>
        public static void SetVolume(float Volume)
        {
            _volume = Volume;
            for (int i = 0; i < allAS.Count; i++)
            {
                allAS[i].volume = Volume;
            }
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="assetPath">文件名</param>
        public static async void PlayEffect(string assetPath, bool forcePlay = false)
        {
            if (GameSaveData.Instance.localSaveData.voiceOff)
            {
                return;
            }
            GameObject effOb = null;
            if (forcePlay)
            {
                // 强制播放从非固定对象池获得对象
                effOb = EffectPool.GetObject();
            }
            else
            {
                // 非强制播放从对象池获取音源节点,不一定拿得到，因为设置了对象池最大对象数
                effOb = FixEffectPool.GetObject();
                if (effOb == null) return;
            }

            effOb.SetActive(true);

            // 获取音源组件
            var AS = effOb.GetComponent<AudioSource>();
            if (!allAS.Contains(AS))
            {
                allAS.Add(AS);
            }

            // 获取资源
            var sound = await Addressables.LoadAssetAsync<AudioClip>(assetPath).Task;

            // 播放音效
            AS.clip = sound;
            AS.Play();
            await Task.Delay(System.TimeSpan.FromSeconds(sound.length));
            effOb.SetActive(false);

            if (forcePlay)
            {
                // 强制播放的音效直接删除
                EffectPool.Release(effOb);
            }
            else
            {
                // 非强制播放
                FixEffectPool.Release(effOb);
            }
        }

        /// <summary>
        /// 播放循环音效
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static async Task<AudioSource> PlayLoopEffect(string assetPath)
        {
            // 获取资源
            var sound = await Addressables.LoadAssetAsync<AudioClip>(assetPath).Task;

            // 从对象池获取音源节点
            GameObject effOb = FixEffectPool.GetObject();
            effOb.SetActive(true);

            // 获取音源组件
            var AS = effOb.GetComponent<AudioSource>();

            // 播放音效
            AS.clip = sound;
            AS.loop = true;
            AS.Play();

            return AS;
        }

        /// <summary>
        /// 停止循环音效
        /// </summary>
        /// <param name="AS"></param>
        public static void StopLoopEffect(AudioSource AS)
        {
            AS.loop = false;
            // 归还对象池
            AS.gameObject.SetActive(false);
            FixEffectPool.Release(AS.gameObject);
        }
    }
}