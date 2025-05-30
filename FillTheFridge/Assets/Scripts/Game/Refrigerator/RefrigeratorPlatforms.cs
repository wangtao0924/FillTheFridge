using MPStudio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 冰箱平台
/// </summary>
public class RefrigeratorPlatforms
{
    public List<RefrigeratorBox> refrigeratorBoxs = new List<RefrigeratorBox>();
    public int platformsType = 0;
    public GameObject gameObject;
    public RefrigeratorPlatforms(GameObject obj)
    {
        gameObject = obj;
    }
    public void InitRefrigeratorBox()
    {
        string boxPath = "Assets/Res/Art/prefab/box/";
        string path = "";
        int loadCount=0;
        switch (platformsType)
        {
            case 1:
                path = boxPath + "Box_2_4_2.prefab";
                loadCount = 3;
                break;
            case 2:
                path = boxPath + "Box_3_4_2.prefab";
                loadCount = 2;
                break;
            case 3:
                path = boxPath + "Box_6_4_2.prefab";
                loadCount = 1;
                break;
        }

        //根据配置初始化盒子
        for (int i = 0; i < loadCount; i++)
        {
            LoadBox(path);
        }
    }

    public async void LoadBox(string path)
    {
        await MPRes.InstantiatePrefab(path, gameObject.transform, LoadPrefabComplete);
    }

    private void LoadPrefabComplete(GameObject obj)
    {
        if (obj!=null)
        {
            RefrigeratorBox refrigeratorBoxBase = null;
            switch (platformsType)
            {
                case 1:
                    obj.transform.localPosition = new Vector3(-0.015f, 0.005f, refrigeratorBoxs.Count * 0.145f - 0.145f);
                    refrigeratorBoxBase = new RefrigeratorBox(obj,new RefrigeratorSize(4,8,4));
                    break;
                case 2:
                    if (refrigeratorBoxs.Count == 0)
                    {
                        obj.transform.localPosition = new Vector3(-0.015f, 0.005f, -0.1f);
                    }
                    else
                    {
                        obj.transform.localPosition = new Vector3(-0.015f, 0.005f, 0.1f);
                    }
                    refrigeratorBoxBase = new RefrigeratorBox(obj, new RefrigeratorSize(6, 8, 4));
                    break;
                case 3:
                    obj.transform.localPosition = new Vector3(-0.015f, 0.005f, 0);
                    refrigeratorBoxBase = new RefrigeratorBox(obj, new RefrigeratorSize(12,8,4));
                    break;
            }
            if (refrigeratorBoxBase != null)
            {
                refrigeratorBoxs.Add(refrigeratorBoxBase);
            }
        }
    }

    public void SetBoxColliderEnable(bool isActive)
    {
        for (int i = 0; i < refrigeratorBoxs.Count; i++)
        {
            refrigeratorBoxs[i].gameObject.GetComponent<BoxCollider>().enabled = isActive;
        }
    }
}
