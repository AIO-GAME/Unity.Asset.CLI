/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AIO.UEngine;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AIO
{
    public static partial class AssetSystem
    {
        /// <summary>
        /// 获取下载器
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IASDownloader GetDownloader()
        {
            return Proxy.GetDownloader();
        }

        /// <summary>
        /// 预下载全部远端资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async Task DownloadPre()
        {
            if (Parameter.ASMode != EASMode.Remote) return;
            var handle = GetDownloader();
            var flow = await handle.UpdatePackageManifestTask();
            if (flow) flow = await handle.UpdatePackageVersionTask();
            if (flow) flow = handle.CreateDownloader();
            if (flow) await handle.BeginDownload();
        }

        /// <summary>
        /// 动态下载远端资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async Task DownloadDynamic()
        {
            if (Parameter.ASMode != EASMode.Remote) return;
            var handle = GetDownloader();
            var flow = await handle.UpdatePackageManifestTask();
            if (flow) await handle.UpdatePackageVersionTask();
        }

        /// <summary>
        /// 资源回收（卸载引用计数为零的资源）
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static void UnloadUnusedAssets()
        {
            Proxy.UnloadUnusedAssets();
        }

        /// <summary>
        /// 强制回收所有资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static void ForceUnloadALLAssets()
        {
            Proxy.UnloadUnusedAssets();
        }

        /// <summary>
        /// 检查资源是否有效
        /// </summary>
        /// <param name="location">资源定位地址</param>
        /// <returns>Ture:有效 False:无效</returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static bool CheckLocationValid(string location)
        {
            return Proxy.CheckLocationValid(location);
        }

        /// <summary>
        /// 是否需要从远端更新下载
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static bool IsNeedDownloadFromRemote(string location)
        {
            return Proxy.IsNeedDownloadFromRemote(location);
        }

        /// <summary>
        /// 释放资源句柄
        /// </summary>
        /// <param name="location">资源地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static void FreeHandle(string location)
        {
            Proxy.FreeHandle(location);
#if UNITY_EDITOR
            UnityEngine.Debug.LogFormat("Asset System FreeHandle Release : {0}", location);
#endif
        }

        [DebuggerNonUserCode, DebuggerHidden]
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
                        return "StandaloneWindows64";
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
        /// 运行平台
        /// </summary>
        public static RuntimePlatform Platform => Application.platform;

        /// <summary>
        /// 获取远端资源包列表
        /// </summary>
        /// <returns></returns>
        public static async Task<AssetsPackageConfig[]> GetRemotePackageList(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
            var remote = Path.Combine(url, "Version", string.Concat(PlatformNameStr, ".json"));
            var config = await AHelper.Net.HTTP.GetAsync(remote);
            return AHelper.Json.Deserialize<AssetsPackageConfig[]>(config);
        }


        /// <summary>
        /// 是否已经加载
        /// </summary>
        /// <param name="location">寻址地址</param>
        /// <returns>Ture 已经加载 False 未加载</returns>
        public static bool IsAlreadyLoad(string location)
        {
            return Proxy.IsAlreadyLoad(Parameter.LoadPathToLower ? location.ToLower() : location);
        }
    }
}