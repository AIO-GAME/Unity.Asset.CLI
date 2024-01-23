/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-15
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections;

namespace AIO
{
    /// <summary>
    /// 网络资源加载进度
    /// </summary>
    public interface IASNetLoading
    {
        /// <summary>
        /// 当前下载进度
        /// </summary>
        IProgressInfo Progress { get; }

        /// <summary>
        /// 当前下载事件
        /// </summary>
        IDownlandAssetEvent Event { get; }

        /// <summary>
        /// 当前下载状态
        /// </summary>
        EProgressState State { get; }
    }

    public interface IDownlandAssetEvent : IProgressEvent
    {
        /// <summary>
        /// 网络不可用
        /// </summary>
        Action<IProgressReport> OnNetReachableNot { get; set; }

        /// <summary>
        /// 移动网络 是否允许在移动网络条件下下载
        /// 如果允许则 调用 Action 继续下载
        /// 如果不允许 则下载暂停 需要手动恢复 也可以直接取消
        /// </summary>
        Action<IProgressReport, Action> OnNetReachableCarrier { get; set; }

        /// <summary>
        /// 磁盘空间不足
        /// </summary>
        Action<IProgressReport> OnDiskSpaceNotEnough { get; set; }

        /// <summary>
        /// 无写入权限
        /// </summary>
        Action<IProgressReport> OnWritePermissionNot { get; set; }

        /// <summary>
        /// 无读取权限
        /// </summary>
        Action<IProgressReport> OnReadPermissionNot { get; set; }
    }

    public struct DownlandAssetEvent : IDownlandAssetEvent
    {
        /// <summary>
        /// 网络不可用
        /// </summary>
        public Action<IProgressReport> OnNetReachableNot { get; set; }

        /// <summary>
        /// 移动网络 是否允许在移动网络条件下下载
        /// 如果允许则 调用 Action 继续下载
        /// 如果不允许 则下载暂停 需要手动恢复 也可以直接取消
        /// </summary>
        public Action<IProgressReport, Action> OnNetReachableCarrier { get; set; }

        /// <summary>
        /// 磁盘空间不足
        /// </summary>
        public Action<IProgressReport> OnDiskSpaceNotEnough { get; set; }

        /// <summary>
        /// 无写入权限
        /// </summary>
        public Action<IProgressReport> OnWritePermissionNot { get; set; }

        /// <summary>
        /// 无读取权限
        /// </summary>
        public Action<IProgressReport> OnReadPermissionNot { get; set; }

        /// <summary>
        /// 下载进度
        /// </summary>
        public Action<IProgressInfo> OnProgress { get; set; }

        /// <summary>
        /// 下载完成
        /// </summary>
        public Action<IProgressReport> OnComplete { get; set; }

        /// <summary>
        /// 下载开始
        /// </summary>
        public Action OnBegin { get; set; }

        /// <summary>
        /// 下载错误
        /// </summary>
        public Action<Exception> OnError { get; set; }

        /// <summary>
        /// 下载恢复
        /// </summary>
        public Action OnResume { get; set; } 

        /// <summary>
        /// 下载暂停
        /// </summary>
        public Action OnPause { get; set; }

        /// <summary>
        /// 下载取消
        /// </summary>
        public Action OnCancel { get; set; }
    }

    /// <summary>
    /// 资源下载器
    /// </summary>
    public interface IASDownloader : IProgressOperation, IDownlandAssetEvent
    {
        /// <summary>
        /// 是否运行继续流程
        /// </summary>
        bool Flow { get; }

        /// <summary>
        /// 更新资源包清单
        /// </summary>
        IEnumerator UpdateHeader();

        /// <summary>
        /// 收集需要下载的所有资源
        /// </summary>
        void CollectNeedAll();

        /// <summary>
        /// 收集需要下载的标签
        /// </summary>
        void CollectNeedTag(params string[] tags);
    }
}