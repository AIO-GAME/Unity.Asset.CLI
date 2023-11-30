﻿/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AIO.UEngine;

namespace AIO
{
    /// <summary>
    /// 资源事件类型
    /// </summary>
    public enum EASEventType
    {
        /// <summary>
        /// 网络错误
        /// </summary>
        [Description("网络错误")] NetWorkError = 0,

        /// <summary>
        /// 找不到指定文件
        /// </summary>
        [Description("找不到指定文件")] NotFoundFile,

        /// <summary>
        /// 磁盘空间不足
        /// </summary>
        [Description("磁盘空间不足")] OutOfDiskSpace,

        /// <summary>
        /// 资源包下载失败
        /// </summary>
        [Description("资源包下载失败")] DownlandPackageFailure,

        /// <summary>
        /// 资源包下载成功
        /// </summary>
        [Description("资源包下载成功")] DownlandPackageSuccess,

        /// <summary>
        /// 指定文件下载失败
        /// </summary>
        [Description("指定文件下载失败")] DownlandFileFailure,

        /// <summary>
        /// 请求最新的资源版本
        /// </summary>
        [Description("请求最新的资源版本")] UpdatePackageVersion,

        /// <summary>
        /// 请求最新的资源清单
        /// </summary>
        [Description("请求最新的资源清单")] UpdatePackageManifest,

        /// <summary>
        /// 开始下载资源包
        /// </summary>
        [Description("开始下载资源包")] BeginDownload,

        /// <summary>
        /// 下载进度
        /// </summary>
        [Description("下载进度")] HotUpdateDownloadFinish,
    }

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

        /// <summary>
        /// 更新资源包版本信息
        /// </summary>
        /// <returns>Ture:有新版本 False:无需更新</returns>
        Task UpdatePackageVersionTask();

        /// <summary>
        /// 向网络端请求并更新补丁清单
        /// </summary>
        /// <returns>Ture:有新版本 False:无需更新</returns>
        Task UpdatePackageManifestTask();

        /// <summary>
        /// 下载序列列表文件
        /// </summary>
        Task DownloadRecordTask(AssetSystem.SequenceRecordQueue queue);

        /// <summary>
        /// 开始下载
        /// </summary>
        Task DownloadTask();

        /// <summary>
        /// 开始标签下载
        /// </summary>
        Task DownloadTagTask(string tag);

        /// <summary>
        /// 开始标签下载
        /// </summary>
        Task DownloadTagTask(IEnumerable<string> tags);

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
        /// 开始下载
        /// </summary>
        IEnumerator DownloadCO();

        /// <summary>
        /// 开始标签下载
        /// </summary>
        IEnumerator DownloadTagCO(string tag);

        /// <summary>
        /// 开始标签下载
        /// </summary>
        IEnumerator DownloadTagCO(IEnumerable<string> tags);

        /// <summary>
        /// 下载序列列表文件
        /// </summary>
        IEnumerator DownloadRecordCO(AssetSystem.SequenceRecordQueue queue);
    }

    internal struct ASDownloaderEmpty : IASDownloader
    {
        public IProgressHandle Progress { get; }

        public bool Flow => false;

        public ASDownloaderEmpty(IProgressEvent iEvent = null)
        {
            Progress = new AProgress(iEvent);
        }

        public Task UpdatePackageVersionTask() => Task.CompletedTask;
        public Task UpdatePackageManifestTask() => Task.CompletedTask;
        public Task DownloadRecordTask(AssetSystem.SequenceRecordQueue queue) => Task.CompletedTask;
        public Task DownloadTask() => Task.CompletedTask;
        public Task DownloadTagTask(string tag) => Task.CompletedTask;

        public Task DownloadTagTask(IEnumerable<string> tags) => Task.CompletedTask;

        public Task DownloadRecordTask() => Task.CompletedTask;

        public IEnumerator UpdatePackageVersionCO()
        {
            yield break;
        }

        public IEnumerator UpdatePackageManifestCO()
        {
            yield break;
        }

        public IEnumerator DownloadCO()
        {
            yield break;
        }

        public IEnumerator DownloadTagCO(string tag)
        {
            yield break;
        }

        public IEnumerator DownloadTagCO(IEnumerable<string> tags)
        {
            yield break;
        }

        public IEnumerator DownloadRecordCO(AssetSystem.SequenceRecordQueue queue)
        {
            yield break;
        }

        public void Dispose()
        {
        }
    }

    public partial class AssetSystem
    {
        private static IASDownloader Downloader;

        /// <summary>
        /// 获取下载器
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IASDownloader GetDownloader(IProgressEvent progress = null)
        {
            return Parameter.ASMode != EASMode.Remote
                ? new ASDownloaderEmpty(progress)
                : Proxy.GetDownloader(progress);
        }

        /// <summary>
        /// 预下载指定标签资源
        /// </summary>
        public static IEnumerator DownloadPreTag(string tag, IProgressEvent progress = null)
        {
            if (Parameter.ASMode != EASMode.Remote) yield break;
            var handle = GetDownloader(progress);
            yield return handle.UpdatePackageManifestCO();
            yield return handle.UpdatePackageVersionCO();
            Log($"【资源下载】 {(handle.Flow ? "有新版本" : "无需更新")}");
            if (!handle.Flow) yield break;
            yield return handle.DownloadTagTask(tag);
            WhiteListLocal.AddRange(GetAssetInfos(tag));
            Log($"【资源下载】 标签资源列表 下载完成");
        }

        /// <summary>
        /// 预下载指定标签资源
        /// </summary>
        public static IEnumerator DownloadPreTag(string tag1, string tag2, IProgressEvent progress = null)
        {
            if (Parameter.ASMode != EASMode.Remote) yield break;
            yield return DownloadPreTag(new[] { tag1, tag2 }, progress);
            WhiteListLocal.AddRange(GetAssetInfos(tag1));
            WhiteListLocal.AddRange(GetAssetInfos(tag2));
            Log($"【资源下载】 标签资源列表 下载完成");
        }

        /// <summary>
        /// 预下载指定标签资源
        /// </summary>
        public static IEnumerator DownloadPreTag(IEnumerable<string> tag, IProgressEvent progress = null)
        {
            if (Parameter.ASMode != EASMode.Remote) yield break;
            var handle = GetDownloader(progress);
            yield return handle.UpdatePackageManifestCO();
            yield return handle.UpdatePackageVersionCO();
            Log($"【资源下载】 {(handle.Flow ? "有新版本" : "无需更新")}");
            if (!handle.Flow) yield break;
            var enumerable = tag as string[] ?? tag.ToArray();
            yield return handle.DownloadTagTask(enumerable);
            WhiteListLocal.AddRange(GetAssetInfos(enumerable));
            Log($"【资源下载】 标签资源列表 下载完成");
        }

        /// <summary>
        /// 预下载记录序列资源
        /// </summary>
        public static IEnumerator DownloadPreRecord(IProgressEvent progress = null)
        {
            if (Parameter.ASMode != EASMode.Remote) yield break;
            var handle = GetDownloader(progress);
            yield return handle.UpdatePackageManifestCO();
            yield return handle.UpdatePackageVersionCO();
            Log($"【资源下载】 {(handle.Flow ? "有新版本" : "无需更新")}");
            if (!handle.Flow) yield break;
            yield return handle.DownloadRecordCO(SequenceRecords);
            WhiteListLocal.AddRange(SequenceRecords.Select(x => x.Location));
            Log($"【资源下载】 序列列表 预下载完成");
        }

        /// <summary>
        /// 预下载全部远端资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator DownloadPre(IProgressEvent progress = null)
        {
            if (Parameter.ASMode != EASMode.Remote) yield break;
            var handle = GetDownloader(progress);
            yield return handle.UpdatePackageManifestCO();
            yield return handle.UpdatePackageVersionCO();
            Log($"【资源下载】 {(handle.Flow ? "有新版本" : "无需更新")}");
            if (!handle.Flow) yield break;
            yield return handle.DownloadCO();
            Log($"【资源下载】 全部资源列表 预下载完成");
        }

        /// <summary>
        /// 动态下载远端资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator DownloadDynamic(IProgressEvent progress = null)
        {
            if (Parameter.ASMode != EASMode.Remote) yield break;
            var handle = GetDownloader(progress);
            yield return handle.UpdatePackageManifestCO();
            yield return handle.UpdatePackageVersionCO();
            Log($"【资源下载】 {(handle.Flow ? "有新版本" : "无需更新")}");
        }
    }
}