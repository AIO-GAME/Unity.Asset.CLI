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
        IProgressHandle Progress { get; }
    }

    /// <summary>
    /// 资源下载器
    /// </summary>
    public interface IASDownloader : IASNetLoading, IDisposable
    {
        /// <summary>
        /// 是否运行继续流程
        /// </summary>
        bool Flow { get; }

        // /// <summary>
        // /// 更新资源包版本信息
        // /// </summary>
        // /// <returns>Ture:有新版本 False:无需更新</returns>
        // Task UpdatePackageVersionTask();
        //
        // /// <summary>
        // /// 向网络端请求并更新补丁清单
        // /// </summary>
        // /// <returns>Ture:有新版本 False:无需更新</returns>
        // Task UpdatePackageManifestTask();
        //
        // /// <summary>
        // /// 下载序列列表文件
        // /// </summary>
        // Task DownloadRecordTask(AssetSystem.SequenceRecordQueue queue);
        //
        // /// <summary>
        // /// 开始下载
        // /// </summary>
        // Task DownloadTask();
        //
        // /// <summary>
        // /// 开始标签下载
        // /// </summary>
        // Task DownloadTagTask(string tag);
        //
        // /// <summary>
        // /// 开始标签下载
        // /// </summary>
        // Task DownloadTagTask(IEnumerable<string> tags);

        /// <summary>
        /// 更新资源包版本信息
        /// </summary>
        /// <returns>Ture:有新版本 False:无需更新</returns>
        IEnumerator UpdatePackageVersionCO();

        /// <summary>
        /// 向网络端请求并更新补丁清单
        /// </summary>
        /// <returns>Ture:有新版本 False:无需更新</returns>
        IEnumerator UpdatePackageManifestCO();

        /// <summary>
        /// 收集需要下载的所有资源
        /// </summary>
        void CollectNeedAll();

        /// <summary>
        /// 收集需要下载的标签
        /// </summary>
        void CollectNeedTag(IEnumerable<string> tags);

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