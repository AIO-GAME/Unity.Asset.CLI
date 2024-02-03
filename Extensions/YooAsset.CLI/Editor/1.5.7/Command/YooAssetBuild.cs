#if SUPPORT_YOOASSET

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using YooAsset;
using YooAsset.Editor;

namespace AIO.UEditor.CLI
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
            var buildArgs = Argument.ResolverCustom<AssetBuildCommand>(args);

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
                buildArgs.CompressOption = ECompressMode.LZ4;

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

        private static IEncryptionServices CreateEncryptionServicesInstance(string EncryptionClassName)
        {
            if (string.IsNullOrEmpty(EncryptionClassName)) return new EncryptionNone();
            return (from item in EditorTools.GetAssignableTypes(typeof(IEncryptionServices))
                where item.FullName == EncryptionClassName
                select (IEncryptionServices)Activator.CreateInstance(item)).FirstOrDefault();
        }

        public static void ArtBuild(AssetBuildCommand command)
        {
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
                SharedPackRule = new ZeroRedundancySharedPackRule(),
                CopyBuildinFileTags = command.CopyBuildinFileTags,
                VerifyBuildingResult = command.VerifyBuildingResult,
                PackageVersion = command.PackageVersion,
                BuildOutputRoot = command.OutputRoot,
                StreamingAssetsRoot = Path.Combine(
                    Application.streamingAssetsPath,
                    ASConfig.GetOrCreate().RuntimeRootDirectory),
                DisableWriteTypeTree = false
            };
            switch (command.OutputNameStyle)
            {
                case EOutputNameStyle.HashName:
                    buildParameters.OutputNameStyle = YooAsset.Editor.EOutputNameStyle.HashName;
                    break;
                case EOutputNameStyle.BundleName_HashName:
                    buildParameters.OutputNameStyle = YooAsset.Editor.EOutputNameStyle.BundleName_HashName;
                    break;
            }

            switch (command.CompressOption)
            {
                case ECompressMode.None:
                    buildParameters.CompressOption = ECompressOption.Uncompressed;
                    break;
                case ECompressMode.LZMA:
                    buildParameters.CompressOption = ECompressOption.LZMA;
                    break;
                case ECompressMode.LZ4:
                    buildParameters.CompressOption = ECompressOption.LZ4;
                    break;
            }

            buildParameters.CopyBuildinFileTags = ASConfig.GetOrCreate().EnableSequenceRecord
                ? string.Concat(buildParameters.CopyBuildinFileTags, ";SequenceRecord").Trim(';')
                : buildParameters.CopyBuildinFileTags;

            buildParameters.CopyBuildinFileOption = !string.IsNullOrEmpty(buildParameters.CopyBuildinFileTags)
                ? YooAsset.Editor.ECopyBuildinFileOption.ClearAndCopyByTags
                : YooAsset.Editor.ECopyBuildinFileOption.None;

            buildParameters.EncryptionServices = CreateEncryptionServicesInstance(command.EncyptionClassName);

            if (command.BuildPipeline == EBuildPipeline.ScriptableBuildPipeline)
            {
                buildParameters.SBPParameters = new BuildParameters.SBPBuildParameters
                {
                    WriteLinkXML = true
                };
            }

            Debug.Log(AHelper.Json.Serialize(buildParameters));

            var builder = new AssetBundleBuilder();
            var buildResult = builder.Run(buildParameters);
            if (buildResult.Success)
            {
                var output = Path.Combine(
                    buildParameters.BuildOutputRoot,
                    buildParameters.BuildTarget.ToString(),
                    buildParameters.PackageName);

                if (command.MergeToLatest)
                {
                    MergeToLatest(output, buildParameters.PackageVersion);
                }
                else ManifestGenerate(Path.Combine(output, buildParameters.PackageVersion));

                if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null) Debug.Log("构建资源成功");
                else if (!EHelper.IsCMD()) EditorUtility.RevealInFinder(buildResult.OutputPackageDirectory);

                AssetProxyEditor.CreateConfig(buildParameters.BuildOutputRoot, command.MergeToLatest);
            }
            else
            {
                if (EHelper.IsCMD()) Debug.LogError($"构建失败 {buildResult.ErrorInfo}");
                else EditorUtility.DisplayDialog("构建失败", buildResult.ErrorInfo, "确定");
            }
        }

        private const string Manifest = "Manifest.json";

        /// <summary>
        /// 生成清单文件
        /// </summary>
        /// <param name="dir">目标路径</param>
        /// <param name="isAgain">重新生成</param>
        private static void ManifestGenerate(string dir, bool isAgain = false)
        {
            var manifestPath = Path.Combine(dir, Manifest);
            if (File.Exists(manifestPath) && !isAgain)
            {
                Debug.LogWarning($"Manifest 文件已存在 : {dir} 文件夹不需要重新生成");
                return;
            }

            var hashtable = AHelper.IO.GetFilesRelative(dir, "*.*", SearchOption.AllDirectories)
                .Where(filePath => filePath != Manifest)
                .ToDictionary(filePath => filePath, filePath => AHelper.IO.GetFileMD5(Path.Combine(dir, filePath)));

            AHelper.IO.WriteJson(manifestPath, hashtable.Sort());
        }

        /// <summary>
        /// 对比清单文件
        /// </summary>
        /// <param name="current">当前清单</param>
        /// <param name="target">对比清单</param>
        /// <returns>
        /// [Item1 : 新增文件列表]
        /// [Item2 : 删除文件列表]
        /// [Item3 : 修改文件列表]
        /// </returns>
        public static Tuple<
                IDictionary<string, string>,
                IDictionary<string, string>,
                IDictionary<string, string>>
            ComparisonManifest(Dictionary<string, string> current, Dictionary<string, string> target)
        {
            var delete = new Dictionary<string, string>(); // 删除
            var change = new Dictionary<string, string>(); // 修改

            var add = current
                .Where(item => !target.ContainsKey(item.Key))
                .ToDictionary(item => item.Key.ToString(), item => item.Value.ToString()); // 新增

            foreach (var item in target) // 遍历最新版本清单
            {
                if (!current.ContainsKey(item.Key)) // 如果当前清单文件中不存在 则代表需要删除目标版本指定文件
                {
                    delete.Add(item.Key, item.Value);
                    continue;
                }

                if (current[item.Key] != item.Value) // 修改
                {
                    change.Add(item.Key, item.Value);
                }
            }

            return Tuple.Create<
                IDictionary<string, string>,
                IDictionary<string, string>,
                IDictionary<string, string>
            >(add, delete, change);
        }

        /// <summary>
        /// 对比清单文件
        /// </summary>
        /// <param name="currentPath">当前清单文件夹</param>
        /// <param name="latestPath">最新清单文件夹</param>
        /// <returns>
        /// [Item1 : 新增文件列表]
        /// [Item2 : 删除文件列表]
        /// [Item3 : 修改文件列表]
        /// </returns>
        public static Tuple<
                IDictionary<string, string>,
                IDictionary<string, string>,
                IDictionary<string, string>>
            ComparisonManifest(string currentPath, string latestPath)
        {
            var current = AHelper.IO.ReadJson<Dictionary<string, string>>(Path.Combine(currentPath, Manifest));
            var latest = AHelper.IO.ReadJson<Dictionary<string, string>>(Path.Combine(latestPath, Manifest));
            return ComparisonManifest(current, latest);
        }

        private static void MergeToLatestExe(string currentPath, string latestPath, string latestManifestPath)
        {
            var tuple = ComparisonManifest(currentPath, latestPath);
            var latest = AHelper.IO.ReadJson<Dictionary<string, string>>(latestManifestPath);
            foreach (var pair in tuple.Item1) // 新增
            {
                latest[pair.Key] = pair.Value;
                var source = Path.Combine(currentPath, pair.Key);
                if (File.Exists(source))
                {
                    var target = Path.Combine(latestPath, pair.Key);
                    Console.WriteLine($"新增文件 : {target}");
                    File.Copy(source, target, true);
                }
                else
                {
                    if (EHelper.IsCMD()) Debug.LogError($"新增文件不存在 : {source} 目标源结构被篡改 请重新构建资源");
                    else EditorUtility.DisplayDialog("Error", $"新增文件不存在 : {source} 目标源结构被篡改 请重新构建资源", "确定");
                    return;
                }
            }

            foreach (var pair in tuple.Item2) // 删除
            {
                latest.Remove(pair.Key);
                var target = Path.Combine(latestPath, pair.Key);
                if (!File.Exists(target)) continue;
                Console.WriteLine($"删除文件 : {target}");
                File.Delete(target);
            }

            foreach (var pair in tuple.Item3) // 修改
            {
                latest[pair.Key] = pair.Value;
                var source = Path.Combine(currentPath, pair.Key);
                if (File.Exists(source))
                {
                    var target = Path.Combine(latestPath, pair.Key);
                    File.Copy(source, target, true);
                }
                else
                {
                    if (EHelper.IsCMD()) Debug.LogError($"新增文件不存在 : {source} 目标源结构被篡改 请重新构建资源");
                    else EditorUtility.DisplayDialog("Error", $"新增文件不存在 : {source} 目标源结构被篡改 请重新构建资源", "确定");
                    return;
                }
            }

            AHelper.IO.WriteJson(latestManifestPath, latest.Sort());
        }

        /// <summary>
        /// 合并到最新版本
        /// </summary>
        /// <param name="rootPath">根目录</param>
        /// <param name="version">版本号</param>
        private static void MergeToLatest(string rootPath, string version)
        {
            var currentPath = Path.Combine(rootPath, version);
            ManifestGenerate(currentPath);

            var latestPath = Path.Combine(rootPath, "Latest");
            if (!Directory.Exists(latestPath)) // 如果不存在 则将当前版本资源全部复制至 Latest
            {
                Directory.CreateDirectory(latestPath);
                AHelper.IO.CopyDirAll(currentPath, latestPath);
                return;
            }

            var latestManifestPath = Path.Combine(latestPath, Manifest);
            if (!File.Exists(latestManifestPath))
            {
                if (EHelper.IsCMD()) Debug.LogError($"最新版本清单文件不存在 : {latestManifestPath} 请请重新构建资源");
                else EditorUtility.DisplayDialog("Error", $"最新版本清单文件不存在 : {latestManifestPath} 请请重新构建资源", "确定");
                return;
            }

            MergeToLatestExe(currentPath, latestPath, latestManifestPath);
        }
    }
}
#endif