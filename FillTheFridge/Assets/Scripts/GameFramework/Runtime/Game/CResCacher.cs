/********************************************************************
   All Right Reserved By Leo
   Created:    2020/8/10 17:00:45
   File: 	   CResCacher.cs
   Author:     Leo

   Purpose:    ��Դ������
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
/// ��Դ������
/// </summary>
public static class CResCacher
{
    /// <summary>
    /// Ԥ���建��
    /// </summary>
    private static Dictionary<string, GameObject> GameObjectCache;

    /// <summary>
    /// ͼ������
    /// </summary>
    private static Dictionary<string, SpriteAtlas> SpriteAtlasCache;

    /// <summary>
    /// �����򻺴�
    /// </summary>
    private static Dictionary<string, Material> MaterialCache;

    /// <summary>
    /// ������Դ����
    /// </summary>
    private static Dictionary<string, AudioClip> AudioCache;

    /// <summary>
    /// ͼ����Դ����
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
    /// ͨ����ǩ����ͼ����Դ
    /// </summary>
    public static Material GetCachedMaterial(string locPath)
    {
        Material mat = null;
        MaterialCache.TryGetValue(locPath, out mat);
        return mat;
    }

    /// <summary>
    /// ͨ����Դ��ǩ������Դ
    /// </summary>
    public static void CacheMaterial(string locPath, Material mat)
    {
        MaterialCache.Add(locPath, mat);
    }

    /// <summary>
    /// ���������
    /// </summary>
    public static AudioClip GetCachedAudio(string locPath)
    {
        AudioClip clip = null;
        AudioCache.TryGetValue(locPath, out clip);
        return clip;
    }

    /// <summary>
    /// ͨ��·������ָ����Դ
    /// </summary>
    public static void CacheAudio(string locPath, AudioClip clip)
    {
        AudioCache.Add(locPath, clip);
    }

    /// <summary>
    /// ʵ����Ԥ����
    /// </summary>
    public static SpriteAtlas GetCachedSpriteAtlas(string locPath)
    {
        SpriteAtlas atlas = null;
        SpriteAtlasCache.TryGetValue(locPath, out atlas);
        return atlas;
    }

    /// <summary>
    /// ����һ��ͼ����һ���ǳ���ͼ������Common��
    /// </summary>
    public static void CacheSpriteAtlas(string locPath, SpriteAtlas atlas)
    {
        if (SpriteAtlasCache.ContainsKey(locPath))
            return;
        SpriteAtlasCache.Add(locPath, atlas);
    }

    /// <summary>
    /// �ӻ����ͼ���л�ȡͼƬ
    /// </summary>
    public static GameObject GetCachedGameObject(string locPath)
    {
        GameObject go = null;
        GameObjectCache.TryGetValue(locPath, out go);
        return go;
    }

    /// <summary>
    /// �õ�ICON
    /// </summary>
    public static void CacheGameObject(string locPath, GameObject go)
    {
        GameObjectCache.Add(locPath, go);
    }

    /// <summary>
    /// ��ȡ�������ļ�����
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
    /// ͨ��key��ȡmaterial
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
