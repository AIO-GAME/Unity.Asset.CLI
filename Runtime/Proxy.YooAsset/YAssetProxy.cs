/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AIO.UEngine.YooAsset;
using UnityEditor;
using UnityEngine;
using YooAsset;

namespace AIO.UEngine
{
    public partial class YAssetProxy : AssetProxy
    {
        public static event Func<ICollection<AssetsPackageConfig>> EventPackages;

        public static event Func<IQueryServices> GetQueryServices;

        public static event Func<AssetsPackageConfig, IRemoteServices> GetRemoteServices;

        public static event Func<YAssetPackage, YAssetParameters> EventParameter;

        private static YAssetParameters GetParameter(YAssetPackage package)
        {
            YAssetParameters yAssetFlow;
            switch (AssetSystem.Parameter.ASMode)
            {
                case EASMode.Remote:
#if UNITY_WEBGL
                    yAssetFlow = new YAParametersWebGLMode
                    {
                        QueryServices =
 GetQueryServices == null ? new ResolverQueryServices() : GetQueryServices.Invoke(),
                        RemoteServices = GetRemoteServices?.Invoke(package.Config),
                        BuildinRootDirectory = Path.Combine(Application.streamingAssetsPath, "BuildinFiles"),
#if UNITY_EDITOR
                        SandboxRootDirectory = Application.dataPath.Replace("Assets", "Sandbox"),
#else
                        SandboxRootDirectory = Path.Combine(Application.persistentDataPath, "BuildinFiles"),
#endif
                    };
#else
                    yAssetFlow = new YAssetParametersHostPlayMode
                    {
                        QueryServices = GetQueryServices == null
                            ? new ResolverQueryServices()
                            : GetQueryServices.Invoke(),
                        RemoteServices = GetRemoteServices?.Invoke(package.Config),
                        BuildinRootDirectory = Path.Combine(Application.streamingAssetsPath, "BuildinFiles"),
#if UNITY_EDITOR
                        SandboxRootDirectory = Path.Combine(Application.dataPath.Replace("Assets", "Sandbox"),
                            EditorUserBuildSettings.activeBuildTarget.ToString()),
#else
                        SandboxRootDirectory = Path.Combine(Application.persistentDataPath, "BuildinFiles"),
#endif
                    };
#endif
                    break;
                case EASMode.Editor: // 编辑器模式
#if UNITY_EDITOR
                    yAssetFlow = new YAssetHandleEditor();
                    break;
#endif
                case EASMode.Local:
                    yAssetFlow = new YAssetParametersOfflinePlayMode
                    {
                        BuildinRootDirectory = Path.Combine(Application.streamingAssetsPath, "BuildinFiles"),
#if UNITY_EDITOR
                        SandboxRootDirectory = Path.Combine(Application.dataPath.Replace("Assets", "Sandbox"),
                            EditorUserBuildSettings.activeBuildTarget.ToString()),
#else
                        SandboxRootDirectory = Path.Combine(Application.persistentDataPath, "BuildinFiles"),
#endif
                    };
                    break;
                default:
                    throw new Exception("EnableHotUpdate is not support");
            }

            if (yAssetFlow is null) throw new Exception("Asset Parameters is null");
            return yAssetFlow;
        }

        public override IEnumerator Initialize()
        {
            EventParameter += GetParameter;
            YAssetSystem.GetParameter += EventParameter;
            YAssetSystem.GetPackages += EventPackages;
            YAssetSystem.Initialize();
            yield return YAssetSystem.LoadCO();
        }

        public override void Dispose()
        {
            EventParameter -= GetParameter;
            YAssetSystem.GetPackages -= EventPackages;
            YAssetSystem.GetParameter -= EventParameter;
            YAssetSystem.Destroy();
        }
    }
}
#endif