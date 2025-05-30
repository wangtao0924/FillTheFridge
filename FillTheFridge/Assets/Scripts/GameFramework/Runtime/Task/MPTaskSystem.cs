using System.Collections.Generic;
using UnityEngine;

namespace MPStudio
{
    /// <summary>
    /// 任务系统
    /// </summary>
    public class MPTaskSystem : MPSingletonMono<MPTaskSystem>
    {
        /// <summary>
        /// 当前任务数
        /// </summary>
        [SerializeField]
        private int m_TaskCount = 0;

        /// <summary>
        /// 任务清单
        /// </summary>
        private List<MPTask> m_Tasks = new List<MPTask>();

        /// <summary>
        /// 当前任务数
        /// </summary>
        public int TaskCount => m_Tasks == null ? 0 : m_Tasks.Count;

        /// <summary>
        /// 创建任务队列
        /// </summary>
        /// <returns></returns>
        public void AddTask(MPTask target)
        {
            m_Tasks.Add(target);
            m_TaskCount++;
        }

        /// <summary>
        /// 每帧执行
        /// </summary>
        private void Update()
        {
            for (int i = 0; i < m_Tasks.Count; i++)
            {
                var task = m_Tasks[i];

                if (task.HasFinish)
                {
                    m_Tasks.RemoveAt(i);
                    m_TaskCount--;
                    i--;
                }
                else if (task.IsRunning && task.Update())
                {
                    task.Finish();
                    m_Tasks.RemoveAt(i);
                    m_TaskCount--;
                    i--;
                }
            }
        }
    }
}