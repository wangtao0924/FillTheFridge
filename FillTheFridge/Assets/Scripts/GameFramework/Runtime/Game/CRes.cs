/********************************************************************
   All Right Reserved By Leo
   Created:    2020/6/16 8:48:41
   File: 	   CRes.cs
   Author:     Leo

   Purpose:    资源管理类
*********************************************************************/

using System;

using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using ProtoBuf;

/// <summary>
/// 客户端资源main.txt json 结构
/// </summary>
[ProtoContract]
public class ResMainConfig
{
    /// <summary>
    /// 打包号
    /// </summary>
    [ProtoMember(1)]
    public string build;

    /// <summary>
    /// 资源版本号
    /// </summary>
    [ProtoMember(2)]
    public string resVer;

    /// <summary>
    /// 打包号
    /// </summary>
    [ProtoMember(3)]
    public int Build => int.Parse(build);
}

/// <summary>
/// 资源管理类
/// </summary>
public class CRes
{
    private const string LOG_TAG = "res";

    /// <summary>
    /// 远端资源服务器地址
    /// </summary>
    public static string ResServerURL { get; set; }

    /// <summary>
    /// 本地资源目录
    /// </summary>
    public static string ResPath = string.Empty;

    /// <summary>
    /// 本地资源版本
    /// </summary>
    public static string ResVersion = string.Empty;

    /// <summary>
    /// 商店路径
    /// </summary>
    public static string StoreURL = string.Empty;

    /// <summary>
    /// 本地bundle信息文件对象
    /// </summary>
    //     public static ResMD5Info LocalFileMD5InfoObj { get; private set; }

    /// <summary>
    /// 处理成文件模式
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string HandlePathFileMode(string path)
    {
#if UNITY_EDITOR_OSX
            return "file://" + path ;
#elif UNITY_EDITOR || UNITY_STANDALONE_WIN
        return "file:///" + path;
#elif UNITY_ANDROID
            return path;
#else
            return "file://" + path;
#endif
    }

    /// <summary>
    /// 得到stream中资源的路径和导出的路径
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="inPath"></param>
    /// <param name="outPath"></param>
    private static void GetStreamingInOutPath(string fileName, out string inPath, out string outPath)
    {
        inPath = Path.Combine(HandlePathFileMode(Application.streamingAssetsPath), fileName);
        outPath = Path.Combine(ResPath, fileName);

        CLOG.I(LOG_TAG, $"Streaming inPath:{inPath} outPath:{outPath}");
    }

    /// <summary>
    /// 得到stream中Addressable资源的路径和导出的路径
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="inPath"></param>
    /// <param name="outPath"></param>
    private static void GetAddressableInOutPath(string fileName, out string inPath, out string outPath)
    {
        var platform = PlatformMappingService.GetPlatform().ToString();
        inPath = Path.Combine(HandlePathFileMode(Addressables.RuntimePath), platform, fileName).Replace("\\", "/");
        outPath = Path.Combine(ResPath, fileName).Replace("\\", "/");

        CLOG.I(LOG_TAG, $"Addressable inPath:{inPath} outPath:{outPath}");
    }


    /// <summary>
    /// 获取是否需要重新解压资源
    /// 说明：
    ///     当游戏版本或者Unity版本发生改变时
    ///     需要重新解压资源到持久化目录
    /// </summary>
    /// <returns></returns>
    private static bool NeedToExtractRes(string versionPath)
    {
        if (File.Exists(versionPath))
        {
            string version = File.ReadAllText(versionPath);
            var runtimeVersion = Application.unityVersion + Application.version;

            CLOG.I($"file version:{version} runtimeVersion:{runtimeVersion}");
            return !version.Equals(runtimeVersion);
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// 更新 Catalog
    ///  Catalog 记录着文件和bundle的关系以及bundle之间的依赖关系等信息
    /// </summary>
    /// <param name="progress">加载进度回调</param>
    public static async Task UpdateCatalog()
    {
        CLOG.I(LOG_TAG, "CheckForCatalogUpdate");
        // 检查是否需要更新Catalog
        var checkHandleAO = Addressables.CheckForCatalogUpdates(false);
        var result = await checkHandleAO.Task;
        CLOG.I(LOG_TAG, $"CheckForCatalogUpdate result count:{result.Count}");

        // 需要更新Catalog
        if (result.Count > 0)
        {
            // 更新 Catalog
            CLOG.I(LOG_TAG, "UpdateCatalogs");
            var updateHandleAO = Addressables.UpdateCatalogs(result, false);
            var locators = await updateHandleAO.Task;

            CLOG.I(LOG_TAG, $"UpdateCatalogs result count:{locators.Count}");
            CLOG.I(LOG_TAG, $"release updateHandleAO");
            Addressables.Release(updateHandleAO);
        }

        CLOG.I(LOG_TAG, $"release checkHandleAO");
        Addressables.Release(checkHandleAO);
    }



    /// <summary>
    /// 实例化一个预制体,销毁时需要自行释放
    /// </summary>
    /// <param name="locpath">资源路径</param>
    /// <param name="parent">父节点</param>
    /// <param name="callback">回调函数，lua用，C#直接await即可</param>
    /// <returns></returns>
    public static async Task<GameObject> InstantiatePrefab(string locpath, Transform parent, Action<GameObject> callback)
    {
        var ao = Addressables.InstantiateAsync(locpath, parent, true);
        var result = await ao.Task;

        callback?.Invoke(result);
        return result;
    }

    /// <summary>
    /// 资源重定向方法
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static string TransformServerPath(IResourceLocation loc)
    {
        return loc.InternalId;
    }

    /// <summary>
    /// 实例化一个预制体,销毁时需要自行释放
    /// 切换场景时系统会自动调用 Addressables.ReleaseInstance
    /// </summary>
    /// <param name="locpath">资源路径</param>
    /// <param name="callback">回调函数，lua用，C#直接await即可</param>
    /// <returns></returns>
    public static async Task<GameObject> InstantiatePrefab(string locpath, Action<GameObject> callback = null)
    {
        var ao = Addressables.InstantiateAsync(locpath);
        var result = await ao.Task;

        callback?.Invoke(result);
        return result;
    }

    /// <summary>
    /// 销毁 GameObject 对象 实例化和释放是成对出现的
    /// 如果这个GameObject对象是通过上面的  Addressables.InstantiateAsync(locpath) 来创建的，那么会处理引用计数和bundle卸载
    /// 如果这个GameObject不是通过 Addressables 创建的，自动调用 GameObject.Destroy来销毁它
    /// </summary>
    /// <param name="target"></param>
    public static void ReleasePrefab(GameObject target)
    {
        if (target != null)
        {
            var successful = Addressables.ReleaseInstance(target);
            if (!successful)
                GameObject.Destroy(target);
        }
    }

    /// <summary>
    /// 加载一个预制体对象，不实例化（需要手动实例化）,一般不释放
    /// </summary>
    /// <param name="locpath">资源路径</param>
    /// <param name="cache">是否缓存，一般都需要缓存</param>
    /// <param name="callback">回调函数，lua用，C#直接await即可</param>
    /// <returns></returns>
    public static async Task<GameObject> LoadPrefab(string locpath, bool cache = true, Action<GameObject> callback = null)
    {
        var go = CResCacher.GetCachedGameObject(locpath);
        if (go != null)
        {
            callback?.Invoke(go);
            return go;
        }

        var ao = Addressables.LoadAssetAsync<GameObject>(locpath);
        var result = await ao.Task;

        if (cache)
        {
            CResCacher.CacheGameObject(locpath, result);
        }

        callback?.Invoke(result);
        return result;
    }

    /// <summary>
    /// 缓存所有Icon
    /// </summary>
    /// <param name="locpath"></param>
    /// <returns></returns>
    public static async Task CacheIcon(string locpath)
    {
        var ao = Addressables.LoadAssetAsync<SpriteAtlas>(locpath);
        var atlas = await ao.Task;

        Sprite[] allSp = new Sprite[atlas.spriteCount];
        atlas.GetSprites(allSp);
        CResCacher.CacheIcons(allSp);
    }

    /// <summary>
    /// 得到缓存的图标
    /// </summary>
    /// <param name="iconName"></param>
    /// <returns></returns>
    public static Sprite GetIcon(string iconName)
    {
        return CResCacher.GetCachedIcon(iconName);
    }

    /// <summary>
    /// 标准化图集路径
    /// string locPath = "Boss"                   ==> 标准化后 locPath = Assets/Res/Atlas/Boss.spriteatlas
    /// string locPath = "Boss.spriteatlas"       ==> 标准化后 locPath = Assets/Res/Atlas/Boss.spriteatlas
    /// string locPath = "/Boss.spriteatlas"      ==> 标准化后 locPath = Assets/Res/Atlas/Boss.spriteatlas
    /// string locPath = "Assets/Res/Atlas/Boss"  ==> 标准化后 locPath = Assets/Res/Atlas/Boss.spriteatlas
    /// </summary>
    /// <param name="locPath"></param>
    /// <returns></returns>
    private static string NormalizeSpriteAtlasPath(string locPath)
    {
        if (!locPath.EndsWith(".spriteatlas"))
        {
            locPath = $"{locPath}.spriteatlas";
        }

        if (locPath.StartsWith("/"))
        {
            locPath = locPath.Substring(1);
        }

        if (!locPath.StartsWith("Assets/Res/Atlas"))
        {
            locPath = $"Assets/Res/Atlas/{locPath}";
        }

        return locPath;
    }

    /// <summary>
    /// 加载图集中的散图资源，图集支持全称，简写
    /// </summary>
    /// <param name="locpath">资源路径</param>
    /// <param name="name">图集中的散图名字</param>
    /// <param name="cache">是否缓存，一般都不需要缓存</param>
    /// <param name="callback">回调函数，lua用，C#直接await即可</param>
    /// <returns></returns>
    public static async Task<Sprite> LoadAtlasSprite(string locpath, string name, bool cache = false, Action<Sprite> callback = null)
    {
        locpath = NormalizeSpriteAtlasPath(locpath);
        var atlas = CResCacher.GetCachedSpriteAtlas(locpath);
        Sprite sp;
        if (atlas == null)
        {
            var ao = Addressables.LoadAssetAsync<SpriteAtlas>(locpath);
            atlas = await ao.Task;
            sp = atlas.GetSprite(name);

            if (cache)
            {
                CResCacher.CacheSpriteAtlas(locpath, atlas);
            }
            else
            {
                // 无需缓存的图集直接Release ,GetSprite会创建图集中小图的内存拷贝，所以图集可以释放掉
                Addressables.Release(ao);
            }
        }
        else
        {
            sp = atlas.GetSprite(name);
        }

        callback?.Invoke(sp);
        return sp;
    }

    /// <summary>
    /// 读取声音资源,声音资源一般不释放
    /// </summary>
    /// <param name="locpath">资源路径</param>
    /// <param name="cache">是否缓存，一般都需要缓存</param>
    /// <param name="callback">回调函数，lua用，C#直接await即可</param>
    /// <returns></returns>
    public static async Task<AudioClip> LoadAudio(string locpath, bool cache = true, Action<AudioClip> callback = null)
    {
        var clip = CResCacher.GetCachedAudio(locpath);
        if (clip != null)
        {
            callback?.Invoke(clip);
            return clip;
        }

        var ao = Addressables.LoadAssetAsync<AudioClip>(locpath);
        var result = await ao.Task;

        if (cache)
        {
            CResCacher.CacheAudio(locpath, result);
        }
        callback?.Invoke(result);
        return result;
    }

    /// <summary>
    /// 读取材质资源
    /// </summary>
    /// <param name="locpath">资源路径</param>
    /// <param name="cache">是否缓存，一般都需要缓存</param>
    /// <param name="callback">回调函数，lua用，C#直接await即可</param>
    /// <returns></returns>
    public static async Task<Material> LoadMaterial(string locpath, bool cache = true, Action<Material> callback = null)
    {
        var mat = CResCacher.GetCachedMaterial(locpath);
        if (mat != null)
        {
            callback?.Invoke(mat);
            return mat;
        }

        var ao = Addressables.LoadAssetAsync<Material>(locpath);
        var result = await ao.Task;

        if (cache)
        {
            CResCacher.CacheMaterial(locpath, result);
        }

        callback?.Invoke(result);
        return result;
    }

    /// <summary>
    /// 读取文本资源，文本资源自动释放
    /// </summary>
    /// <param name="locpath">资源路径</param>
    /// <param name="callback">回调函数，lua用，C#直接await即可</param>
    /// <returns></returns>
    public static async Task<string> LoadTextAsset(string locpath, Action<string> callback = null)
    {
        var ao = Addressables.LoadAssetAsync<TextAsset>(locpath);
        var result = await ao.Task;
        var text = result.text;
        Addressables.Release(ao);
        callback?.Invoke(text);
        return text;
    }

    /// <summary>
    /// 读取二进制资源，二进制资源自动释放
    /// </summary>
    /// <param name="locpath">资源路径</param>
    /// <param name="callback">回调函数，lua用，C#直接await即可</param>
    /// <returns></returns>
    public static async Task<byte[]> LoadBinaryAsset(string locpath, Action<byte[]> callback = null)
    {
        if (!locpath.EndsWith(".bytes"))
        {
            CLOG.W(LOG_TAG, $"the path:{locpath} is not end with .bytes!! so it won't work!");
            callback?.Invoke(null);
            return null;
        }

        var ao = Addressables.LoadAssetAsync<TextAsset>(locpath);
        var result = await ao.Task;
        var bytes = result.bytes;
        Addressables.Release(ao);

        callback?.Invoke(bytes);
        return bytes;
    }

    /// <summary>
    /// 读取Json，返回对象，文本资源自动释放
    /// </summary>
    /// <typeparam name="T">Json对象类型</typeparam>
    /// <param name="locpath">资源路径</param>
    /// <param name="callback">回调函数，lua用，C#直接await即可</param>
    /// <returns></returns>
    //public static async Task<T> LoadJson<T>(string locpath, Action<T> callback = null)
    //{
    //    var ao = Addressables.LoadAssetAsync<TextAsset>(locpath);
    //    await ao.Task;
    //    var json = JsonMapper.ToObject<T>(ao.Result.text);
    //    Addressables.Release(ao);
    //    callback?.Invoke(json);
    //    return json;
    //}

    /// <summary>
    /// 读取序列化资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="locpath"></param>
    /// <returns></returns>
    public static async Task<T> LoadSerializableAsset<T>(string locpath)
    {
        var ao = Addressables.LoadAssetAsync<T>(locpath);
        var result = await ao.Task;

        return result;
    }

    /// <summary>
    /// 获得单独的热更版本号
    /// </summary>
    /// <returns></returns>
    public static string GetUpdateVersion()
    {
        if (!string.IsNullOrEmpty(ResVersion))
        {
            string[] str = ResVersion.Split('_');
            return str[1];
        }
        return "-1";
    }
}
