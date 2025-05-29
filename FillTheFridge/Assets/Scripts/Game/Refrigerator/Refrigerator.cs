using DG.Tweening;
using EasyButtons;
using ExcelDataClass;
using MPStudio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ±ùÏä
/// </summary>
public class Refrigerator
{
    public Transform door;
    public Transform platformsTrans;
    public List<RefrigeratorPlatforms> platformsLst;
    private GameObject gameObject;
    private string[] platforms;

    public Refrigerator(GameObject obj, RefrigeratorDataItem refrigeratorDataItem)
    {
        platforms = refrigeratorDataItem.platforms.Split(';');
        platformsLst = new List<RefrigeratorPlatforms>();
        gameObject = obj;
        gameObject.transform.localPosition = Vector3.zero;
        platformsTrans = gameObject.transform.Find("Platforms");
        door = gameObject.transform.Find("Fridge/Door");
        InitRefrigeratorPlatforms();
    }
 
    public void InitRefrigeratorPlatforms()
    {
        for (int i = 0; i < platformsTrans.childCount; i++)
        {
            Transform child = platformsTrans.GetChild(i);
            string[] platformInfo = platforms[i].Split('_');
            RefrigeratorPlatforms refrigeratorPlatforms = new RefrigeratorPlatforms(child.gameObject);
            refrigeratorPlatforms.platformsType = int.Parse(platformInfo[0]);
            refrigeratorPlatforms.InitRefrigeratorBox();
            platformsLst.Add(refrigeratorPlatforms);
        }
       
    }

    public void OpenDoor(Action action)
    {
        door.transform.DORotate(new Vector3(0, -120, 0), 1, RotateMode.Fast).onComplete = () => {
            action();
        };
        MPSoundEffect.PlayEffect("Assets/Res/Art/AudioClip/fridgeOpen.wav");
    }

    public void CloseDoor(Action action)
    {
        door.transform.DORotate(new Vector3(0, 0, 0), 1, RotateMode.Fast).onComplete = () =>
        {
            action();
        }; ;
        MPSoundEffect.PlayEffect("Assets/Res/Art/AudioClip/fridgeClose.wav");
    }
}

public class RefrigeratorSize
{
    public int length;
    public int width;
    public int height;

    public RefrigeratorSize(int length, int width, int height)
    {
        this.length = length;
        this.width = width;
        this.height = height;
    }
}
