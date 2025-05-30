using MPStudio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : BaseUI
{
    public Button btnVoice;
    public Button btnShake;
    public Button btnContinue;

    public Image voiceOn;
    public Image shakeOn;

    public RectTransform voiceCursor;
    public RectTransform shakeCursor;
    protected override void InitOnStart()
    {
        btnVoice.onClick.AddListener(OnClickVoice);
        btnShake.onClick.AddListener(OnClickShake);
        btnContinue.onClick.AddListener(OnClickContinue);
    }

    public override void ShowUI()
    {
        base.ShowUI();
        ShowSettingsUI();
    }

    private void ShowSettingsUI()
    {
        voiceOn.enabled = !GameSaveData.Instance.localSaveData.voiceOff;
        if (!GameSaveData.Instance.localSaveData.voiceOff)
        {
            voiceCursor.anchoredPosition = new Vector2(90, 4);
        }
        else
        {
            voiceCursor.anchoredPosition = new Vector2(-90, 4);
        }

        shakeOn.enabled = !GameSaveData.Instance.localSaveData.shakeOff;
        if (!GameSaveData.Instance.localSaveData.shakeOff)
        {
            shakeCursor.anchoredPosition = new Vector2(90, 4);
        }
        else
        {
            shakeCursor.anchoredPosition = new Vector2(-90, 4);
        }
    }

    private void OnClickContinue()
    {
        HideUI();
    }

    private void OnClickShake()
    {
        GameSaveData.Instance.localSaveData.shakeOff = !GameSaveData.Instance.localSaveData.shakeOff;
        GameSaveData.Instance.SaveData();
        MPMsg.Emit(MPMsgEvent.GameShakeSettings, GameSaveData.Instance.localSaveData.shakeOff);
        ShowSettingsUI();
    }

 

    private void OnClickVoice()
    {
        GameSaveData.Instance.localSaveData.voiceOff = !GameSaveData.Instance.localSaveData.voiceOff;
        GameSaveData.Instance.SaveData();
        MPMsg.Emit(MPMsgEvent.GameVoiceSettings, GameSaveData.Instance.localSaveData.voiceOff);
        ShowSettingsUI();
    }
}
