using GoogleMobileAds.Api;
using MPStudio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Vector3 boxPutPos = new Vector3(-1.5f, 0.1f, 0f);
    public GameLevelManager levelManager;
    private Ray ray;
    private RaycastHit hit;
    private GameObject clickObj;
    private RefrigeratorBox curSelectBox;
    private Basket curSelectBasket;
    private bool isBoxOut = false;
    private RefrigeratorSize boxCheckColliderPos;
    private bool isRefrigeratorOpen;
    private bool isUndoCost;
    private BoxCollider boxClickCollider;

    [HideInInspector]
    public AdMgr adMgr;
    [HideInInspector]
    public int curStar;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        MPMsg.On(MPMsgEvent.OnClickBoxPutBack, OnClickBoxPutBack, this);
        MPMsg.On(MPMsgEvent.OnClickUndoCost, OnClickUndoCost, this);
        MPMsg.On(MPMsgEvent.OnClickNextLevel, OnClickNextLevel, this);
        MPMsg.On(MPMsgEvent.PutInRefrigeratorBox, PutInRefrigeratorBox, this);
        MPMsg.On(MPMsgEvent.OnClickRetryLevel, OnClickRetryLevel, this);
        MPMsg.On(MPMsgEvent.GoodsOutBox, GoodsOutBox, this);
        //初始化数据模块 TODO
        //读取数据 TODO
        InitAds();
        levelManager = new GameLevelManager();
        levelManager.Init();
    }
    List<string> deveceIds;
    void InitAds()
    {
        deveceIds = new List<string>();
        deveceIds.Add("3AC3161C884AAF219B5C7AD6C57B9A62");
        RequestConfiguration requestConfiguration = new RequestConfiguration.Builder().SetTestDeviceIds(deveceIds).build();
        adMgr = new AdMgr();
        adMgr.TenjinInit();
        adMgr.Init();
    }

    private void OnClickRetryLevel(string msg, object[] param)
    {
        ResetLevelState();
        levelManager.ResetLevel();
    }

    private void OnClickNextLevel(string msg, object[] param)
    {
        ResetLevelState();
        levelManager.GameCustoms();
        MPMsg.Emit(MPMsgEvent.GameLevelPass);
    }

    private void ResetLevelState()
    {
        curSelectBox = null;
        curSelectBasket = null;
        boxCheckColliderPos = null;
        boxClickCollider = null;
        clickObj = null;
        isRefrigeratorOpen = false;
        isBoxOut = false;
        isUndoCost = false;
    }

    private void PutInRefrigeratorBox(string msg, object[] param)
    {
        int goodsCount = 0;
        List<Basket> baskets = levelManager.curLevel.baskets;
        for (int i = 0; i < baskets.Count; i++)
        {
            goodsCount += baskets[i].goodsLst.Count;
        }
        MPMsg.Emit(MPMsgEvent.LevelStarsUpdate, goodsCount, levelManager.curLevel.goodsAllCount);
        if (goodsCount == 0)
        {
            curSelectBox.ClickBoxMoveReturn(() =>
            {
                levelManager.curLevel.refrigerator.CloseDoor(() =>
                {
                    UIManager.Inst.ShowUI(E_UiId.LevelCompleted);
                    GameSaveData.Instance.localSaveData.gameLevel += 1;
                    GameSaveData.Instance.SaveData();
                });

            });
        }
    }
    private void GoodsOutBox(string msg, object[] param)
    {
        int goodsCount = 0;
        List<Basket> baskets = levelManager.curLevel.baskets;
        for (int i = 0; i < baskets.Count; i++)
        {
            goodsCount += baskets[i].goodsLst.Count;
        }
        MPMsg.Emit(MPMsgEvent.LevelStarsUpdate, goodsCount, levelManager.curLevel.goodsAllCount);
    }
    private void OnClickUndoCost(string msg, object[] param)
    {
        isUndoCost = true;
        SetSelectBoxGoodsColliderEnabled(true);
        if (curSelectBasket != null)
        {
            curSelectBasket.ClickGiveUpSelectBasket();
            curSelectBasket = null;
        }
    }

    private void SetSelectBoxGoodsColliderEnabled(bool enabled)
    {
        SetAllGoodsColliderEnabled(enabled);
    }


    private void OnClickBoxPutBack(string msg, object[] param)
    {
        SetSelectBoxGoodsColliderEnabled(false);
        if (isBoxOut)
        {
            isBoxOut = false;
            curSelectBox.ClickBoxMoveReturn(() => { SetAllRefrigeratorBoxEnabled(true); });
        }
        isUndoCost = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRefrigeratorOpen)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    clickObj = hit.collider.gameObject;
                    if (clickObj == levelManager.curLevel.refrigerator.door.gameObject)
                    {
                        levelManager.curLevel.refrigerator.OpenDoor(() => { isRefrigeratorOpen = true; });
                    }
                }
            }
        }
        if (isRefrigeratorOpen)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000, ~(1 << LayerMask.NameToLayer("Water"))))
                {
                    clickObj = hit.collider.gameObject;
                    if (curSelectBox!=null)
                    {
                        curSelectBox.lastfillPos.Clear();
                    }
                    BoxMoveOut();
                    SelectBasket();
                }
            }
            if (Input.GetMouseButton(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    clickObj = hit.collider.gameObject;
                    if (!isUndoCost)
                    {
                        BoxPutGoods();
                    }
                    else
                    {
                        BoxGoodsOut();
                    }
                }
            }
        }
    }

    private void SelectBasket()
    {
        List<Basket> baskets = levelManager.curLevel.baskets;
        for (int i = 0; i < baskets.Count; i++)
        {
            if (baskets[i].gameObject == clickObj)
            {
                if (curSelectBasket != null)
                {
                    curSelectBasket.ClickGiveUpSelectBasket();
                }
                curSelectBasket = baskets[i];
                curSelectBasket.ClickSelectBasket();
                isUndoCost = false;
                SetSelectBoxGoodsColliderEnabled(false);
            }
        }
    }

    private void BoxMoveOut()
    {
        if (!isBoxOut)
        {
            List<RefrigeratorPlatforms> refrigeratorPlatforms = levelManager.curLevel.refrigerator.platformsLst;
            for (int i = 0; i < refrigeratorPlatforms.Count; i++)
            {
                List<RefrigeratorBox> boxs = refrigeratorPlatforms[i].refrigeratorBoxs;
                for (int j = 0; j < boxs.Count; j++)
                {
                    if (boxs[j].gameObject == clickObj)
                    {
                        curSelectBox = boxs[j];
                        isBoxOut = true;
                        curSelectBox.ClickBoxMoveOut(() =>
                        {
                            SetAllRefrigeratorBoxEnabled(false);
                            int random = UnityEngine.Random.Range(0, 100);
                            if (random < 30)
                            {
                                ShowInerstitialAd();
                            }
                        });
                    }
                }
            }
        }
    }

    private void SetAllRefrigeratorBoxEnabled(bool enabled)
    {
        List<RefrigeratorPlatforms> refrigeratorPlatforms = levelManager.curLevel.refrigerator.platformsLst;
        for (int i = 0; i < refrigeratorPlatforms.Count; i++)
        {
            List<RefrigeratorBox> boxs = refrigeratorPlatforms[i].refrigeratorBoxs;
            for (int j = 0; j < boxs.Count; j++)
            {
                boxs[j].gameObject.GetComponent<BoxCollider>().enabled = enabled;
            }
        }
    }

    private void BoxPutGoods()
    {
        if (isBoxOut)
        {
            BoxCollider boxCollider = clickObj.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxClickCollider = boxCollider;
                boxCheckColliderPos = curSelectBox.GetCheckColliderPos(boxClickCollider);
                if (boxCheckColliderPos != null)
                {
                    if (curSelectBasket != null)
                    {
                        if (curSelectBasket.goodsLst.Count > 0)
                        {
                            curSelectBox.CheckPutBox(curSelectBasket.goodsLst[curSelectBasket.goodsLst.Count - 1], boxCheckColliderPos);
                        }
                    }
                }
            }
        }
    }

    private void BoxGoodsOut()
    {
        if (isBoxOut)
        {
            if (curSelectBox != null)
            {
                for (int i = 0; i < curSelectBox.boxGoods.Count; i++)
                {
                    if (clickObj.transform.parent.gameObject == curSelectBox.boxGoods[i].gameObject)
                    {
                        curSelectBox.GoodsOut(curSelectBox.boxGoods[i]);
                    }
                }
            }
        }
    }

    private void SetAllGoodsColliderEnabled(bool enabled)
    {
        List<Basket> baskets = levelManager.curLevel.baskets;
        for (int i = 0; i < baskets.Count; i++)
        {
            List<RefrigeratorGoods> goods = baskets[i].goodsLst;
            for (int j = 0; j < goods.Count; j++)
            {
                goods[j].SetGoodsColliderEnabled(enabled);
            }
        }

        List<RefrigeratorPlatforms> platformsLst = levelManager.curLevel.refrigerator.platformsLst;
        for (int i = 0; i < platformsLst.Count; i++)
        {
            List<RefrigeratorBox> refrigeratorBoxes = platformsLst[i].refrigeratorBoxs;
            for (int j = 0; j < refrigeratorBoxes.Count; j++)
            {
                refrigeratorBoxes[j].SetBoxGoodsColliderEnabled(enabled);
            }
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            adMgr.TenjinInit();
        }
    }
    public void ShowInerstitialAd()
    {
//#if UNITY_EDITOR
//        return;
//#endif
        adMgr.InterstitialAd.Show();
        adMgr.InterstitialAd.OnClose = () =>
        {
            adMgr.InterstitialAd.Hide();
            adMgr.InterstitialAd.Create();
        };
    }
    public void ShowRewardedAd(UnityEngine.Events.UnityAction action)
    {
//#if UNITY_EDITOR
//        return;
//#endif
        adMgr.RewardedAd.Show();
        adMgr.RewardedAd.OnUserEarnedReward = () =>
         {
             adMgr.RewardedAd.Hide();//销毁现在的
             adMgr.RewardedAd.Create();//广告结束后重新创建新的
             action?.Invoke();
         };
    }
}
