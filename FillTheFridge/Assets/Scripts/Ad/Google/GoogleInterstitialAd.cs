using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class GoogleInterstitialAd : AdBase
{
    private InterstitialAd interstitial;
    private PlatformBase platform;
    public GoogleInterstitialAd(PlatformBase platformBase)
    {
        platform = platformBase;
    }
    public override void Create()
    {
        this.interstitial = new InterstitialAd(ADConfig.InterstitialAd);

        interstitial.OnAdClosed += Closed;
        interstitial.OnAdFailedToLoad += AdFailedToLoad;
        interstitial.OnAdLoaded += AdLoaded;



        AdRequest request = new AdRequest.Builder().Build();
        this.interstitial.LoadAd(request);
    }

    public override void Hide()
    {
        interstitial.Destroy();
    }

    public override void Show()
    {
        if (!platform.IsInit) return;
        interstitial.Show();
    }
    public void Closed(object sender, EventArgs args)
    {
        if (OnClose == null) return;
        OnClose();
    }


    public void AdFailedToLoad(object sender, EventArgs args)
    {
        Debug.Log("插屏广告加载失败");
        if (OnClose == null) return;
        OnClose();
    }

    private void AdLoaded(object sender, EventArgs args)
    {
        Debug.Log("插屏广告加载完成");
        if (this.OnAdLoaded == null) return;
        this.OnAdLoaded();
    }
}
