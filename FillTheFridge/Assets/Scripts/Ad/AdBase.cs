using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AdBase : IAdBase
{
    /// <summary>
    /// 广告加载完成时
    /// </summary>
    public UnityAction OnAdLoaded;
    /// <summary>
    /// 广告加载失败时
    /// </summary>
    public UnityAction OnAdFailedToLoad;
    /// <summary>
    /// 用户点按广告时
    /// </summary>
    public UnityAction OnAdOpening;
    /// <summary>
    /// 用户点击打开其他应用
    /// </summary>
    public UnityAction OnAdLeavingApplication;
    /// <summary>
    /// 广告关事
    /// </summary>
    public UnityAction OnClose;

    /// <summary>
    /// 用户观看完给与奖励
    /// </summary>
    public UnityAction OnUserEarnedReward;

    /// <summary>
    /// 展示广告时
    /// </summary>
    public UnityAction OnShow;

    public abstract void Create();


    public abstract void Hide();


    public abstract void Show();
  
}
