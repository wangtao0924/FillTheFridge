using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefrigeratorGoods
{
    public RefrigeratorSize goodSize;
    public Vector3 originalPos;
    public Vector3 originalRotate;
    public GameObject gameObject;
    public List<RefrigeratorSize> boxFillPos;
    private GameObject goodsColliderObj;
    public Basket basket;
    public RefrigeratorGoods(GameObject obj, RefrigeratorSize size)
    {
        gameObject = obj;
        goodSize = size;
        boxFillPos = new List<RefrigeratorSize>();
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            BoxCollider boxCollider = gameObject.transform.GetChild(i).GetComponent<BoxCollider>();
            if (boxCollider!=null)
            {
                goodsColliderObj = gameObject.transform.GetChild(i).gameObject;
            }
        }
        SetGoodsColliderEnabled(false);
    }

    public void SetBelongBasket(Basket basket)
    {
        this.basket = basket;
        originalPos = gameObject.transform.localPosition;
        originalRotate = gameObject.transform.localEulerAngles;
    }


    public void ReSetPos()
    {
        gameObject.transform.SetParent(basket.gameObject.transform);
        gameObject.transform.localPosition = originalPos;
        gameObject.transform.localEulerAngles = originalRotate;
        basket.goodsLst.Add(this);
    }

    public void SetBoxFillPos(List<RefrigeratorSize> pos)
    {
        boxFillPos = pos;
    }

    public List<RefrigeratorSize> GetBoxFillPos()
    {
        return boxFillPos;
    }

    public void SetGoodsColliderEnabled(bool enabled)
    {
        goodsColliderObj.SetActive(enabled);
    }

    public void EnterBox(Transform parent,Vector3 pos)
    {
        gameObject.transform.SetParent(parent);
        gameObject.transform.localPosition = pos;
        gameObject.transform.localEulerAngles = Vector3.zero;
        gameObject.transform.DOScale(Vector3.one,0.2f);
        basket.goodsLst.Remove(this);
    }
}
