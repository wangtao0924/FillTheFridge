using MPStudio;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// 游戏主页面流程
/// </summary>
public class MainPageProcedure : MPProcedure
{
    public override void OnInit()
    {
        base.OnInit();
    }

    public override void OnEnter(MPProcedure lastProcedure)
    {
        base.OnEnter(lastProcedure);

        Addressables.LoadSceneAsync("Assets/Scenes/Game.unity"); // 进入Main场景
        SceneManager.sceneLoaded += MainSceneLoaded;
        
        // Addressables.LoadSceneAsync("Assets/TestDemo/TestDemo.unity"); // 进入TestDemo场景
    }

    private void MainSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "Game")
        {
            //SceneManager.sceneLoaded -= MainSceneLoaded;
            UIManager.Inst.ShowUI(E_UiId.MainUI);
        }
    }
}