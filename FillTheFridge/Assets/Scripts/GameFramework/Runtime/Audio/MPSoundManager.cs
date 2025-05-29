using DG.Tweening;

using UnityEngine;

namespace MPStudio
{
    /// <summary>
    /// 音效库
    /// </summary>
    public static class MPSoundManager
    {
        /// <summary>
        /// 播放通用背景音乐
        /// </summary>
        public static void PlayBattleBG()
        {
            // 播放背景音乐
            MPSoundBG.PlayBGMusic("Assets/Res/Audio/battle/bg_slow.mp3");
        }

        /// <summary>
        /// 播放转场
        /// </summary>
        public static void PlayChange()
        {
            // 播放背景音乐
            MPSoundBG.PlayBGMusic("Assets/Res/Audio/battle/bg_change.wav");
        }

        /// <summary>
        /// 播放慢速战鼓
        /// </summary>
        public static void PlayBattleSlow()
        {
            // 播放背景音乐
            MPSoundBG.PlayBGMusic("Assets/Res/Audio/battle/bg_fast.mp3");
        }

        /// <summary>
        /// 播放点击音效
        /// </summary>
        public static void PlayClickEffect()
        {
            MPSoundEffect.PlayEffect("Assets/Res/Audio/button.mp3");
        }

        /// <summary>
        /// 播放点击音效
        /// </summary>
        public static void PlayUseCoinEffect()
        {
            MPSoundEffect.PlayEffect("Assets/Res/Audio/use_coin.mp3");
        }

        /// <summary>
        /// 播放战斗开始
        /// </summary>
        public static void PlayBattleStartEffect()
        {
            MPSoundEffect.PlayEffect("Assets/Res/Audio/battle/start.mp3");
        }

        /// <summary>
        /// 播放战斗结束
        /// </summary>
        public static void PlayBattleWinEffect()
        {
            MPSoundEffect.PlayEffect("Assets/Res/Audio/battle/win.mp3");
        }

        /// <summary>
        /// 播放战斗结束
        /// </summary>
        public static void PlayBattleLoseEffect()
        {
            MPSoundEffect.PlayEffect("Assets/Res/Audio/battle/lose.mp3");
        }

        /// <summary>
        /// 播放技能砸地
        /// </summary>
        public static void PlayBoomEffect()
        {
            MPSoundEffect.PlayEffect("Assets/Res/Audio/meteor/boom.mp3");
        }

        /// <summary>
        /// 播放近战音效
        /// </summary>
        public static void PlayOnHit()
        {
            if (MPMath.CanRatioBingo(50, EPrecentType.PRECENT_100))
            {
                MPSoundEffect.PlayEffect($"Assets/Res/Audio/hit{Random.Range(1, 2)}.mp3");
            }
        }

        /// <summary>
        /// 播放远程音效
        /// </summary>
        public static void PlayFarAttackEffect()
        {
            if (MPMath.CanRatioBingo(50, EPrecentType.PRECENT_100))
            {
                MPSoundEffect.PlayEffect($"Assets/Res/Audio/shoot.mp3");
            }
        }
    }
}