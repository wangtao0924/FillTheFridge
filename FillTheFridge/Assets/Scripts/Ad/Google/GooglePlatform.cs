using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
public class GooglePlatform : PlatformBase
{
        List<string> deveceIds = new List<string>();
    public override void Init()
    {
        Debug.Log("googleƽ̨��ʼ��ʼ��");
        this.IsInit = true;
        //MobileAds.Initialize(initStatus => {

        //    //Debug.Log("��Ӳ��Ի�");
        //    //deveceIds.Add("BD6F80A96CD35E9E6BD61D8553BD164B");
        //    //RequestConfiguration requestConfiguration = new RequestConfiguration.Builder().SetTestDeviceIds(deveceIds).build();
        //    //if (GameController.Ins.Level < 1) return;//����ǵ�һ�ز������
            

        //});
        
    }

    // Start is called before the first frame update
  
}
