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
    public partial class AssetSystem
    {
        /// <summary>
        /// 资源回收（卸载引用计数为零的资源）
        /// </summary>
        /// <param name="isForce">强制回收所有资源</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static void UnloadUnusedAssets(bool isForce = false)
        {
            Proxy.UnloadUnusedAssets(isForce);
        }

        /// <summary>
        /// 释放资源句柄
        /// </summary>
        /// <param name="location">资源地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static void UnloadAsset(string location)
        {
            Proxy.FreeHandle(location);
#if UNITY_EDITOR
            LogFormat("Asset System FreeHandle Release : {0}", location);
#endif
        }

        /// <summary>
        /// 释放资源句柄
        /// </summary>
        /// <param name="locations">资源定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static void UnloadAsset(IEnumerable<string> locations)
        {
            Proxy.FreeHandle(locations);
        }

        /// <summary>
        /// 释放场景资源句柄
        /// </summary>
        /// <param name="location">资源定位地址</param>
        /// <param name="onLoadComplete">回调</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator UnloadSceneCO(string location, Action onLoadComplete)
        {
            return Proxy.UnloadSceneCO(location, onLoadComplete);
        }

        /// <summary>
        /// 释放场景资源句柄
        /// </summary>
        /// <param name="location">资源定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task UnloadSceneTask(string location)
        {
            return Proxy.UnloadSceneTask(location);
        }
    }
}