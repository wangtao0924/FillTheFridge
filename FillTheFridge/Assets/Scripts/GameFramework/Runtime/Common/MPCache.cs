using System.Collections.Generic;

using MPStudio;

/// <summary>
/// 缓存类
/// </summary>
public class MPCache<T, V>
{
    /// <summary>
    /// 缓存
    /// </summary>
    private Dictionary<T, V> m_Cache;

    public MPCache()
    {
        m_Cache = new Dictionary<T, V>();
    }

    /// <summary>
    /// 缓存一个对象
    /// </summary>
    /// <param name="key"></param>
    /// <param name="target"></param>
    public void Add(T key, V target)
    {
        if (m_Cache.ContainsKey(key))
        {
            MPLOG.I("cache", $"the cache key:{key} has res so replace it!");
            m_Cache[key] = target;
        }
        else
        {
            m_Cache.Add(key, target);
        }
    }

    /// <summary>
    /// 通过key 获得对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public V Get(T key)
    {
        if (m_Cache.ContainsKey(key))
        {
            return m_Cache[key];
        }

        return default(V);
    }

    /// <summary>
    /// 检查是否已经缓存某个对象
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Has(T key)
    {
        return m_Cache.ContainsKey(key);
    }

    /// <summary>
    /// 释放所有资源
    /// </summary>
    public void ReleaseAll()
    {
        m_Cache.Clear();
    }

    /// <summary>
    /// 通过key来释放缓存
    /// </summary>
    /// <param name="key"></param>
    public bool RemoveByKey(T key)
    {
        return m_Cache.Remove(key);
    }

    /// <summary>
    /// 通过值来释放缓存
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool RemoveByValue(V data)
    {
        foreach (var item in m_Cache)
        {
            if (item.Value.Equals(data))
            {
                return m_Cache.Remove(item.Key);
            }
        }

        return false;
    }
}