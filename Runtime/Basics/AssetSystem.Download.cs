/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.ComponentModel;
using System.Diagnostics;
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
        [Description("网络错误")] NetWorkError,

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
        /// <summary>
        /// 当前下载进度
        /// </summary>
        double Progress { get; }

        /// <summary>
        /// 总下载大小
        /// </summary>
        long TotalDownloadBytes { get; }

        /// <summary>
        /// 当前下载大小
        /// </summary>
        long CurrentDownloadBytes { get; }

        /// <summary>
        /// 总下载数量
        /// </summary>
        int TotalDownloadCount { get; }

        /// <summary>
        /// 当前下载数量
        /// </summary>
        int CurrentDownloadCount { get; }
    }

    /// <summary>
    /// 资源下载器
    /// </summary>
    public interface IASDownloader : IASNetLoading, IDisposable
    {
        /// <summary>
        /// 更新资源包版本信息
        /// </summary>
        /// <returns>Ture:有新版本 False:无需更新</returns>
        Task<bool> UpdatePackageVersionTask();

        /// <summary>
        /// 向网络端请求并更新补丁清单
        /// </summary>
        /// <returns>Ture:有新版本 False:无需更新</returns>
        Task<bool> UpdatePackageManifestTask();

        /// <summary>
        /// 创建下载器
        /// </summary>
        /// <returns>Ture:有新版本 False:无需更新</returns>
        bool CreateDownloader();

        /// <summary>
        /// 开始下载
        /// </summary>
        Task BeginDownload();
    }

    internal class ASDownloaderEmpty : IASDownloader
    {
        public double Progress => 1;
        public long TotalDownloadBytes => 0;
        public long CurrentDownloadBytes => 0;
        public int TotalDownloadCount => 0;
        public int CurrentDownloadCount => 0;
        public Task<bool> UpdatePackageVersionTask() => Task.FromResult(false);
        public Task<bool> UpdatePackageManifestTask() => Task.FromResult(false);
        public bool CreateDownloader() => false;
        public Task BeginDownload() => Task.CompletedTask;

        public void Dispose()
        {
        }
    }

    public partial class AssetSystem
    {
        /// <summary>
        /// 预加载记录
        /// </summary>
        public static async Task DownloadPreRecord(ProgressArgs progressArgs = default)
        {
            if (Parameter.ASMode != EASMode.Remote) return;
            var handle = GetDownloader();
            var flow = await handle.UpdatePackageManifestTask();
            if (flow) flow = await handle.UpdatePackageVersionTask();
            Log($"【资源下载】 {(flow ? "有新版本" : "无需更新")}");
            if (flow) await Proxy.PreRecord(SequenceRecordQueue, progressArgs);
        }

        /// <summary>
        /// 获取下载器
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IASDownloader GetDownloader()
        {
            return Parameter.ASMode != EASMode.Remote
                ? new ASDownloaderEmpty()
                : Proxy.GetDownloader();
        }

        /// <summary>
        /// 预下载全部远端资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async Task DownloadPre()
        {
            if (Parameter.ASMode != EASMode.Remote) return;
            var handle = GetDownloader();
            var flow = await handle.UpdatePackageManifestTask();
            if (flow) flow = await handle.UpdatePackageVersionTask();
            if (flow) flow = handle.CreateDownloader();
            Log($"【资源下载】 {(flow ? "有新版本" : "无需更新")}");
            if (flow) await handle.BeginDownload();
        }

        /// <summary>
        /// 动态下载远端资源
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static async Task DownloadDynamic()
        {
            if (Parameter.ASMode != EASMode.Remote) return;
            var handle = GetDownloader();
            var flow = await handle.UpdatePackageManifestTask();
            if (flow) await handle.UpdatePackageVersionTask();
            Log($"【资源下载】 {(flow ? "有新版本" : "无需更新")}");
        }
    }
}