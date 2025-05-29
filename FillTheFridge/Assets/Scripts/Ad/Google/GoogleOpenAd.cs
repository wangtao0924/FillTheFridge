using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class GoogleOpenAd : AdBase
{
    private AppOpenAd appOpenAd;
    private static GoogleOpenAd instance;
    private bool isShowingAd = false;
    public PlatformBase platform;




    public static GoogleOpenAd Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GoogleOpenAd();
            }

            return instance;
        }
    }

    private bool IsAdAvailable
    {
        get
        {
            return appOpenAd != null;
        }
    }

    public void LoadAd()
    {
        AdRequest request = new AdRequest.Builder().Build();

        AppOpenAd.LoadAd(ADConfig.OpenAd, ScreenOrientation.Portrait, request, ((AppOpenAd, error) =>
        {
            if (error != null)
            {
                // Handle the error.
                if (this.OnAdFailedToLoad == null) return;
                this.OnAdFailedToLoad();
                Debug.Log("广告加载失败");
                if (this.OnClose == null) return;
                this.OnClose();
                Debug.LogFormat("Failed to load the ad. (reason: {0})", error.LoadAdError.GetMessage());
                return;
            }
            // App open ad is loaded.
            appOpenAd = AppOpenAd;
            Show();//加载完成直接展示
        }));
    }


    public override void Create()
    {
        LoadAd();
    }

    public override void Hide()
    {

    }

    public override void Show()
    {
        if (!IsAdAvailable || isShowingAd) return;
        if (!platform.IsInit) return;
        if (appOpenAd == null) return;
        appOpenAd.Show();
        Debug.Log("展示开屏广告");
    }

    public void Close()
    {
    }
}
