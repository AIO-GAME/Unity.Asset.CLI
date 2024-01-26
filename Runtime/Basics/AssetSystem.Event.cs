/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using UnityEngine;

namespace AIO
{
    public partial class AssetSystem
    {
        /// <summary>
        /// 是否允许流量下载
        /// </summary>
        public static bool AllowReachableCarrier { get; set; }

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

        /// <summary>
        /// 主下载器
        /// </summary>
        public static IASNetLoading DownloadHandle { get; private set; }

        /// <summary>
        /// 重置下载器
        /// </summary>
        public static void ResetDownloadHandle()
        {
            HandleReset = true;
            if (DownloadHandle != null) DownloadHandle.Cancel();
            var temp = Proxy.GetLoadingHandle();
            temp.Event.OnNetReachableCarrier = DownloadEvent.OnNetReachableCarrier;
            temp.Event.OnWritePermissionNot = DownloadEvent.OnWritePermissionNot;
            temp.Event.OnReadPermissionNot = DownloadEvent.OnReadPermissionNot;
            temp.Event.OnDiskSpaceNotEnough = DownloadEvent.OnDiskSpaceNotEnough;
            temp.Event.OnNetReachableNot = DownloadEvent.OnNetReachableNot;
            temp.Event.OnProgress = DownloadEvent.OnProgress;
            temp.Event.OnError = DownloadEvent.OnError;
            temp.Event.OnComplete = DownloadEvent.OnComplete;
            temp.Event.OnCancel = DownloadEvent.OnCancel;
            temp.Event.OnBegin = DownloadEvent.OnBegin;
            temp.Event.OnResume = DownloadEvent.OnResume;
            temp.Event.OnPause = DownloadEvent.OnPause;
            DownloadHandle = temp;
            StatusStop = false;
        }

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
    }
}