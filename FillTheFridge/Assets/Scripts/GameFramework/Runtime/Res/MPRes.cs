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

namespace MPStudio
{
    /// <summary>
    /// 客户端资源main.txt json 结构
    /// </summary>
    [Serializable]
    public class ResMainConfig
    {
        /// <summary>
        /// 打包号
        /// </summary>
        public string build;

        /// <summary>
        /// 资源版本号
        /// </summary>
        public string resVer;

        /// <summary>
        /// 打包号
        /// </summary>
        public int Build => int.Parse(build);
    }

    /// <summary>
    /// 资源管理类
    /// </summary>
    public class MPRes
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
        public static ResMD5Info LocalFileMD5InfoObj { get; private set; }

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

            MPLOG.I(LOG_TAG, $"Streaming inPath:{inPath} outPath:{outPath}");
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

            MPLOG.I(LOG_TAG, $"Addressable inPath:{inPath} outPath:{outPath}");
        }

        /// <summary>
        /// 释放资源到持久化目录
        /// 释放3种资源 1.main.txt 记录资源版本号
        ///             2.file.index 记录资源md5
        ///             3.各个bundle 各个Bundle
        /// </summary>
        /// <returns></returns>
        public static async Task ExtractRes()
        {
            // 版本路径
            var versionPath = Path.Combine(Application.persistentDataPath, "unity_version").Replace("\\", "/");
            if (!NeedToExtractRes(versionPath))
            {
                MPLOG.I($"need not extract res");
                return;
            }

            MPLOG.I($"---------- start extract res ----------");

            // 显示解压进度条
            // OpeningProgress.Inst.ShowProgress(OpeningLanguage.GetLanguage("extract"));
            var startProgress = 0f;

            // 1.释放 main.txt
            string mainIn, mainOut;
            GetStreamingInOutPath("main.txt", out mainIn, out mainOut);
            await ExtractOneFile(mainIn, mainOut);

            startProgress = 0.05f;
            // OpeningProgress.Inst.SetProgress(startProgress);

            // 2.释放 file.index
            string fileIn, fileOut;
            GetAddressableInOutPath("file.index", out fileIn, out fileOut);
            await ExtractOneFile(fileIn, fileOut);

            startProgress = 0.1f;
            // OpeningProgress.Inst.SetProgress(startProgress);

            // 3.读取本地file.index 生成资源MD5对象（此时不要读取解压目录的file.index,因为刚刚解压，有可能读取不到）
            LocalFileMD5InfoObj = await LoadFileIndexObject(fileIn);

            // 4.开始解压bundle
            if (LocalFileMD5InfoObj != null && LocalFileMD5InfoObj.Infos != null && LocalFileMD5InfoObj.Infos.Length > 0)
            {
                var count = LocalFileMD5InfoObj.Infos.Length;
                float delta = 0.9f / count;

                // 开始释放bundle
                foreach (var file in LocalFileMD5InfoObj.Infos)
                {
                    string inBundlePath, outBundlePath;
                    GetAddressableInOutPath(file.BundleName, out inBundlePath, out outBundlePath);
                    await ExtractOneFile(inBundlePath, outBundlePath);

                    // 更新进度条
                    startProgress += delta;
                    // OpeningProgress.Inst.SetProgress(startProgress);
                }
            }
            else
            {
                // OpeningProgress.Inst.SetProgress(1f);
            }

            // 5.写入资源版本
            File.WriteAllText(versionPath, Application.unityVersion + Application.version);
            MPLOG.I(LOG_TAG, $"write {versionPath} content:{Application.unityVersion + Application.version}");

            new WaitForSeconds(0.1f); // await
            // OpeningProgress.Inst.HideProgress();

            // 打点 解压资源完成
            // Report.UnpackRes();
        }

        /// <summary>
        /// 读取资源md5文件 file.index 返回对象
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static async Task<ResMD5Info> LoadFileIndexObject(string filePath)
        {
            if (!filePath.EndsWith("file.index"))
            {
                Debug.LogError($"file.index path error!! filePath");
                return null;
            }

            //MPLOG.I(LOG_TAG, $"load file.index from {filePath}");
            //using (UnityWebRequest webRequest = UnityWebRequest.Get(filePath))
            //{
            //    webRequest.SendWebRequest();
            //    if (webRequest.isDone && webRequest.error == null)
            //    {
            //        MPLOG.I(LOG_TAG, $"text:{webRequest.downloadHandler.text}");
            //        return JsonMapper.ToObject<ResMD5Info>(webRequest.downloadHandler.text);
            //    }
            //    else
            //    {
            //        MPLOG.I(LOG_TAG, $"error:{webRequest.error}");
            //    }
            //}

            return null;
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

                MPLOG.I($"file version:{version} runtimeVersion:{runtimeVersion}");
                return !version.Equals(runtimeVersion);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 从stream解压一个文件到持久化目录
        /// </summary>
        /// <param name="infile"></param>
        /// <param name="outfile"></param>
        /// <returns></returns>
        private static async Task ExtractOneFile(string infile, string outfile)
        {
            MPLOG.I(LOG_TAG, $"extract file:{infile} ==> out:{outfile}");
            using (UnityWebRequest webRequest = UnityWebRequest.Get(infile))
            {
                webRequest.SendWebRequest();
                if (webRequest.isDone && webRequest.downloadHandler.data.Length > 0)
                {
                    MPLOG.I(LOG_TAG, $"successful!!! {infile} ==> out:{outfile}");
                    File.WriteAllBytes(outfile, webRequest.downloadHandler.data);
                }
                else
                {
                    MPLOG.E(LOG_TAG, $"the file {infile} load failed  so return!");
                }
            }
        }

        /// <summary>
        /// 更新 Catalog
        ///  Catalog 记录着文件和bundle的关系以及bundle之间的依赖关系等信息
        /// </summary>
        /// <param name="progress">加载进度回调</param>
        public static async Task UpdateCatalog()
        {
            MPLOG.I(LOG_TAG, "CheckForCatalogUpdate");
            // 检查是否需要更新Catalog
            var checkHandleAO = Addressables.CheckForCatalogUpdates(false);
            var result = await checkHandleAO.Task;
            MPLOG.I(LOG_TAG, $"CheckForCatalogUpdate result count:{result.Count}");

            // 需要更新Catalog
            if (result.Count > 0)
            {
                // 更新 Catalog
                MPLOG.I(LOG_TAG, "UpdateCatalogs");
                var updateHandleAO = Addressables.UpdateCatalogs(result, false);
                var locators = await updateHandleAO.Task;

                MPLOG.I(LOG_TAG, $"UpdateCatalogs result count:{locators.Count}");
                MPLOG.I(LOG_TAG, $"release updateHandleAO");
                Addressables.Release(updateHandleAO);
            }

            MPLOG.I(LOG_TAG, $"release checkHandleAO");
            Addressables.Release(checkHandleAO);
        }

        /// <summary>
        /// 确认开始下载文件
        /// </summary>
        /// <param name="downlist">下载文件列表</param>
        /// <param name="totalSize">累计需要下载</param>
        public static async Task StartDownFile(List<MD5Info> downlist, ulong totalSize)
        {
            // 更新文本
            // var UpdateText = OpeningLanguage.GetLanguage("update_res");

            // 总下载尺寸文本（兆或者KB)
            // var totalSizeText = OpeningLanguage.GetSizeTxt(totalSize);

            // 更新清单文件中
            // OpeningProgress.Inst.ShowProgress(UpdateText);

            // 当前累计已下载
            ulong nowSize = 0;
            // 当前已完成下载的bundle的尺寸
            ulong nowReadedSize = 0;

            // 开始更新
            // Report.UpdateStart();

            foreach (var info in downlist)
            {
                var remotePath = Path.Combine(ResServerURL, info.BundleName).Replace("\\", "/");
                var localPath = Path.Combine(ResPath, info.BundleName).Replace("\\", "/");

                MPLOG.I(LOG_TAG, $"down load res file: {remotePath} to {localPath}");

                // 读取远端文件
                using (UnityWebRequest webRequest = new UnityWebRequest(remotePath))
                {
                    // 文件下载模式
                    var fileDownload = new DownloadHandlerFile(localPath);
                    fileDownload.removeFileOnAbort = true;
                    webRequest.downloadHandler = fileDownload;
                    var ao = webRequest.SendWebRequest();

                    while (!ao.isDone)
                    {
                        nowSize = ao.webRequest.downloadedBytes + nowReadedSize;
                        var progressValue = (float)nowSize / (float)totalSize;

                        // 设置下载文本和下载进度
                        // OpeningProgress.Inst.SetContent($"{UpdateText} { OpeningLanguage.GetSizeTxt(nowSize) }/{ totalSizeText }");
                        // OpeningProgress.Inst.SetProgress(progressValue);

                        MPLOG.I(LOG_TAG, $"load progress size:{nowSize}/{totalSize} p:{progressValue} ");
                        // await Awaiters.NextFrame;
                    }

                    // 每读取一个文件，当前size++
                    nowReadedSize += (ulong)info.Size;

                    if (ao.webRequest.error != null)
                    {
                        MPLOG.E(LOG_TAG, $"the remote file: {remotePath} load failed!");
                        // 某个文件更新失败
                        // Report.UpdateFailed(remotePath, GetUpdateVersion());
                    }
                }
            }

            new WaitForSeconds(0.1f);

            // 更新完成
            // Report.UpdateDone(GetUpdateVersion());

            // OpeningProgress.Inst.HideProgress();
        }

        /// <summary>
        /// 得到要下载的文件清单
        /// </summary>
        /// <param name="remoteFileList"></param>
        /// <returns></returns>
        public static List<MD5Info> GetDifficultBundleList(ResMD5Info remoteFileList)
        {
            var downlist = new List<MD5Info>();
            foreach (var info in remoteFileList.Infos)
            {
                if (info.BundleName.StartsWith("lua32"))
                {
                    // 对比MD5时，如果系统是64位的，那么lua32 bundle跳过
                    if (System.IntPtr.Size != 4)
                    {
                        continue;
                    }
                }
                else if (info.BundleName.StartsWith("lua64"))
                {
                    // 对比MD5时，如果系统是32位的，那么lua64 bundle跳过
                    if (System.IntPtr.Size == 4)
                    {
                        continue;
                    }
                }

                // 先判断本地有没有，无则直接添加
                var localPath = Path.Combine(ResPath, info.BundleName).Replace("\\", "/");
                if (!File.Exists(localPath))
                {
                    downlist.Add(info);
                    MPLOG.I(LOG_TAG, $"add remote file {info.BundleName} to downlist");
                }
                else
                {
                    // 本地存在，对比MD5
                    // var localMD5 = LuaFramework.Util.md5file(localPath);
                    // MPLOG.I(LOG_TAG, $"file {localPath} local MD5:{localMD5}");
                    // MPLOG.I(LOG_TAG, $"file {info.BundleName} remote MD5:{info.MD5}");
                    //
                    // if (localMD5 != info.MD5)
                    // {
                    //     MPLOG.I(LOG_TAG, $"add remote file {info.BundleName} to downlist");
                    //     downlist.Add(info);
                    // }
                }
            }

            return downlist;
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
            var go = MPResCacher.GetCachedGameObject(locpath);
            if (go != null)
            {
                callback?.Invoke(go);
                return go;
            }

            var ao = Addressables.LoadAssetAsync<GameObject>(locpath);
            var result = await ao.Task;

            if (cache)
            {
                MPResCacher.CacheGameObject(locpath, result);
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
            MPResCacher.CacheIcons(allSp);
        }

        /// <summary>
        /// 得到缓存的图标
        /// </summary>
        /// <param name="iconName"></param>
        /// <returns></returns>
        public static Sprite GetIcon(string iconName)
        {
            return MPResCacher.GetCachedIcon(iconName);
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
            var atlas = MPResCacher.GetCachedSpriteAtlas(locpath);
            Sprite sp;
            if (atlas == null)
            {
                var ao = Addressables.LoadAssetAsync<SpriteAtlas>(locpath);
                atlas = await ao.Task;
                sp = atlas.GetSprite(name);

                if (cache)
                {
                    MPResCacher.CacheSpriteAtlas(locpath, atlas);
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
            var clip = MPResCacher.GetCachedAudio(locpath);
            if (clip != null)
            {
                callback?.Invoke(clip);
                return clip;
            }

            var ao = Addressables.LoadAssetAsync<AudioClip>(locpath);
            var result = await ao.Task;

            if (cache)
            {
                MPResCacher.CacheAudio(locpath, result);
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
            var mat = MPResCacher.GetCachedMaterial(locpath);
            if (mat != null)
            {
                callback?.Invoke(mat);
                return mat;
            }

            var ao = Addressables.LoadAssetAsync<Material>(locpath);
            var result = await ao.Task;

            if (cache)
            {
                MPResCacher.CacheMaterial(locpath, result);
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
                MPLOG.W(LOG_TAG, $"the path:{locpath} is not end with .bytes!! so it won't work!");
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
}