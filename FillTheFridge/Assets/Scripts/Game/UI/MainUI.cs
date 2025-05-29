using MPStudio;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : BaseUI
{
    public Button btnRetry;
    public Button btnSetttings;
    public Button btnUndoCost;
    public Button btnTick;
    public TextMeshProUGUI textLevel;
    public Slider starsProgress;
    public List<Image> imageStarts;
    public Button btnNext;

    protected override void InitOnStart()
    {
        btnRetry.onClick.AddListener(OnClickRetry);
        btnSetttings.onClick.AddListener(OnClickSetttings);
        btnUndoCost.onClick.AddListener(OnClickRetryLevel);
        btnTick.onClick.AddListener(OnClickTick);
        btnNext.onClick.AddListener(OnClickNext);
        MPMsg.On(MPMsgEvent.LevelStarsUpdate,OnLevelStarsUpdate,this);
        MPMsg.On(MPMsgEvent.GameLevelPass, OnGameLevelPass, this);
        MPMsg.On(MPMsgEvent.OnClickRetryLevel, OnClickRetryLevel, this);
    }

    private void OnClickRetryLevel(string msg, object[] param)
    {
        starsProgress.value = 0;
    }

    public override void ShowUI()
    {
        base.ShowUI();
        textLevel.text = "Level " + GameSaveData.Instance.localSaveData.gameLevel;
        btnNext.gameObject.SetActive(false);
    }

    private void OnGameLevelPass(string msg, object[] param)
    {
        starsProgress.value = 0;
        textLevel.text = "Level " + GameSaveData.Instance.localSaveData.gameLevel;
    }

    private void OnClickTick()
    {
        MPMsg.Emit(MPMsgEvent.OnClickBoxPutBack);
    }
    void OnClickNext()
    {
        GameSaveData.Instance.localSaveData.gameLevel += 1;
        GameSaveData.Instance.SaveData();
        MPMsg.Emit(MPMsgEvent.OnClickNextLevel);
        btnNext.gameObject.SetActive(false);
        
        textLevel.text = "Level " + GameSaveData.Instance.localSaveData.gameLevel;
    }

    private void OnClickRetryLevel()
    {
        MPMsg.Emit(MPMsgEvent.OnClickUndoCost);
    }

    private void OnLevelStarsUpdate(string msg, object[] param)
    {
        int curStarValue = (int)param[0];
        int maxStarValue = (int)param[1];
        starsProgress.value = 1-(curStarValue*1.0f / maxStarValue);
        for (int i = 0; i < imageStarts.Count; i++)
        {
            imageStarts[i].enabled = starsProgress.value >=1.0f/imageStarts.Count * (i+1);
        }
        int star =(int) (starsProgress.value / (1f / imageStarts.Count));
        if (GameManager.instance.curStar< star&& star< imageStarts.Count)
        {
            GameManager.instance.curStar = star;
            if (GameManager.instance.curStar >= 1)
            {
                btnNext.gameObject.SetActive(true);
            }
            GameManager.instance.ShowInerstitialAd();
        }
    }

    private void OnClickSetttings()
    {
        UIManager.Inst.ShowUI(E_UiId.SettingsUI);
    }

    private void OnClickRetry()
    {
        GameManager.instance.ShowRewardedAd(() =>
        {
            MPMsg.Emit(MPMsgEvent.OnClickRetryLevel);
            btnNext.gameObject.SetActive(false);
        });
       
    }
}
