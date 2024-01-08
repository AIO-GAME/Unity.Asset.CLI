/*|============|*|
|*|Author:     |*| xinan
|*|Date:       |*| 2024-01-07
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/

#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AIO.UEngine;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

namespace AIO.UEditor.CLI
{
    internal class AssetProxyEditor_Yoo_157 : IAssetProxyEditor
    {
        public string Version => Ghost.YooAsset.Version;
        public string Scopes => Ghost.YooAsset.Scopes;
        public string Name => Ghost.YooAsset.Name;

        public void CreateConfig(string bundlesDir)
        {
            CreateConfig157(bundlesDir);
        }

        public void ConvertConfig(AssetCollectRoot config)
        {
            ConvertYooAsset.Convert(config);
        }

        public void BuildArt(AssetBuildCommand command)
        {
            YooAssetBuild.ArtBuild(command);
        }

        public void BuildArt(ASBuildConfig config)
        {
            var buildCommand = new AssetBuildCommand
            {
                PackageVersion = config.BuildVersion,
                BuildPackage = config.PackageName,
                CompressOption = config.CompressedMode,
                ActiveTarget = config.BuildTarget,
                BuildPipeline = config.BuildPipeline,
                OutputRoot = config.BuildOutputPath,
                BuildMode = config.BuildMode,
                CopyBuildinFileTags = config.FirstPackTag
            };
            YooAssetBuild.ArtBuild(buildCommand);
        }

        [MenuItem("YooAsset/Create Config")]
        public static void CreateConfig()
        {
            CreateConfig157(Path.Combine(EHelper.Path.Project, "Bundles"));
        }

        /// <summary>
        /// 创建配置
        /// </summary>
        /// <param name="BundlesDir"></param>
        private static void CreateConfig157(string BundlesDir)
        {
            if (!Directory.Exists(BundlesDir))
            {
                Debug.LogWarningFormat("Bundles 目录不存在 : 无需创建配置文件");
                return;
            }

            var BundlesConfigDir = Path.Combine(BundlesDir, "Version");
            if (!Directory.Exists(BundlesConfigDir)) Directory.CreateDirectory(BundlesConfigDir);

            var BundlesInfo = new DirectoryInfo(BundlesDir);
            var Versions = new List<DirectoryInfo>();
            var TableDic = new Dictionary<BuildTarget, Dictionary<string, AssetsPackageConfig>>();

            foreach (var PlatformInfo in BundlesInfo.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                if (PlatformInfo.Name.StartsWith("Version")) continue;
                switch (PlatformInfo.Name)
                {
                    case nameof(BuildTarget.Android):
                    case nameof(BuildTarget.WebGL):
                    case nameof(BuildTarget.iOS):
                    case nameof(BuildTarget.StandaloneWindows):
                    case nameof(BuildTarget.StandaloneWindows64):
                    case nameof(BuildTarget.StandaloneOSX):
                        break;
                    default: continue;
                }


                foreach (var PackageInfo in new DirectoryInfo(PlatformInfo.FullName).GetDirectories("*",
                             SearchOption.TopDirectoryOnly))
                {
                    Versions.Clear();
                    Versions.AddRange(PackageInfo.GetDirectories("*", SearchOption.TopDirectoryOnly)
                        .Where(VersionInfo => !VersionInfo.Name.EndsWith("OutputCache"))
                        .Where(VersionInfo => !VersionInfo.Name.EndsWith("Simulate")));

                    if (Versions.Count <= 0) continue;
                    if (Enum.TryParse<BuildTarget>(PlatformInfo.Name, out var enums))
                    {
                        if (!TableDic.ContainsKey(enums))
                            TableDic[enums] = new Dictionary<string, AssetsPackageConfig>();
                        if (!TableDic[enums].ContainsKey(PackageInfo.Name))
                            TableDic[enums].Add(PackageInfo.Name, new AssetsPackageConfig());

                        var last = AHelper.IO.GetLastWriteTimeUtc(Versions);
                        TableDic[enums][PackageInfo.Name].Version = last.Name;
                        TableDic[enums][PackageInfo.Name].Name = PackageInfo.Name;
                        var table = AHelper.IO.ReadJson<Hashtable>(Path.Combine(last.FullName,
                            $"BuildReport_{PackageInfo.Name}_{last.Name}.json"));
                        if (table is null) continue;

                        switch (table.Cast<DictionaryEntry>()
                                    .FirstOrDefault(entry => entry.Key.ToString() == "Summary").Value?
                                    .To<JObject>()?
                                    .Value<int>("CompressOption"))
                        {
                            case (int)ECompressOption.LZMA:
                                TableDic[enums][PackageInfo.Name].CompressMode = ECompressMode.LZMA;
                                break;
                            case (int)ECompressOption.LZ4:
                                TableDic[enums][PackageInfo.Name].CompressMode = ECompressMode.LZ4;
                                break;
                            default:
                                TableDic[enums][PackageInfo.Name].CompressMode = ECompressMode.None;
                                break;
                        }
                    }
                    else Debug.LogWarningFormat("未知平台 : {0}", PackageInfo.Name);
                }
            }

            foreach (var pair in TableDic
                         .Where(hashtable => hashtable.Value.Count > 0))
            {
                AHelper.IO.WriteJsonUTF8(
                    Path.Combine(BundlesConfigDir, string.Concat(pair.Key.ToString(), ".json")),
                    pair.Value.Values.ToArray()
                );
            }
        }
    }
}

#endif