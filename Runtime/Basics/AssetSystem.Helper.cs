#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using AIO.UEngine;

#endregion

namespace AIO
{
    [IgnoreConsoleJump]
    partial class AssetSystem
    {
        private static readonly Dictionary<string, string> LocalPathCache = new Dictionary<string, string>(64);

        /// <summary>
        ///     是否需要从远端更新下载
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static bool IsNeedDownloadFromRemote(string location)
        {
            return Parameter.ASMode == EASMode.Remote &&
                   Proxy.CheckNeedDownloadFromRemote(SettingToLocalPath(location));
        }

        /// <summary>
        ///     检查资源是否有效
        /// </summary>
        /// <param name="location">资源定位地址</param>
        /// <returns>Ture:有效 False:无效</returns>
        [DebuggerNonUserCode, DebuggerHidden, ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static bool CheckLocationValid(string location)
        {
            return Proxy.CheckLocationValid(SettingToLocalPath(location));
        }

        /// <summary>
        ///     是否已经加载
        /// </summary>
        /// <param name="location">寻址地址</param>
        /// <returns>Ture 已经加载 False 未加载</returns>
        [DebuggerNonUserCode, DebuggerHidden, ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static bool IsAlreadyLoad(string location)
        {
            return Proxy.AlreadyLoad(SettingToLocalPath(location));
        }

        /// <summary>
        ///     根据设置 获取资源定位地址
        /// </summary>
        /// <param name="location">资源定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden, ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        internal static string SettingToLocalPath(in string location)
        {
            if (string.IsNullOrEmpty(location))
            { // 为空不支持
                LogException("input location is null or empty");
                return string.Empty;
            }

            if (location.EndsWith("/", StringComparison.CurrentCulture) ||
                location.EndsWith("\\", StringComparison.CurrentCulture))
            { // 判断是否以 / 或者 \ 结尾
                LogException($"input location is end with / or \\ -> {location}");
                return string.Empty;
            }

            if (!Parameter.LoadPathToLower) return location;
            if (LocalPathCache.TryGetValue(location, out var address)) return address;
            return LocalPathCache[location] = location.ToLower(); // 转小写 会产生40b左右的GC
        }
    }
}