#if SUPPORT_YOOASSET
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    internal partial class YAssetSystem
    {
        #region Downloader Patch

        /// <summary>
        /// 创建资源下载器，用于下载当前资源版本所有的资源包文件
        /// </summary>
        /// <param name="tag">资源标签</param>
        /// <param name="downloadingMaxNumber">同时下载的最大文件数</param>
        /// <param name="failedTryAgain">下载失败的重试次数</param>
        public static ResourceDownloaderOperation CreateResourceDownloaderDefault(string tag, int downloadingMaxNumber,
            int failedTryAgain)
        {
            return DefaultPackage.CreateResourceDownloader(new string[] { tag }, downloadingMaxNumber, failedTryAgain);
        }

        /// <summary>
        /// 创建资源下载器，用于下载当前资源版本所有的资源包文件
        /// </summary>
        /// <param name="tags">资源标签列表</param>
        /// <param name="downloadingMaxNumber">同时下载的最大文件数</param>
        /// <param name="failedTryAgain">下载失败的重试次数</param>
        public static ResourceDownloaderOperation CreateResourceDownloaderDefault(string[] tags,
            int downloadingMaxNumber, int failedTryAgain)
        {
            return DefaultPackage.CreateResourceDownloader(tags, downloadingMaxNumber, failedTryAgain);
        }

        /// <summary>
        /// 创建资源下载器，用于下载当前资源版本所有的资源包文件
        /// </summary>
        /// <param name="downloadingMaxNumber">同时下载的最大文件数</param>
        /// <param name="failedTryAgain">下载失败的重试次数</param>
        public static ResourceDownloaderOperation CreateResourceDownloaderDefault(int downloadingMaxNumber,
            int failedTryAgain)
        {
            return DefaultPackage.CreateResourceDownloader(downloadingMaxNumber, failedTryAgain);
        }

        #endregion

        #region Downloader Bundle

        /// <summary>
        /// 创建资源下载器，用于下载指定的资源列表依赖的资源包文件
        /// </summary>
        /// <param name="assetInfos">资源信息列表</param>
        /// <param name="downloadingMaxNumber">同时下载的最大文件数</param>
        /// <param name="failedTryAgain">下载失败的重试次数</param>
        public static ResourceDownloaderOperation CreateBundleDownloaderDefault(AssetInfo[] assetInfos,
            int downloadingMaxNumber, int failedTryAgain)
        {
            return DefaultPackage.CreateBundleDownloader(assetInfos, downloadingMaxNumber, failedTryAgain);
        }

        #endregion

        #region Update

        /// <summary>
        /// 向网络端请求最新的资源版本
        /// </summary>
        /// <param name="appendTimeTicks">在URL末尾添加时间戳</param>
        /// <param name="timeout">超时时间（默认值：60秒）</param>
        public static UpdatePackageVersionOperation UpdatePackageVersionDefaultAsync(bool appendTimeTicks = true,
            int timeout = 60)
        {
            return DefaultPackage.UpdatePackageVersionAsync(appendTimeTicks, timeout);
        }

        /// <summary>
        /// 向网络端请求并更新补丁清单
        /// </summary>
        /// <param name="packageVersion">更新的包裹版本</param>
        /// <param name="autoSaveVersion">自动激活清单</param>
        /// <param name="timeout">超时时间（默认值：60秒）</param>
        public static UpdatePackageManifestOperation UpdatePackageManifestDefaultAsync(string packageVersion,
            bool autoSaveVersion = true, int timeout = 60)
        {
            return DefaultPackage.UpdatePackageManifestAsync(packageVersion, autoSaveVersion, timeout);
        }

        #endregion

        #region Pre Download

        /// <summary>
        /// 预下载指定版本的包裹资源
        /// </summary>
        /// <param name="packageVersion">下载的包裹版本</param>
        /// <param name="timeout">超时时间（默认值：60秒）</param>
        public static PreDownloadContentOperation PreDownloadContentDefaultAsync(string packageVersion,
            int timeout = 60)
        {
            return DefaultPackage.PreDownloadContentAsync(packageVersion, timeout);
        }

        #endregion

        #region Downloader Patch

        /// <summary>
        /// 创建资源下载器，用于下载当前资源版本所有的资源包文件
        /// </summary>
        /// <param name="package">包名</param>
        /// <param name="tag">资源标签</param>
        /// <param name="downloadingMaxNumber">同时下载的最大文件数</param>
        /// <param name="failedTryAgain">下载失败的重试次数</param>
        /// <param name="timeout">超时时间</param>
        public static ResourceDownloaderOperation CreateResourceDownloader(string package, string tag,
            int downloadingMaxNumber, int failedTryAgain, int timeout = 60)
        {
            if (!Dic.TryGetValue(package, out var asset)) return null;
            return asset.CreateResourceDownloader(new string[] { tag }, downloadingMaxNumber, failedTryAgain, timeout);
        }

        /// <summary>
        /// 创建资源下载器，用于下载当前资源版本所有的资源包文件
        /// </summary>
        /// <param name="tags">资源标签列表</param>
        /// <param name="downloadingMaxNumber">同时下载的最大文件数</param>
        /// <param name="failedTryAgain">下载失败的重试次数</param>
        /// <param name="package">包名</param>
        /// <param name="timeout">超时时间</param>
        public static ResourceDownloaderOperation CreateResourceDownloader(string package, string[] tags,
            int downloadingMaxNumber, int failedTryAgain, int timeout = 60)
        {
            if (!Dic.TryGetValue(package, out var asset)) return null;
            return asset.CreateResourceDownloader(tags, downloadingMaxNumber, failedTryAgain, timeout);
        }

        /// <summary>
        /// 创建资源下载器，用于下载当前资源版本所有的资源包文件
        /// </summary>
        /// <param name="downloadingMaxNumber">同时下载的最大文件数</param>
        /// <param name="failedTryAgain">下载失败的重试次数</param>
        /// <param name="package">包名</param>
        /// <param name="timeout">超时时间</param>
        public static ResourceDownloaderOperation CreateResourceDownloader(string package, int downloadingMaxNumber,
            int failedTryAgain, int timeout = 60)
        {
            if (!Dic.TryGetValue(package, out var asset)) return null;
            return asset.CreateResourceDownloader(downloadingMaxNumber, failedTryAgain, timeout);
        }

        #endregion

        #region Downloader Bundle

        /// <summary>
        /// 创建补丁下载器，用于下载更新指定的资源列表依赖的资源包文件
        /// </summary>
        /// <param name="package">包名</param>
        /// <param name="assetInfos">资源信息列表</param>
        /// <param name="downloadingMaxNumber">同时下载的最大文件数</param>
        /// <param name="failedTryAgain">下载失败的重试次数</param>
        public static ResourceDownloaderOperation CreateBundleDownloader(string package,
            AssetInfo[] assetInfos, int downloadingMaxNumber, int failedTryAgain)
        {
            if (!Dic.TryGetValue(package, out var asset)) return null;
            return asset.CreateBundleDownloader(assetInfos, downloadingMaxNumber, failedTryAgain);
        }

        #endregion

        #region Update

        /// <summary>
        /// 向网络端请求最新的资源版本
        /// </summary>
        /// <param name="package">包名</param>
        /// <param name="appendTimeTicks">在URL末尾添加时间戳</param>
        /// <param name="timeout">超时时间（默认值：60秒）</param>
        public static UpdatePackageVersionOperation UpdatePackageVersionAsync(string package,
            bool appendTimeTicks = true, int timeout = 60)
        {
            if (!Dic.TryGetValue(package, out var asset)) return null;
            return asset.UpdatePackageVersionAsync(appendTimeTicks, timeout);
        }

        /// <summary>
        /// 向网络端请求并更新补丁清单
        /// </summary>
        /// <param name="package">包名</param>
        /// <param name="packageVersion">更新的包裹版本</param>
        /// <param name="autoSaveVersion">自动激活清单</param>
        /// <param name="timeout">超时时间（默认值：60秒）</param>
        public static UpdatePackageManifestOperation UpdatePackageManifestAsync(string package, string packageVersion,
            bool autoSaveVersion = true, int timeout = 60)
        {
            if (!Dic.TryGetValue(package, out var asset)) return null;
            return asset.UpdatePackageManifestAsync(packageVersion, autoSaveVersion, timeout);
        }

        #endregion

        #region Pre Download

        /// <summary>
        /// 预下载指定版本的包裹资源
        /// </summary>
        /// <param name="package">包名</param>
        /// <param name="packageVersion">下载的包裹版本</param>
        /// <param name="timeout">超时时间（默认值：60秒）</param>
        public static PreDownloadContentOperation PreDownloadContentAsync(string package, string packageVersion,
            int timeout = 60)
        {
            if (!Dic.TryGetValue(package, out var asset)) return null;
            return asset.PreDownloadContentAsync(packageVersion, timeout);
        }

        #endregion
    }
}
#endif