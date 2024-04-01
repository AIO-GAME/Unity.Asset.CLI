using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AIO
{
    partial class AssetSystem
    {
        /// <summary>
        ///     卸载资源（卸载引用计数为零的资源）
        /// </summary>
        /// <param name="isForce">强制回收所有资源</param>
        [DebuggerNonUserCode]
        [DebuggerHidden]
        [ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static void UnloadUnusedAssets(bool isForce = false)
        {
            Proxy.UnloadUnusedAssets(isForce);
        }

        /// <summary>
        ///     卸载资源
        /// </summary>
        /// <param name="location">资源地址</param>
        [DebuggerNonUserCode]
        [DebuggerHidden]
        [ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static void UnloadAsset(string location)
        {
            Proxy.HandleFree(SettingToLocalPath(location));
        }

        /// <summary>
        ///     卸载资源
        /// </summary>
        /// <param name="locations">资源定位地址</param>
        [DebuggerNonUserCode]
        [DebuggerHidden]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static void UnloadAsset(IEnumerable<string> locations)
        {
            foreach (var location in locations) Proxy.HandleFree(SettingToLocalPath(location));
        }

        /// <summary>
        ///     卸载资源
        /// </summary>
        /// <param name="tags">资源标签</param>
        [DebuggerNonUserCode]
        [DebuggerHidden]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static void UnloadAssetByTags(IEnumerable<string> tags)
        {
            foreach (var location in Proxy.GetAddressByTag(tags)) Proxy.HandleFree(location);
        }

        /// <summary>
        ///     卸载资源
        /// </summary>
        /// <param name="tag">资源标签</param>
        [DebuggerNonUserCode]
        [DebuggerHidden]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static void UnloadAssetByTags(string tag)
        {
            foreach (var location in Proxy.GetAddressByTag(new[] { tag })) Proxy.HandleFree(location);
        }

        /// <summary>
        ///     卸载场景资源
        /// </summary>
        /// <param name="location">资源定位地址</param>
        /// <param name="onLoadComplete">回调</param>
        [DebuggerNonUserCode]
        [DebuggerHidden]
        [ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static IEnumerator UnloadSceneCO(string location, Action onLoadComplete)
        {
            return Proxy.UnloadSceneCO(SettingToLocalPath(location), onLoadComplete);
        }

        /// <summary>
        ///     释放场景资源句柄
        /// </summary>
        /// <param name="location">资源定位地址</param>
        [DebuggerNonUserCode]
        [DebuggerHidden]
        [ProfilerScope]
#if UNITY_2022_1_OR_NEWER
        [HideInCallstack]
#endif
        public static Task UnloadSceneTask(string location)
        {
            return Proxy.UnloadSceneTask(SettingToLocalPath(location));
        }
    }
}