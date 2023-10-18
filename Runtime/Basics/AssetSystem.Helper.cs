/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AIO
{
    public static partial class AssetSystem
    {
        /// <summary>
        /// 资源回收（卸载引用计数为零的资源）
        /// </summary>
        public static IASDownloader GetDownloader()
        {
            return Proxy.GetDownloader();
        }

        /// <summary>
        /// 资源回收（卸载引用计数为零的资源）
        /// </summary>
        public static void UnloadUnusedAssets()
        {
            Proxy.UnloadUnusedAssets();
        }

        /// <summary>
        /// 强制回收所有资源
        /// </summary>
        public static void ForceUnloadALLAssets()
        {
            Proxy.UnloadUnusedAssets();
        }

        /// <summary>
        /// 检查资源是否有效
        /// </summary>
        /// <param name="location">资源定位地址</param>
        /// <returns>Ture:有效 False:无效</returns>
        public static bool CheckLocationValid(string location)
        {
            return Proxy.CheckLocationValid(location);
        }

        /// <summary>
        /// 是否需要从远端更新下载
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static bool IsNeedDownloadFromRemote(string location)
        {
            return Proxy.IsNeedDownloadFromRemote(location);
        }

        /// <summary>
        /// 释放资源句柄
        /// </summary>
        /// <param name="location">资源地址</param>
        public static void FreeHandle(string location)
        {
            Proxy.FreeHandle(location);
        }

        public static void FreeHandle(IEnumerable<string> locations)
        {
            foreach (var location in locations) Proxy.FreeHandle(location);
        }

        /// <summary>
        /// 平台名称 字符串
        /// </summary>
        /// <returns></returns>
        public static string PlatformNameStr
        {
            get
            {
#if UNITY_EDITOR
                return EditorUserBuildSettings.activeBuildTarget.ToString();
#else
                switch (Application.platform)
                {
                    case RuntimePlatform.WindowsPlayer:
                    case RuntimePlatform.WindowsEditor:
                        return "StandaloneWindows";
                    case RuntimePlatform.OSXPlayer:
                    case RuntimePlatform.OSXEditor:
                        return "StandaloneOSX";
                    case RuntimePlatform.IPhonePlayer:
                        return "iOS";
                    case RuntimePlatform.Android:
                        return "Android";
                    case RuntimePlatform.WebGLPlayer:
                        return "WebGL";
                    default: return Application.platform.ToString();
                }
#endif
            }
        }

        /// <summary>
        /// 平台
        /// </summary>
        public static RuntimePlatform Platform
        {
            get { return Application.platform; }
        }
    }
}
