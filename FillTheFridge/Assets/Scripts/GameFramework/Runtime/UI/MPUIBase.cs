﻿using System;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MPStudio
{
    /// <summary>
    /// UI基类
    /// </summary>
    public abstract class MPUIBase : MonoBehaviour
    {
        /// <summary>
        /// 矩形变换组件
        /// </summary>
        public RectTransform rectTransform => transform as RectTransform;

        /// <summary>
        /// 创建UI并返回组件实例
        /// </summary>
        /// <param name="parent">父节点</param>
        public static T CreateUIObject<T>(string prefab_path, Transform parent) where T : MPUIBase
        {
            // 当创建的父节点为空时，自动挂载到当前场景的画布下
            // 需要场景中存在一个Canvas
            if (parent == null)
                parent = MPSceneManager.RunningScene?.UICanvas?.transform;

            if (parent == null)
            {
                MPLOG.E("ui", "the ui parent is null and the scene has no canvas");
                return null;
            }

            // 获取预制体内存镜像
            var source = MPResCacher.GetCachedGameObject(prefab_path);
            if (source == null)
            {
                MPLOG.E("ui", $"the ui:{prefab_path} has not cached!!!");
                return null;
            }

            // 创建UI预制体实例
            GameObject CreatedUI = GameObject.Instantiate(source, parent, false);
            // 向UI上添加自身组件
            T Ins = CreatedUI.AddComponent<T>();

            //返回UI实例
            return Ins;
        }

        /// <summary>
        /// 解析绑定特性
        /// 只用解析一次即可
        /// </summary>
        protected static CUIBind ParseBindInfo<T>() where T : MPUIBase
        {
            // UI类型
            Type Tp = typeof(T);

            // 获取UI预制体注入特性
            var attrs = Tp.GetCustomAttributes(typeof(CUIBind), false);
            if (attrs != null && attrs.Length > 0)
            {
                return attrs[0] as CUIBind;
            }
            else
            {
                MPLOG.E("ui", $"the UI type:{Tp.Name} has no bind prefab url");
                return null;
            }
        }
    }

    /// <summary>
    /// UI特性
    /// 指定一个UI类的预制体
    /// 通过UI模组创建该UI时
    /// 会自动处理
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CUIBind : Attribute
    {
        /// <summary>
        /// 模态背景色
        /// </summary>
        public string BackColor { get; set; }

        /// <summary>
        /// 是否是模态UI
        /// </summary>
        public bool IsModel { get; set; } = false;

        /// <summary>
        /// 已缓存的
        /// UI预制体名Key
        /// </summary>
        public string Prefab { get; set; }
    }
}