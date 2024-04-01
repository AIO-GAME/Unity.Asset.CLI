using System.ComponentModel;

namespace AIO.UEngine
{
    /// <summary>
    ///     资源事件类型
    /// </summary>
    public enum EASEventType
    {
        /// <summary>
        ///     网络错误
        /// </summary>
        [Description("网络错误")] NetWorkError = 0,

        /// <summary>
        ///     找不到指定文件
        /// </summary>
        [Description("找不到指定文件")] NotFoundFile,

        /// <summary>
        ///     磁盘空间不足
        /// </summary>
        [Description("磁盘空间不足")] OutOfDiskSpace,

        /// <summary>
        ///     资源包下载失败
        /// </summary>
        [Description("资源包下载失败")] DownlandPackageFailure,

        /// <summary>
        ///     资源包下载成功
        /// </summary>
        [Description("资源包下载成功")] DownlandPackageSuccess,

        /// <summary>
        ///     指定文件下载失败
        /// </summary>
        [Description("指定文件下载失败")] DownlandFileFailure,

        /// <summary>
        ///     请求最新的资源版本
        /// </summary>
        [Description("请求最新的资源版本")] UpdatePackageVersion,

        /// <summary>
        ///     请求最新的资源清单
        /// </summary>
        [Description("请求最新的资源清单")] UpdatePackageManifest,

        /// <summary>
        ///     开始下载资源包
        /// </summary>
        [Description("开始下载资源包")] BeginDownload,

        /// <summary>
        ///     下载进度
        /// </summary>
        [Description("下载进度")] HotUpdateDownloadFinish
    }
}