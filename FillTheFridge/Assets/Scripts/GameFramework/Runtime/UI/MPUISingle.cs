using System;
using System.Threading.Tasks;

using DG.Tweening;

using UnityEngine;

using Image = UnityEngine.UI.Image;

namespace MPStudio
{
    /// <summary>
    /// 单体UI，无论创建多少次，永远只能得到一个UI实例
    /// </summary>
    public class MPUISingle<T> : MPUIBase where T : MPUISingle<T>
    {
        /// <summary>
        /// 模态背景
        /// </summary>
        protected static Image _ModelBack = null;

        /// <summary>
        /// UI实例
        /// </summary>
        protected static T _uinst = null;

        /// <summary>
        /// UI是否存在
        /// </summary>
        /// <returns></returns>
        public static bool IsExist => _uinst != null;

        /// <summary>
        /// UI实例
        /// </summary>
        public static T UInst => _uinst;

        /// <summary>
        /// 创建UI
        /// </summary>
        /// <param name="parent">父节点，为空则自动创建到画布下</param>
        /// <returns></returns>
        public static T CreateUI(Transform parent = null)
        {
            // 单一UI无论创建多少次都返回第一个创建的对象
            if (_uinst != null)
            {
                return _uinst;
            }

            // 解析特性
            var bindInfo = ParseBindInfo<T>();

            // 检查是否模态，是否启动模态背景色淡入动画
            if (bindInfo.IsModel)
            {
                // 是模态背景则需要创建模态背景层
                _ModelBack = CreateModelBack(bindInfo.BackColor, parent);
            }

            // 创建UI
            _uinst = CreateUIObject<T>(bindInfo.Prefab, parent);
            _uinst.gameObject.name = MPFile.GetFileName(bindInfo.Prefab);

            return _uinst;
        }

        /// <summary>
        /// <para>创建UI</para>
        /// <para>包含动画</para>
        /// <para>此操作是一个异步过程</para>
        /// <para>await UI_Test.CreateUIWithAnim();</para>
        /// <para>将在动画执行完毕后返回调用点</para>
        /// </summary>
        /// <param name="InAnim">入场动画</param>
        /// <param name="parent">父节点，为空则自动创建到画布下</param>
        public static async Task<T> CreateUIWithAnim(CUIAnimIn InAnim = null, Transform parent = null)
        {
            var ui = CreateUI(parent);

            // 播放入场动画
            await MPUIAnimtaion.PlayInAnim(ui.rectTransform, InAnim);

            return ui;
        }

        /// <summary>
        /// 销毁UI界面
        /// </summary>
        /// <param name="immediate">是否立刻销毁，在一帧内干掉自己</param>
        public static void DestroyUI(bool immediate = false)
        {
            // UI不存在
            if (_uinst == null)
            {
                return;
            }

            // 销毁模态背景
            DestroyModelBack();

            if (_uinst && _uinst.gameObject)
            {
                if (immediate)
                {
                    DestroyImmediate(_uinst.gameObject);
                }
                else
                {
                    Destroy(_uinst.gameObject);
                }
                _uinst = null;
            }
        }

        /// <summary>
        /// <para>干掉UI界面</para>
        /// <para>包含动画</para>
        /// <para>此操作是一个异步过程</para>
        /// <para>await UI_Test.DestroyUIWithAnim( xxx动画 );</para>
        /// <para>将在动画执行完毕后返回调用点</para>
        /// </summary>
        /// <param name="OutAnim">退场动画</param>
        public static async Task DestroyUIWithAnim(CUIAnimOut OutAnim = null)
        {
            // UI不存在
            if (_uinst == null)
            {
                return;
            }

            // 播放退场动画
            await MPUIAnimtaion.PlayOutAnim(_uinst.rectTransform, OutAnim);

            // 销毁模态背景
            DestroyModelBack();

            if (_uinst && _uinst.gameObject)
            {
                Destroy(_uinst.gameObject);
                _uinst = null;
            }
        }

        /// <summary>
        /// 隐藏UI
        /// </summary>
        public static void Hide()
        {
            if (IsExist)
            {
                _uinst.gameObject.SetActive(false);
                if (_ModelBack != null)
                {
                    _ModelBack.gameObject.SetActive(false);
                }
                _uinst.OnHide();
            }
        }

        public virtual void OnHide()
        {

        }

        /// <summary>
        /// 显示UI
        /// </summary>
        public static void Show()
        {
            if (IsExist)
            {
                _uinst.gameObject.SetActive(true);
                if (_ModelBack != null)
                {
                    _ModelBack.gameObject.SetActive(true);
                }
            }
        }

        /// <summary>
        /// 重设坐标
        /// </summary>
        /// <returns></returns>
        public void Reset()
        {
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition = Vector2.zero;
        }

        /// <summary>
        /// 销毁时
        /// </summary>
        protected virtual void OnDestroy()
        {
            DestroyModelBack();
            _uinst = null;
        }

        /// <summary>
        /// 创建模态背景
        /// </summary>
        /// <returns></returns>
        private static Image CreateModelBack(string color, Transform parent)
        {
            // 当创建的父节点为空时，自动挂载到当前场景的画布下
            // 需要场景中存在一个Canvas
            if (parent == null)
            {
                parent = MPSceneManager.RunningScene?.UICanvas?.transform;
                if (parent == null)
                {
                    MPLOG.E("ui", "the ui parent is null and the scene has no canvas");
                    return null;
                }
            }

            // 创建UI预制体实例
            GameObject Back = new GameObject("Back");
            var rt = Back.AddComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;

            var image = Back.AddComponent<Image>();

            Color nowColor;
            ColorUtility.TryParseHtmlString(color, out nowColor);
            image.color = nowColor;

            return image;
        }

        /// <summary>
        /// 销毁模态背景
        /// </summary>
        private static void DestroyModelBack()
        {
            if (_ModelBack != null && _ModelBack.gameObject != null)
            {
                GameObject.Destroy(_ModelBack.gameObject);
                _ModelBack = null;
            }
        }
    }
}