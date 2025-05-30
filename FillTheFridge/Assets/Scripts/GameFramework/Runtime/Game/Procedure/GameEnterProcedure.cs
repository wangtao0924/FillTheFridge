using System;
using System.Threading.Tasks;
using GameAdvertisement;
using MPStudio;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class GameEnterProcedure : MPProcedure
{
    private bool isLoadedLoginScene;
    // 添加流程时调用
    public override void OnInit()
    {
        base.OnInit();
    }

    public override void OnEnter(MPProcedure lastProcedure)
    {
        base.OnEnter(lastProcedure);
        UnityEngine.Debug.Log("GameEnterProcedure");
        // 初始化Manager
        InitManager();

    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (UIManager.Inst.IsInitFinished && !isLoadedLoginScene)
        {
            isLoadedLoginScene = true;
            MPProcedureManager.Inst.SwitchProcedure(MPAppConst.MainPageProcedureName);
            // todo 使用MPSceneManager
            // Addressables.LoadSceneAsync("Assets/Scenes/Login.unity");
            // MPProcedureManager.Inst.SwitchProcedure(MPAppConst.MainPageProcedureName); // 跳过登陆流程 测试用
        }
    }

    public override void OnLeave(MPProcedure nextProcedure)
    {
        base.OnLeave(nextProcedure);
    }


    /// <summary>
    /// 初始化Manager
    /// </summary>
    private async void InitManager()
    {
        InitConfigManager();
        InitUIManager();
        InitNetWorkManager();
        //InitUserDataManager();
    }

    private void InitConfigManager()
    {
       var dataManager= DataManager.Inst;
        DataManager.Inst.LoadAll();
    }
    // 初始化UIManager
    private void InitUIManager()
    {
        var uiManager = UIManager.Inst;
    }


    private void InitNetWorkManager()
    {
        //var manager = MPNetworkManager.Inst;
    }
}