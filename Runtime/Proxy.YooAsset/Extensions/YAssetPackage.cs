#if SUPPORT_YOOASSET
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;
using Object = UnityEngine.Object;

namespace AIO.UEngine.YooAsset
{
    public class YAssetPackage : IDisposable
    {
        /// <summary>
        /// 包配置
        /// </summary>
        public AssetsPackageConfig Config { get; protected set; }

        /// <summary>
        /// 资源包
        /// </summary>
        public ResourcePackage Package { get; protected set; }

        /// <summary>
        /// 资源模式
        /// </summary>
        public EPlayMode Mode { get; protected set; }

        /// <summary>
        /// 包裹名
        /// </summary>
        public string PackageName => Package.PackageName;

        /// <summary>
        /// 初始化状态
        /// </summary>
        public EOperationStatus InitializeStatus => Package.InitializeStatus;

        public YAssetPackage(AssetsPackageConfig config)
        {
            Config = config;
            Package = YooAssets.TryGetPackage(Config.Name) ?? YooAssets.CreatePackage(Config.Name);
            if (Config.IsDefault) YooAssets.SetDefaultPackage(Package);
        }

        public InitializationOperation InitializeAsync(YAssetParameters parameters)
        {
#if UNITY_EDITOR
            Debug.LogFormat($"Assets System Initialize Config -> | Mode -> {parameters.Mode} Start");
#endif

            Mode = parameters.Mode;
            parameters.UpdateParameters();
#if UNITY_EDITOR
            Debug.LogFormat("Assets System {0} is {1}",
                Mode,
                parameters.Parameters is null ? "null" : "not null");
#endif
            switch (Mode)
            {
#if UNITY_EDITOR
                case EPlayMode.EditorSimulateMode:
                {
                    if (parameters.Parameters is EditorSimulateModeParameters simulateModeParameters)
                    {
                        simulateModeParameters.SimulateManifestFilePath =
                            EditorSimulateModeHelper.SimulateBuild(Config.Name);
                        return Package.InitializeAsync(simulateModeParameters);
                    }

                    break;
                }
#endif
                case EPlayMode.HostPlayMode:
                {
                    if (parameters.Parameters is HostPlayModeParameters Parameters)
                    {
                        if (Parameters.RemoteServices is null)
                            throw new Exception($"Assets System {Mode} RemoteServices is null");
                        return Package.InitializeAsync(Parameters);
                    }

                    break;
                }
                case EPlayMode.WebPlayMode:
                {
                    if (parameters.Parameters is WebPlayModeParameters Parameters)
                    {
                        if (Parameters.RemoteServices is null)
                            throw new Exception($"Assets System {Mode} RemoteServices is null");
                        return Package.InitializeAsync(Parameters);
                    }

                    break;
                }
                case EPlayMode.OfflinePlayMode:
                default: break;
            }

            return Package.InitializeAsync(parameters.Parameters);
        }

        public void Dispose()
        {
            Package.ForceUnloadAllAssets();
        }

        /// <summary>
        /// 向网络端请求最新的资源版本
        /// </summary>
        /// <param name="appendTimeTicks">在URL末尾添加时间戳</param>
        /// <param name="timeout">超时时间（默认值：60秒）</param>
        public UpdatePackageVersionOperation UpdatePackageVersionAsync(bool appendTimeTicks = true, int timeout = 60)
        {
            return Package.UpdatePackageVersionAsync(appendTimeTicks, timeout);
        }

        /// <summary>
        /// 向网络端请求并更新清单
        /// </summary>
        /// <param name="packageVersion">更新的包裹版本</param>
        /// <param name="autoSaveVersion">更新成功后自动保存版本号，作为下次初始化的版本。</param>
        /// <param name="timeout">超时时间（默认值：60秒）</param>
        public UpdatePackageManifestOperation UpdatePackageManifestAsync(string packageVersion,
            bool autoSaveVersion = true, int timeout = 60)
        {
            return Package.UpdatePackageManifestAsync(packageVersion, autoSaveVersion, timeout);
        }

        /// <summary>
        /// 预下载指定版本的包裹资源
        /// </summary>
        /// <param name="packageVersion">下载的包裹版本</param>
        /// <param name="timeout">超时时间（默认值：60秒）</param>
        public PreDownloadContentOperation PreDownloadContentAsync(string packageVersion, int timeout = 60)
        {
            return Package.PreDownloadContentAsync(packageVersion, timeout);
        }

        /// <summary>
        /// 清理包裹未使用的缓存文件
        /// </summary>
        public ClearUnusedCacheFilesOperation ClearUnusedCacheFilesAsync()
        {
            return Package.ClearUnusedCacheFilesAsync();
        }

        /// <summary>
        /// 获取本地包裹的版本信息
        /// </summary>
        public string GetPackageVersion()
        {
            return Package?.GetPackageVersion();
        }

        /// <summary>
        /// 资源回收（卸载引用计数为零的资源）
        /// </summary>
        /// <param name="isForce">是否强制回收</param>
        public void UnloadUnusedAssets(bool isForce = false)
        {
            if (isForce) Package.ForceUnloadAllAssets();
            else Package.UnloadUnusedAssets();
        }

        #region 资源信息

        /// <summary>
        /// 是否需要从远端更新下载
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public bool IsNeedDownloadFromRemote(string location)
        {
            return AssetSystem.Parameter.ASMode == EASMode.Remote && Package.IsNeedDownloadFromRemote(location);
        }

        /// <summary>
        /// 是否需要从远端更新下载
        /// </summary>
        /// <param name="assetInfo">资源的定位地址</param>
        public bool IsNeedDownloadFromRemote(AssetInfo assetInfo)
        {
            return AssetSystem.Parameter.ASMode == EASMode.Remote && Package.IsNeedDownloadFromRemote(assetInfo);
        }

        /// <summary>
        /// 获取资源信息列表
        /// </summary>
        /// <param name="tag">资源标签</param>
        public AssetInfo[] GetAssetInfos(string tag)
        {
            return Package.GetAssetInfos(tag);
        }

        /// <summary>
        /// 获取资源信息列表
        /// </summary>
        /// <param name="tags">资源标签列表</param>
        public AssetInfo[] GetAssetInfos(string[] tags)
        {
            return Package.GetAssetInfos(tags);
        }

        /// <summary>
        /// 获取资源信息
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public AssetInfo GetAssetInfo(string location)
        {
            return Package.GetAssetInfo(location);
        }

        /// <summary>
        /// 检查资源定位地址是否有效
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public bool CheckLocationValid(string location)
        {
            return Package.CheckLocationValid(location);
        }

        #endregion

        #region 原生文件

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        public RawFileOperationHandle LoadRawFileSync(AssetInfo assetInfo)
        {
            return Package.LoadRawFileSync(assetInfo);
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public RawFileOperationHandle LoadRawFileSync(string location)
        {
            return Package.LoadRawFileSync(location);
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        public RawFileOperationHandle LoadRawFileAsync(AssetInfo assetInfo)
        {
            return Package.LoadRawFileAsync(assetInfo);
        }

        /// <summary>
        /// 异步加载原生文件
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public RawFileOperationHandle LoadRawFileAsync(string location)
        {
            return Package.LoadRawFileAsync(location);
        }

        #endregion

        #region 场景加载

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="location">场景的定位地址</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        public SceneOperationHandle LoadSceneAsync(
            string location,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = true,
            int priority = 100)
        {
            return Package.LoadSceneAsync(location, sceneMode, suspendLoad, priority);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="assetInfo">场景的资源信息</param>
        /// <param name="sceneMode">场景加载模式</param>
        /// <param name="suspendLoad">场景加载到90%自动挂起</param>
        /// <param name="priority">优先级</param>
        public SceneOperationHandle LoadSceneAsync(
            AssetInfo assetInfo,
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = true,
            int priority = 100)
        {
            return Package.LoadSceneAsync(assetInfo, sceneMode, suspendLoad, priority);
        }

        #endregion

        #region 资源加载

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        public AssetOperationHandle LoadAssetSync(AssetInfo assetInfo)
        {
            return Package.LoadAssetSync(assetInfo);
        }

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public AssetOperationHandle LoadAssetSync<TObject>(string location) where TObject : Object
        {
            return Package.LoadAssetSync<TObject>(location);
        }

        /// <summary>
        /// 同步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        public AssetOperationHandle LoadAssetSync(string location, Type type)
        {
            return Package.LoadAssetSync(location, type);
        }


        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        public AssetOperationHandle LoadAssetAsync(AssetInfo assetInfo)
        {
            return Package.LoadAssetAsync(assetInfo);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public AssetOperationHandle LoadAssetAsync<TObject>(string location) where TObject : Object
        {
            return Package.LoadAssetAsync<TObject>(location);
        }

        /// <summary>
        /// 异步加载资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">资源类型</param>
        public AssetOperationHandle LoadAssetAsync(string location, Type type)
        {
            return Package.LoadAssetAsync(location, type);
        }

        #endregion

        #region 资源加载

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        public SubAssetsOperationHandle LoadSubAssetsSync(AssetInfo assetInfo)
        {
            return Package.LoadSubAssetsSync(assetInfo);
        }

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public SubAssetsOperationHandle LoadSubAssetsSync<TObject>(string location) where TObject : Object
        {
            return Package.LoadSubAssetsSync<TObject>(location);
        }

        /// <summary>
        /// 同步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        public SubAssetsOperationHandle LoadSubAssetsSync(string location, Type type)
        {
            return Package.LoadSubAssetsSync(location, type);
        }


        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="assetInfo">资源信息</param>
        public SubAssetsOperationHandle LoadSubAssetsAsync(AssetInfo assetInfo)
        {
            return Package.LoadSubAssetsAsync(assetInfo);
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <typeparam name="TObject">资源类型</typeparam>
        /// <param name="location">资源的定位地址</param>
        public SubAssetsOperationHandle LoadSubAssetsAsync<TObject>(string location) where TObject : Object
        {
            return Package.LoadSubAssetsAsync<TObject>(location);
        }

        /// <summary>
        /// 异步加载子资源对象
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        /// <param name="type">子对象类型</param>
        public SubAssetsOperationHandle LoadSubAssetsAsync(string location, Type type)
        {
            return Package.LoadSubAssetsAsync(location, type);
        }

        #endregion

        #region 资源下载

        /// <summary>
        /// 创建资源下载器，用于下载当前资源版本所有的资源包文件
        /// </summary>
        /// <param name="tag">资源标签</param>
        /// <param name="downloadingMaxNumber">同时下载的最大文件数</param>
        /// <param name="failedTryAgain">下载失败的重试次数</param>
        /// <param name="timeout">超时时间</param>
        public ResourceDownloaderOperation CreateResourceDownloader(string tag, int downloadingMaxNumber,
            int failedTryAgain, int timeout = 60)
        {
            return Package.CreateResourceDownloader(new string[] { tag }, downloadingMaxNumber, failedTryAgain,
                timeout);
        }

        /// <summary>
        /// 创建资源下载器，用于下载当前资源版本所有的资源包文件
        /// </summary>
        /// <param name="tags">资源标签列表</param>
        /// <param name="downloadingMaxNumber">同时下载的最大文件数</param>
        /// <param name="failedTryAgain">下载失败的重试次数</param>
        /// <param name="timeout">超时时间</param>
        public ResourceDownloaderOperation CreateResourceDownloader(string[] tags, int downloadingMaxNumber,
            int failedTryAgain, int timeout = 60)
        {
            return Package.CreateResourceDownloader(tags, downloadingMaxNumber, failedTryAgain, timeout);
        }

        /// <summary>
        /// 创建资源下载器，用于下载当前资源版本所有的资源包文件
        /// </summary>
        /// <param name="downloadingMaxNumber">同时下载的最大文件数</param>
        /// <param name="failedTryAgain">下载失败的重试次数</param>
        /// <param name="timeout">超时时间</param>
        public ResourceDownloaderOperation CreateResourceDownloader(int downloadingMaxNumber, int failedTryAgain,
            int timeout = 60)
        {
            return Package.CreateResourceDownloader(downloadingMaxNumber, failedTryAgain, timeout);
        }

        /// <summary>
        /// 创建资源下载器，用于下载指定的资源列表依赖的资源包文件
        /// </summary>
        /// <param name="assetInfos">资源信息列表</param>
        /// <param name="downloadingMaxNumber">同时下载的最大文件数</param>
        /// <param name="failedTryAgain">下载失败的重试次数</param>
        /// <param name="timeout">超时时间</param>
        public ResourceDownloaderOperation CreateBundleDownloader(AssetInfo[] assetInfos, int downloadingMaxNumber,
            int failedTryAgain, int timeout = 60)
        {
            return Package.CreateBundleDownloader(assetInfos, downloadingMaxNumber, failedTryAgain, timeout);
        }

        /// <summary>
        /// 创建资源下载器，用于下载指定的资源列表依赖的资源包文件
        /// </summary>
        /// <param name="assetInfos">资源信息列表</param>
        /// <param name="downloadingMaxNumber">同时下载的最大文件数</param>
        /// <param name="failedTryAgain">下载失败的重试次数</param>
        /// <param name="timeout">超时时间</param>
        public ResourceDownloaderOperation CreateBundleDownloader(AssetInfo assetInfos, int downloadingMaxNumber,
            int failedTryAgain, int timeout = 60)
        {
            return Package.CreateBundleDownloader(new AssetInfo[] { assetInfos }, downloadingMaxNumber, failedTryAgain,
                timeout);
        }

        #endregion

        #region 资源解压

        /// <summary>
        /// 创建内置资源解压器
        /// </summary>
        /// <param name="tag">资源标签</param>
        /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
        /// <param name="failedTryAgain">解压失败的重试次数</param>
        public ResourceUnpackerOperation CreateResourceUnpacker(string tag, int unpackingMaxNumber, int failedTryAgain)
        {
            return Package.CreateResourceUnpacker(new string[] { tag }, unpackingMaxNumber, failedTryAgain);
        }

        /// <summary>
        /// 创建内置资源解压器
        /// </summary>
        /// <param name="tags">资源标签列表</param>
        /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
        /// <param name="failedTryAgain">解压失败的重试次数</param>
        public ResourceUnpackerOperation CreateResourceUnpacker(string[] tags, int unpackingMaxNumber,
            int failedTryAgain)
        {
            return Package.CreateResourceUnpacker(tags, unpackingMaxNumber, failedTryAgain);
        }

        /// <summary>
        /// 创建内置资源解压器
        /// </summary>
        /// <param name="tags">资源标签列表</param>
        /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
        /// <param name="failedTryAgain">解压失败的重试次数</param>
        public ResourceUnpackerOperation CreateResourceUnpacker(IEnumerable<string> tags, int unpackingMaxNumber,
            int failedTryAgain)
        {
            return Package.CreateResourceUnpacker(tags.ToArray(), unpackingMaxNumber, failedTryAgain);
        }

        /// <summary>
        /// 创建内置资源解压器
        /// </summary>
        /// <param name="unpackingMaxNumber">同时解压的最大文件数</param>
        /// <param name="failedTryAgain">解压失败的重试次数</param>
        public ResourceUnpackerOperation CreateResourceUnpacker(int unpackingMaxNumber, int failedTryAgain)
        {
            return Package.CreateResourceUnpacker(unpackingMaxNumber, failedTryAgain);
        }

        #endregion
    }
}
#endif