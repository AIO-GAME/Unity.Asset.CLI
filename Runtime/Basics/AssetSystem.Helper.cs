/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Diagnostics;
using System.IO;
using AIO.UEngine;
using UnityEngine;
using Debug = UnityEngine.Debug;
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
        public static bool IsAlreadyLoad(string location)
        {
            return Proxy.IsAlreadyLoad(SettingToLocalPath(location));
        }

        /// <summary>
        /// 根据设置 获取资源定位地址
        /// </summary>
        /// <param name="location">资源定位地址</param>
        private static string SettingToLocalPath(string location)
        {
            if (string.IsNullOrEmpty(location)) return string.Empty;
            return Parameter.LoadPathToLower ? location.ToLower() : location;
        }

        /// <summary>
        /// 获取可用磁盘空间
        /// </summary>
        /// <returns></returns>
        public static long GetAvailableDiskSpace()
        {
#if UNITY_EDITOR

#if UNITY_EDITOR_WIN
            var drive = new DriveInfo(Application.dataPath.Substring(0, 1));
            return drive.AvailableFreeSpace;
#elif UNITY_EDITOR_OSX
            var drive = new DriveInfo(Application.dataPath.Substring(0, 1));
            return drive.AvailableFreeSpace;
#else
            return 1024;
#endif

#else
#if UNITY_STANDALONE_WIN
            var drive = System.IO.DriveInfo.GetDrives();
            return drive.AvailableFreeSpace;

#elif UNITY_ANDROID

            try
            {
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var unityPluginLoader = new AndroidJavaClass("java类全名");
                return unityPluginLoader.CallStatic<long>("GetFreeDiskSpace");
            }
            catch (Exception e)
            {
                LogException(e);
            }
#elif UNITY_IPHONE && !UNITY_EDITOR
            return (long)_IOS_GetFreeDiskSpace();

#elif UNITY_WEBGL
            var drive = new DriveInfo(Application.dataPath.Substring(0, 1));
            return drive.AvailableFreeSpace;
#endif
            return 1024;
#endif
        }

        #region LOG

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump]
        internal static void LogException(Exception e)
        {
            if (Parameter.OutputLog) Debug.LogException(e);
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump]
        internal static void LogException(string e)
        {
            if (Parameter.OutputLog) Debug.LogException(new SystemException(e));
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump]
        internal static void LogException(string format, params object[] args)
        {
            if (Parameter.OutputLog)
                Debug.LogException(new SystemException(string.Format(format, args)));
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump]
        internal static void Log(string e)
        {
            if (Parameter.OutputLog) Debug.Log(e);
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump]
        internal static void LogWarning(string e)
        {
            if (Parameter.OutputLog) Debug.LogWarning(e);
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump]
        internal static void LogFormat(string format, params object[] args)
        {
            if (Parameter.OutputLog) Debug.LogFormat(format, args);
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump]
        internal static void LogError(string e)
        {
            if (Parameter.OutputLog) Debug.LogError(e);
        }

        [DebuggerHidden, DebuggerNonUserCode, IgnoreConsoleJump]
        internal static void LogErrorFormat(string format, params object[] args)
        {
            if (Parameter.OutputLog) Debug.LogErrorFormat(format, args);
        }

        #endregion
    }
}