using DG.Tweening;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MPStudio
{
    /// <summary>
    /// UGUI工具类
    /// </summary>
    public static class MPUITools
    {
        /// <summary>
        /// 获取一个对象的画布坐标
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector2 GetGameObjectCanvasPos(GameObject target)
        {
            var canvas = MPSceneManager.RunningScene.UICanvas;
            var canvasRT = canvas.transform as RectTransform;
            Vector2 pos;
            var spos = canvas.worldCamera.WorldToScreenPoint(target.transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, spos, canvas.worldCamera, out pos);
            return pos;
        }

        /// <summary>
        /// 得到屏幕指定点上的UI组件
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <param name="ResultList">结果组件</param>
        /// <returns></returns>
        public static bool GetScreenPosOverUIObjects(Vector2 screenPosition, ref List<RaycastResult> ResultList)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = screenPosition;
            EventSystem.current.RaycastAll(eventDataCurrentPosition, ResultList);
            return ResultList.Count > 0;
        }

        /// <summary>
        /// UGUI刷新布局
        /// </summary>
        /// <param name="rect"></param>
        public static void UpdataLayout(RectTransform rect)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }

        /// <summary>
        /// ScrollView定位。自动获取子物体来得到对应scrollRect.vertical(horizontal)NormalizedPosition
        /// 不支持vertical和horizontal同时勾选的情况
        /// </summary>
        /// <returns>0 ~ 1</returns>
        public static float GetScrollViewNormalizedPosition(ScrollRect scrollRect, int currentIndex)
        {
            if (currentIndex == 0) return 1f;

            var childTrans = scrollRect.content.GetChild(currentIndex)?.transform as RectTransform;
            if (childTrans == null)
            {
                Debug.LogError("ScrollView的Content下没有物体或者物体没有RectTransform");
                return 1f;
            }

            Rect viewportRect = scrollRect.viewport.rect;
            Rect contentRect = scrollRect.content.rect;
            var diff = viewportRect.height - contentRect.height;
            var upTop = childTrans.anchoredPosition.y + childTrans.sizeDelta.y / 2f;

            if (upTop < diff)
                return 0;

            return 1f - upTop / diff;
        }
    }
}