using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace AIO
{
    partial class AssetSystem
    {
        /// <summary>
        /// 是否允许流量下载
        /// </summary>
        public static bool AllowReachableCarrier { get; set; }

        /// <summary>
        /// 主下载器
        /// </summary>
        public static IASNetLoading DownloadHandle { get; private set; }

        /// <summary>
        /// 当前主下载器事件
        /// </summary>
        public static IDownlandAssetEvent DownloadEvent => DownloadHandle?.Event;

        /// <summary>
        /// 下载器状态
        /// </summary>
        public static EProgressState DownloadState => DownloadHandle?.State ?? EProgressState.Finish;

        /// <summary>
        /// 判断当前网络环境是否为流量
        /// </summary>
        /// <returns></returns>
        public static bool IsWifi =>
            Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;

        /// <summary>
        /// 判断当前网络环境是否为无网络
        /// </summary>
        public static bool IsNoNet =>
            Application.internetReachability == NetworkReachability.NotReachable;

        /// <summary>
        /// 判断当前网络环境是否为流量
        /// </summary>
        public static bool IsNetReachable =>
            Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork;

        /// <summary>
        /// 运行时 内置文件目录 
        /// </summary>
        /// <remarks>
        /// (Application.streamingAssetsPath)/RuntimeRootDirectory
        /// </remarks>
        public static string BuildInRootDirectory { get; private set; }

        /// <summary>
        /// 运行时 缓存文件目录 
        /// </summary>
        /// <remarks>
        /// (Application.persistentDataPath)/RuntimeRootDirectory
        /// </remarks>
        public static string SandboxRootDirectory { get; private set; }

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
                        return System.Environment.Is64BitOperatingSystem
                            ? "StandaloneWindows64"
                            : "StandaloneWindows";
                    case RuntimePlatform.OSXPlayer:
                    case RuntimePlatform.OSXEditor:
                        return "StandaloneOSX";
                    case RuntimePlatform.IPhonePlayer:
                        return "iOS";
                    case RuntimePlatform.Android:
                        return "Android";
                    case RuntimePlatform.WebGLPlayer:
                        return "WebGL";
                    case RuntimePlatform.Switch:
                        return "Switch";
                    case RuntimePlatform.PS4:
                        return "PS4";
                    case RuntimePlatform.PS5:
                        return "PS5";
                    case RuntimePlatform.LinuxPlayer:
                    case RuntimePlatform.LinuxEditor:
                        return System.Environment.Is64BitOperatingSystem
                            ? "StandaloneLinux64"
                            : "StandaloneLinux";
                    case RuntimePlatform.WSAPlayerARM:
                        return "WSAPlayer";
                    case RuntimePlatform.XboxOne:
                        return "XboxOne";
                    case RuntimePlatform.tvOS:
                        return "tvOS";
                    case RuntimePlatform.Lumin:
                        return "Lumin";
                    case RuntimePlatform.Stadia:
                        return "Stadia";
                    case RuntimePlatform.CloudRendering:
                        return "CloudRendering";
                    case RuntimePlatform.GameCoreXboxOne:
                        return "GameCoreXboxOne";

                    default: return Application.platform.ToString();
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
        internal static bool StatusStop { get; set; }

        /// <summary>
        /// 下载器是否重置
        /// </summary>
        internal static bool HandleReset { get; set; }
    }
}