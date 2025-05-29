/********************************************************************
	All Right Reserved By Leo
	Created:	2019/01/14 15:21
	File :      CSingleton.cs
	author:		Leo

	purpose:	��������
                ����
                ��ͨ��������
                Mono��������  �߱�Mono�������ڣ�����;���Update

*********************************************************************/

using UnityEngine;

/// <summary>
/// ��������
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class CSingleton<T> where T : class, new()
{
    /// <summary>
    /// ���͵���ʵ��
    /// </summary>
    protected static T m_Inst = null;

    /// <summary>
    /// ����
    /// </summary>
    public static T Inst
    {
        get
        {
            if (m_Inst == null)
            {
                m_Inst = new T();
            }

            return m_Inst;
        }
    }
}

/// <summary>
/// �߱�MonoBehaviour�������ڵĵ�������
/// �õ���һ�����ã����ڵ�ǰ��������һ��GameObject
/// �õ��������GameObject�������ϵĽű����ʵ��
/// !!!
/// �̳� CSingletonMono �ĵ����������ʹ��Э��
/// �̳� CSingleton     �ĵ���������ʹ��Э��
/// </summary>
/// <typeparam name="T">���� T ,����̳б���</typeparam>
public abstract class CSingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// ���ʵ��
    /// </summary>
    protected static T m_Inst = null;

    /// <summary>
    /// ����
    /// </summary>
    public static T Inst
    {
        get
        {
            // û���ҵ�ʵ��
            if (m_Inst == null)
            {
                m_Inst = GetSingletonNode().AddComponent<T>();
            }

            return m_Inst;
        }
    }

    private static GameObject GetSingletonNode()
    {
        var parent = GameObject.Find("CSingletonMono");
        if (parent == null)
        {
            parent = new GameObject("CSingletonMono");
            DontDestroyOnLoad(parent);
        }
        return parent;
    }
}
