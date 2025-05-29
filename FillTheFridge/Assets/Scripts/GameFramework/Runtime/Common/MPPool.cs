using System;
using System.Collections.Generic;
using UnityEngine;

namespace MPStudio
{
    /// <summary>
    /// 简易对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MPPool<T>
    {
        /// <summary>
        /// 创建方法
        /// </summary>
        private Func<T> _CreateFunc;

        /// <summary>
        /// 对象池
        /// </summary>
        private Stack<T> _Pool;

        /// <summary>
        /// 私有构造
        /// 请通过 CreatePoolWithCreateFunc 创建池子
        /// </summary>
        private MPPool()
        {
            this._Pool = new Stack<T>();
        }

        /// <summary>
        /// 当前对象池可用对象数
        /// </summary>
        public int Size { get => _Pool.Count; }

        /// <summary>
        /// 最大数量
        /// </summary>
        private int MaxSize;

        /// <summary>
        /// 当前容量
        /// </summary>
        private int NowSize;

        /// <summary>
        /// 创建池子
        /// </summary>
        /// <returns></returns>
        public static MPPool<T> CreatePoolWithCreateFunc(Func<T> createFunc, int maxSize = -1)
        {
            if (createFunc == null)
            {
                // MPLOG.E("pool", "the pool create func is null");
                return null;
            }
            var pool = new MPPool<T>();
            pool._CreateFunc = createFunc;
            pool.MaxSize = maxSize;
            return pool;
        }

        /// <summary>
        /// 清理对象池
        /// </summary>
        public void Clear()
        {
            this._Pool.Clear();
        }

        /// <summary>
        /// 从对象池获得对象
        /// </summary>
        /// <returns></returns>
        public T GetObject()
        {
            if (MaxSize > 0 && NowSize >= MaxSize) return default(T);

            NowSize++;
            if (_Pool.Count == 0)
            {
                var obj = _CreateFunc.Invoke();
                return obj;
            }

            return _Pool.Pop();
        }

        /// <summary>
        /// 返回到池子
        /// </summary>
        public void Release(T obj)
        {
            NowSize--;
            this._Pool.Push(obj);
        }
    }
}