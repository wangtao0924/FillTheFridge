using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAdBase
{
    /// <summary>
    /// 展示广告
    /// </summary>
    void Show();


    /// <summary>
    /// 显示广告
    /// </summary>
    void Hide();

    /// <summary>
    /// 创建广告
    /// </summary>
    void Create();

    /// <summary>
    /// 关闭广告
    /// </summary>
    //void Close();


    ///// <summary>
    ///// 广告加载完成时，系统会执行 OnAdLoaded 事件。
    ///// </summary>
    //void OnAdLoaded();

    ///// <summary>
    ///// 广告加载失败时，系统会调用 OnAdFailedToLoad 事件。Message 参数描述发生的故障类型。
    ///// </summary>
    //void OnAdFailedToLoad();


    ///// <summary>
    ///// 用户点按广告时，系统会调用此方法。如果您使用分析产品包跟踪点击，则此方法很适合记录点击。
    ///// </summary>
    //void OnAdOpening();


    ///// <summary>
    ///// 用户点击打开其他应用（例如，Google Play 商店）时，系统会先调用 onAdOpened，再调用此方法，从而在后台运行当前应用。
    ///// </summary>
    //void OnAdLeavingApplication();

}
