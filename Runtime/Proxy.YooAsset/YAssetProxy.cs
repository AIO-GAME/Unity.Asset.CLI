/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.IO;
using AIO.UEngine.YooAsset;
using UnityEditor;
using UnityEngine;
using YooAsset;

namespace AIO.UEngine
{
    public partial class YAssetProxy : AssetProxy
    {
        /// <summary>
        /// 获取内置查询服务
        /// </summary>
        public static event Func<IQueryServices> EventQueryServices;

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
            var BuildInRootDirectory = Path.Combine(Application.streamingAssetsPath, "BuildinFiles");
            var SandboxRootDirectory =
#if UNITY_EDITOR
                string.Concat(Directory.GetParent(Application.dataPath)?.FullName,
                    Path.DirectorySeparatorChar, "Sandbox", Path.DirectorySeparatorChar,
                    EditorUserBuildSettings.activeBuildTarget.ToString());
#else
                Path.Combine(Application.persistentDataPath, "BuildinFiles");
#endif

            YAssetParameters yAssetFlow;
            switch (AssetSystem.Parameter.ASMode)
            {
                case EASMode.Remote:
                    var QueryServices = EventQueryServices == null
                        ? new ResolverQueryServices()
                        : EventQueryServices.Invoke();

                    var RemoteServices = EventRemoteServices is null
                        ? new ResolverRemoteServices(package.Config)
                        : EventRemoteServices?.Invoke(package.Config);

#if UNITY_WEBGL
                    yAssetFlow = new YAParametersWebGLMode
#else
                    yAssetFlow = new YAssetParametersHostPlayMode
#endif
                    {
                        QueryServices = QueryServices,
                        RemoteServices = RemoteServices,
                        BuildInRootDirectory = BuildInRootDirectory,
                        SandboxRootDirectory = SandboxRootDirectory,
                    };
                    break;
                case EASMode.Editor: // 编辑器模式
#if UNITY_EDITOR
                    yAssetFlow = new YAssetHandleEditor();
                    break;
#endif
                case EASMode.Local:
                    yAssetFlow = new YAssetParametersOfflinePlayMode
                    {
                        BuildInRootDirectory = BuildInRootDirectory,
                        SandboxRootDirectory = SandboxRootDirectory,
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
    }
}
#endif