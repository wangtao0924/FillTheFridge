using System;

namespace MPStudio
{
    public interface MPIState
    {
        /// <summary>
        /// 状态名称
        /// </summary>
        /// <value></value>
        string stateName {get; set;}

        /// <summary>
        /// 状态初始化
        /// </summary>
        void OnInitialization();
        /// <summary>
        /// 状态进入事件
        /// </summary>
        void OnEnter();
        
        /// <summary>
        /// 状态停留事件（Update）
        /// </summary>
        void OnStay();
        
        /// <summary>
        /// 状态退出事件
        /// </summary>
        void OnExit();

        /// <summary>
        /// 状态终止事件
        /// </summary>
        void OnTermination();
    }
}
