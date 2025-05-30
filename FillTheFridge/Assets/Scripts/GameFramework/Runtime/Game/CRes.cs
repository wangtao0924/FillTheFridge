/********************************************************************
   All Right Reserved By Leo
   Created:    2020/6/16 8:48:41
   File: 	   CRes.cs
   Author:     Leo

   Purpose:    ��Դ������
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
/// �ͻ�����Դmain.txt json �ṹ
/// </summary>
[ProtoContract]
public class ResMainConfig
{
    /// <summary>
    /// �����
    /// </summary>
    [ProtoMember(1)]
    public string build;

    /// <summary>
    /// ��Դ�汾��
    /// </summary>
    [ProtoMember(2)]
    public string resVer;

    /// <summary>
    /// �����
    /// </summary>
    [ProtoMember(3)]
    public int Build => int.Parse(build);
}

/// <summary>
/// ��Դ������
/// </summary>
public class CRes
{
    private const string LOG_TAG = "res";

    /// <summary>
    /// Զ����Դ��������ַ
    /// </summary>
    public static string ResServerURL { get; set; }

    /// <summary>
    /// ������ԴĿ¼
    /// </summary>
    public static string ResPath = string.Empty;

    /// <summary>
    /// ������Դ�汾
    /// </summary>
    public static string ResVersion = string.Empty;

    /// <summary>
    /// �̵�·��
    /// </summary>
    public static string StoreURL = string.Empty;

    /// <summary>
    /// ����bundle��Ϣ�ļ�����
    /// </summary>
    //     public static ResMD5Info LocalFileMD5InfoObj { get; private set; }

    /// <summary>
    /// ������ļ�ģʽ
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
    /// �õ�stream����Դ��·���͵�����·��
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
    /// �õ�stream��Addressable��Դ��·���͵�����·��
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
    /// ��ȡ�Ƿ���Ҫ���½�ѹ��Դ
    /// ˵����
    ///     ����Ϸ�汾����Unity�汾�����ı�ʱ
    ///     ��Ҫ���½�ѹ��Դ���־û�Ŀ¼
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
    /// ���� Catalog
    ///  Catalog ��¼���ļ���bundle�Ĺ�ϵ�Լ�bundle֮���������ϵ����Ϣ
    /// </summary>
    /// <param name="progress">���ؽ��Ȼص�</param>
    public static async Task UpdateCatalog()
    {
        CLOG.I(LOG_TAG, "CheckForCatalogUpdate");
        // ����Ƿ���Ҫ����Catalog
        var checkHandleAO = Addressables.CheckForCatalogUpdates(false);
        var result = await checkHandleAO.Task;
        CLOG.I(LOG_TAG, $"CheckForCatalogUpdate result count:{result.Count}");

        // ��Ҫ����Catalog
        if (result.Count > 0)
        {
            // ���� Catalog
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
    /// ʵ����һ��Ԥ����,����ʱ��Ҫ�����ͷ�
    /// </summary>
    /// <param name="locpath">��Դ·��</param>
    /// <param name="parent">���ڵ�</param>
    /// <param name="callback">�ص�������lua�ã�C#ֱ��await����</param>
    /// <returns></returns>
    public static async Task<GameObject> InstantiatePrefab(string locpath, Transform parent, Action<GameObject> callback)
    {
        var ao = Addressables.InstantiateAsync(locpath, parent, true);
        var result = await ao.Task;

        callback?.Invoke(result);
        return result;
    }

    /// <summary>
    /// ��Դ�ض��򷽷�
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static string TransformServerPath(IResourceLocation loc)
    {
        return loc.InternalId;
    }

    /// <summary>
    /// ʵ����һ��Ԥ����,����ʱ��Ҫ�����ͷ�
    /// �л�����ʱϵͳ���Զ����� Addressables.ReleaseInstance
    /// </summary>
    /// <param name="locpath">��Դ·��</param>
    /// <param name="callback">�ص�������lua�ã�C#ֱ��await����</param>
    /// <returns></returns>
    public static async Task<GameObject> InstantiatePrefab(string locpath, Action<GameObject> callback = null)
    {
        var ao = Addressables.InstantiateAsync(locpath);
        var result = await ao.Task;

        callback?.Invoke(result);
        return result;
    }

    /// <summary>
    /// ���� GameObject ���� ʵ�������ͷ��ǳɶԳ��ֵ�
    /// ������GameObject������ͨ�������  Addressables.InstantiateAsync(locpath) �������ģ���ô�ᴦ�����ü�����bundleж��
    /// ������GameObject����ͨ�� Addressables �����ģ��Զ����� GameObject.Destroy��������
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
    /// ����һ��Ԥ������󣬲�ʵ��������Ҫ�ֶ�ʵ������,һ�㲻�ͷ�
    /// </summary>
    /// <param name="locpath">��Դ·��</param>
    /// <param name="cache">�Ƿ񻺴棬һ�㶼��Ҫ����</param>
    /// <param name="callback">�ص�������lua�ã�C#ֱ��await����</param>
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
    /// ��������Icon
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
    /// �õ������ͼ��
    /// </summary>
    /// <param name="iconName"></param>
    /// <returns></returns>
    public static Sprite GetIcon(string iconName)
    {
        return CResCacher.GetCachedIcon(iconName);
    }

    /// <summary>
    /// ��׼��ͼ��·��
    /// string locPath = "Boss"                   ==> ��׼���� locPath = Assets/Res/Atlas/Boss.spriteatlas
    /// string locPath = "Boss.spriteatlas"       ==> ��׼���� locPath = Assets/Res/Atlas/Boss.spriteatlas
    /// string locPath = "/Boss.spriteatlas"      ==> ��׼���� locPath = Assets/Res/Atlas/Boss.spriteatlas
    /// string locPath = "Assets/Res/Atlas/Boss"  ==> ��׼���� locPath = Assets/Res/Atlas/Boss.spriteatlas
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
    /// ����ͼ���е�ɢͼ��Դ��ͼ��֧��ȫ�ƣ���д
    /// </summary>
    /// <param name="locpath">��Դ·��</param>
    /// <param name="name">ͼ���е�ɢͼ����</param>
    /// <param name="cache">�Ƿ񻺴棬һ�㶼����Ҫ����</param>
    /// <param name="callback">�ص�������lua�ã�C#ֱ��await����</param>
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
                // ���軺���ͼ��ֱ��Release ,GetSprite�ᴴ��ͼ����Сͼ���ڴ濽��������ͼ�������ͷŵ�
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
    /// ��ȡ������Դ,������Դһ�㲻�ͷ�
    /// </summary>
    /// <param name="locpath">��Դ·��</param>
    /// <param name="cache">�Ƿ񻺴棬һ�㶼��Ҫ����</param>
    /// <param name="callback">�ص�������lua�ã�C#ֱ��await����</param>
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
    /// ��ȡ������Դ
    /// </summary>
    /// <param name="locpath">��Դ·��</param>
    /// <param name="cache">�Ƿ񻺴棬һ�㶼��Ҫ����</param>
    /// <param name="callback">�ص�������lua�ã�C#ֱ��await����</param>
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
    /// ��ȡ�ı���Դ���ı���Դ�Զ��ͷ�
    /// </summary>
    /// <param name="locpath">��Դ·��</param>
    /// <param name="callback">�ص�������lua�ã�C#ֱ��await����</param>
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
    /// ��ȡ��������Դ����������Դ�Զ��ͷ�
    /// </summary>
    /// <param name="locpath">��Դ·��</param>
    /// <param name="callback">�ص�������lua�ã�C#ֱ��await����</param>
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
    /// ��ȡJson�����ض����ı���Դ�Զ��ͷ�
    /// </summary>
    /// <typeparam name="T">Json��������</typeparam>
    /// <param name="locpath">��Դ·��</param>
    /// <param name="callback">�ص�������lua�ã�C#ֱ��await����</param>
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
    /// ��ȡ���л���Դ
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
    /// ��õ������ȸ��汾��
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
