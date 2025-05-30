using DG.Tweening;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MPStudio
{
    /// <summary>
    /// 背景音乐管理器
    /// </summary>
    public static class MPSoundBG
    {
        /// <summary>
        /// 音乐播放组件
        /// </summary>
        private static AudioSource mMusicComp = null;

        /// <summary>
        /// 当前正在播放的背景音乐
        /// </summary>
        public static string NowBGMusic { get; private set; }

        /// <summary>
        /// 背景音乐淡入淡出缓动控制
        /// </summary>
        private static Sequence backSoundSequence = null;

        /// <summary>
        /// 安全的得到音乐组件
        /// </summary>
        private static AudioSource GetMusicComponent()
        {
            if (mMusicComp != null)
            {
                return mMusicComp;
            }

            GameObject MusicOB = new GameObject("SoundBG");
            //记录音源组件
            mMusicComp = MusicOB.AddComponent<AudioSource>();
            mMusicComp.loop = true;
            GameObject.DontDestroyOnLoad(MusicOB);

            return mMusicComp;
        }

        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="assetPath">资源名</param>
        /// <param name="fade">是否淡入</param>
        public static async void PlayBGMusic(string assetPath, bool fade = true)
        {
            // 当前播放的音乐与将要播放的音乐不同时
            if (NowBGMusic != assetPath)
            {
                // 若依然处于控制队列中，那么强行杀死队列
                if (backSoundSequence != null)
                {
                    backSoundSequence.Kill();
                    backSoundSequence = null;
                    NowBGMusic = assetPath;
                }

                AudioSource AS = GetMusicComponent();

                // 获取声音资源
                var sound = await Addressables.LoadAssetAsync<AudioClip>(assetPath).Task;

                // 非淡入淡出的情况，直接切换即可
                if (!fade)
                {
                    AS.clip = sound;
                    AS.Play();
                    return;
                }

                // 淡入淡出队列控制
                backSoundSequence = DOTween.Sequence();
                backSoundSequence.Append(AS.DOFade(0f, 1f));

                backSoundSequence.AppendCallback(() => {
                    AS.clip = sound;
                    AS.Play();
                    NowBGMusic = assetPath;
                });
                backSoundSequence.Append(AS.DOFade(1f, 1f));
                backSoundSequence.OnComplete(() => { backSoundSequence = null; }).SetUpdate(true);
            }
        }

        /// <summary>
        /// 设置当前背景音乐音量
        /// </summary>
        /// <param name="Volume">音量 0.0f-1.0f </param>
        public static void SetVolume(float Volume)
        {
            GetMusicComponent().volume = Volume;
        }

        /// <summary>
        /// 停止音乐播放
        /// 播放头回到开始
        /// </summary>
        public static void StopMusic()
        {
            if (backSoundSequence != null)
            {
                backSoundSequence.Kill();
                backSoundSequence = null;
            }

            var AS = GetMusicComponent();
            AS.DOFade(0f, 1f).OnComplete(() => AS.Stop()).SetUpdate(true);
        }

        /// <summary>
        /// 暂停音乐播放
        /// </summary>
        public static void PauseMusic()
        {
            if (backSoundSequence != null)
            {
                backSoundSequence.Kill();
                backSoundSequence = null;
            }

            var AS = GetMusicComponent();
            backSoundSequence = DOTween.Sequence();
            backSoundSequence.Append(AS.DOFade(0f, 1f)).OnComplete(() =>
            {
                AS.Pause();
            }).SetUpdate(true);
        }

        /// <summary>
        /// 恢复音乐播放
        /// </summary>
        public static bool ResumeMusic()
        {
            if (string.IsNullOrEmpty(NowBGMusic))
                return false;

            if (backSoundSequence != null)
            {
                backSoundSequence.Kill();
                backSoundSequence = null;
            }

            var AS = GetMusicComponent();
            AS.Play();
            AS.volume = 0f;
            AS.DOFade(1f, 1f).SetUpdate(true);
            return true;
        }

        public static void OnClear()
        {
            NowBGMusic = string.Empty;
            if (backSoundSequence != null)
            {
                backSoundSequence.Kill();
                backSoundSequence = null;
            }
        }
    }
}