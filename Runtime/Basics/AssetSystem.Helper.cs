﻿/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System.Diagnostics;
using AIO.UEngine;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AIO
{
    [IgnoreConsoleJump]
    public static partial class AssetSystem
    {
        /// <summary>
        /// 运行平台
        /// </summary>
        public static RuntimePlatform Platform => Application.platform;

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
        /// 是否需要从远端更新下载
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public static bool IsNeedDownloadFromRemote(string location)
        {
            return Parameter.ASMode == EASMode.Remote && Proxy.IsNeedDownloadFromRemote(SettingToLocalPath(location));
        }

        /// <summary>
        /// 检查资源是否有效
        /// </summary>
        /// <param name="location">资源定位地址</param>
        /// <returns>Ture:有效 False:无效</returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static bool CheckLocationValid(string location)
        {
            return Proxy.CheckLocationValid(SettingToLocalPath(location));
        }

        /// <summary>
        /// 是否已经加载
        /// </summary>
        /// <param name="location">寻址地址</param>
        /// <returns>Ture 已经加载 False 未加载</returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static bool IsAlreadyLoad(string location)
        {
            return Proxy.IsAlreadyLoad(SettingToLocalPath(location));
        }

        /// <summary>
        /// 根据设置 获取资源定位地址
        /// </summary>
        /// <param name="location">资源定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        private static string SettingToLocalPath(string location)
        {
            if (string.IsNullOrEmpty(location)) return string.Empty;
            return Parameter.LoadPathToLower ? location.ToLower() : location;
        }
    }
}