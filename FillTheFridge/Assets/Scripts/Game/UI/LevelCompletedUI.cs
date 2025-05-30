using MPStudio;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompletedUI : BaseUI
{
    public List<GameObject> starImages;
    public Button btnNext;
    public TextMeshProUGUI textLevel;
    protected override void InitOnStart()
    {
        btnNext.onClick.AddListener(OnClickNextLevel);
    }
    public override void ShowUI()
    {
        base.ShowUI();
        textLevel.text = "Level " + GameSaveData.Instance.localSaveData.gameLevel;
    }
    private void OnClickNextLevel()
    {
        MPMsg.Emit(MPMsgEvent.OnClickNextLevel);
        this.gameObject.SetActive(false);
    }
}
