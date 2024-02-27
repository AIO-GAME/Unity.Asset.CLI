/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

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
        /// 卸载资源（卸载引用计数为零的资源）
        /// </summary>
        /// <param name="isForce">强制回收所有资源</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static void UnloadUnusedAssets(bool isForce = false)
        {
            Proxy.UnloadUnusedAssets(isForce);
        }

        /// <summary>
        /// 卸载资源 
        /// </summary>
        /// <param name="location">资源地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static void UnloadAsset(string location)
        {
            Proxy.HandleFree(SettingToLocalPath(location));
            Log("Free Asset Handle Release : {0}", location);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="locations">资源定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static void UnloadAsset(IEnumerable<string> locations)
        {
            foreach (var location in locations) Proxy.HandleFree(SettingToLocalPath(location));
        }

        /// <summary>
        /// 卸载场景资源
        /// </summary>
        /// <param name="location">资源定位地址</param>
        /// <param name="onLoadComplete">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator UnloadSceneCO(string location, Action onLoadComplete)
        {
            Log("Free Scene Handle Release : {0}", location);
            return Proxy.UnloadSceneCO(SettingToLocalPath(location), onLoadComplete);
        }

        /// <summary>
        /// 释放场景资源句柄
        /// </summary>
        /// <param name="location">资源定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task UnloadSceneTask(string location)
        {
            Log("Free Scene Handle Release : {0}", location);
            return Proxy.UnloadSceneTask(SettingToLocalPath(location));
        }
    }
}