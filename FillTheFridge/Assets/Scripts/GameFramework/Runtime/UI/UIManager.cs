using System.Net.Sockets;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using MPStudio;
using System.Threading.Tasks;

//这里我把单例模式写成了一个基类UnitySingleton并继承
public class UIManager : MPStudio.MPSingletonMono<UIManager>
{
    public bool IsInitFinished = false;

    //缓存所有打开过的窗体
    private Dictionary<E_UiId, BaseUI> dicAllUI;

    //缓存正在显示的窗体
    private Dictionary<E_UiId, BaseUI> dicShowUI;

    //缓存最近显示出来的窗体
    private BaseUI currentUI = null;

    //缓存上一个窗体
    // private BaseUI beforeUI = null;
    private E_UiId beforeUiId = E_UiId.NullUI;

    //缓存画布
    private Transform canvas;

    //缓存保持在最前方的窗体的父节点
    private Transform backUIRoot;

    //缓存普通窗体的父节点
    private Transform defaultUIRoot;

    private Transform popUIRoot;
    private Transform messageUIRoot;

    private Transform topUIRoot;

    //UI层级
    private Dictionary<E_UIRootType, int> UIOrder;

    Camera _uicamera;

    public Camera UICamera
    {
        get { return _uicamera; }
        set { _uicamera = value; }
    }

    private void Awake()
    {
        GameDefine.InitUIDictionary();
        //Test.Instance.Show();
        dicAllUI = new Dictionary<E_UiId, BaseUI>();
        dicShowUI = new Dictionary<E_UiId, BaseUI>();
        UIOrder = new Dictionary<E_UIRootType, int>();
        InitUIManager();
    }

    //初始化UI管理类
    private async void InitUIManager()
    {
        if (canvas == null)
        {
            var bot = await MPRes.InstantiatePrefab("Assets/Res/UI/UIRoot.prefab");
            canvas = bot.transform.Find("UICanvas");
            UICamera = bot.transform.Find("UICamera").GetComponent<Camera>();
        }

        //设置画布在场景切换的时候不被销毁，因为整个游戏共用唯一的一个画布
        DontDestroyOnLoad(canvas.parent);
        if (backUIRoot == null)
        {
            backUIRoot = SetUIRoot("BackUI");
        }

        if (defaultUIRoot == null)
        {
            defaultUIRoot = SetUIRoot("DefaultUI");
        }

        if (popUIRoot == null)
        {
            popUIRoot = SetUIRoot("PopUI");
        }

        if (messageUIRoot == null)
        {
            messageUIRoot = SetUIRoot("MessageUI");
        }

        if (topUIRoot == null)
        {
            topUIRoot = SetUIRoot("TopUI");
        }

        IsInitFinished = true;
    }

    public Transform SetUIRoot(string Name)
    {
        GameObject aa = new GameObject(Name);
        aa.transform.SetParent(canvas, false);
        aa.layer = LayerMask.NameToLayer("UI");
        Canvas ca = aa.AddComponent<Canvas>();
        ca.overrideSorting = true;
        ca.sortingLayerName = Name;
        RectTransform rec = aa.GetComponent<RectTransform>();
        rec.anchoredPosition = Vector2.zero;
        rec.anchorMin = Vector2.zero;
        rec.anchorMax = Vector2.one;
        rec.sizeDelta = Vector2.zero;
        return aa.transform;
    }

    //供外界调用，销毁窗体
    public void DestroyUI(E_UiId uiId)
    {
        if (dicAllUI.ContainsKey(uiId))
        {
            //存在该窗体，去销毁
            GameObject.Destroy(dicAllUI[uiId].gameObject);
            dicAllUI.Remove(uiId);
            dicShowUI.Remove(uiId);
        }
    }

    //供外界调用的，显示窗体的方法
    public async Task<BaseUI> ShowUI(E_UiId uiId, E_UIRootType setLayer = E_UIRootType.Default,
        bool isSaveBeforeUiId = true, object data = null) 
    {
        if (uiId == E_UiId.NullUI)
        {
            return null;
        }

        BaseUI baseUI = await JudgeShowUI(uiId, setLayer, data);
        if (baseUI != null)
        {
            baseUI.ShowUI();
        }

        if (isSaveBeforeUiId)
        {
            baseUI.BeforeUiId = beforeUiId;
        }

        UnityEngine.Debug.Log("显示UI:" + baseUI.gameObject.name);
        return baseUI;
    }

    //供外界调用，反向切换窗体的方法
    public async void ReturnBeforeUI(E_UiId uiId, E_UIRootType setLayer = E_UIRootType.Default, object data = null)
    {
        await ShowUI(uiId, setLayer, false, data);
    }

    //供外界调用的，隐藏单个窗体的方法
    public void HideSingleUI(E_UiId uiId, Action del = null)
    {
        if (!dicShowUI.ContainsKey(uiId))
        {
            return;
        }

        dicShowUI[uiId].HideUI(del);
        dicShowUI.Remove(uiId);
    }

    private async Task<BaseUI> JudgeShowUI(E_UiId uiId, E_UIRootType rootType, object data = null)
    {
        //判断将要显示的窗体是否已经正在显示了
        if (dicShowUI.ContainsKey(uiId))
        {
            //如果已经正在显示了，就不需要处理其他逻辑了
            return dicShowUI[uiId];
        }

        //判断窗体是否有加载过
        BaseUI baseUI = GetBaseUI(uiId);
        if (baseUI == null)
        {
            //说明这个窗体没显示过（没有加载过）,要去动态加载
            string path = GameDefine.dicPath[uiId];

            GameObject willShowUI = await MPRes.InstantiatePrefab(path);

            if (willShowUI != null)
            {
                //窗体生成出来后，要确保有挂对应的UI脚本
                baseUI = willShowUI.GetComponent<BaseUI>();
                
                
                baseUI.SetUIType(uiId);
                baseUI.SetData(data);
                baseUI.uiType.uiRootType = rootType;
                //if (baseUI == null)
                //{
                //    //说明生成出来的这个窗体上面没有挂载对应的UI脚本
                //    //那么就需要给这个窗体自动添加对应的脚本
                //    Type type = GameDefine.GetTypeByString(uiId);
                //    baseUI = willShowUI.AddComponent(type) as BaseUI;
                //    baseUI.SetUIType(uiId);
                //    baseUI.SetData(data);
                //    baseUI.uiType.uiRootType = rootType;
                //}

                //判断这个窗体是属于哪个父节点的
                Transform uiRoot = GetTheUIRoot(baseUI);
                //GameTool.AddChildToParent(uiRoot, willShowUI.transform);
                willShowUI.transform.SetParent(uiRoot, false);
                // 为什么要设为(0, 0)?
                // willShowUI.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                Canvas ca = willShowUI.GetComponent<Canvas>();
                if (ca == null)
                {
                    ca = willShowUI.AddComponent<Canvas>();
                }

                ca.overrideSorting = true;
                ca.sortingLayerName = ca.transform.parent.GetComponent<Canvas>().sortingLayerName;
                int order = 0;
                UIOrder.TryGetValue(baseUI.uiType.uiRootType, out order);
                UIOrder[baseUI.uiType.uiRootType] = order + 100;
                ca.sortingOrder = UIOrder[baseUI.uiType.uiRootType];
                //这个窗体是第一次加载显示出来的，那么就需要缓存起来
                dicAllUI.Add(uiId, baseUI);
                foreach (var item in willShowUI.GetComponentsInChildren<AutoSetSort>())
                {
                    item.SetCanvas();
                }
            }
            else
            {
                Debug.LogError("指定路径下面找不到对应的预制体");
            }
        }

        UpdateDicShowUIAndHideUI(baseUI);
        return baseUI;
    }

    //更新缓存正在显示的窗体的字典并且隐藏对应的窗体
    private void UpdateDicShowUIAndHideUI(BaseUI baseUI)
    {
        //判断是否需要隐藏其他窗体
        if (baseUI.IsHideOtherUI())
        {
            //如果返回值为true, E_ShowUIMode.HideOther与 E_ShowUIMode.HideAll
            //需要隐藏其他窗体
            if (dicShowUI.Count > 0)
            {
                //有窗体正在显示，就要隐藏对应的窗体
                if (baseUI.uiType.showMode == E_ShowUIMode.HideOther)
                {
                    HideAllUI(false, baseUI);
                }
                else
                {
                    HideAllUI(true, baseUI);
                }
            }
        }

        //更新缓存正在显示的窗体的字典
        dicShowUI.Add(baseUI.GetUiId, baseUI);
    }

    public void HideAllUI(bool isHideAboveUI, BaseUI baseUI = null)
    {
        if (isHideAboveUI)
        {
            //1、隐藏所有的窗体，不管是普通窗体还是保持在最前方的窗体，都需要全部隐藏
            foreach (KeyValuePair<E_UiId, BaseUI> uiItem in dicShowUI)
            {
                uiItem.Value.HideUI();
            }

            dicShowUI.Clear();
        }
        else
        {
            //2、隐藏所有的窗体，但是不包含保持在最前方的窗体
            //缓存所有被隐藏的窗体
            List<E_UiId> list = new List<E_UiId>();
            foreach (KeyValuePair<E_UiId, BaseUI> uiItem in dicShowUI)
            {
                //如果不是保持在最前方的窗体
                if (uiItem.Value.uiType.uiRootType != E_UIRootType.Top)
                {
                    uiItem.Value.HideUI();
                    //存储上一个窗体的ID
                    beforeUiId = uiItem.Key;
                    // baseUI.BeforeUiId= uiItem.Key;
                    list.Add(uiItem.Key);
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                dicShowUI.Remove(list[i]);
            }
        }
    }

    //判断窗体的父物体
    private Transform GetTheUIRoot(BaseUI baseUI)
    {
        switch (baseUI.uiType.uiRootType)
        {
            case E_UIRootType.Back:
                return backUIRoot;
            case E_UIRootType.Default:
                return defaultUIRoot;
            case E_UIRootType.Pop:
                return popUIRoot;
            case E_UIRootType.Message:
                return messageUIRoot;
            case E_UIRootType.Top:
                return topUIRoot;
        }

        return defaultUIRoot;
    }

    private BaseUI GetBaseUI(E_UiId UiId)
    {
        if (dicAllUI.ContainsKey(UiId))
        {
            return dicAllUI[UiId];
        }
        else
        {
            return null;
        }
    }
}