using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DG.Tweening;

using UnityEngine;

namespace MPStudio
{
    /// <summary>
    /// RectTransform 扩展
    /// </summary>
    public static class MPExpandRectTransform
    {
        /// <summary>
        /// 渐入一个UI节点
        /// </summary>
        /// <param name="Target">this扩展</param>
        /// <param name="Duration">持续时间</param>
        /// <param name="Callback">延时回调</param>
        public static void FadeInUINode(this RectTransform Target, float Duration, Action Callback = null)
        {
            if (!Target.gameObject.activeSelf)
            {
                Target.gameObject.SetActive(true);
            }

            var cg = Target.GetCanvasGroup();
            cg.alpha = 0f;
            cg.DOFade(1f, Duration).OnComplete(() => Callback?.Invoke());
        }

        /// <summary>
        /// 渐出一个UI节点
        /// </summary>
        /// <param name="Target">this扩展</param>
        /// <param name="Duration">持续时间</param>
        /// <param name="DisActive">淡出后是否自动屏蔽对象</param>
        /// <param name="Callback">延时回调</param>
        public static void FadeOutUINode(this RectTransform Target, float Duration, bool DisActive = false, Action Callback = null)
        {
            if (!Target.gameObject.activeSelf)
            {
                Target.gameObject.SetActive(true);
            }

            var cg = Target.GetCanvasGroup();
            if (DisActive)
            {
                cg.DOFade(0f, Duration).OnComplete(() => {
                    Target.gameObject.SetActive(false);
                    Callback?.Invoke();
                });
            }
            else
            {
                cg.DOFade(0f, Duration).OnComplete(() => Callback?.Invoke());
            }
        }

        /// <summary>
        /// 深度查找子节点
        /// </summary>
        /// <param name="target"></param>
        /// <param name="targetName">子节点名</param>
        /// <returns></returns>
        public static RectTransform DeepFindChild(this RectTransform target, string targetName)
        {
            Transform _result = null;
            _result = target.Find(targetName);
            if (_result == null)
            {
                foreach (RectTransform child in target)
                {
                    _result = DeepFindChild(child, targetName);
                    if (_result != null)
                    {
                        return _result as RectTransform;
                    }
                }
            }
            return _result as RectTransform;
        }
    }
}