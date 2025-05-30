using System;
using UnityEngine;
using System.Collections.Generic;

namespace MPStudio
{
    public class MPFSM 
    {
        //状态列表 存储状态机内所有状态
        protected readonly List<MPIState> states = new List<MPIState>();
        /// <summary>
        /// 状态机名称
        /// </summary>
        public string fsmName { get; set; }
        /// <summary>
        /// 当前状态
        /// </summary>
        public MPIState currentState { get; protected set; }

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>添加成功返回true 否则返回false</returns>
        public bool Add(MPState state)
        {
            //判断是否已经存在
            if (!states.Contains(state))
            {
                //判断是否存在同名状态
                if (states.Find(m => m.stateName == state.stateName) == null)
                {
                    //存储到列表
                    states.Add(state);
                    //执行状态初始化事件
                    state.OnInitialization();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 添加状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="stateName">状态命名</param>
        /// <returns>添加成功返回true 否则返回false</returns>
        public bool Add<T>(string stateName = null) where T : MPState, new()
        {
            Type type = typeof(T);
            T t = (T)Activator.CreateInstance(type);
            t.stateName = string.IsNullOrEmpty(stateName) ? type.Name : stateName;
            return Add(t);
        }

        /// <summary>
        /// 移除状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>移除成功返回true 否则返回false</returns>
        public bool Remove(MPIState state)
        {
            //判断是否存在
            if (states.Contains(state))
            {
                //如果要移除的状态为当前状态 首先执行当前状态退出事件
                if (currentState == state)
                {
                    currentState.OnExit();
                    currentState = null;
                }
                //执行状态终止事件
                state.OnTermination();
                return states.Remove(state);
            }
            return false;
        }

        /// <summary>
        /// 移除状态
        /// </summary>
        /// <param name="stateName">状态名称</param>
        /// <returns>移除成功返回true 否则返回false</returns>
        public bool Remove(string stateName)
        {
            var targetIndex = states.FindIndex(m => m.stateName == stateName);
            if (targetIndex != -1)
            {
                var targetState = states[targetIndex];
                if (currentState == targetState)
                {
                    currentState.OnExit();
                    currentState = null;
                }
                targetState.OnTermination();
                return states.Remove(targetState);
            }
            return false;
        }

        /// <summary>
        /// 移除状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <returns>移除成返回true 否则返回false</returns>
        public bool Remove<T>() where T : MPIState
        {
            return Remove(typeof(T).Name);
        }
        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>切换成功返回true 否则返回false</returns>
        public bool Switch(MPIState state)
        {
            //如果当前状态已经是切换的目标状态 无需切换 返回false
            if (currentState == state) return false;
            //当前状态不为空则执行状态退出事件
            currentState?.OnExit();
            //判断切换的目标状态是否存在于列表中
            if (!states.Contains(state)) return false;
            //更新当前状态
            currentState = state;
            //更新后 当前状态不为空则执行状态进入事件
            currentState?.OnEnter();
            return true;
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="stateName">状态名称</param>
        /// <returns>切换成功返回true 否则返回false</returns>
        public bool Switch(string stateName)
        {
            //根据状态名称在列表中查询
            var targetState = states.Find(m => m.stateName == stateName);
            return Switch(targetState);
        }
        /// <summary>
        /// 切换状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <returns>切换成返回true 否则返回false</returns>
        public bool Switch<T>() where T : MPIState
        {
            return Switch(typeof(T).Name);
        }

        /// <summary>
        /// 切换至下一状态
        /// </summary>
        public void SwitchNextState()
        {
            if (states.Count != 0)
            {
                //如果当前状态不为空 则根据当前状态找到下一个状态
                if (currentState != null)
                {
                    int index = states.IndexOf(currentState);
                    //当前状态的索引值+1后若小于列表中的数量 则下一状态的索引为index+1
                    //否则表示当前状态已经是列表中的最后一个 下一状态则回到列表中的第一个状态 索引为0
                    index = index + 1 < states.Count ? index + 1 : 0;
                    MPIState targetState = states[index];
                    //首先执行当前状态的退出事件 再更新到下一状态
                    currentState.OnExit();
                    currentState = targetState;
                }
                //当前状态为空 则直接进入列表中的第一个状态
                else
                {
                    currentState = states[0];
                }
                //执行状态进入事件
                currentState.OnEnter();
            }
        }

        /// <summary>
        /// 切换至上一状态
        /// </summary>
        public void SwitchLastState()
        {
            if (states.Count != 0)
            {
                //如果当前状态不为空 则根据当前状态找到上一个状态
                if (currentState != null)
                {
                    int index = states.IndexOf(currentState);
                    //当前状态的索引值-1后若大等于0 则下一状态的索引为index-1
                    //否则表示当前状态是列表中的第一个 上一状态则回到列表中的最后一个状态
                    index = index - 1 >= 0 ? index - 1 : states.Count - 1;
                    MPIState targetState = states[index];
                    //首先执行当前状态的退出事件 再更新到上一状态
                    currentState.OnExit();
                    currentState = targetState;
                }
                //当前状态为空 则直接进入列表中的最后一个状态
                else
                {
                    currentState = states[states.Count - 1];
                }
                //执行状态进入事件
                currentState.OnEnter();
            }
        }

        /// <summary>
        /// 切换至空状态（退出当前状态）
        /// </summary>
        public void SwitchNullState()
        {
            if (currentState != null)
            {
                currentState.OnExit();
                currentState = null;
            }
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="stateName">状态名称</param>
        /// <returns>状态</returns>
        public T GetState<T>(string stateName) where T : MPIState
        {
            return (T)states.Find(m => m.stateName == stateName);
        }
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <returns>状态</returns>
        public T GetState<T>() where T : MPIState
        {
            return (T)states.Find(m => m.stateName == typeof(T).Name);
        }

        /// <summary>
        /// 状态机刷新事件
        /// </summary>
        public void OnUpdate()
        {
            //若当前状态不为空 执行状态停留事件
            if (currentState != null)
            {
                currentState.OnStay();
            }
        }
        /// <summary>
        /// 状态机销毁事件
        /// </summary>
        public void OnDestroy()
        {
            //执行状态机内所有状态的状态终止事件
            for (int i = 0; i < states.Count; i++)
            {
                states[i].OnTermination();
            }
        }        
 
        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <param name="stateMachineName">状态机名称</param>
        /// <returns>状态机</returns>
        public static MPFSM Create(string fsmName = null)
        {
            return MPFSMManager.Inst.Create<MPFSM>(fsmName);
        }
        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <typeparam name="T">状态机类型</typeparam>
        /// <param name="stateMachineName">状态机名称</param>
        /// <returns>状态机</returns>
        public static T Create<T>(string fsmName = null) where T : MPFSM, new()
        {
            return MPFSMManager.Inst.Create<T>(fsmName);
        }
        /// <summary>
        /// 销毁状态机
        /// </summary>
        /// <param name="stateMachineName">状态机名称</param>
        /// <returns>销毁成功返回true 否则返回false</returns>
        public static bool Destroy(string fsmName)
        {
            return MPFSMManager.Inst.Destroy(fsmName);
        }
        /// <summary>
        /// 销毁状态机
        /// </summary>
        /// <typeparam name="T">状态机类型</typeparam>
        /// <returns>销毁成功返回true 否则返回false</returns>
        public static bool Destroy<T>() where T : MPFSM
        {
            return MPFSMManager.Inst.Destroy(typeof(T).Name);
        }
        /// <summary>
        /// 获取状态机
        /// </summary>
        /// <param name="fsmName">状态机名称</param>
        /// <returns>状态机</returns>
        public MPFSM Get(string fsmName)
        {
            return MPFSMManager.Inst.GetFSM<MPFSM>(fsmName);
        }
        /// <summary>
        /// 获取状态机
        /// </summary>
        /// <typeparam name="T">状态机类型</typeparam>
        /// <param name="fsmName">状态机名称</param>
        /// <returns>状态机</returns>
        public static T Get<T>(string fsmName) where T : MPFSM
        {
            return MPFSMManager.Inst.GetFSM<T>(fsmName);
        }
        /// <summary>
        /// 获取状态机
        /// </summary>
        /// <typeparam name="T">状态机类型</typeparam>
        /// <returns>状态机</returns>
        public static T Get<T>() where T : MPFSM
        {
            return MPFSMManager.Inst.GetFSM<T>(typeof(T).Name);
        }
    }
}