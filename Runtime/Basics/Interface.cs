/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-15
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace AIO
{
    /// <summary>
    /// 网络资源加载进度
    /// </summary>
    public interface IASNetLoading
    {
        IProgressInfo Progress { get; }
    }

    public struct DownlandAssetEvent : IProgressEvent
    {
        /// <summary>
        /// 网络不可用
        /// </summary>
        public Action<IProgressReport> OnNetReachableNot { get; set; }

        /// <summary>
        /// 移动网络 是否允许在移动网络条件下下载
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

        public Action<IProgressInfo> OnProgress { get; set; }
        public Action<IProgressReport> OnComplete { get; set; }
        public Action OnBegin { get; set; }
        public Action<Exception> OnError { get; set; }
        public Action OnResume { get; set; }
        public Action OnPause { get; set; }
        public Action OnCancel { get; set; }
    }

    /// <summary>
    /// 资源下载器
    /// </summary>
    public interface IASDownloader : IProgressOperation
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

        /// <summary>
        /// 收集需要下载的序列
        /// </summary>
        void CollectNeedRecord(AssetSystem.SequenceRecordQueue queue);

        /// <summary>
        /// 等待下载完成
        /// </summary>
        IEnumerator WaitCo();
    }
}