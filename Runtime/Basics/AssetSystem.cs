/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AIO.UEngine;
using UnityEngine;

namespace AIO
{
    /// <summary>
    /// 资源管理系统
    /// </summary>
    public static partial class AssetSystem
    {
        private static AssetProxy Proxy;

        /// <summary>
        /// 资源包配置
        /// </summary>
        public static ICollection<AssetsPackageConfig> PackageConfigs => Parameter.Packages;

        /// <summary>
        /// 资源热更新配置
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static ASConfig Parameter { get; private set; }

        /// <summary>
        /// 是否已经初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static bool IsInitialized { get; private set; }

        public struct SequenceRecord
        {
            /// <summary>
            /// 资源包名
            /// </summary>
            public string Name;

            /// <summary>
            /// 资源包寻址路径
            /// </summary>
            public string Location;

            /// <summary>
            /// 记录时间
            /// </summary>
            public DateTime Time;
            
            /// <summary>
            /// 记录大小
            /// </summary>
            public long Bytes;

            /// <summary>
            /// 记录数量
            /// </summary>
            public int Count;
        }

        /// <summary>
        /// 序列记录队列
        /// </summary>
        private static Queue<SequenceRecord> SequenceRecordQueue;

        /// <summary>
        /// 序列记录路径
        /// </summary>
        internal static readonly string SequenceRecordPath =
            Path.Combine(Application.persistentDataPath, "aio.asset.record.json");

        /// <summary>
        /// 系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize<T>(ASConfig config) where T : AssetProxy, new()
        {
            return Initialize(Activator.CreateInstance<T>(), config);
        }

        /// <summary>
        /// 系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize<T>(T proxy) where T : AssetProxy
        {
            return Initialize(proxy, ASConfig.GetOrCreate());
        }

        /// <summary>
        /// 系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize<T>() where T : AssetProxy, new()
        {
            return Initialize(Activator.CreateInstance<T>(), ASConfig.GetOrCreate());
        }

        /// <summary>
        /// 系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize<T>(T proxy, ASConfig config) where T : AssetProxy
        {
            IsInitialized = false;
            Parameter = config;
            Proxy = proxy;
            yield return Proxy.Initialize();
            IsInitialized = true;
#if !UNITY_WEBGL
            if (Parameter.AutoSequenceRecord)
                SequenceRecordQueue = File.Exists(SequenceRecordPath)
                    ? AHelper.IO.ReadJsonUTF8<Queue<SequenceRecord>>(SequenceRecordPath)
                    : new Queue<SequenceRecord>();
#endif
        }

        /// <summary>
        /// 获取序列记录
        /// </summary>
        /// <param name="record">记录</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static void AddSequenceRecord(SequenceRecord record)
        {
#if !UNITY_WEBGL
            if (Parameter.AutoSequenceRecord) SequenceRecordQueue.Enqueue(record);
#endif
        }

        /// <summary>
        /// 销毁资源管理系统
        /// </summary>
        /// <returns></returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async Task Destroy()
        {
            Proxy.Dispose();
#if !UNITY_WEBGL
            if (Parameter.AutoSequenceRecord)
            {
                await AHelper.IO.WriteJsonUTF8Async(SequenceRecordPath, SequenceRecordQueue);
                SequenceRecordQueue = null;
            }
#endif
        }
    }
}