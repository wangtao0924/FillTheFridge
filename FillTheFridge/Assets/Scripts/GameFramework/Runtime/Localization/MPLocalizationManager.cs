using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace  MPStudio
{
    /// <summary>
    /// 语言类型
    /// </summary>
    public enum MPLanguage
    {
        Chinese,
        English
    }

    public class MPLocalizationManager : MonoBehaviour
    {
        #region 单例
        private static MPLocalizationManager m_Instance = null;
        public static MPLocalizationManager Instance
        {
            get
            {
                if (m_Instance == null) 
                {
                    m_Instance = new GameObject("[MPStudio.MPLocalizationManager]").AddComponent<MPLocalizationManager>();
                    DontDestroyOnLoad(m_Instance);
                }
                
                return m_Instance;
            }
        }
        #endregion
        // 当前的语言
        private MPLanguage m_Language = MPLanguage.English;
        public MPLanguage Language
        {
            get
            {
                return m_Language;
            }
        }
        // 语言数据字典
        private Dictionary<string, string> m_LanguageCfgTabel;
        // 注册的回调
        private Action m_Callback;
        
        // 初始化
        public void Init(string configPath)
        {
            m_LanguageCfgTabel = new Dictionary<string, string>();
        }

        public string GetLanguage(int langID)
        {
            //switch (Language)
            //{
            //    case MPLanguage.Chinese :
            //    return ConfigManager.Inst.GetlanguageConfigById(langID).chinese;
            //    case MPLanguage.English:
            //    return ConfigManager.Inst.GetlanguageConfigById(langID).english;
            //}
            Debug.LogError("当前语言类型错误");
            return null;
         }

        // 设置语言
        public void SetLanguage(MPLanguage language)
        {
            if (m_Language != language)
            {
                this.m_Language = language;
                if (m_Callback != null)
                {
                    m_Callback();
                }
            }
        }

        // 获取本地化数据
        public string GetLocalizationData(string key)
        {
            string data;
            if(!m_LanguageCfgTabel.TryGetValue(key,out data))
            {
                throw new System.Exception(string.Format("获取数据失败,key:{0}不存在！",key));
            }
            return data;
        }
        
        // 注册更改语言后的回调
        public void Register(Action callback)
        {
            m_Callback += callback;
        }
        
        // 取消注册回调
        public void Unregister(Action callback)
        {
            m_Callback -= callback;
        }
    
    }
}
