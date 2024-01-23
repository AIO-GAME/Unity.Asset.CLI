using System.ComponentModel;

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

    public partial class AssetSystem
    {
        /// <summary>
        /// 资源管理系统 版本号
        /// </summary>
        private const string ASSET_SYSTEM_VERSION = "1.0.0";

        /// <summary>
        /// 资源管理系统 名称
        /// </summary>
        private const string ASSET_SYSTEM_NAME = nameof(AssetSystem);

        /// <summary>
        /// 资源管理系统 错误异常
        /// </summary>
        private const string ERROR = ASSET_SYSTEM_NAME + " Error : ";

        /// <summary>
        /// 资源管理系统 未知错误异常
        /// </summary>
        private const string ERROR_UNKNOWN = ASSET_SYSTEM_NAME + "Error Unknown  : ";

        /// <summary>
        /// 资源管理系统 错误异常
        /// </summary>
        private const string ERROR_NET = ASSET_SYSTEM_NAME + " Error Net : ";

        /// <summary>
        /// 资源管理系统 未知错误异常
        /// </summary>
        private const string ERROR_NET_UNKNOWN = ASSET_SYSTEM_NAME + "Error Unknown Net : ";
    }
}