/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System;
using System.Collections;
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
                    yAssetFlow = new YAssetParametersRemote(AssetSystem.Parameter)
                    {
                        BuildInRootDirectory = AssetSystem.BuildInRootDirectory,
                        SandboxRootDirectory = AssetSystem.SandboxRootDirectory,
                        DeliveryQueryServices = EventDeliveryQueryServices == null
                            ? new ResolverDeliveryQueryServices()
                            : EventDeliveryQueryServices.Invoke(),

                        QueryServices = EventQueryServices == null
                            ? new ResolverQueryServices()
                            : EventQueryServices.Invoke(),

                        RemoteServices = EventRemoteServices is null
                            ? new ResolverRemoteServices(package.Config)
                            : EventRemoteServices?.Invoke(package.Config),
                    };
                    break;
                case EASMode.Editor: // 编辑器模式
#if UNITY_EDITOR
                    yAssetFlow = new YAssetHandleEditor(AssetSystem.Parameter);
                    break;
#endif
                case EASMode.Local:
                    yAssetFlow = new YAssetParametersLocal(AssetSystem.Parameter)
                    {
                        BuildInRootDirectory = AssetSystem.BuildInRootDirectory,
                        SandboxRootDirectory = AssetSystem.SandboxRootDirectory,
                    };
                    break;
                default:
                    throw new Exception("enable hot update is not support");
            }

            if (yAssetFlow is null) throw new Exception("asset parameters is null");
            return yAssetFlow;
        }

        public override IEnumerator Initialize()
        {
            if (EventParameter is null) EventParameter += GetParameter;
            YAssetSystem.GetParameter += EventParameter;
            YAssetSystem.Initialize();
            yield return YAssetSystem.LoadCO();
        }

        public override void Dispose()
        {
            YAssetSystem.GetParameter -= EventParameter;
            EventParameter = null;
            YAssetSystem.Destroy();
        }

        public override void CleanCache(Action<bool> cb)
        {
            YAssetSystem.ClearCacheResource(cb);
        }
    }
}
#endif