using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// 数学库
/// </summary>
namespace MPStudio
{
    /// <summary>
    /// 数学类
    /// </summary>
    public static class MPMath
    {
        /// <summary>
        /// 角度转弧度 直接乘
        /// </summary>
        public const float Angle_2_Radian = 0.017453292519f;

        /// <summary>
        /// 弧度转角度 直接乘
        /// </summary>
        public const float Radian_2_Angle = 57.295779513082f;

        /// <summary>
        /// 得到一个概率是否命中
        /// </summary>
        /// <param name="Ratio">概率</param>
        /// <returns></returns>
        public static bool CanRatioBingo(int Ratio, EPrecentType precentType = EPrecentType.PRECENT_10000)
        {
            return Random.Range(1, (int)precentType) <= Ratio;
        }

        /// <summary>
        /// 判断2个浮点数是否相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Equal(float a, float b)
        {
            return (a - b > -0.000001f) && (a - b) < 0.000001f;
        }

        /// <summary>
        /// 判断2个浮点数是否相等
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Equal(double a, double b)
        {
            return (a - b > -0.000001d) && (a - b) < 0.000001d;
        }

        /// <summary>
        /// 让一个角度标准化  归入 [0,360) 度之间
        /// </summary>
        /// <param name="Angle">角度</param>
        public static float MakeAngleNormalize(float Angle)
        {
            while (Angle >= 360)
            {
                Angle -= 360;
            }

            while (Angle < 0)
            {
                Angle += 360;
            }

            return Angle;
        }

        /// <summary>
        /// 得到一个二维向量的角度
        /// 右边为0度
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float GetVector2Angle(Vector2 target)
        {
            return MakeAngleNormalize(Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg);
        }

        /// <summary>
        /// 截断
        /// </summary>
        /// <param name="v"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Clamp(int v, int min, int max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        /// <summary>
        /// 截断
        /// </summary>
        /// <param name="v"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Clamp(float v, float min, float max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        /// <summary>
        /// 截断
        /// </summary>
        /// <param name="v"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double Clamp(double v, double min, double max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        /// <summary>
        /// 包装Unity的随机数
        /// </summary>
        /// <param name="max">最大数</param>
        /// <param name="min">最小数,默认为0</param>
        /// <returns>随机数</returns>
        public static int Rand(int max, int min = 0)
        {
            if (max < min)
            {
                return Random.Range(max, min);
            }

            return Random.Range(min, max);
        }

        /// <summary>
        /// 返回0-1之间的一个随机数
        /// </summary>
        /// <returns>随机数</returns>
        public static float Rand()
        {
            return Random.value;
        }

        /// <summary>
        /// 过屏幕射线与指定高度的水平面的焦点
        /// 本方法采用纯数学计算
        /// 无需依赖物理
        /// 性能拔群
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetPlaneInteractivePoint(Vector3 screenPos, float plane = 0)
        {
            var ray = Camera.main.ScreenPointToRay(screenPos);
            Vector3 dir = ray.direction;

            if (dir.y.Equals(0)) return Vector3.zero;
            float num = (plane - ray.origin.y) / dir.y;
            return ray.origin + ray.direction * num;
        }

        /// <summary>
        /// 在一个绑定盒里随机一个坐标
        /// </summary>
        /// <param name="bound"></param>
        /// <returns></returns>
        public static Vector3 RandInBounds(Bounds bound)
        {
            var offset = new Vector3(Random.Range(-bound.size.x / 2, bound.size.x / 2),
                Random.Range(-bound.size.y / 2, bound.size.y / 2), Random.Range(-bound.size.z / 2, bound.size.z / 2));
            return bound.center + offset;
        }

        /// <summary>
        /// 包装Unity的随机数
        /// </summary>
        /// <param name="max">最大数</param>
        /// <param name="min">最小数,默认为0</param>
        /// <returns></returns>
        public static float Rand(float max, float min = 0)
        {
            if (max < min)
            {
                return Random.Range(max, min);
            }

            return Random.Range(min, max);
        }

        /// <summary>
        /// 把一个数字从当前的 min,max区间缩放到 newMin , newMax区间
        /// 如 区间 0-100 数字50
        /// 缩放区间倒 20-40 则返回数字30
        /// </summary>
        /// <param name="Num">要处理的数字</param>
        /// <param name="min">原缩放区间左值</param>
        /// <param name="max">原缩放区间右值</param>
        /// <param name="newMin">新区间左值</param>
        /// <param name="newMax">新区间右值</param>
        /// <returns></returns>
        public static float ReMap(float num, float min, float max, float newMin, float newMax)
        {
            if (num <= min)
            {
                return newMin;
            }

            if (num >= max)
            {
                return newMax;
            }

            return (num - min) / (max - min) * (newMax - newMin) + newMin;
        }

        /// <summary>
        /// 把一个数字从当前的 min,max区间缩放到 newMin , newMax区间
        /// 如 区间 0-100 数字50
        /// 缩放区间倒 20-40 则返回数字30
        /// </summary>
        /// <param name="Num">要处理的数字</param>
        /// <param name="min">原缩放区间左值</param>
        /// <param name="max">原缩放区间右值</param>
        /// <param name="newMin">新区间左值</param>
        /// <param name="newMax">新区间右值</param>
        /// <returns></returns>
        public static int ReMap(int num, int min, int max, int newMin, int newMax)
        {
            if (num <= min)
            {
                return newMin;
            }

            if (num >= max)
            {
                return newMax;
            }

            return (num - min) / (max - min) * (newMax - newMin) + newMin;
        }

        /// <summary>
        /// 随机打乱一个数组
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="array">泛型数组引用</param>
        public static void Shuffle<T>(T[] array)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                int TargetPos = Rand(array.Length);
                T temp = array[i];
                array[i] = array[TargetPos];
                array[TargetPos] = temp;
            }
        }

        /// <summary>
        /// 随机打乱一个List
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="List">泛型List引用</param>
        public static void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                int TargetPos = Rand(list.Count);
                T temp = list[i];
                list[i] = list[TargetPos];
                list[TargetPos] = temp;
            }
        }

        /// <summary>
        /// 随机打乱一个List
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="count">打乱基数，默认为1，完全打乱，值越大，打乱次数越小，性能越好，最小打乱1次</param>
        /// <param name="List">泛型List引用</param>
        public static void Shuffle<T>(List<T> list, int count = 1)
        {
            var shuffleCount = list.Count / count;
            shuffleCount = Mathf.Max(1, shuffleCount);
            for (int i = 0; i < shuffleCount; ++i)
            {
                int TargetPos = Rand(list.Count);
                T temp = list[i];
                list[i] = list[TargetPos];
                list[TargetPos] = temp;
            }
        }

        /// <summary>
        /// 获得2条线段交点
        /// </summary>
        /// <param name="a">第一条线段起点</param>
        /// <param name="b">第一条线段终点</param>
        /// <param name="c">第二条线段起点</param>
        /// <param name="d">第二条线段终点</param>
        /// <param name="intersect">交点</param>
        /// <returns></returns>
        public static bool GetSegmentIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d, out Vector2 intersect)
        {
            intersect = Vector2.zero;

            // 线段ab的法线N1
            var nx1 = (b.y - a.y);
            var ny1 = (a.x - b.x);

            // 线段cd的法线N2
            var nx2 = (d.y - c.y);
            var ny2 = (c.x - d.x);

            // 两条法线做叉乘, 如果结果为0, 说明线段ab和线段cd平行或共线,不相交
            var denominator = nx1 * ny2 - ny1 * nx2;
            if (denominator == 0)
            {
                return false;
            }

            // 在法线N2上的投影
            var distC_N2 = nx2 * c.x + ny2 * c.y;
            var distA_N2 = nx2 * a.x + ny2 * a.y - distC_N2;
            var distB_N2 = nx2 * b.x + ny2 * b.y - distC_N2;

            // 点a投影和点b投影在点c投影同侧 (对点在线段上的情况,本例当作不相交处理);
            if (distA_N2 * distB_N2 >= 0)
            {
                return false;
            }

            // 判断点c点d 和线段ab的关系, 原理同上
            // 在法线N1上的投影
            var distA_N1 = nx1 * a.x + ny1 * a.y;
            var distC_N1 = nx1 * c.x + ny1 * c.y - distA_N1;
            var distD_N1 = nx1 * d.x + ny1 * d.y - distA_N1;
            if (distC_N1 * distD_N1 >= 0)
            {
                return false;
            }

            // 计算交点坐标
            var fraction = distA_N2 / denominator;
            var dx = fraction * ny1;
            var dy = -fraction * nx1;
            intersect = new Vector2(a.x + dx, a.y + dy);
            return true;
        }
    }
}