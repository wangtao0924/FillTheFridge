using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class GoogleBanner : AdBase 
{

    private BannerView bannerView;
    private PlatformBase platform;

    public GoogleBanner(PlatformBase platformBase)
    {
        platform = platformBase;
    }


    public override void Create()
    {
        this.bannerView = new BannerView(ADConfig.BannerAd, AdSize.Banner, AdPosition.Bottom);
        bannerView.OnAdClosed += Closed;
        bannerView.OnAdFailedToLoad += AdFailedToLoad;
        bannerView.OnAdLoaded += AdLoaded;
        AdRequest request = new AdRequest.Builder().Build();
        this.bannerView.LoadAd(request);
        //bannerView.OnPaidEvent +=AdLoaded;
    }



    private void AdLoaded(object sender, EventArgs args)
    {
        Debug.Log("广告加载完成");
        if (this.OnAdLoaded == null) return;
        this.OnAdLoaded();
    }


    private void AdFailedToLoad(object sender, EventArgs args)
    {
        Debug.Log("广告加载失败");
        if (this.OnClose == null) return;
        this.OnClose();//
    }

    private void Closed(object sender, EventArgs args)
    {
        Debug.Log("用户关闭广告广告");
        if (this.OnClose == null) return;
        this.OnClose();//
    }


    public override void Hide()
    {
        bannerView.Hide();
    }





    public override void Show()
    {
        if (!platform.IsInit) return;
        if (bannerView == null)
        {
            Create();
        }
        else
        {
            bannerView.Show();
        }
    }


}
