using System.Collections.Generic;
using System.Diagnostics;
using AIO.UEngine;

namespace AIO
{
    partial class AssetSystem
    {
        internal static ASProxy Proxy;

        /// <summary>
        ///     白名单 - 定位指定白名单 - 允许同步加载
        /// </summary>
        internal static List<string> WhiteListLocal { get; } = new List<string>();

        /// <summary>
        ///     白名单 - 全部白名单 - 允许同步加载
        /// </summary>
        public static bool WhiteAll { get; set; }

        /// <summary>
        ///     资源包配置
        /// </summary>
        public static ICollection<AssetsPackageConfig> PackageConfigs => Parameter.Packages;

        /// <summary>
        ///     资源热更新配置
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static ASConfig Parameter { get; internal set; }

        /// <summary>
        ///     是否已经初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static bool IsInitialized => Proxy?.IsInitialize ?? false;

        /// <summary>
        ///     添加白名单
        /// </summary>
        public static void AddWhite(params string[] list)
        {
            WhiteListLocal.AddRange(list);
        }

        /// <summary>
        ///     添加白名单
        /// </summary>
        public static void AddWhite(IEnumerable<string> list)
        {
            WhiteListLocal.AddRange(list);
        }

        /// <summary>
        ///     判断是否在白名单中
        /// </summary>
        public static bool IsWhite(string location)
        {
            return WhiteAll || WhiteListLocal.Contains(location);
        }
    }
}