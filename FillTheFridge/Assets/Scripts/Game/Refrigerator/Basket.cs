using DG.Tweening;
using ExcelDataClass;
using MPStudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket
{
    //0.09  -0.09   0.05  -0.05   行最大0.1 6个单位   列最大0.18 9个单位   
    public List<RefrigeratorGoods> goodsLst = new List<RefrigeratorGoods>();
    public GameObject gameObject;
    private int line;
    private int row;
    private GoodsItem goodsItem;
    private int goodsCount;
    private int goodsindex = 0;
    public Basket(GameObject obj)
    {
        gameObject = obj;
    }

    public void InitBasket(GoodsItem goodsItem, int goodsCount)
    {
        line = 6 / (int)goodsItem.goodsSize[0];
        line= line==0?1: line;
        row = 9 / (int)goodsItem.goodsSize[1];
        row=row==0?1: row;
        this.goodsItem = goodsItem;
        this.goodsCount = goodsCount;
        string boxPath = goodsItem.goodsPath;
        LoadGoods(boxPath);
    }

    public async void LoadGoods(string path)
    {
        for (int i = 0; i < goodsCount; i++)
        {
            await MPRes.InstantiatePrefab(path, gameObject.transform, LoadPrefabComplete);
        }
    }

    private void LoadPrefabComplete(GameObject obj)
    {
        RefrigeratorGoods goods = new RefrigeratorGoods(obj, new RefrigeratorSize((int)goodsItem.goodsSize[0], (int)goodsItem.goodsSize[1], (int)goodsItem.goodsSize[2]));
        int heightIndex = goodsindex / (line * row);
        int lineIndex = (goodsindex - (heightIndex * (line * row))) / line;
        int rowIndex = goodsindex % line;
        float posx = 0;
        if (row>1)
        {
            posx = 0.09f - (lineIndex * ((int)goodsItem.goodsSize[1] * 0.025f));
        }
        float poy = heightIndex * (0.05f * (int)goodsItem.goodsSize[2]);
        float posz = 0;
        if (line > 1)
        {
            posz = 0.05f - (rowIndex * ((goodsItem.goodsSize[0]) * 0.025f));
        }
        obj.transform.localPosition = new Vector3(posx, poy, posz);
        obj.transform.localEulerAngles = Vector3.zero;
        goods.SetBelongBasket(this);
        goodsLst.Add(goods);
        goodsindex++;
    }

    public void ClickSelectBasket()
    {
        gameObject.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f);
    }

    public void ClickGiveUpSelectBasket()
    {
        gameObject.transform.DOScale(Vector3.one, 0.5f);
    }

}