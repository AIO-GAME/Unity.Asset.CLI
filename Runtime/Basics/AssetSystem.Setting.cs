#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using AIO.UEngine;
using UnityEngine.Scripting;

#endregion

namespace AIO
{
    partial class AssetSystem
    {
        [Preserve]
        internal static ASProxy Proxy;

        /// <summary>
        /// 白名单 - 定位指定白名单 - 允许同步加载
        /// </summary>
        [Preserve]
        internal static HashSet<string> WhiteListLocal { get; } = new HashSet<string>();

        /// <summary>
        /// 白名单 - 全部白名单 - 允许同步加载
        /// </summary>
        [Preserve]
        public static bool WhiteAll { get; set; }

        /// <summary>
        /// 资源包配置
        /// </summary>
        [Preserve]
        public static ICollection<AssetsPackageConfig> PackageConfigs => Parameter.Packages;

        /// <summary>
        /// 资源热更新配置
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        [Preserve]
        public static ASConfig Parameter { get; internal set; }

        /// <summary>
        /// 是否已经初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        [Preserve]
        public static bool IsInitialized => Proxy?.IsInitialize ?? false;

        /// <summary>
        /// 添加白名单
        /// </summary>
        [Preserve]
        public static void AddWhite(params string[] list)
        {
            foreach (var item in list) WhiteListLocal.Add(item);
        }

        /// <summary>
        /// 添加白名单
        /// </summary>
        [Preserve]
        public static void AddWhite(IEnumerable<string> list)
        {
            foreach (var item in list) WhiteListLocal.Add(item);
        }

        /// <summary>
        /// 判断是否在白名单中
        /// </summary>
        [Preserve]
        public static bool IsWhite(string location) { return WhiteAll || WhiteListLocal.Contains(location); }
    }
}