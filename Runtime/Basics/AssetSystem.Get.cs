/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2024-01-02
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace AIO
{
    partial class AssetSystem
    {
        /// <summary>
        /// 获取指定标签资源可寻址列表
        /// </summary>
        /// <param name="tag">资源标签</param>
        /// <returns>寻址列表</returns>
        public static ICollection<string> GetAddressByTag(string tag)
        {
            return Proxy.GetAddressByTag(new[] { tag });
        }

        /// <summary>
        /// 根据资源标签获取资源信息
        /// </summary>
        /// <param name="tag">资源标签</param>
        /// <param name="tags">资源标签</param>
        public static ICollection<string> GetAddressByTag(string tag, params string[] tags)
        {
            return Proxy.GetAddressByTag(tags.Append(tag));
        }

        /// <summary>
        /// 获取指定标签资源可寻址列表
        /// </summary>
        /// <param name="tags">资源标签</param>
        /// <returns>寻址列表</returns>
        public static ICollection<string> GetAddressByTag(IEnumerable<string> tags)
        {
            return Proxy.GetAddressByTag(tags);
        }

        /// <summary>
        /// 获取是否有读取权限
        /// </summary>
        public static bool GetHasReadPermission()
        {
#if UNITY_EDITOR

#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
            return new DriveInfo(SandboxRootDirectory).IsReady;
#else
            UnityEngine.Debug.LogWarning("Platform not support");
            return false;
#endif

#else
#if UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID || UNITY_WEBGL
            return true;
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            return new DriveInfo(SandboxRootDirectory).IsReady;      
#else
            UnityEngine.Debug.LogWarning("Platform not support");
            return false;
#endif

#endif
        }

        /// <summary>
        /// 获取是否有写入权限
        /// </summary>
        public static bool GetHasWritePermission()
        {
#if UNITY_EDITOR

#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
            return new DriveInfo(SandboxRootDirectory).IsReady;
#else
            UnityEngine.Debug.LogWarning("Platform not support");
            return false;
#endif

#else
#if UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID || UNITY_WEBGL
            return true;
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            return new DriveInfo(SandboxRootDirectory).IsReady;
#else
            UnityEngine.Debug.LogWarning("Platform not support");
            return false;
#endif

#endif
        }

#if UNITY_IOS || UNITY_IPHONE
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern long _IOS_GetFreeDiskSpace();
#endif

#if UNITY_WEBGL
        // [System.Runtime.InteropServices.DllImport("__Internal")]
        // private static extern long _WEBGL_GetFreeDiskSpace();
#endif

        /// <summary>
        /// 获取可用磁盘空间
        /// </summary>
        /// <returns>剩余空间字节数</returns>
        public static long GetAvailableDiskSpace()
        {
            try
            {
#if UNITY_EDITOR

#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                return string.IsNullOrEmpty(SandboxRootDirectory)
                    ? new DriveInfo(Application.dataPath).AvailableFreeSpace
                    : new DriveInfo(SandboxRootDirectory).AvailableFreeSpace;
#else
                UnityEngine.Debug.LogWarning("Platform not support");
                return 0;
#endif

#else
#if UNITY_WEBGL
                return 0;
#elif UNITY_IOS || UNITY_IPHONE
                return _IOS_GetFreeDiskSpace();
#elif UNITY_ANDROID
                var unityPluginLoader = new UnityEngine.AndroidJavaClass("IOHelper");
                return unityPluginLoader.CallStatic<long>("GetFreeDiskSpace");
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
                return new DriveInfo(SandboxRootDirectory).AvailableFreeSpace;
#else
                UnityEngine.Debug.LogWarning("Platform not support");
                return 0;
#endif
#endif
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return 0;
            }
        }
    }
}