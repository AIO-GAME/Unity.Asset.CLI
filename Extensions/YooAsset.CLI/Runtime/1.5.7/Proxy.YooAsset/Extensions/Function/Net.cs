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
        public static ResourceDownloaderOperation CreateResourceDownloaderDefault(string tag)
        {
            return DefaultPackage.CreateResourceDownloader(new string[] { tag });
        }

        /// <summary>
        /// 创建资源下载器，用于下载当前资源版本所有的资源包文件
        /// </summary>
        /// <param name="tags">资源标签列表</param>
        public static ResourceDownloaderOperation CreateResourceDownloaderDefault(string[] tags)
        {
            return DefaultPackage.CreateResourceDownloader(tags);
        }

        /// <summary>
        /// 创建资源下载器，用于下载当前资源版本所有的资源包文件
        /// </summary>
        public static ResourceDownloaderOperation CreateResourceDownloaderDefault()
        {
            return DefaultPackage.CreateResourceDownloader();
        }

        #endregion

        #region Downloader Bundle

        /// <summary>
        /// 创建资源下载器，用于下载指定的资源列表依赖的资源包文件
        /// </summary>
        /// <param name="assetInfos">资源信息列表</param>
        public static ResourceDownloaderOperation CreateBundleDownloaderDefault(AssetInfo[] assetInfos)
        {
            return DefaultPackage.CreateBundleDownloader(assetInfos);
        }

        #endregion

        #region Update

        /// <summary>
        /// 向网络端请求最新的资源版本
        /// </summary>
        public static UpdatePackageVersionOperation UpdatePackageVersionDefaultAsync()
        {
            return DefaultPackage.UpdatePackageVersionAsync();
        }

        /// <summary>
        /// 向网络端请求并更新补丁清单
        /// </summary>
        /// <param name="packageVersion">更新的包裹版本</param>
        public static UpdatePackageManifestOperation UpdatePackageManifestDefaultAsync(string packageVersion)
        {
            return DefaultPackage.UpdatePackageManifestAsync(packageVersion);
        }

        #endregion

        #region Pre Download

        /// <summary>
        /// 预下载指定版本的包裹资源
        /// </summary>
        /// <param name="packageVersion">下载的包裹版本</param>
        public static PreDownloadContentOperation PreDownloadContentDefaultAsync(string packageVersion)
        {
            return DefaultPackage.PreDownloadContentAsync(packageVersion);
        }

        #endregion

        #region Downloader Patch

        /// <summary>
        /// 创建资源下载器，用于下载当前资源版本所有的资源包文件
        /// </summary>
        /// <param name="package">包名</param>
        /// <param name="tag">资源标签</param>
        public static ResourceDownloaderOperation CreateResourceDownloader(string package, string tag)
        {
            return !Dic.TryGetValue(package, out var asset)
                ? null
                : asset.CreateResourceDownloader(new string[] { tag });
        }

        /// <summary>
        /// 创建资源下载器，用于下载当前资源版本所有的资源包文件
        /// </summary>
        /// <param name="tags">资源标签列表</param>
        /// <param name="package">包名</param>
        public static ResourceDownloaderOperation CreateResourceDownloader(string package, string[] tags)
        {
            return !Dic.TryGetValue(package, out var asset)
                ? null
                : asset.CreateResourceDownloader(tags);
        }

        /// <summary>
        /// 创建资源下载器，用于下载当前资源版本所有的资源包文件
        /// </summary>
        /// <param name="package">包名</param>
        public static ResourceDownloaderOperation CreateResourceDownloader(string package)
        {
            return !Dic.TryGetValue(package, out var asset)
                ? null
                : asset.CreateResourceDownloader();
        }

        #endregion

        #region Downloader Bundle

        /// <summary>
        /// 创建补丁下载器，用于下载更新指定的资源列表依赖的资源包文件
        /// </summary>
        /// <param name="package">包名</param>
        /// <param name="assetInfos">资源信息列表</param>
        public static ResourceDownloaderOperation CreateBundleDownloader(string package,
            AssetInfo[] assetInfos)
        {
            return !Dic.TryGetValue(package, out var asset) ? null : asset.CreateBundleDownloader(assetInfos);
        }

        #endregion

        #region Update

        /// <summary>
        /// 向网络端请求最新的资源版本
        /// </summary>
        /// <param name="package">包名</param>
        public static UpdatePackageVersionOperation UpdatePackageVersionAsync(string package)
        {
            return !Dic.TryGetValue(package, out var asset) ? null : asset.UpdatePackageVersionAsync();
        }

        /// <summary>
        /// 向网络端请求并更新补丁清单
        /// </summary>
        /// <param name="package">包名</param>
        /// <param name="packageVersion">更新的包裹版本</param>
        public static UpdatePackageManifestOperation UpdatePackageManifestAsync(string package, string packageVersion)
        {
            return !Dic.TryGetValue(package, out var asset) ? null : asset.UpdatePackageManifestAsync(packageVersion);
        }

        #endregion

        #region Pre Download

        /// <summary>
        /// 预下载指定版本的包裹资源
        /// </summary>
        /// <param name="package">包名</param>
        /// <param name="packageVersion">下载的包裹版本</param>
        public static PreDownloadContentOperation PreDownloadContentAsync(string package, string packageVersion)
        {
            return !Dic.TryGetValue(package, out var asset) ? null : asset.PreDownloadContentAsync(packageVersion);
        }

        #endregion
    }
}
#endif