using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdMgr
{
    public PlatformBase platform = new GooglePlatform();
    public AdBase OpenAd;
    public AdBase Banner;
    public AdBase InterstitialAd;
    public AdBase RewardedAd;
    private BaseTenjin Ins_Tenjin = null;   
    public void Init()
    {
        platform.Init();
        OpenAd = GoogleOpenAd.Instance;
        GoogleOpenAd.Instance.platform = platform;
        Banner = new GoogleBanner(platform);
        InterstitialAd = new GoogleInterstitialAd(platform);
        RewardedAd = new GoogleRewardedAd(platform);

        OpenAd.Create();//������ɻ��Զ���ʾ
        InterstitialAd.Create();
        RewardedAd.Create();
        Banner.Show();
    }





    public void TenjinInit()
    {
        Debug.Log("��ʼ��TenJin");
        Ins_Tenjin = Tenjin.getInstance("UQPHZ6CGDXFP7UBYK7FKPDNZMYGZBLZK");
     //   Ins_Tenjin.OptOut();//>>>>
        Ins_Tenjin.SetAppStoreType(AppStoreType.googleplay);
        Ins_Tenjin.Connect();//����
        Debug.Log("TenJin���ӳɹ�");
    }



   
}
