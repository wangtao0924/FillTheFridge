using System;

using UnityEngine;

namespace MPStudio
{
    /// <summary>
    /// 简单任务
    /// </summary>

    public class MPTask
    {
        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool HasFinish { get; private set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// 任务名
        /// </summary>
        public string Name => ToString();

        /// <summary>
        /// 完成任务
        /// 任务调度器会在Update为true时自动调用本函数结束任务
        /// </summary>
        public virtual void Finish()
        {
            IsRunning = false;
            HasFinish = true;
            OnFinish();
            Debug.Log($"Task:{Name} finished!");
        }

        /// <summary>
        /// 结束时
        /// 子类需要重写来实现自己的任务
        /// </summary>
        public virtual void OnFinish() { }

        /// <summary>
        /// 开始时
        /// 子类需要重写来实现自己的任务
        /// </summary>
        public virtual void OnStart() { }

        /// <summary>
        /// 开始任务
        /// </summary>
        public virtual void Run(bool inSystem = true)
        {
            MPLOG.I($"Task:{Name} Start");
            if (inSystem)
                MPTaskSystem.Inst.AddTask(this);

            IsRunning = true;
            HasFinish = false;
            OnStart();
        }

        /// <summary>
        /// 更新状态
        /// 返回true 代表该任务执行完毕
        /// 自动进入 OnFinish
        /// </summary>
        public virtual bool Update() { return false; }
    }
}