using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AdBase : IAdBase
{
    /// <summary>
    /// ���������ʱ
    /// </summary>
    public UnityAction OnAdLoaded;
    /// <summary>
    /// ������ʧ��ʱ
    /// </summary>
    public UnityAction OnAdFailedToLoad;
    /// <summary>
    /// �û��㰴���ʱ
    /// </summary>
    public UnityAction OnAdOpening;
    /// <summary>
    /// �û����������Ӧ��
    /// </summary>
    public UnityAction OnAdLeavingApplication;
    /// <summary>
    /// ������
    /// </summary>
    public UnityAction OnClose;

    /// <summary>
    /// �û��ۿ�����뽱��
    /// </summary>
    public UnityAction OnUserEarnedReward;

    /// <summary>
    /// չʾ���ʱ
    /// </summary>
    public UnityAction OnShow;

    public abstract void Create();


    public abstract void Hide();


    public abstract void Show();
  
}
