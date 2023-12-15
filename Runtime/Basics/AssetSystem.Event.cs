/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;

namespace AIO
{
    public partial class AssetSystem
    {
        /// <summary>
        /// 事件通知
        /// </summary>
        public static event Action<EASEventType, string> OnEventNotify;

        /// <summary>
        /// 资源正在下载
        /// </summary>
        public static event Action<IProgressHandle> OnEventDownloading;

        public static event Action OnNoNetwork;
        
        internal static void InvokeNotify(EASEventType eventType, string message)
        {
            OnEventNotify?.Invoke(eventType, message);
        }

        internal static bool HasEvent_OnDownloading()
        {
            return OnEventDownloading != null;
        }

        internal static void InvokeDownloading<T>(T loading) where T : IProgressHandle
        {
            OnEventDownloading?.Invoke(loading);
        }
    }
}
