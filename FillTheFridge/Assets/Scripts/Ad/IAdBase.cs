using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAdBase
{
    /// <summary>
    /// չʾ���
    /// </summary>
    void Show();


    /// <summary>
    /// ��ʾ���
    /// </summary>
    void Hide();

    /// <summary>
    /// �������
    /// </summary>
    void Create();

    /// <summary>
    /// �رչ��
    /// </summary>
    //void Close();


    ///// <summary>
    ///// ���������ʱ��ϵͳ��ִ�� OnAdLoaded �¼���
    ///// </summary>
    //void OnAdLoaded();

    ///// <summary>
    ///// ������ʧ��ʱ��ϵͳ����� OnAdFailedToLoad �¼���Message �������������Ĺ������͡�
    ///// </summary>
    //void OnAdFailedToLoad();


    ///// <summary>
    ///// �û��㰴���ʱ��ϵͳ����ô˷����������ʹ�÷�����Ʒ�����ٵ������˷������ʺϼ�¼�����
    ///// </summary>
    //void OnAdOpening();


    ///// <summary>
    ///// �û����������Ӧ�ã����磬Google Play �̵꣩ʱ��ϵͳ���ȵ��� onAdOpened���ٵ��ô˷������Ӷ��ں�̨���е�ǰӦ�á�
    ///// </summary>
    //void OnAdLeavingApplication();

}
