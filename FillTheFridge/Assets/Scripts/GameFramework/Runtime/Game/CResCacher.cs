/********************************************************************
   All Right Reserved By Leo
   Created:    2020/8/10 17:00:45
   File: 	   CResCacher.cs
   Author:     Leo

   Purpose:    资源缓存类
*********************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;

//using UnityEngine.Windows;

/// <summary>
/// 资源缓存器
/// </summary>
public static class CResCacher
{
    /// <summary>
    /// 预制体缓存
    /// </summary>
    private static Dictionary<string, GameObject> GameObjectCache;

    /// <summary>
    /// 图集缓存
    /// </summary>
    private static Dictionary<string, SpriteAtlas> SpriteAtlasCache;

    /// <summary>
    /// 材质球缓存
    /// </summary>
    private static Dictionary<string, Material> MaterialCache;

    /// <summary>
    /// 声音资源缓存
    /// </summary>
    private static Dictionary<string, AudioClip> AudioCache;

    /// <summary>
    /// 图标资源缓存
    /// </summary>
    private static Dictionary<string, Sprite> IconCache;

    static CResCacher()
    {
        GameObjectCache = new Dictionary<string, GameObject>();
        SpriteAtlasCache = new Dictionary<string, SpriteAtlas>();
        MaterialCache = new Dictionary<string, Material>();
        AudioCache = new Dictionary<string, AudioClip>();
        IconCache = new Dictionary<string, Sprite>();
    }

    /// <summary>
    /// 通过标签缓存图集资源
    /// </summary>
    public static Material GetCachedMaterial(string locPath)
    {
        Material mat = null;
        MaterialCache.TryGetValue(locPath, out mat);
        return mat;
    }

    /// <summary>
    /// 通过资源标签加载资源
    /// </summary>
    public static void CacheMaterial(string locPath, Material mat)
    {
        MaterialCache.Add(locPath, mat);
    }

    /// <summary>
    /// 缓存材质球
    /// </summary>
    public static AudioClip GetCachedAudio(string locPath)
    {
        AudioClip clip = null;
        AudioCache.TryGetValue(locPath, out clip);
        return clip;
    }

    /// <summary>
    /// 通过路径缓存指定资源
    /// </summary>
    public static void CacheAudio(string locPath, AudioClip clip)
    {
        AudioCache.Add(locPath, clip);
    }

    /// <summary>
    /// 实例化预制体
    /// </summary>
    public static SpriteAtlas GetCachedSpriteAtlas(string locPath)
    {
        SpriteAtlas atlas = null;
        SpriteAtlasCache.TryGetValue(locPath, out atlas);
        return atlas;
    }

    /// <summary>
    /// 缓存一个图集（一般是常用图集比如Common）
    /// </summary>
    public static void CacheSpriteAtlas(string locPath, SpriteAtlas atlas)
    {
        if (SpriteAtlasCache.ContainsKey(locPath))
            return;
        SpriteAtlasCache.Add(locPath, atlas);
    }

    /// <summary>
    /// 从缓存的图集中获取图片
    /// </summary>
    public static GameObject GetCachedGameObject(string locPath)
    {
        GameObject go = null;
        GameObjectCache.TryGetValue(locPath, out go);
        return go;
    }

    /// <summary>
    /// 得到ICON
    /// </summary>
    public static void CacheGameObject(string locPath, GameObject go)
    {
        GameObjectCache.Add(locPath, go);
    }

    /// <summary>
    /// 读取二进制文件数据
    /// </summary>
    /// <param name="path"></param>
    public static void CacheIcons(Sprite[] allSp)
    {
        foreach (var sp in allSp)
        {
            sp.name = sp.name.Replace("(Clone)", string.Empty);
            if (IconCache.ContainsKey(sp.name))
            {
                CLOG.W($"the icon :{sp.name} has already cached!!");
                continue;
            }
            IconCache.Add(sp.name, sp);
        }
    }

    /// <summary>
    /// 通过key获取material
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Sprite GetCachedIcon(string iconName)
    {
        Sprite icon = null;
        IconCache.TryGetValue(iconName, out icon);
        return icon;
    }
}
