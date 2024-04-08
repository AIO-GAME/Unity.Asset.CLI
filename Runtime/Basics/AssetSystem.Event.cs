using System;

namespace AIO
{
    partial class AssetSystem
    {
        internal static ASException _Exception;

        /// <summary>
        ///     系统初始化异常
        /// </summary>
        public static event Action<ASException> OnException;

        /// <summary>
        ///     重置下载器
        /// </summary>
        public static void ResetDownloadHandle()
        {
            HandleReset = true;
            if (DownloadHandle != null) DownloadHandle.Cancel();
            var temp = Proxy.GetLoadingHandle();
            var dEvent = DownloadEvent;
            if (dEvent != null)
            {
                temp.Event.OnNetReachableCarrier = dEvent.OnNetReachableCarrier;
                temp.Event.OnWritePermissionNot  = dEvent.OnWritePermissionNot;
                temp.Event.OnReadPermissionNot   = dEvent.OnReadPermissionNot;
                temp.Event.OnDiskSpaceNotEnough  = dEvent.OnDiskSpaceNotEnough;
                temp.Event.OnNetReachableNot     = dEvent.OnNetReachableNot;
                temp.Event.OnProgress            = dEvent.OnProgress;
                temp.Event.OnError               = dEvent.OnError;
                temp.Event.OnComplete            = dEvent.OnComplete;
                temp.Event.OnCancel              = dEvent.OnCancel;
                temp.Event.OnBegin               = dEvent.OnBegin;
                temp.Event.OnResume              = dEvent.OnResume;
                temp.Event.OnPause               = dEvent.OnPause;
            }

            DownloadHandle = temp;
            StatusStop     = false;
        }
    }
}