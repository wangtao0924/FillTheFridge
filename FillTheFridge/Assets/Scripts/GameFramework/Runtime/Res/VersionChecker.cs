using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPStudio;
using Protobuf;
using UnityEngine;
using UnityEngine.Networking;

public class VersionChecker
{
    /// <summary>
    /// 本地Main Obj
    /// </summary>
    public static ResMainConfig LocalMainObj = null;

    /// <summary>
    /// 初始化本地Main
    /// </summary>
    public static string InitLocalMain()
    {
        var mainPath = $"{MPRes.ResPath}/main.bin";
        LocalMainObj=ProtobufTool.ReadLocalSaveData<ResMainConfig>("LocalMainObj");
        if (LocalMainObj!=null)
        {
            return LocalMainObj.resVer;
        }

        return string.Empty;
    }

    /// <summary>
    /// 版本检查
    /// </summary>
    /// <param name="finishCall">完成回调</param>
    public static async void Check(Action<string> finishCall)
    {
        // 编辑器下不执行 VersionCheck
        if (Application.isEditor)
        {
            finishCall?.Invoke(null);
            return;
        }

        var platform = 2; // 2 安卓， 1 ios
        var platformStr = "android";
#if UNITY_ANDROID
        platform = 2;
        platformStr = "android";
#elif UNITY_IOS
        platform = 1;
        platformStr = "ios";
#endif
        // app版本号
        var appVer = Application.version;
        // 资源版本号
        var resVer = InitLocalMain();
        // 环境
        var env = Debug.isDebugBuild ? "debug" : "release";
        // 服务器地址
        //var server = MPNetworkManager.Inst.GetHost();
        
        // 请求地址
        //var url =
        //    $"{server}/version/check?platform={platform}&pkgVer={appVer}&resVer={resVer}&env={env}&inviteCode=0&r=50002";

        //using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        //{
        //    webRequest.timeout = 5;
        //    webRequest.SendWebRequest();
        //    if (webRequest.isDone && webRequest.error == null)
        //    {
        //        var rspTxt = webRequest.downloadHandler.text;

        //        var rspObj = JsonMapper.ToObject(rspTxt);
        //        var data = rspObj["data"];

        //        // 记录商店路径
        //        if (data["storeUrl"] != null)
        //        {
        //            MPRes.StoreURL = data["storeUrl"].ToString();
        //        }

        //        // 是否需要强制更新
        //        var forceUpdate = int.Parse(data["forceUpdate"].ToString());
        //        if (forceUpdate == 1 && MPRes.StoreURL != string.Empty)
        //        {
        //            Application.OpenURL(MPRes.StoreURL);
        //            MPApp.ExitGame();
        //            return;
        //        }

        //        // 热更服务器地址
        //        var hotUpdateUrl = data["hotUpdateUrl"].ToString();
        //        var rspEnv = data["env"].ToString();
        //        if (!string.IsNullOrEmpty(hotUpdateUrl))
        //        {
        //            // 版本检查成功
        //            var serverURL = $"{hotUpdateUrl}{platformStr}_{rspEnv}";
        //            finishCall?.Invoke(serverURL);
        //            return;
        //        }
        //    }

        //    // 请求检查资源服务器失败
        //    finishCall?.Invoke(null);
        //}
    }
}