
#define AdTest
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ADConfig : MonoBehaviour
{


#if AdTest
    /// <summary>
    /// appID
    /// </summary>
    public static string AppId = "ca-app-pub-4981464583804603~1506905551";
    /// <summary>
    /// �������
    /// </summary>
    public static string OpenAd = "ca-app-pub-4981464583804603/2588052937";
    /// <summary>
    /// ������
    /// </summary>
    public static string BannerAd = "ca-app-pub-4981464583804603/4679863805";

    /// <summary>
    /// ��ҳʽ���
    /// </summary>
    public static string InterstitialAd = "ca-app-pub-4981464583804603/9880768332";

    /// <summary>
    /// �������
    /// </summary>
    public static string RewardedAd = "ca-app-pub-4981464583804603/4871435490";
#else

   



    /// <summary>
    /// appID
    /// </summary>
    public static string AppId = "ca-app-pub-3092727148892286~5088765834";
    /// <summary>
    /// �������
    /// </summary>
    public static string OpenAd = "ca-app-pub-4981464583804603/2309158824";
    /// <summary>
    /// ������
    /// </summary>
    public static string BannerAd = "ca-app-pub-4981464583804603/1768999124";

    /// <summary>
    /// ��ҳʽ���
    /// </summary>
    public static string InterstitialAd = "ca-app-pub-4981464583804603/6821181109";

    /// <summary>
    /// �������
    /// </summary>
    public static string RewardedAd = "ca-app-pub-4981464583804603/4195017767";

#endif

}
