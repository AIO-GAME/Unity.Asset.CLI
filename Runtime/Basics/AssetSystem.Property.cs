#region

using UnityEngine;
using UnityEngine.Scripting;
#if UNITY_EDITOR
using UnityEditor;
#endif

#endregion

namespace AIO
{
    partial class AssetSystem
    {
        /// <summary>
        /// 是否允许流量下载
        /// </summary>
        [Preserve]
        public static bool AllowReachableCarrier { get; set; }

        /// <summary>
        /// 主下载器
        /// </summary>
        [Preserve]
        private static IASNetLoading _DownloadHandle { get; set; }

        /// <summary>
        /// 主下载器
        /// </summary>
        [Preserve]
        public static IASNetLoading DownloadHandle
        {
            get
            {
                if (_DownloadHandle is null) _DownloadHandle = Proxy.GetLoadingHandle();
                return _DownloadHandle;
            }
            internal set => _DownloadHandle = value;
        }

        /// <summary>
        /// 当前主下载器事件
        /// </summary>
        [Preserve]
        public static IDownlandAssetEvent DownloadEvent => DownloadHandle?.Event;

        /// <summary>
        /// 下载器状态
        /// </summary>
        [Preserve]
        public static EProgressState DownloadState => DownloadHandle?.State ?? EProgressState.Finish;

        /// <summary>
        /// 判断当前网络环境是否为流量
        /// </summary>
        /// <returns></returns>
        [Preserve]
        public static bool IsWifi =>
            Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;

        /// <summary>
        /// 判断当前网络环境是否为无网络
        /// </summary>
        [Preserve]
        public static bool IsNoNet =>
            Application.internetReachability == NetworkReachability.NotReachable;

        /// <summary>
        /// 判断当前网络环境是否为流量
        /// </summary>
        [Preserve]
        public static bool IsNetReachable =>
            Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork;

        /// <summary>
        /// 运行时 内置文件目录
        /// </summary>
        /// <remarks>
        /// (Application.streamingAssetsPath)/RuntimeRootDirectory
        /// </remarks>
        [Preserve]
        public static string BuildInRootDirectory { get; internal set; }

        /// <summary>
        /// 运行时 缓存文件目录
        /// </summary>
        /// <remarks>
        /// (Application.persistentDataPath)/RuntimeRootDirectory
        /// </remarks>
        [Preserve]
        public static string SandboxRootDirectory { get; internal set; }

        /// <summary>
        /// 运行平台
        /// </summary>
        [Preserve]
        public static RuntimePlatform Platform => Application.platform;

        /// <summary>
        /// 平台名称 字符串
        /// </summary>
        /// <returns></returns>
        [Preserve]
        public static string PlatformNameStr
        {
            get
            {
#if UNITY_EDITOR
                return string.Intern(EditorUserBuildSettings.activeBuildTarget.ToString());
#else
                switch (Application.platform)
                {
                    case RuntimePlatform.WindowsPlayer:
                    case RuntimePlatform.WindowsEditor:
                        return string.Intern(System.Environment.Is64BitOperatingSystem
                            ? "StandaloneWindows64"
                            : "StandaloneWindows");
                    case RuntimePlatform.OSXPlayer:
                    case RuntimePlatform.OSXEditor:
                        return string.Intern("StandaloneOSX");
                    case RuntimePlatform.IPhonePlayer:
                        return string.Intern("iOS");
                    case RuntimePlatform.Android:
                        return string.Intern("Android");
                    case RuntimePlatform.WebGLPlayer:
                        return string.Intern("WebGL");
                    case RuntimePlatform.Switch:
                        return string.Intern("Switch");
                    case RuntimePlatform.PS4:
                        return string.Intern("PS4");
                    case RuntimePlatform.PS5:
                        return string.Intern("PS5");
                    case RuntimePlatform.LinuxPlayer:
                    case RuntimePlatform.LinuxEditor:
                        return string.Intern(System.Environment.Is64BitOperatingSystem
                            ? "StandaloneLinux64"
                            : "StandaloneLinux");
                    case RuntimePlatform.WSAPlayerARM:
                        return string.Intern("WSAPlayer");
                    case RuntimePlatform.XboxOne:
                        return string.Intern("XboxOne");
                    case RuntimePlatform.tvOS:
                        return string.Intern("tvOS");
                    case RuntimePlatform.Lumin:
                        return string.Intern("Lumin");
                    case RuntimePlatform.Stadia:
                        return string.Intern("Stadia");
                    case RuntimePlatform.CloudRendering:
                        return string.Intern("CloudRendering");
                    case RuntimePlatform.GameCoreXboxOne:
                        return string.Intern("GameCoreXboxOne");

                    default: return string.Intern(Application.platform.ToString());
                }
#endif
            }
        }

        /// <summary>
        /// 队列暂停中
        /// </summary>
        /// <remarks>
        /// Ture: 暂停中
        /// False: 没有暂停
        /// </remarks>
        [Preserve]
        internal static bool StatusStop { get; set; }

        /// <summary>
        /// 下载器是否重置
        /// </summary>
        [Preserve]
        internal static bool HandleReset { get; set; }
    }
}