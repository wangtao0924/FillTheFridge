using ExcelDataClass;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// πÿø®π‹¿Ì
/// </summary>
public class GameLevelManager
{
    public GameLevel curLevel;
    public GameLevel nextLevel;
   
    public void Init()
    {
        curLevel = new GameLevel();
        nextLevel = new GameLevel();
        LevelItem levelItem = DataManager.Inst.GetLevelItemByID(GameSaveData.Instance.localSaveData.gameLevel);
        LevelItem nextLevelItem = DataManager.Inst.GetLevelItemByID(GameSaveData.Instance.localSaveData.gameLevel+1);
        curLevel.InitGameLevel(levelItem);
        nextLevel.InitGameLevel(nextLevelItem);
    }

    public void GameCustoms()
    {
        GameObject.DestroyImmediate(curLevel.gameObject);
        curLevel = null;
        curLevel = nextLevel;
        curLevel.gameObject.SetActive(true);
        nextLevel = null;
        nextLevel = new GameLevel();
        LevelItem nextLevelItem = DataManager.Inst.GetLevelItemByID(GameSaveData.Instance.localSaveData.gameLevel + 1);
        nextLevel.InitGameLevel(nextLevelItem);
    }

    public void ResetLevel()
    {
        GameObject.DestroyImmediate(curLevel.gameObject);
        curLevel = null;
        curLevel = new GameLevel();
        LevelItem levelItem = DataManager.Inst.GetLevelItemByID(GameSaveData.Instance.localSaveData.gameLevel);
        curLevel.InitGameLevel(levelItem);
    }

}
