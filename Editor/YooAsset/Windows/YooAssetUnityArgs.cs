#if SUPPORT_YOOASSET
using System;
using System.Collections.Generic;
using System.IO;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor.Build
{
    [Serializable]
    public class YooAssetUnityArgs
    {
        public static readonly int[] Versions = new int[] { 2017, 2018, 2019, 2020, 2021, 2022, 2023 };

        /// <summary>
        /// Unity版本 2018 2019 2020 2021 2022 2023
        /// </summary>
        public int VersionIndex;

        /// <summary>
        /// 工程导出路径
        /// </summary>
        public string OutputRoot;

        public BuildTarget BuildTarget { get; private set; }

        public YooAssetUnityArgs(BuildTarget target)
        {
            BuildTarget = target;
#if UNITY_2017
                VersionIndex = 0;
#elif UNITY_2018
                VersionIndex = 1;
#elif UNITY_2019
            VersionIndex = 2;
#elif UNITY_2020
                VersionIndex = 3;
#elif UNITY_2021
                VersionIndex = 4;
#elif UNITY_2022
                VersionIndex = 5;
#elif UNITY_2023
                VersionIndex = 6;
#endif
        }

        public int GetVersion()
        {
            return Versions.Get(VersionIndex);
        }

        public async void BuiltUpToStreamingAssets(
            BuildTarget buildTarget,
            string packageRoot,
            IDictionary<string, string> packages
        )
        {
            BuildTarget = buildTarget;
            if (string.IsNullOrEmpty(packageRoot))
            {
                Debug.LogError("请设置YooAsset资源包根目录");
                return;
            }

            if (AHelper.IO.ExistsFolder(Path.Combine(packageRoot, buildTarget.ToString())))
            {
                Debug.LogError("请设置YooAsset资源包根目录 -> " + buildTarget);
                return;
            }

            if (packages == null || packages.Count == 0)
            {
                Debug.LogError("请设置YooAsset资源包");
                return;
            }

            var output = Application.streamingAssetsPath.PathCombine(ASConfig.GetOrCreate().RuntimeRootDirectory);
            if (AHelper.IO.ExistsFolder(output)) await PrPlatform.Folder.Del(output).Async();
            AHelper.IO.CreateFolder(output);
            foreach (var package in packages)
            {
                var packagePath = Path.Combine(output, package.Key);
                var packageVersionPathSource = Path.Combine(packageRoot, package.Key, package.Value);
                await PrPlatform.Folder.Symbolic(packagePath, packageVersionPathSource).Async();
            }

            EditorUtility.DisplayDialog("提示", "资源合并完成", "确定");
            await PrPlatform.Open.Path(output).Async();
        }

        public async void BuiltUp(BuildTarget buildTarget, string packageRoot, IDictionary<string, string> packages)
        {
            BuildTarget = buildTarget;
            if (string.IsNullOrEmpty(packageRoot))
            {
                Debug.LogError("请设置YooAsset资源包根目录");
                return;
            }

            if (AHelper.IO.ExistsFolder(Path.Combine(packageRoot, buildTarget.ToString())))
            {
                Debug.LogError("请设置YooAsset资源包根目录 -> " + buildTarget);
                return;
            }

            if (packages == null || packages.Count == 0)
            {
                Debug.LogError("请设置YooAsset资源包");
                return;
            }

            if (string.IsNullOrEmpty(OutputRoot))
            {
                Debug.LogError("请设置工程导出路径");
                return;
            }

            var output = string.Empty;
            switch (buildTarget)
            {
                case BuildTarget.Android:

                    if (GetVersion() >= 2020)
                    {
                        output = Path.Combine(OutputRoot, "unityLibrary", "src", "main", "assets",
                            ASConfig.GetOrCreate().RuntimeRootDirectory);
                    }
                    else
                    {
                        output = Path.Combine(OutputRoot, "src", "main", "assets",
                            ASConfig.GetOrCreate().RuntimeRootDirectory);
                    }

                    break;
                case BuildTarget.iOS:
                    break;
                default:
                    Debug.LogErrorFormat("不支持目标平台资源合并 {0}", buildTarget.ToString());
                    break;
            }

            if (string.IsNullOrEmpty(output)) return;
            if (AHelper.IO.ExistsFolder(output)) await PrPlatform.Folder.Del(output).Async();
            AHelper.IO.CreateFolder(output);
            foreach (var package in packages)
            {
                var packagePath = Path.Combine(output, package.Key);
                var packageVersionPathSource = Path.Combine(packageRoot, package.Key, package.Value);
                await PrPlatform.Folder.Symbolic(packagePath, packageVersionPathSource).Async();
            }

            EditorUtility.DisplayDialog("提示", "资源合并完成", "确定");
            await PrPlatform.Open.Path(output).Async();
        }
    }
}
#endif