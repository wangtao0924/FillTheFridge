using System;
using UnityEngine;
using System.Collections.Generic;

namespace MPStudio
{
    public class MPProcedureHelper
    {
        // 流程列表
        protected readonly List<MPProcedure> procedures = new List<MPProcedure>();
        /// <summary>
        /// 当前流程
        /// </summary>
        public MPProcedure currentProcedure { get; protected set; }

        /// <summary>
        /// 初始化流程方法
        /// </summary>
        /// <param name="procedures"></param>
        public void InitProcedures(List<MPProcedure> procedures)
        {
            procedures.AddRange(procedures);
        }

        /// <summary>
        /// 添加流程
        /// </summary>
        /// <param name="procedure">流程</param>
        /// <returns>添加成功返回true 否则返回false</returns>
        public bool Add(MPProcedure procedure)
        {
            //判断是否已经存在
            if (!procedures.Contains(procedure))
            {
                //判断是否存在同名流程
                if (procedures.Find(m => m.procedureName == procedure.procedureName) == null)
                {
                    //存储到列表
                    procedures.Add(procedure);
                    //执行流程初始化事件
                    procedure.OnInit();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 添加流程
        /// </summary>
        /// <typeparam name="T">流程类型</typeparam>
        /// <param name="stateName">流程命名</param>
        /// <returns>添加成功返回true 否则返回false</returns>
        public bool Add<T>(string procedureName = null) where T : MPProcedure, new()
        {
            Type type = typeof(T);
            T t = (T)Activator.CreateInstance(type);
            t.procedureName = string.IsNullOrEmpty(procedureName) ? type.Name : procedureName;
            return Add(t);
        }

        /// <summary>
        /// 移除流程
        /// </summary>
        /// <param name="procedure">流程</param>
        /// <returns>移除成功返回true 否则返回false</returns>
        public bool Remove(MPProcedure procedure)
        {
            //判断是否存在
            if (procedures.Contains(procedure))
            {
                //如果要移除的流程为当前流程 首先执行当前流程退出事件
                if (currentProcedure == procedure)
                {
                    int index = procedures.IndexOf(procedure);
                    MPProcedure nextProcedure = null;
                    if (index < procedures.Count) 
                    {
                        nextProcedure = procedures[index];
                    }
                    currentProcedure.OnLeave(nextProcedure);
                    currentProcedure = null;
                }
                //执行流程终止事件
                procedure.OnDestroy();
                return procedures.Remove(procedure);
            }
            return false;
        }

        /// <summary>
        /// 移除流程
        /// </summary>
        /// <param name="procedureName">流程名称</param>
        /// <returns>移除成功返回true 否则返回false</returns>
        public bool Remove(string procedureName)
        {
            var targetIndex = procedures.FindIndex(m => m.procedureName == procedureName);
            if (targetIndex != -1)
            {
                var targetProcedure = procedures[targetIndex];
                if (currentProcedure == targetProcedure)
                {
                    MPProcedure nextProcedure = null;
                    if (targetIndex < procedures.Count) 
                    {
                        nextProcedure = procedures[targetIndex];
                    }
                    currentProcedure.OnLeave(nextProcedure);
                    currentProcedure = null;
                }
                targetProcedure.OnDestroy();
                return procedures.Remove(targetProcedure);
            }
            return false;
        }

        /// <summary>
        /// 移除流程
        /// </summary>
        /// <typeparam name="T">流程类型</typeparam>
        /// <returns>移除成返回true 否则返回false</returns>
        public bool Remove<T>() where T : MPProcedure
        {
            return Remove(typeof(T).Name);
        }

        /// <summary>
        /// 切换流程
        /// </summary>
        /// <param name="state">流程</param>
        /// <returns>切换成功返回true 否则返回false</returns>
        public bool Switch(MPProcedure procedure)
        {
            //如果当前流程已经是切换的目标流程 无需切换 返回false
            if (currentProcedure == procedure) return false;
            //当前流程不为空则执行流程退出事件
            currentProcedure?.OnLeave(null);
            //判断切换的目标流程是否存在于列表中
            if (!procedures.Contains(procedure)) return false;
            // 更新当前流程
            currentProcedure = procedure;
            //更新后 当前流程不为空则执行流程进入事件
            currentProcedure?.OnEnter(null);
            return true;
        }

        /// <summary>
        /// 切换流程
        /// </summary>
        /// <param name="stateName">流程名称</param>
        /// <returns>切换成功返回true 否则返回false</returns>
        public bool Switch(string procedureName)
        {
            //根据流程名称在列表中查询
            var targetProcedures = procedures.Find(m => m.procedureName == procedureName);
            return Switch(targetProcedures);
        }
        /// <summary>
        /// 切换流程
        /// </summary>
        /// <typeparam name="T">流程类型</typeparam>
        /// <returns>切换成返回true 否则返回false</returns>
        public bool Switch<T>() where T : MPProcedure
        {
            return Switch(typeof(T).Name);
        }

        /// <summary>
        /// 切换至下一流程
        /// </summary>
        public void SwitchNextProcedure()
        {
            if (procedures.Count != 0)
            {
                MPProcedure nextProcedure = null;
                // 如果当前流程不为空 则根据当前流程找到下一个流程
                if (currentProcedure != null)
                {
                    int index = procedures.IndexOf(currentProcedure);
                    //当前流程的索引值+1后若小于列表中的数量 则下一流程的索引为index+1
                    //否则表示当前流程已经是列表中的最后一个 下一流程则回到列表中的第一个流程 索引为0
                    index = index + 1 < procedures.Count ? index + 1 : 0;
                    nextProcedure = procedures[index];
                    //首先执行当前流程的退出事件 再更新到下一流程
                    currentProcedure.OnLeave(nextProcedure);
                }
                //当前流程为空 则直接进入列表中的第一个流程
                else
                {
                    nextProcedure = procedures[0];
                }
                //执行流程进入事件
                nextProcedure.OnEnter(currentProcedure);
                currentProcedure = nextProcedure;
            }
        }

        /// <summary>
        /// 切换至上一流程
        /// </summary>
        public void SwitchLastProcedure()
        {
            if (procedures.Count != 0)
            {
                MPProcedure lastProcedure = null;
                //如果当前流程不为空 则根据当前流程找到上一个流程
                if (currentProcedure != null)
                {
                    int index = procedures.IndexOf(currentProcedure);
                    //当前流程的索引值-1后若大等于0 则下一流程的索引为index-1
                    //否则表示当前流程是列表中的第一个 上一流程则回到列表中的最后一个流程
                    index = index - 1 >= 0 ? index - 1 : procedures.Count - 1;
                    lastProcedure = procedures[index];
                    //首先执行当前流程的退出事件 再更新到上一流程
                    currentProcedure.OnLeave(lastProcedure);
                }
                //当前流程为空 则直接进入列表中的最后一个流程
                else
                {
                    lastProcedure = procedures[procedures.Count - 1];
                }
                //执行流程进入事件
                lastProcedure.OnEnter(currentProcedure);
                currentProcedure = lastProcedure;
            }
        }

        /// <summary>
        /// 切换至空流程（退出当前流程）
        /// </summary>
        public void SwitchNullProcedure()
        {
            if (currentProcedure != null)
            {
                currentProcedure.OnLeave(null);
                currentProcedure = null;
            }
        }

        /// <summary>
        /// 获取流程
        /// </summary>
        /// <typeparam name="T">流程类型</typeparam>
        /// <param name="procedureName">流程名称</param>
        /// <returns>流程</returns>
        public T GetProcedure<T>(string procedureName) where T : MPProcedure
        {
            return (T)procedures.Find(m => m.procedureName == procedureName);
        }
        /// <summary>
        /// 获取流程
        /// </summary>
        /// <typeparam name="T">流程类型</typeparam>
        /// <returns>流程</returns>
        public T GetProcedure<T>() where T : MPProcedure
        {
            return (T)procedures.Find(m => m.procedureName == typeof(T).Name);
        }

        /// <summary>
        /// 刷新事件
        /// </summary>
        public void OnUpdate()
        {
            //若当前流程不为空 执行流程停留事件
            currentProcedure?.OnUpdate();
        }
        /// <summary>
        /// 销毁事件
        /// </summary>
        public void OnDestroy()
        {
            //所有流程终止事件
            for (int i = 0; i < procedures.Count; i++)
            {
                procedures[i].OnDestroy();
            }
            procedures.Clear();
            currentProcedure = null;
        }
    }
}