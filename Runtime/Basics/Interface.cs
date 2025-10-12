#region

using System;
using UnityEngine.Scripting;

#endregion

namespace AIO
{
    /// <summary>
    /// 网络资源加载进度
    /// </summary>
    [Preserve]
    public interface IASNetLoading
    {
        /// <summary>
        /// 当前下载进度
        /// </summary>
        [Preserve]
        IProgressInfo Progress { get; }

        /// <summary>
        /// 当前下载事件
        /// </summary>
        [Preserve]
        IDownlandAssetEvent Event { get; }

        /// <summary>
        /// 当前下载状态
        /// </summary>
        [Preserve]
        EProgressState State { get; }

        /// <summary>
        /// 取消当前所有下载
        /// </summary>
        [Preserve]
        void Cancel();

        /// <summary>
        /// 清空当前下载事件
        /// </summary>
        [Preserve]
        void CleanEvent();
    }

    [Preserve]
    public interface IDownlandAssetEvent : IProgressEvent
    {
        /// <summary>
        /// 网络不可用
        /// </summary>
        [Preserve]
        Action<IProgressReport> OnNetReachableNot { get; set; }

        /// <summary>
        /// 移动网络 是否允许在移动网络条件下下载
        /// 如果允许则 调用 Action 继续下载
        /// 如果不允许 则下载暂停 需要手动恢复 也可以直接取消
        /// </summary>
        [Preserve]
        Action<IProgressReport, Action> OnNetReachableCarrier { get; set; }

        /// <summary>
        /// 磁盘空间不足
        /// </summary>
        [Preserve]
        Action<IProgressReport> OnDiskSpaceNotEnough { get; set; }

        /// <summary>
        /// 无写入权限
        /// </summary>
        [Preserve]
        Action<IProgressReport> OnWritePermissionNot { get; set; }

        /// <summary>
        /// 无读取权限
        /// </summary>
        [Preserve]
        Action<IProgressReport> OnReadPermissionNot { get; set; }
    }

    [Preserve]
    public struct DownlandAssetEvent : IDownlandAssetEvent
    {
        /// <summary>
        /// 网络不可用
        /// </summary>
        [Preserve]
        public Action<IProgressReport> OnNetReachableNot { get; set; }

        /// <summary>
        /// 移动网络 是否允许在移动网络条件下下载
        /// 如果允许则 调用 Action 继续下载
        /// 如果不允许 则下载暂停 需要手动恢复 也可以直接取消
        /// </summary>
        [Preserve]
        public Action<IProgressReport, Action> OnNetReachableCarrier { get; set; }

        /// <summary>
        /// 磁盘空间不足
        /// </summary>
        [Preserve]
        public Action<IProgressReport> OnDiskSpaceNotEnough { get; set; }

        /// <summary>
        /// 无写入权限
        /// </summary>
        [Preserve]
        public Action<IProgressReport> OnWritePermissionNot { get; set; }

        /// <summary>
        /// 无读取权限
        /// </summary>
        [Preserve]
        public Action<IProgressReport> OnReadPermissionNot { get; set; }

        /// <summary>
        /// 下载进度
        /// </summary>
        [Preserve]
        public Action<IProgressInfo> OnProgress { get; set; }

        /// <summary>
        /// 下载完成
        /// </summary>
        [Preserve]
        public Action<IProgressReport> OnComplete { get; set; }

        /// <summary>
        /// 下载开始
        /// </summary>
        [Preserve]
        public Action OnBegin { get; set; }

        /// <summary>
        /// 下载错误
        /// </summary>
        [Preserve]
        public Action<Exception> OnError { get; set; }

        /// <summary>
        /// 下载恢复
        /// </summary>
        [Preserve]
        public Action OnResume { get; set; }

        /// <summary>
        /// 下载暂停
        /// </summary>
        [Preserve]
        public Action OnPause { get; set; }

        /// <summary>
        /// 下载取消
        /// </summary>
        [Preserve]
        public Action OnCancel { get; set; }

        [Preserve]
        public void Dispose()
        {
            OnProgress            = null;
            OnComplete            = null;
            OnBegin               = null;
            OnError               = null;
            OnResume              = null;
            OnPause               = null;
            OnCancel              = null;
            OnNetReachableNot     = null;
            OnNetReachableCarrier = null;
            OnDiskSpaceNotEnough  = null;
            OnWritePermissionNot  = null;
            OnReadPermissionNot   = null;
        }
    }

    /// <summary>
    /// 资源下载器
    /// </summary>
    [Preserve]
    public interface IASDownloader : IProgressOperation, IDownlandAssetEvent
    {
        /// <summary>
        /// 是否运行继续流程
        /// </summary>
        [Preserve]
        bool Flow { get; }

        /// <summary>
        /// 收集需要下载的所有资源
        /// </summary>
        [Preserve]
        void CollectNeedAll();

        /// <summary>
        /// 收集需要下载的标签
        /// </summary>
        [Preserve]
        void CollectNeedTag(params string[] tags);
    }
}