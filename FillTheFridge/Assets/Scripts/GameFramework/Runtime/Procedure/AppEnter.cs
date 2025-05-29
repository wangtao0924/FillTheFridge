using System.Collections;
using System.Collections.Generic;
using MPStudio;
using Protobuf;
using ProtoBuf;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using static Protobuf.ProtobufTool;

public class AppEnter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartProcedure();
    }

    /// <summary>
    /// 启动流程框架
    /// </summary>
    private void StartProcedure()
    {
        var manager = MPProcedureManager.Inst;
        // 按顺序添加流程
        manager.AddProcedure<GameEnterProcedure>(MPAppConst.GameEnterProcedureName); // 游戏启动流程
        manager.AddProcedure<LoginProcedure>(MPAppConst.LoginProcedureName); // 登陆流程
        manager.AddProcedure<MainPageProcedure>(MPAppConst.MainPageProcedureName); // 主页流程

        // 游戏启动流程
        manager.SwitchProcedure(MPAppConst.GameEnterProcedureName);
    }
}
