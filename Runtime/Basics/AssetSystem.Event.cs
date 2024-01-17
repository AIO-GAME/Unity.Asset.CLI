/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using UnityEngine;

namespace AIO
{
    public partial class AssetSystem
    {
        /// <summary>
        /// 主下载器
        /// </summary>
        public static IASNetLoading MainDownloadHandle { get; private set; }

        /// <summary>
        /// 当前主下载器事件
        /// </summary>
        public static IDownlandAssetEvent DownloadEvent => MainDownloadHandle?.Event;

        /// <summary>
        /// 下载器状态
        /// </summary>
        public static EProgressState DownloadHandleState => MainDownloadHandle?.State ?? EProgressState.Finish;

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