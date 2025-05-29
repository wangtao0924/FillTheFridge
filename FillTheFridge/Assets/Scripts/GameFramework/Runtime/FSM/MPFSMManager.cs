using System;
using UnityEngine;
using System.Collections.Generic;

namespace MPStudio
{
    public class MPFSMManager: MonoBehaviour
    {
        private static MPFSMManager instance;
        //状态机列表
        private List<MPFSM> fsms;

        public static MPFSMManager Inst
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("[MPStudio.MPFSMManager]").AddComponent<MPFSMManager>();
                    instance.fsms = new List<MPFSM>();
                    DontDestroyOnLoad(instance);
                }
                return instance;
            }
        }

        public void Init() 
        {
            this.fsms = new List<MPFSM>();
        }
 
        #region NonPublic Methods
        private void Update()
        {
            for (int i = 0; i < fsms.Count; i++)
            {
                //更新状态机
                fsms[i].OnUpdate();
            }
        }

        private void OnDestroy() {
            instance = null;
        }
        #endregion
 
        #region Public Methods
        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <typeparam name="T">状态机类型</typeparam>
        /// <param name="fsms">状态机名称</param>
        /// <returns>状态机</returns>
        public T Create<T>(string fsmName = null) where T : MPFSM, new()
        {
            Type type = typeof(T);
            fsmName = string.IsNullOrEmpty(fsmName) ? type.Name : fsmName;
            if (fsms.Find(m => m.fsmName == fsmName) == null)
            {
                T fsm = (T)Activator.CreateInstance(type);
                fsm.fsmName = fsmName;
                fsms.Add(fsm);
                return fsm;
            }
            return default;
        }
        /// <summary>
        /// 销毁状态机
        /// </summary>
        /// <param name="fsmName">状态机名称</param>
        /// <returns>销毁成功返回true 否则返回false</returns>
        public bool Destroy(string fsmName)
        {
            var targetFsm = fsms.Find(m => m.fsmName == fsmName);
            if (targetFsm != null)
            {
                targetFsm.OnDestroy();
                fsms.Remove(targetFsm);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取状态机
        /// </summary>
        /// <typeparam name="T">状态机类型</typeparam>
        /// <param name="fsmName">状态机名称</param>
        /// <returns>状态机</returns>
        public T GetFSM<T>(string fsmName) where T : MPFSM
        {
            return (T)fsms.Find(fsm => fsm.fsmName == fsmName);
        }
        #endregion
    }
}