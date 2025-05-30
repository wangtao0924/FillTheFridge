using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Runtime.InteropServices;

namespace MPStudio
{
    /// <summary>
    /// 任务序列
    /// </summary>
    public class MpTaskQueue : MPTask
    {
        /// <summary>
        /// 任务队列
        /// </summary>
        private Queue<MPTask> m_MissionQueue = new Queue<MPTask>();

        /// <summary>
        /// 当前处理的任务
        /// </summary>
        private MPTask m_NowExeMission = null;

        /// <summary>
        /// 增加一个简单任务到队列
        /// </summary>
        /// <param name=""></param>
        public void Append(MPTask task)
        {
            var heap = MpSubTaskHeap.Create(task);
            m_MissionQueue.Enqueue(heap);
        }

        /// <summary>
        /// 增加一个函数调用
        /// </summary>
        public void AppendCallFunc(Action Callfunc)
        {
            var callTask = MpSubTaskCallFunc.Create(Callfunc);
            m_MissionQueue.Enqueue(callTask);
        }

        /// <summary>
        /// 增加一个延时
        /// </summary>
        /// <param name="WaitTime"></param>
        public void AppendInteval(float WaitTime)
        {
            var waitTask = MpSubTaskInteval.Create(WaitTime);
            m_MissionQueue.Enqueue(waitTask);
        }

        /// <summary>
        /// 添加一个同步任务到队列
        /// </summary>
        /// <param name="Mission"></param>
        public void Join(MPTask Mission)
        {
            if (m_MissionQueue.Count == 0)
            {
                Debug.LogWarning($"TaskQueue:{Name} has no taskheap to join!!!");
                return;
            }

            var last = m_MissionQueue.Last();
            if (last is MpSubTaskHeap)
            {
                (last as MpSubTaskHeap).Join(Mission);
            }
            else
            {
                Debug.LogWarning($"TaskQueue:{Name} can not join an task to the last queue where is not a taskheap");
            }
        }

        /// <summary>
        /// 执行下一个任务
        /// </summary>
        public override void OnStart()
        {
            ExecuteNextMission();
        }

        /// <summary>
        /// 每帧刷新
        /// 如果本任务序列被执行完毕了，则返回true
        /// 未执行完毕，则返回false
        /// </summary>
        public override bool Update()
        {
            if (m_NowExeMission != null)
            {
                // 当前任务运行完毕
                if (m_NowExeMission.IsRunning && m_NowExeMission.Update())
                {
                    // 执行完成接口
                    m_NowExeMission.Finish();
                    // 检查是否需要下一个任务
                    if (m_MissionQueue.Count == 0)
                    {
                        return true;
                    }
                    else
                    {
                        ExecuteNextMission();
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 执行下一个任务
        /// </summary>
        private void ExecuteNextMission()
        {
            m_NowExeMission = m_MissionQueue.Dequeue();

#if SHOW_TASK_LOG
            MPLOG.I( "task",$"TaskQueue:{Name} Ready To Start Next Task:{m_NowExeMission.Name}");
#endif
            m_NowExeMission.Run(false);
        }
    }

    #region 子任务 Action

    /// <summary>
    /// Action 子任务
    /// </summary>
    internal class MpSubTaskCallFunc : MPTask
    {
        public Action CallBack { get; set; }

        private MpSubTaskCallFunc()
        {
        }

        /// <summary>
        /// 创建一个Action任务
        /// </summary>
        /// <param name="CallFunc"></param>
        /// <returns></returns>
        public static MpSubTaskCallFunc Create(Action CallFunc)
        {
            return new MpSubTaskCallFunc() { CallBack = CallFunc };
        }

        /// <summary>
        /// 结束时
        /// </summary>
        public override void OnFinish()
        {
            CallBack?.Invoke();
        }

        public override bool Update()
        {
            return true;
        }
    }

    #endregion 子任务 Action

    #region 子任务堆

    /// <summary>
    /// 子任务堆
    /// 一堆可一起执行的任务
    /// </summary>
    internal class MpSubTaskHeap : MPTask
    {
        /// <summary>
        /// 同步执行的任务堆
        /// </summary>
        private List<MPTask> Tasks = new List<MPTask>();

        private MpSubTaskHeap()
        {
        }

        /// <summary>
        /// 创建子任务堆
        /// </summary>
        /// <param name="firstTask"></param>
        /// <returns></returns>
        public static MpSubTaskHeap Create(MPTask firstTask = null)
        {
            var heap = new MpSubTaskHeap();
            heap.Join(firstTask);
            return heap;
        }

        /// <summary>
        /// 添加一个同步任务到队列
        /// </summary>
        /// <param name="Mission"></param>
        public void Join(MPTask task)
        {
            if (task != null)
                Tasks.Add(task);
        }

        /// <summary>
        /// 运行任务堆
        /// </summary>
        /// <param name="inSystem"></param>
        public override void Run(bool inSystem = false)
        {
            base.Run(inSystem);

            for (int i = 0; i < Tasks.Count; i++)
            {
                Tasks[i].Run(false);
            }
        }

        /// <summary>
        /// 检查任务堆是否完成
        /// </summary>
        public override bool Update()
        {
            for (int i = 0; i < Tasks.Count; i++)
            {
                if (Tasks[i].IsRunning && Tasks[i].Update())
                {
                    Tasks[i].Finish();
                }
            }

            for (int i = 0; i < Tasks.Count; i++)
            {
                if (!Tasks[i].HasFinish)
                {
                    return false;
                }
            }

            return true;
        }
    }

    #endregion 子任务堆

    #region 子任务 延时

    /// <summary>
    /// 延时子任务
    /// </summary>
    internal class MpSubTaskInteval : MPTask
    {
        /// <summary>
        /// 延时
        /// </summary>
        private float m_DelayTime = 0f;

        /// <summary>
        /// 当前经过时间
        /// </summary>
        private float m_NowTime = 0f;

        private MpSubTaskInteval()
        {
        }

        /// <summary>
        /// 创建延时子任务
        /// </summary>
        /// <param name="DelayTime"></param>
        /// <returns></returns>
        public static MpSubTaskInteval Create(float DelayTime)
        {
            return new MpSubTaskInteval() { m_DelayTime = DelayTime };
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        public override bool Update()
        {
            m_NowTime += Time.deltaTime;
            return m_NowTime >= m_DelayTime;
        }
    }

    #endregion 子任务 延时
}