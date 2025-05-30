using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
namespace GameAdvertisement
{
//    public class GamAdvertisementManager : MPStudio.MPSingletonMono<GamAdvertisementManager>, IUnityAdsListener
//    {
//#if UNITY_IOS
//    private string gameId = "4801860";
//#elif UNITY_ANDROID
//        private string gameId = "4801861";
//#else   
//        private string gameId="0";
//#endif
//        private string rewardedPlacementId = "rewardedVideo";

//        private string bannderPlacementId = "BannerPlacementID";


//        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
//        {
//            switch (showResult)
//            {
//                case ShowResult.Failed:
//                    //Debug.Log("显示错误");
//                    break;
//                case ShowResult.Skipped:
//                    //Debug.Log("已跳过");
//                    break;
//                case ShowResult.Finished:
//                    //Debug.Log("得到奖励！");
//                    break;
//                default:
//                    break;
//            }
//        }

//        public void OnUnityAdsReady(string placementId)
//        {
//            //Debug.Log("广告准备好了！");
          
//        }

//        // Start is called before the first frame update
//        void Start()
//        {
//            Advertisement.AddListener(this);
//            Advertisement.Initialize(gameId, true);
//            StartCoroutine(ShowBannerWhenReady());
//            //Debug.Log("广告初始化完成！");
//        }
//        /// <summary>
//        /// 横幅广告
//        /// </summary>
//        /// <returns></returns>
//        IEnumerator ShowBannerWhenReady()
//        {
//            //Advertisement.Banner.Load(bannderPlacementId);
//            //while (!Advertisement.Banner.isLoaded)
//            //{
//            //    Debug.LogError("横幅广告在加载！");
//            //    Debug.LogError("bannerLoadOptions.loadCallback:");
//            //    yield return new WaitForSeconds(0.5f);
//            //}
//            //Debug.LogError("横幅广告准备好了！");

//            //Advertisement.Banner.Load();
//            //yield return new WaitForSeconds(5);
//            //Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
//            //Advertisement.Banner.Show();

//            while (!Advertisement.IsReady(bannderPlacementId))
//            {
//                //Debug.Log("横幅广告在加载！");
//                yield return new WaitForSeconds(0.5f);
//            }
//            //Debug.Log("横幅广告准备好了！");
//            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
//            Advertisement.Banner.Show(bannderPlacementId);
//        }


//        // Update is called once per frame
//        void Update()
//        {

//        }
//        /// <summary>
//        /// 展示激励广告
//        /// </summary>
//        public void ShowRewardedVideo()
//        {
//            Advertisement.Show(rewardedPlacementId);
//        }
//        /// <summary>
//        /// 全屏广告
//        /// </summary>
//        public void ShowVideo()
//        {
//            Advertisement.Show();
//        }

//        public void OnUnityAdsDidError(string message)
//        {

//        }

//        public void OnUnityAdsDidStart(string placementId)
//        {

//        }
//    }
}