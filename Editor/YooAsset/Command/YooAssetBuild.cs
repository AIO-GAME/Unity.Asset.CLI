#if SUPPORT_YOOASSET

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;

namespace AIO.UEditor
{
    /// <summary>
    ///  Yoo Asset
    /// </summary>
    public static class YooAssetBuild
    {
        public static void ArtBuild()
        {
            var cmd = Argument.ResolverCustomCur<UnityArgsCommand>();
            var args = cmd.executeMethod;
            var buildArgs = Argument.ResolverCustom<YooAssetBuildCommand>(args);

            if (string.IsNullOrEmpty(buildArgs.BuildPackage)) throw new ArgumentNullException($"构建包: 名称不能为 NULL");
            if (string.IsNullOrEmpty(buildArgs.PackageVersion)) throw new ArgumentNullException($"构建包: 版本不能为 NULL");

            if (string.IsNullOrEmpty(buildArgs.OutputRoot))
                buildArgs.OutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();

            if (Enum.IsDefined(typeof(BuildTarget), buildArgs.ActiveTarget) ||
                buildArgs.ActiveTarget == BuildTarget.NoTarget)
                buildArgs.ActiveTarget = EditorUserBuildSettings.activeBuildTarget;

            if (Enum.IsDefined(typeof(EOutputNameStyle), buildArgs.OutputNameStyle))
                buildArgs.OutputNameStyle = EOutputNameStyle.BundleName_HashName;

            if (Enum.IsDefined(typeof(ECopyBuildinFileOption), buildArgs.CopyBuildinFileOption))
                buildArgs.CopyBuildinFileOption = ECopyBuildinFileOption.None;

            if (Enum.IsDefined(typeof(ECompressOption), buildArgs.CompressOption))
                buildArgs.CompressOption = ECompressOption.LZ4;

            if (Enum.IsDefined(typeof(EBuildPipeline), buildArgs.BuildPipeline))
                buildArgs.BuildPipeline =
#if UNITY_2021_3_OR_NEW
                    EBuildPipeline.ScriptableBuildPipeline;
#else
                    EBuildPipeline.BuiltinBuildPipeline;
#endif

            if (Enum.IsDefined(typeof(EBuildMode), buildArgs.BuildMode))
                buildArgs.BuildMode = EBuildMode.ForceRebuild;


            ArtBuild(buildArgs);
        }

        /// <summary>
        /// 修改配置
        /// </summary>
        public static void ChangeSetting()
        {
        }

        /// <summary>
        /// 构建版本相关
        /// </summary>
        /// <returns></returns>
        private static string GetBuildPackageVersion()
        {
            var totalMinutes = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
            return DateTime.Now.ToString("yyyy-MM-dd") + "-" + totalMinutes;
        }

        private static IEncryptionServices CreateEncryptionServicesInstance(string EncryptionClassName)
        {
            if (string.IsNullOrEmpty(EncryptionClassName)) return null;
            return (from item in EditorTools.GetAssignableTypes(typeof(IEncryptionServices))
                where item.FullName == EncryptionClassName
                select (IEncryptionServices)Activator.CreateInstance(item)).FirstOrDefault();
        }

        public static void ArtBuild(YooAssetBuildCommand command)
        {
            Debug.Log(AHelper.Json.Serialize(command));
            YooAsset.Editor.EBuildPipeline buildPipeline;
            switch (command.BuildPipeline)
            {
                default:
                case EBuildPipeline.BuiltinBuildPipeline:
                    buildPipeline = YooAsset.Editor.EBuildPipeline.BuiltinBuildPipeline;
                    break;
                case EBuildPipeline.ScriptableBuildPipeline:
                    buildPipeline = YooAsset.Editor.EBuildPipeline.ScriptableBuildPipeline;
                    break;
            }

            YooAsset.Editor.EBuildMode buildMode;
            switch (command.BuildMode)
            {
                default:
                case EBuildMode.ForceRebuild:
                    buildMode = YooAsset.Editor.EBuildMode.ForceRebuild;
                    break;
                case EBuildMode.IncrementalBuild:
                    var target = Path.Combine(
                        command.OutputRoot,
                        command.ActiveTarget.ToString(),
                        command.BuildPackage);
                    if (Directory.Exists(target))
                    {
                        var dirs = Directory.GetDirectories(target)
                            .Where(directory => !directory.EndsWith("Simulate") && !directory.EndsWith("OutputCache"))
                            .ToArray();
                        if (dirs.Length > 0)
                        {
                            // 如果为增量更新 则判断是否需要清理缓存 
                            buildMode = YooAsset.Editor.EBuildMode.IncrementalBuild;
                            var cleanCacheNum = ASBuildConfig.GetOrCreate().AutoCleanCacheNum;
                            if (dirs.Length >= cleanCacheNum)
                            {
                                var caches = dirs.SortQuick((s, t) =>
                                {
                                    // 如果缓存数量大于等于设置的缓存数量 则清理缓存 缓存清理机制为删除最早的缓存
                                    var st = Directory.GetCreationTimeUtc(s);
                                    var tt = Directory.GetCreationTimeUtc(t);
                                    var result = tt.CompareTo(st);
                                    if (result == 0) // 如果时间相同 则比较名称
                                    {
                                        result = string.Compare(
                                            Path.GetFileName(s),
                                            Path.GetFileName(t),
                                            StringComparison.CurrentCulture);
                                    }

                                    return result;
                                });

                                for (var index = 0; index < dirs.Length - cleanCacheNum + 1; index++)
                                {
                                    Directory.Delete(caches[index], true);
                                }
                            }
                        }
                        else buildMode = YooAsset.Editor.EBuildMode.ForceRebuild;
                    }
                    else buildMode = YooAsset.Editor.EBuildMode.ForceRebuild;

                    break;
                case EBuildMode.DryRunBuild:
                    buildMode = YooAsset.Editor.EBuildMode.DryRunBuild;
                    break;
                case EBuildMode.SimulateBuild:
                    buildMode = YooAsset.Editor.EBuildMode.SimulateBuild;
                    command.PackageVersion = "Simulate";
                    break;
            }

            var buildParameters = new BuildParameters
            {
                BuildTarget = command.ActiveTarget,
                BuildPipeline = buildPipeline,
                BuildMode = buildMode,
                PackageName = command.BuildPackage,
                CompressOption = command.CompressOption,
                OutputNameStyle = command.OutputNameStyle,
                SharedPackRule = new ZeroRedundancySharedPackRule(),
                CopyBuildinFileOption = command.CopyBuildinFileOption,
                CopyBuildinFileTags = command.CopyBuildinFileTags,
                VerifyBuildingResult = command.VerifyBuildingResult,
                PackageVersion = command.PackageVersion,
                BuildOutputRoot = command.OutputRoot,
                StreamingAssetsRoot = Application.streamingAssetsPath,
                DisableWriteTypeTree = false
            };

            if (string.IsNullOrEmpty(command.EncyptionClassName))
                buildParameters.EncryptionServices = CreateEncryptionServicesInstance(command.EncyptionClassName);

            if (command.BuildPipeline == EBuildPipeline.ScriptableBuildPipeline)
            {
                buildParameters.SBPParameters = new BuildParameters.SBPBuildParameters
                {
                    WriteLinkXML = true
                };
            }

            var builder = new AssetBundleBuilder();
            var buildResult = builder.Run(buildParameters);
            if (buildResult.Success)
            {
                EditorUtility.RevealInFinder(buildResult.OutputPackageDirectory);
                MenuItem_YooAssets.CreateConfig(buildParameters.BuildOutputRoot);
            }
            else
            {
                EditorUtility.DisplayDialog("构建失败", buildResult.ErrorInfo, "确定");
            }
        }
    }
}
#endif