using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MPStudio;

namespace  MPStudio 
{
    public abstract class MPUILocalization<T> : MonoBehaviour where T : Component
    {
        [SerializeField]
        protected T m_Target;
        [SerializeField]
        protected string m_Key;
        // 替换参数
        protected object[] m_Args = null;
        
        // 设置键
        public virtual void SetKey(string key,params object[] args)
        {
            if (key != m_Key)
            {
                m_Key = key;
                m_Args = args;
                OnChange();
            }
        }

        public void OnChange()
        {
            if (!string.IsNullOrEmpty(m_Key) && m_Target != null)
            {
                RefreshShow();
            }
            m_Args = null;
        }
        
        // 刷新显示
        protected abstract void RefreshShow();
        protected virtual void Start()
        {
            MPLocalizationManager.Instance.Register(OnChange);
            OnChange();
        }

        // 获取本地化数据
        protected string GetLocalizationData()
        {
            string data = MPLocalizationManager.Instance.GetLocalizationData(m_Key);
            if (m_Args != null && m_Args.Length > 0)
            {
                return string.Format(data, m_Args);
            }
            return data;
        }
        
        // 获取组件
        protected virtual void OnValidate()
        {
            if (m_Target == null) m_Target = GetComponent<T>();
        }
        
        protected virtual void OnDestroy()
        {
            MPLocalizationManager.Instance.Unregister(OnChange);
        }
    }
}
