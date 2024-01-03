/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2024-01-02
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using UnityEngine;

namespace AIO
{
    public partial class AssetSystem
    {
        /// <summary>
        /// 获取是否有读取权限
        /// </summary>
        public static bool GetHasReadPermission()
        {
#if UNITY_EDITOR
            return true;
#else
#if UNITY_WEBGL // WebGL 不需要权限
            return true;
#elif UNITY_STANDALONE_WIN
            var drive = new System.IO.DriveInfo(Application.dataPath.Substring(0, 1));
            return drive.IsReady;
#elif UNITY_STANDALONE_OSX // Mac
            var drive = new System.IO.DriveInfo(Application.dataPath.Substring(0, 1));
            return drive.AvailableFreeSpace;
#elif UNITY_IOS || UNITY_IPHONE // IOS
            return true;
#elif UNITY_ANDROID
            try
            {
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var unityPluginLoader = new AndroidJavaClass("java类全名");
                return unityPluginLoader.CallStatic<bool>("HasReadPermission");
            }
            catch (System.Exception e)
            {
                LogException(e);
            }
#endif

            return false;
#endif
        }

        /// <summary>
        /// 获取是否有写入权限
        /// </summary>
        public static bool GetHasWritePermission()
        {
#if UNITY_EDITOR
            return true;
#else
#if UNITY_WEBGL // WebGL 不需要权限
            return true;
#elif  UNITY_STANDALONE_WIN
            var drive = new System.IO.DriveInfo(Application.dataPath.Substring(0, 1));
            return drive.IsReady;
#elif  UNITY_STANDALONE_OSX // Mac
            var drive = new System.IO.DriveInfo(Application.dataPath.Substring(0, 1));
            return drive.AvailableFreeSpace;
#elif  UNITY_IOS || UNITY_IPHONE // IOS
            return true;
#elif UNITY_ANDROID // Android
            try
            {
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var unityPluginLoader = new AndroidJavaClass("java类全名");
                return unityPluginLoader.CallStatic<bool>("HasWritePermission");
            }
            catch (System.Exception e)
            {
                LogException(e);
            }
#endif
            return false;
#endif
        }

        /// <summary>
        /// 获取可用磁盘空间
        /// </summary>
        /// <returns></returns>
        public static long GetAvailableDiskSpace()
        {
#if !UNITY_EDITOR
#if UNITY_EDITOR_WIN
            var drive = new System.IO.DriveInfo(Application.dataPath.Substring(0, 1));
            return drive.AvailableFreeSpace;
#elif UNITY_EDITOR_OSX
            var drive = new System.IO.DriveInfo(Application.dataPath.Substring(0, 1));
            return drive.AvailableFreeSpace;
#else
            return 1024;
#endif

#else
#if UNITY_STANDALONE_WIN
            var drive = new System.IO.DriveInfo(Application.dataPath.Substring(0, 1));
            return drive.AvailableFreeSpace;

#elif UNITY_ANDROID
            try
            {
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var unityPluginLoader = new AndroidJavaClass("java类全名");
                return unityPluginLoader.CallStatic<long>("GetFreeDiskSpace");
            }
            catch (System.Exception e)
            {
                LogException(e);
            }
#elif UNITY_IPHONE && !UNITY_EDITOR
            return (long)_IOS_GetFreeDiskSpace();

#elif UNITY_WEBGL
            var drive = new System.IO.DriveInfo(Application.dataPath.Substring(0, 1));
            return drive.AvailableFreeSpace;
#endif
            return 1024;
#endif
        }
    }
}