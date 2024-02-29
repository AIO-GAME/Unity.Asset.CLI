#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    /// <summary>
    /// 资源加载管理器
    /// 该类只提供封装API函数
    /// </summary>
    partial class Proxy
    {
        /// <summary>
        /// 获取内置查询服务
        /// </summary>
        public static event Func<IBuildinQueryServices> EventQueryServices;

        /// <summary>
        /// 获取内置查询服务
        /// </summary>
        public static event Func<IDeliveryQueryServices> EventDeliveryQueryServices;

        /// <summary>
        /// 获取远程查询服务
        /// </summary>
        public static event Func<AssetsPackageConfig, IRemoteServices> EventRemoteServices;

        /// <summary>
        /// 获取参数配置
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
                            : EventRemoteServices.Invoke(package.Config),
                    };
#endif
                    break;
                case EASMode.Local:
                    parameter = new YAParametersLocal(AssetSystem.Parameter);
                    break;
                case EASMode.Editor: // 编辑器模式
#if UNITY_EDITOR
                    parameter = new YAParametersEditor(AssetSystem.Parameter);
                    break;
#endif
                default:
                    AssetSystem.ExceptionEvent(AssetSystemException.NoSupportEASMode);
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
                AssetSystem.ExceptionEvent(AssetSystemException.ASConfigPackagesIsNull);
                return;
            }

            if (EventParameter is null) EventParameter = GetParameter;
            var capacity = AssetSystem.PackageConfigs.Count;
            ReferenceOPHandle = new Dictionary<string, OperationHandleBase>();
            InitializationOperations = new List<InitializationOperation>(capacity);
            DownloaderOperations = new Dictionary<string, DownloaderOperation>(64);
            Dic = new Dictionary<string, ResPackage>(capacity);

            foreach (var item in AssetSystem.PackageConfigs)
            {
                var package = new ResPackage(item);
                if (package.Config.IsDefault)
                {
                    DefaultPackage = package;
                    DefaultPackageName = item.Name;
                }

                if (Dic.ContainsKey(item.Name))
                {
                    AssetSystem.LogError("Asset Package Name Repeat : {0}", item.Name);
                    continue;
                }

                Dic[item.Name] = package;

                var parameters = EventParameter.Invoke(package);
                if (parameters is null)
                {
                    AssetSystem.ExceptionEvent(AssetSystemException.ASConfigPackagesIsNull);
                    AssetSystem.LogException($"AssetSystem {package.Config.Name} Parameter is null");
                }

                var operation = package.InitializeAsync(parameters);
                if (operation is null)
                {
                    AssetSystem.ExceptionEvent(AssetSystemException.ASConfigPackagesIsNull);
                    AssetSystem.LogException("{Load} -> {0}", package.Config);
                    continue;
                }

                InitializationOperations.Add(operation);
            }
        }

        /// <summary>
        /// 更新资源包列表
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public override IEnumerator UpdatePackagesCO(ASConfig config)
        {
            switch (config.ASMode)
            {
                case EASMode.Local:
                    UpdatePackagesLocal(config);
                    break;
                case EASMode.Remote:
                    yield return UpdatePackagesRemote(config);
                    break;
                case EASMode.Editor:
#if UNITY_EDITOR
                    UpdatePackagesEditor(config);
                    break;
#endif
                default:
                    AssetSystem.ExceptionEvent(AssetSystemException.NoSupportEASMode);
                    break;
            }
        }

        private static void UpdatePackagesLocal(ASConfig config)
        {
            config.Packages = AHelper.IO.ReadJsonUTF8<AssetsPackageConfig[]>(
                $"{AssetSystem.BuildInRootDirectory}/Version/{AssetSystem.PlatformNameStr}.json");
            if (config.Packages is null)
                AssetSystem.ExceptionEvent(AssetSystemException.ASConfigPackagesIsNull);
        }

        [Conditional("UNITY_EDITOR")]
        private static void UpdatePackagesEditor(ASConfig config)
        {
            var assembly = Assembly.Load("AIO.Asset.Editor");
            var type = assembly.GetType("AIO.UEditor.AssetCollectRoot", true);
            var getOrCreate = type.GetMethod("GetOrCreate", BindingFlags.Static | BindingFlags.Public);
            var CollectRoot = getOrCreate?.Invoke(null, new object[] { });
            if (CollectRoot is null)
            {
                AssetSystem.ExceptionEvent(AssetSystemException.NoFoundAssetCollectRoot);
                return;
            }

            var packages = type.GetField("Packages", BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(CollectRoot);
            if (!(packages is Array array))
            {
                AssetSystem.ExceptionEvent(AssetSystemException.ASConfigPackagesIsNull);
                return;
            }

            var fieldInfo = assembly
                .GetType("AIO.UEditor.AssetCollectPackage", true)
                .GetField("Name", BindingFlags.Instance | BindingFlags.Public);
            if (fieldInfo is null)
            {
                AssetSystem.ExceptionEvent(AssetSystemException.ASConfigPackagesIsNull);
                return;
            }

            var list = (
                from object item in array
                where item != null
                select new AssetsPackageConfig
                {
                    Name = fieldInfo.GetValue(item) as string,
                    Version = "-.-.-",
                }).ToArray();

            if (list.Length <= 0)
            {
                AssetSystem.ExceptionEvent(AssetSystemException.ASConfigPackagesIsNull);
                return;
            }

            config.Packages = list;
            config.Packages[0].IsDefault = true;
        }

        private static IEnumerator UpdatePackagesRemote(ASConfig config)
        {
            if (string.IsNullOrEmpty(config.URL))
            {
                AssetSystem.ExceptionEvent(AssetSystemException.ASConfigRemoteUrlIsNull);
                yield break;
            }

            var remote = $"{config.URL}/Version/{AssetSystem.PlatformNameStr}.json?t={DateTime.Now.Ticks}";
            var content = string.Empty;
            yield return AssetSystem.NetLoadStringCO(remote, data => content = data);
            if (string.IsNullOrEmpty(content))
            {
                AssetSystem.ExceptionEvent(AssetSystemException.ASConfigRemoteUrlRemoteVersionRequestFailure);
                AssetSystem.LogError($"{remote} Request failed");
                yield break;
            }

            try
            {
                config.Packages = AHelper.Json.Deserialize<AssetsPackageConfig[]>(content);
            }
            catch (Exception)
            {
                AssetSystem.ExceptionEvent(AssetSystemException.ASConfigRemoteUrlRemoteVersionParsingJsonFailure);
                yield break;
            }

            if (config.Packages is null || config.Packages.Length == 0)
            {
                AssetSystem.ExceptionEvent(AssetSystemException.ASConfigPackagesIsNull);
                yield break;
            }

            foreach (var item in config.Packages)
            {
                item.IsLatest = item.Version == "Latest"; // 如果使用Latest则认为是最新版本 同时需要获取最新版本号
                if (!item.IsLatest) continue;
                var url = string.Format("{0}/{1}/{2}/{3}/PackageManifest_{4}.version?t={5}",
                    AssetSystem.Parameter.URL,
                    AssetSystem.PlatformNameStr,
                    item.Name,
                    item.Version,
                    item.Name,
                    DateTime.Now.Ticks);
                yield return AssetSystem.NetLoadStringCO(url, data =>
                {
                    if (string.IsNullOrEmpty(data))
                    {
                        AssetSystem.ExceptionEvent(AssetSystemException.ASConfigRemoteUrlRemoteVersionRequestFailure);
                        AssetSystem.LogError($"{url} Request failed");
                        return;
                    }

                    item.Version = data;
                });
            }
        }
    }
}
#endif