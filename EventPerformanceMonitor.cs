using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LikeItemFind
{
    // 测试在地图上绘制标记物品的性能
    public class EventPerformanceMonitor
    {
        private static Dictionary<string, float> eventTimings;
        private static Dictionary<string, int> eventCounts;

        static EventPerformanceMonitor()
        {
            eventTimings = new Dictionary<string, float>();
            eventCounts = new Dictionary<string, int>();
        }

        public static void StartTiming(string eventName)
        {
            eventTimings[eventName] = Time.realtimeSinceStartup;
        }

        public static void EndTiming(string eventName)
        {
            if (eventTimings.ContainsKey(eventName))
            {
                float duration = Time.realtimeSinceStartup - eventTimings[eventName];
                Debug.Log($"事件 {eventName} 耗时: {duration * 1000:F2}ms");

                if (eventCounts.ContainsKey(eventName))
                {
                    eventCounts[eventName]++;
                }
                else
                {
                    eventCounts[eventName] = 1;
                }
            }
        }

        public static void ShowPerformanceReport()
        {
            Debug.Log("=== 事件性能报告 ===");
            foreach (var kvp in eventCounts)
            {
                Debug.Log($"事件 {kvp.Key}: 触发 {kvp.Value} 次");
            }
        }
    }
}
