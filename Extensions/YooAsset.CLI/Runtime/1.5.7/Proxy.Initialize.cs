#if SUPPORT_YOOASSET
using System;
using System.Collections.Generic;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    /// <summary>
    ///     资源加载管理器
    ///     该类只提供封装API函数
    /// </summary>
    partial class Proxy
    {
        /// <summary>
        ///     获取内置查询服务
        /// </summary>
        public static event Func<IBuildinQueryServices> EventQueryServices;

        /// <summary>
        ///     获取内置查询服务
        /// </summary>
        public static event Func<IDeliveryQueryServices> EventDeliveryQueryServices;

        /// <summary>
        ///     获取远程查询服务
        /// </summary>
        public static event Func<AssetsPackageConfig, IRemoteServices> EventRemoteServices;

        /// <summary>
        ///     获取参数配置
        /// </summary>
        public static event Func<ResPackage, YAssetParameters> EventParameter;

        private static YAssetParameters GetParameter(ResPackage package)
        {
            YAssetParameters parameter;
            switch (AssetSystem.Parameter.ASMode)
            {
                case EASMode.Remote:
#if UNITY_EDITOR && UNITY_WEBGL
                    parameter = new YAssetHandleEditor(AssetSystem.Parameter);
#else
                    parameter = new YAParametersRemote(AssetSystem.Parameter)
                    {
#if !UNITY_WEBGL
                        DeliveryQueryServices = EventDeliveryQueryServices is null
                            ? new ResolverDeliveryQueryServices()
                            : EventDeliveryQueryServices.Invoke(),
#endif

                        QueryServices = EventQueryServices is null
                            ? new ResolverQueryServices()
                            : EventQueryServices.Invoke(),

                        RemoteServices = EventRemoteServices is null
                            ? new ResolverRemoteServices(package.Config)
                            : EventRemoteServices.Invoke(package.Config)
                    };
#endif
                    break;

                case EASMode.Editor: // 编辑器模式
#if UNITY_EDITOR
                    parameter = new YAParametersEditor(AssetSystem.Parameter);
                    break;
#endif
                case EASMode.Local:
                    parameter = new YAParametersLocal(AssetSystem.Parameter);
                    break;
                default:
                    AssetSystem.ExceptionEvent(ASException.NoSupportEASMode);
                    return null;
            }

            parameter.BuildInRootDirectory = AssetSystem.BuildInRootDirectory;
            parameter.SandboxRootDirectory = AssetSystem.SandboxRootDirectory;
            return parameter;
        }

        private void Initialize_Internal()
        {
            if (IsInitialize) return;

#if UNITY_WEBGL // 此处为了适配WX小游戏，因为WX小游戏不支持WebGL缓存
            YooAssets.SetCacheSystemDisableCacheOnWebGL();
#endif

            YooAssets.Initialize(new YALogger());
            YooAssets.SetOperationSystemMaxTimeSlice(AssetSystem.Parameter.AsyncMaxTimeSlice);
            if (AssetSystem.PackageConfigs is null)
            {
                AssetSystem.LogError("AssetSystem PackageConfigs is null");
                AssetSystem.ExceptionEvent(ASException.ASConfigPackagesIsNull);
                return;
            }

            if (EventParameter is null) EventParameter = GetParameter;
            var capacity                               = AssetSystem.PackageConfigs.Count;
            InitializationOperations = new List<InitializationOperation>(capacity);
            ReferenceOPHandle        = new Dictionary<string, OperationHandleBase>();
            DownloaderOperations     = new Dictionary<string, DownloaderOperation>(64);
            Dic                      = new Dictionary<string, ResPackage>(capacity);

            foreach (var item in AssetSystem.PackageConfigs)
            {
                var package = new ResPackage(item);
                if (package.Config.IsDefault)
                {
                    DefaultPackage     = package;
                    DefaultPackageName = item.Name;
                }

                if (Dic.ContainsKey(item.Name))
                {
                    AssetSystem.LogErrorFormat("Asset Package Name Repeat : {0}", item.Name);
                    continue;
                }

                Dic[item.Name] = package;

                var parameters = EventParameter.Invoke(package);
                if (parameters is null)
                {
                    AssetSystem.ExceptionEvent(ASException.ASConfigPackagesIsNull);
                    AssetSystem.LogException($"AssetSystem {package.Config.Name} Parameter is null");
                }

                var operation = package.InitializeAsync(parameters);
                if (operation is null)
                {
                    AssetSystem.ExceptionEvent(ASException.ASConfigPackagesIsNull);
                    AssetSystem.LogExceptionFormat("{Load} -> {0}", package.Config);
                    continue;
                }

                InitializationOperations.Add(operation);
            }
        }
    }
}
#endif