using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AIO.UEngine;
using UnityEngine;

namespace AIO
{
    public partial class AssetSystem
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
        /// 资源包记录序列
        /// </summary>
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
            /// 资源路径
            /// </summary>
            public string AssetPath;

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
    }
}