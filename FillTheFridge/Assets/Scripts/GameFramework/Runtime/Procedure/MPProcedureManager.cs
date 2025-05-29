using System;
using UnityEngine;
using System.Collections.Generic;

namespace MPStudio
{
    public class MPProcedureManager: MonoBehaviour
    {
        private static MPProcedureManager instance;
        //状态机列表
        private MPProcedureHelper procedureHelper;

        public static MPProcedureManager Inst
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("[MPStudio.MPProcedureManager]").AddComponent<MPProcedureManager>();
                    instance.procedureHelper = new MPProcedureHelper();
                    DontDestroyOnLoad(instance);
                }
                return instance;
            }
        }

        /// <summary>
        /// 当前流程
        /// </summary>
        public MPProcedure CurrentProcedure
        {
            get
            {
                return procedureHelper.currentProcedure;
            }
        }

        public void InitProcedures(List<MPProcedure> procedures) {
            procedureHelper.InitProcedures(procedures);
        }

        /// <summary>
        /// 获取流程
        /// </summary>
        /// <typeparam name="T">流程类</typeparam>
        /// <returns>流程对象</returns>
        public T GetProcedure<T>() where T : MPProcedure
        {
            return procedureHelper.GetProcedure<T>();
        }

        /// <summary>
        /// 获取流程
        /// </summary>
        /// <param name="type">流程类</param>
        /// <returns>流程对象</returns>
        public MPProcedure GetProcedure(string procedureName)
        {
            return procedureHelper.GetProcedure<MPProcedure>(procedureName);
        }
        /// <summary>
        /// 移除流程
        /// </summary>
        /// <param name="procedure"></param>
        /// <returns></returns>
        public bool RemoveProcedure(MPProcedure procedure)
        {
            return procedureHelper.Remove(procedure);
        }
        /// <summary>
        /// 移除流程
        /// </summary>
        /// <param name="procedureName">流程名称</param>
        /// <returns></returns>
        public bool RemoveProcedure(string procedureName)
        {
            return procedureHelper.Remove(procedureName);
        }
        /// <summary>
        /// 移除流程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool Remove<T>() where T : MPProcedure
        {
            return procedureHelper.Remove<T>();
        }
        /// <summary>
        /// 增加流程
        /// </summary>
        /// <param name="procedureName">流程名字</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool AddProcedure<T>(string procedureName) where T : MPProcedure, new()
        {
            return procedureHelper.Add<T>(procedureName);
        }
        /// <summary>
        /// 增加流程
        /// </summary>
        /// <param name="procedure"></param>
        /// <returns></returns>
        public bool AddProcedure(MPProcedure procedure)
        {
            return procedureHelper.Add(procedure);
        }
        /// <summary>
        /// 切换流程
        /// </summary>
        /// <param name="procedure"></param>
        public void SwitchProcedure(MPProcedure procedure)
        {
            procedureHelper.Switch(procedure);
        }
        /// <summary>
        /// 切换流程
        /// </summary>
        /// <param name="procedureName"></param>
        public void SwitchProcedure(string procedureName) 
        {
            procedureHelper.Switch(procedureName);
        }
        /// <summary>
        /// 切换至下一流程
        /// </summary>
        public void SwitchNextProcedure()
        {
            procedureHelper.SwitchNextProcedure();
        }
        /// <summary>
        /// 切换至上一流程
        /// </summary>
        public void SwitchLastProcedure()
        {
            procedureHelper.SwitchLastProcedure();
        }
        /// <summary>
        /// 切换至空流程(退出当前流程)
        /// </summary>
        public void SwitchNullProcedure()
        {
            procedureHelper.SwitchNullProcedure();
        }

        private void Update()
        {
            procedureHelper.OnUpdate();
        }

        private void OnDestroy() {
            procedureHelper.OnDestroy();
            procedureHelper = null;
            instance = null;
        }
    }
}