using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.Events;

public class GoogleRewardedAd : AdBase
{
    private RewardedAd rewardedAd;
    private PlatformBase platform;
    

    public GoogleRewardedAd(PlatformBase platformBase)
    {
        platform = platformBase;
    }


    public override void Create()
    {
        rewardedAd = new RewardedAd(ADConfig.RewardedAd);
        rewardedAd.OnAdClosed += Closed;
        rewardedAd.OnAdFailedToLoad += AdFailedToLoad;
        rewardedAd.OnAdFailedToShow += AdFailedToLoad;
        rewardedAd.OnUserEarnedReward += AdUserEarnedReward;
        AdRequest request = new AdRequest.Builder().Build();
        this.rewardedAd.LoadAd(request);
    }


    public void AdUserEarnedReward(object sender, Reward args)
    {
        Debug.Log("�û��ۿ����淢�Ž���");
        if (this.OnUserEarnedReward == null) return;
        this.OnUserEarnedReward();
    }
    private void AdLoaded(object sender, EventArgs args)
    {
        Debug.Log("���������");
        if (this.OnAdLoaded == null) return;
        this.OnAdLoaded();
    }


    private void AdFailedToLoad(object sender, EventArgs args)
    {
        Debug.Log("������ʧ��");
        if (this.OnClose == null) return;
        this.OnClose();//
    }

    private void Closed(object sender, EventArgs args)
    {
        Debug.Log("�û��رչ����");
        if (this.OnClose == null) return;
        this.OnClose();//
    }

    public override void Hide()
    {
        rewardedAd.Destroy();
    }

    public override void Show()
    {
        if (!platform.IsInit) return;
        if (rewardedAd == null) return;
        rewardedAd.Show();
        if (this.OnShow == null) return;
        OnShow();

    }






    public void Close()
    {
    }
}
