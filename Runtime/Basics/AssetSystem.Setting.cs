using System.Collections.Generic;
using System.Diagnostics;
using AIO.UEngine;

namespace AIO
{
    public partial class AssetSystem
    {
        private static AssetProxy Proxy;

        /// <summary>
        /// 白名单 - 定位指定白名单 - 允许同步加载
        /// </summary>
        private static List<string> WhiteListLocal { get; set; } = new List<string>();

        /// <summary>
        /// 白名单 - 全部白名单 - 允许同步加载
        /// </summary>
        public static bool WhiteAll;

        /// <summary>
        /// 添加白名单
        /// </summary>
        public static void AddWhite(params string[] list)
        {
            WhiteListLocal.AddRange(list);
        }

        /// <summary>
        /// 添加白名单
        /// </summary>
        public static void AddWhite(IEnumerable<string> list)
        {
            WhiteListLocal.AddRange(list);
        }

        /// <summary>
        /// 判断是否在白名单中
        /// </summary>
        public static bool IsWhite(string location)
        {
            return WhiteAll || WhiteListLocal.Contains(location);
        }

#if UNITY_EDITOR
        /// <summary>
        /// 获取序列记录
        /// </summary>
        /// <param name="record">记录</param>
        [DebuggerNonUserCode, DebuggerHidden, Conditional("UNITY_EDITOR")]
        public static void AddSequenceRecord(SequenceRecord record)
        {
            Parameter.SequenceRecord.Add(record);
            WhiteListLocal.Add(record.Location);
        }
#endif

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
    }
}