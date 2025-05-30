using ExcelDataClass;
using MPStudio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 关卡
/// </summary>
public class GameLevel
{
    public Refrigerator refrigerator;
    public GameObject gameObject;
    private string prefabPath= "Assets/Res/Art/prefab/";
    public List<Basket> baskets = new List<Basket>();
    private int basketIndex=-1;
    public Transform basketsTrans;
    private RefrigeratorDataItem refrigeratorDataItem;
    private BasketDataItem  basketDataItem;
    public int goodsAllCount;
    LevelItem levelItem;
    public void InitGameLevel(LevelItem levelItem)//传入配置数据
    {
        this.levelItem = levelItem;
        refrigeratorDataItem = DataManager.Inst.GetRefrigeratorDataItemByID(levelItem.levelRefrigerator);
        basketDataItem = DataManager.Inst.GetBasketDataItemByID(levelItem.levelbasket);
        string[] basketInfo = basketDataItem.basketInfo.Split(';');
        for (int i = 0; i < basketInfo.Length; i++)
        {
            string[] info = basketInfo[i].Split('_');
            goodsAllCount += int.Parse(info[2]);
        }
        LoadLevel();
        GameManager.instance.curStar = 0;
    }

    public async void LoadLevel()
    {
        string levelPrefabPath = prefabPath+ "GameLevel.prefab";
        string backgroundPrefabPath = levelItem.levelBackground;
        string refrigeratorPrefabPath = refrigeratorDataItem.refrigeratorPath;
        await MPRes.InstantiatePrefab(levelPrefabPath, LoadLevelPrefabComplete);
        Transform transform = gameObject.transform;
        await MPRes.InstantiatePrefab(backgroundPrefabPath, transform.Find("Background"), null);
        await MPRes.InstantiatePrefab(refrigeratorPrefabPath, transform.Find("Refrigerator"),LoadRefrigeratorPrefabComplete);

    }

    private void LoadRefrigeratorPrefabComplete(GameObject obj)
    {
        refrigerator = new Refrigerator(obj, refrigeratorDataItem);
        string[] basketInfo = basketDataItem.basketInfo.Split(';');
        for (int i = 0; i < basketInfo.Length; i++)
        {
            LoadBaskets();
        }
    }

    private void LoadLevelPrefabComplete(GameObject obj)
    {
        gameObject = obj;
        if (levelItem.id != GameSaveData.Instance.localSaveData.gameLevel)
        {
            gameObject.SetActive(false);
        }
        basketsTrans = gameObject.transform.Find("Baskets");
    }

    public async void LoadBaskets()
    {
        string[] basketInfo = basketDataItem.basketInfo.Split(';');
        string[] info = basketInfo[basketIndex + 1].Split('_');
        string basketPrefabPath = "";
        switch (info[0])
        {
            case "1":
                basketPrefabPath = prefabPath + "Basket/Basket_g.prefab";
                break;
            case "2":
                basketPrefabPath = prefabPath + "Basket/Basket_g.prefab";
                break;
            case "3":
                basketPrefabPath = prefabPath + "Basket/Basket_g.prefab";
                break;
        }
        await MPRes.InstantiatePrefab(basketPrefabPath, LoadBasketPrefabComplete);
    }

    private void LoadBasketPrefabComplete(GameObject obj)
    {
        string[] basketInfo = basketDataItem.basketInfo.Split(';');
        string[] info = basketInfo[basketIndex + 1].Split('_');
        int goodsId = int.Parse(info[1]);
        int goodsCount = int.Parse(info[2]);
        GoodsItem goodsItem = DataManager.Inst.GetGoodsItemByID(goodsId);
        obj.transform.SetParent(basketsTrans);
        Basket basket = new Basket(obj);
        basket.InitBasket(goodsItem, goodsCount);
        obj.transform.localPosition = new Vector3(0, 0, basketIndex * -0.3f);
        obj.transform.localEulerAngles = Vector3.zero;
        baskets.Add(basket);
        basketIndex++;
    }
}
