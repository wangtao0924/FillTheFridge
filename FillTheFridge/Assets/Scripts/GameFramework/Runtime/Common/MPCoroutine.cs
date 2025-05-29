using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace MPStudio
{
    /// <summary>
    /// 协程管理类
    /// 赋予非 MonoBehaviour 以调用协程的能力
    /// </summary>
    public class MPCoroutine : MPSingletonMono<MPCoroutine>
    {
        /// <summary>
        /// 中止一个协程
        /// </summary>
        /// <param name="routine"></param>
        public static void BreakCoroutine(Coroutine Cor)
        {
            Inst.StopCoroutine(Cor);
        }

        /// <summary>
        /// 中止一个协程
        /// 连同里面的子协程一起中止
        /// </summary>
        /// <param name="ie"></param>
        public static void BreakIEnumeratorNested(IEnumerator ie)
        {
            var cur = ie;
            MPLOG.I("IEnumerator {0} will Stop", cur.ToString());
            Inst.StopCoroutine(cur);

            while (cur.Current is IEnumerator)
            {
                cur = cur.Current as IEnumerator;
                MPLOG.I("Inner IEnumerator {0} will Stop", cur.ToString());
                Inst.StopCoroutine(cur);
            }
        }

        /// <summary>
        /// 延时一定秒数做什么事
        /// </summary>
        /// <param name="time"></param>
        /// <param name="ac"></param>
        public static async void DelayToDo(float time, Action ac)
        {
            if (time == 0f)
            {
                ac?.Invoke();
                return;
            }

            new WaitForSeconds(time); // await
            ac?.Invoke();
        }

        /// <summary>
        /// 延时一定帧数做什么事
        /// </summary>
        /// <param name="time"></param>
        /// <param name="ac"></param>
        //public static async void DelayToDo(int frame, Action ac)
        //{
        //    for (int i = 0; i < frame; i++)
        //    {
        //        new WaitForUpdate(); // await
        //    }

        //    ac?.Invoke();
        //}

        /// <summary>
        /// 启动一个协程
        /// </summary>
        /// <param name="routine"></param>
        public static Coroutine RunCoroutine(IEnumerator routine)
        {
            return Inst.StartCoroutine(routine);
        }
    }
}