/*|==========|*|
|*|Author:   |*| -> xi nan
|*|Date:     |*| -> 2023-06-03
|*|==========|*/

#if SUPPORT_YOOASSET
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO.UEditor
{
    public partial class MenuItem_YooAssets

    {
        [MenuItem("YooAsset/Create Config")]
        public static void CreateConfig()
        {
            var BundlesDir = Application.dataPath.Replace("Assets", "Bundles");
            if (!Directory.Exists(BundlesDir))
            {
                Debug.LogWarningFormat("Bundles 目录不存在 : 无需创建配置文件");
                return;
            }

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
            if (Directory.Exists(BundlesConfigDir)) Directory.Delete(BundlesConfigDir, true);
            Directory.CreateDirectory(BundlesConfigDir);

            var TabelDic = new Dictionary<BuildTarget, Dictionary<string, AssetsPackageConfig>>();
            TabelDic.Add(BuildTarget.Android, new Dictionary<string, AssetsPackageConfig>());
            TabelDic.Add(BuildTarget.WebGL, new Dictionary<string, AssetsPackageConfig>());
            TabelDic.Add(BuildTarget.iOS, new Dictionary<string, AssetsPackageConfig>());
            TabelDic.Add(BuildTarget.StandaloneWindows, new Dictionary<string, AssetsPackageConfig>());
            TabelDic.Add(BuildTarget.StandaloneWindows64, new Dictionary<string, AssetsPackageConfig>());
            TabelDic.Add(BuildTarget.StandaloneOSX, new Dictionary<string, AssetsPackageConfig>());

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
                    foreach (var VersionInfo in PackageInfo.GetDirectories("*", SearchOption.TopDirectoryOnly))
                    {
                        if (VersionInfo.Name.StartsWith("OutputCache")) continue;
                        if (VersionInfo.Name.StartsWith("Simulate")) continue;
                        versions.Add(VersionInfo);
                    }

                    if (versions.Count <= 0) continue;
                    var last = AHelper.IO.GetLastWriteTimeUtc(versions);
                    if (Enum.TryParse<BuildTarget>(PlatformInfo.Name, out var enums))
                    {
                        if (TabelDic[enums].ContainsKey(PackageInfo.Name))
                        {
                            TabelDic[enums][PackageInfo.Name].Version = last.Name;
                            TabelDic[enums][PackageInfo.Name].Name = PackageInfo.Name;
                        }
                        else
                        {
                            TabelDic[enums].Add(PackageInfo.Name, new AssetsPackageConfig
                            {
                                Version = last.Name,
                                Name = PackageInfo.Name,
                                IsDefault = false,
                                IsSidePlayWithDownload = false,
                            });
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
                File.WriteAllText(filePath, AHelper.Json.Serialize(hashtable.Value.Values.ToArray()));
            }
        }

        [MenuItem("YooAsset/Open/Bundles")]
        public static async void OpenBundles()
        {
            var path = Application.dataPath.Replace("Assets", "Bundles");
            if (AHelper.IO.ExistsFolder(path)) await PrPlatform.Open.Path(path);
        }

        [MenuItem("YooAsset/Open/Sandbox")]
        public static async void OpenSandbox()
        {
            var path = Application.dataPath.Replace("Assets", "Sandbox");
            if (AHelper.IO.ExistsFolder(path)) await PrPlatform.Open.Path(path);
        }

        [MenuItem("YooAsset/Clear/Bundles")]
        public static async void ClearBundles()
        {
            var path = Application.dataPath.Replace("Assets", "Bundles");
            if (AHelper.IO.ExistsFolder(path)) await PrPlatform.Folder.Del(path);
        }

        [MenuItem("YooAsset/Clear/Sandbox")]
        public static async void ClearSandbox()
        {
            var path = Application.dataPath.Replace("Assets", "Sandbox");
            if (AHelper.IO.ExistsFolder(path)) await PrPlatform.Folder.Del(path);
        }
    }
}
#endif