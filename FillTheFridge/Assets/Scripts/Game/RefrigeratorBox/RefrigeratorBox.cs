using DG.Tweening;
using MPStudio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefrigeratorBox
{
    private RefrigeratorSize boxSizi;
    private bool[,,] isFillBox;
    private BoxCollider[,] checkColliders;
    private Vector3 boxMovePos = new Vector3(-2.8f, 0.97f, 0);
    private Vector3 boxMoveRota = new Vector3(0, 0, 60);
    private Vector3 initialPos;
    private Vector3 initialRota;
    private float moveTime = 0.8f;
    public GameObject gameObject;
    public List<RefrigeratorGoods> boxGoods;
    public List<RefrigeratorSize> lastfillPos;

    public RefrigeratorBox(GameObject obj, RefrigeratorSize size)
    {
        lastfillPos = new List<RefrigeratorSize>();
        boxGoods = new List<RefrigeratorGoods>();
        gameObject = obj;
        initialPos = gameObject.transform.position;
        initialRota = gameObject.transform.localEulerAngles;
        boxSizi = size;
        isFillBox = new bool[(int)boxSizi.width, (int)boxSizi.length, (int)boxSizi.height];
        InitCheckColliders((int)boxSizi.width, (int)boxSizi.length);
    }

    public void InitCheckColliders(int width, int length)
    {
        checkColliders = new BoxCollider[width, length];
        Transform transform = gameObject.transform.Find("UnitBox");
        if (transform != null)
        {
            int widthIndex = 0;
            int lengthIndex = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                widthIndex = i / length;
                if (i % length == 0 && widthIndex > 0)
                {
                    lengthIndex = 0;
                }
                checkColliders[widthIndex, lengthIndex] = transform.GetChild(i).GetComponent<BoxCollider>();
                lengthIndex++;
            }
        }
    }

    public void SetCheckCollidersEnabled(bool enabled)
    {
        for (int i = 0; i < checkColliders.GetLength(0); i++)
        {
            for (int j = 0; j < checkColliders.GetLength(1); j++)
            {
                checkColliders[i, j].enabled = enabled;
            }
        }
    }
    public RefrigeratorSize GetCheckColliderPos(BoxCollider boxCollider)
    {
        for (int i = 0; i < checkColliders.GetLength(0); i++)
        {
            for (int j = 0; j < checkColliders.GetLength(1); j++)
            {
                if (boxCollider == checkColliders[i, j])
                {
                    return new RefrigeratorSize(j, i, 0);
                }
            }
        }
        return null;
    }

    public void CheckPutBox(RefrigeratorGoods goods, RefrigeratorSize boxColliderPos)
    {
        if (CheckClickFillSame(boxColliderPos))
        {
            return;
        }
        int length = boxColliderPos.length;
        int width = boxColliderPos.width;
        float posx = 0;
        float posy = 0;
        float posz = 0;
        List<RefrigeratorSize> fillPos = new List<RefrigeratorSize>();
        int heightCount = 0;
        int heightIndex = 0;
        if (goods.goodSize.length > boxSizi.length)
        {
            return;
        }
        for (int i = 0; i < boxSizi.height; i++)
        {
            bool isFill = CheckBoxFill(boxColliderPos, goods.goodSize, i, fillPos);
            if (isFill)
            {
                heightCount = 0;
                fillPos.Clear();
            }
            else
            {
                heightCount++;
            }
            heightIndex++;
            if (heightCount >= goods.goodSize.height)
            {
                break;
            }
        }
        if (fillPos.Count == 0 || (goods.goodSize.height - heightCount) > 0)
        {
            return;
        }
        for (int i = 0; i < fillPos.Count; i++)
        {
            lastfillPos.Add(fillPos[i]);
        }
        goods.SetBoxFillPos(fillPos);
        for (int i = 0; i < fillPos.Count; i++)
        {
            isFillBox[fillPos[i].width, fillPos[i].length, fillPos[i].height] = true;
        }
        int ergodicCount = fillPos.Count / goods.goodSize.height;
        for (int i = 0; i < ergodicCount; i++)
        {
            if (i / goods.goodSize.length == 0)
            {
                posz += checkColliders[fillPos[i].width, fillPos[i].length].transform.localPosition.z;
            }
            if (i % goods.goodSize.length == 0)
            {
                posx += checkColliders[fillPos[i].width, fillPos[i].length].transform.localPosition.x;
            }
        }
        posz = posz / goods.goodSize.length;
        posx = posx / goods.goodSize.width;
        posy = 0.006f + ((heightIndex - heightCount) * 0.05f);
        Transform transform = gameObject.transform.Find("UnitBox");
        Vector3 pos = new Vector3(posx, posy, posz);
        goods.EnterBox(transform, pos);
        boxGoods.Add(goods);
        MPMsg.Emit(MPMsgEvent.PutInRefrigeratorBox);
        MPSoundEffect.PlayEffect("Assets/Res/Art/AudioClip/HeavyRunningWoodFootsteps.wav");
    }


    private bool CheckBoxFill(RefrigeratorSize boxColliderPos, RefrigeratorSize goodsSize, int boxHeight, List<RefrigeratorSize> fillPos)
    {
        int finalLength = 0;
        int finalWidth = 0;
        int startLength = boxColliderPos.length - (boxColliderPos.length % 2);
        int startWidth = boxColliderPos.width;
        if (goodsSize.length+boxColliderPos.length>boxSizi.length)
        {
            startLength = boxSizi.length - goodsSize.length;
        }
        if (goodsSize.width+boxColliderPos.width > boxSizi.width)
        {
            startWidth = boxSizi.width - goodsSize.width;
        }
        for (int i = 0; i < goodsSize.width; i++)
        {
            for (int j = 0; j < goodsSize.length; j++)
            {
                finalLength = startLength + j;
                finalWidth = startWidth + i;
                if (isFillBox[finalWidth, finalLength, boxHeight])
                {
                    return true;
                }
                fillPos.Add(new RefrigeratorSize(finalLength, finalWidth, boxHeight));
            }
        }
        return false;
    }

    private bool CheckClickFillSame(RefrigeratorSize boxColliderPos)
    {
        for (int i = 0; i < lastfillPos.Count; i++)
        {
            if (lastfillPos[i].width == boxColliderPos.width && lastfillPos[i].length == boxColliderPos.length)
            {
                return true;
            }
        }
        return false;
    }

    public void GoodsOut(RefrigeratorGoods goods)
    {
        for (int i = 0; i < goods.boxFillPos.Count; i++)
        {
            isFillBox[goods.boxFillPos[i].width, goods.boxFillPos[i].length, goods.boxFillPos[i].height] = false;
        }
        boxGoods.Remove(goods);
        goods.ReSetPos();
        MPMsg.Emit(MPMsgEvent.GoodsOutBox);
        MPSoundEffect.PlayEffect("Assets/Res/Art/AudioClip/PopSound.wav");
    }
    public void SetBoxGoodsColliderEnabled(bool enabled)
    {
        for (int i = 0; i < boxGoods.Count; i++)
        {
            boxGoods[i].SetGoodsColliderEnabled(enabled);
        }
    }
    public void ClickBoxMoveOut(Action action)
    {
        gameObject.transform.DOMove(boxMovePos, moveTime).onComplete = () => { action(); };
        gameObject.transform.DORotate(boxMoveRota, moveTime, RotateMode.Fast);
        MPSoundEffect.PlayEffect("Assets/Res/Art/AudioClip/boxOpen.wav");
    }

    public void ClickBoxMoveReturn(Action action)
    {
        gameObject.transform.DOMove(initialPos, moveTime).onComplete = () => { action(); };
        gameObject.transform.DORotate(initialRota, moveTime, RotateMode.Fast);
        MPSoundEffect.PlayEffect("Assets/Res/Art/AudioClip/boxClose.wav");
    }
}
