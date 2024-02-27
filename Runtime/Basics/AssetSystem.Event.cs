/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using UnityEngine;

namespace AIO
{
    partial class AssetSystem
    {
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
    }
}