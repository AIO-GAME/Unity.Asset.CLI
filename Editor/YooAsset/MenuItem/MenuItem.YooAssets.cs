/*|==========|*|
|*|Author:   |*| -> xi nan
|*|Date:     |*| -> 2023-06-03
|*|==========|*/

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

namespace AIO.UEditor
{
    public static class MenuItem_YooAssets
    {
        [MenuItem("YooAsset/Create Config")]
        public static void CreateConfig()
        {
            var BundlesDir = Path.Combine(EHelper.Path.Project, "Bundles");
            CreateConfig(BundlesDir);
        }

        public static void CreateConfig(string BundlesDir)
        {
            if (!Directory.Exists(BundlesDir))
            {
                Debug.LogWarningFormat("Bundles 目录不存在 : 无需创建配置文件");
                return;
            }

            var BundlesConfigDir = Path.Combine(BundlesDir, "Version");
            if (!Directory.Exists(BundlesConfigDir)) Directory.CreateDirectory(BundlesConfigDir);

            var TabelDic = new Dictionary<BuildTarget, Dictionary<string, AssetsPackageConfig>>
            {
                { BuildTarget.Android, new Dictionary<string, AssetsPackageConfig>() },
                { BuildTarget.WebGL, new Dictionary<string, AssetsPackageConfig>() },
                { BuildTarget.iOS, new Dictionary<string, AssetsPackageConfig>() },
                { BuildTarget.StandaloneWindows, new Dictionary<string, AssetsPackageConfig>() },
                { BuildTarget.StandaloneWindows64, new Dictionary<string, AssetsPackageConfig>() },
                { BuildTarget.StandaloneOSX, new Dictionary<string, AssetsPackageConfig>() }
            };

            var BundlesInfo = new DirectoryInfo(BundlesDir);
            var versions = new List<DirectoryInfo>();

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
                    versions.Clear();
                    versions.AddRange(PackageInfo.GetDirectories("*", SearchOption.TopDirectoryOnly)
                        .Where(VersionInfo => !VersionInfo.Name.EndsWith("OutputCache"))
                        .Where(VersionInfo => !VersionInfo.Name.EndsWith("Simulate")));

                    if (versions.Count <= 0) continue;
                    var last = AHelper.IO.GetLastWriteTimeUtc(versions);
                    if (Enum.TryParse<BuildTarget>(PlatformInfo.Name, out var enums))
                    {
                        if (!TabelDic[enums].ContainsKey(PackageInfo.Name))
                            TabelDic[enums].Add(PackageInfo.Name, new AssetsPackageConfig());

                        TabelDic[enums][PackageInfo.Name].Version = last.Name;
                        TabelDic[enums][PackageInfo.Name].Name = PackageInfo.Name;
                        var table = AHelper.IO.ReadJson<Hashtable>(
                            Path.Combine(last.FullName, $"BuildReport_{PackageInfo.Name}_{last.Name}.json"));
                        if (table == null) continue;
                        foreach (var item in table)
                        {
                            if (item is not DictionaryEntry entry) continue;
                            if (entry.Key.ToString() != "Summary") continue;
                            switch (entry.Value.To<JObject>().Value<int>("CompressOption"))
                            {
                                case (int)ECompressOption.LZMA:
                                    TabelDic[enums][PackageInfo.Name].CompressMode = ECompressMode.LZMA;
                                    break;
                                case (int)ECompressOption.LZ4:
                                    TabelDic[enums][PackageInfo.Name].CompressMode = ECompressMode.LZ4;
                                    break;
                                default:
                                    TabelDic[enums][PackageInfo.Name].CompressMode = ECompressMode.None;
                                    break;
                            }

                            break;
                        }
                    }
                    else Debug.LogWarningFormat("未知平台 : {0}", PackageInfo.Name);
                }
            }

            var BundlesConfigInfo = new DirectoryInfo(BundlesConfigDir);
            foreach (var hashtable in TabelDic)
            {
                if (hashtable.Value.Count <= 0) continue;
                var filename = hashtable.Key.ToString();
                var filePath = Path.Combine(BundlesConfigInfo.FullName, string.Concat(filename, ".json"));
                AHelper.IO.WriteJsonUTF8(filePath, hashtable.Value.Values.ToArray());
            }
        }
    }
}
#endif