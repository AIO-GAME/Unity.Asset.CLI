/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using AIO.UEngine.YooAsset;
using YooAsset;

namespace AIO.UEngine
{
    [IgnoreConsoleJump]
    public partial class YAssetProxy : AssetProxy
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
        public static event Func<YAssetPackage, YAssetParameters> EventParameter;

        private static YAssetParameters GetParameter(YAssetPackage package)
        {
            YAssetParameters yAssetFlow;
            switch (AssetSystem.Parameter.ASMode)
            {
                case EASMode.Remote:
#if UNITY_EDITOR && UNITY_WEBGL
                    yAssetFlow = new YAssetHandleEditor(AssetSystem.Parameter);
#else
                    yAssetFlow = new YAParametersRemote(AssetSystem.Parameter)
                    {
#if !UNITY_WEBGL
                        DeliveryQueryServices = EventDeliveryQueryServices == null
                            ? new ResolverDeliveryQueryServices()
                            : EventDeliveryQueryServices.Invoke(),
#endif

                        QueryServices = EventQueryServices == null
                            ? new ResolverQueryServices()
                            : EventQueryServices.Invoke(),

                        RemoteServices = EventRemoteServices is null
                            ? new ResolverRemoteServices(package.Config)
                            : EventRemoteServices?.Invoke(package.Config),
                    };
#endif
                    break;
                case EASMode.Editor: // 编辑器模式
#if UNITY_EDITOR
                    yAssetFlow = new YAssetHandleEditor(AssetSystem.Parameter);
                    break;
#endif
                case EASMode.Local:
                    yAssetFlow = new YAParametersLocal(AssetSystem.Parameter);
                    break;
                default:
                    AssetSystem.ExceptionEvent(AssetSystemException.NoSupportEASMode);
                    return null;
            }

            yAssetFlow.BuildInRootDirectory = AssetSystem.BuildInRootDirectory;
            yAssetFlow.SandboxRootDirectory = AssetSystem.SandboxRootDirectory;
            return yAssetFlow;
        }

        public override IEnumerator Initialize()
        {
            if (EventParameter is null) EventParameter += GetParameter;
            YAssetSystem.GetParameter += EventParameter;
            YAssetSystem.Initialize();
            yield return YAssetSystem.LoadCO();
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

            var list = (from object item in array
                where item is not null
                select new AssetsPackageConfig
                    { Name = fieldInfo.GetValue(item) as string, Version = "-.-.-", IsDefault = false }).ToArray();

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

            var remote = Path.Combine(config.URL, "Version",
                string.Concat(AssetSystem.PlatformNameStr, ".json?t=", DateTime.Now.Ticks));

            var content = string.Empty;
            yield return AssetSystem.NetLoadStringCO(remote, data => { content = data; });
            if (string.IsNullOrEmpty(content))
            {
#if UNITY_EDITOR
                AssetSystem.LogError($"{remote} Request failed");
#endif
                AssetSystem.ExceptionEvent(AssetSystemException.ASConfigRemoteUrlRemoteVersionRequestFailure);
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

            if (config.Packages is null)
            {
                AssetSystem.ExceptionEvent(AssetSystemException.ASConfigPackagesIsNull);
                yield break;
            }


            foreach (var item in config.Packages)
            {
                item.IsLatest = item.Version == "Latest";
                if (!item.IsLatest) continue;
                var temp = Path.Combine(AssetSystem.Parameter.URL,
                    AssetSystem.PlatformNameStr, item.Name, item.Version,
                    $"PackageManifest_{item.Name}.version?t={DateTime.Now.Ticks}");
                yield return AssetSystem.NetLoadStringCO(temp, data =>
                {
                    if (string.IsNullOrEmpty(data))
                    {
                        AssetSystem.ExceptionEvent(AssetSystemException.ASConfigRemoteUrlRemoteVersionRequestFailure);
#if UNITY_EDITOR
                        AssetSystem.LogError($"{temp} Request failed");
#endif
                        return;
                    }

                    item.Version = data;
                });
            }
        }

        /// <summary>
        /// 更新资源包列表
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public override IEnumerator UpdatePackages(ASConfig config)
        {
            switch (config.ASMode)
            {
                case EASMode.Editor:
#if UNITY_EDITOR
                    UpdatePackagesEditor(config);
                    break;
#endif
                case EASMode.Remote:
                    yield return UpdatePackagesRemote(config);
                    break;
                case EASMode.Local:
                    config.Packages = AHelper.IO.ReadJsonUTF8<AssetsPackageConfig[]>(
                        Path.Combine(AssetSystem.BuildInRootDirectory, $"Version/{AssetSystem.PlatformNameStr}.json"));
                    if (config.Packages is null)
                        AssetSystem.ExceptionEvent(AssetSystemException.ASConfigPackagesIsNull);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Dispose()
        {
            YAssetSystem.GetParameter -= EventParameter;
            EventParameter = null;
            YAssetSystem.Destroy();
        }

        public override IASNetLoading GetLoadingHandle()
        {
            return new YAssetSystem.LoadingInfo();
        }

        public override void CleanCache(Action<bool> cb)
        {
            YAssetSystem.ClearCacheResource(cb);
        }
    }
}
#endif