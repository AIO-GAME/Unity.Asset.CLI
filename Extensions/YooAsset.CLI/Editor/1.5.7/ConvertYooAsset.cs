/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-05
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

#if SUPPORT_YOOASSET
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YooAsset.Editor;

namespace AIO.UEditor.CLI
{
    internal static partial class ConvertYooAsset
    {
        private static readonly Dictionary<AssetCollectItem, bool> Collectors;

        private static AssetCollectRoot _Instance;

        private static AssetCollectRoot Instance
        {
            get
            {
                if (_Instance is null) _Instance = AssetCollectRoot.GetOrCreate();
                return _Instance;
            }
        }

        static ConvertYooAsset()
        {
            AssetCollectSetting.Initialize();
            Collectors = new Dictionary<AssetCollectItem, bool>();
        }

        private static IEnumerable<AssetBundleCollectorPackage> Convert(IEnumerable<AssetCollectPackage> packages)
            => packages is null
                ? Array.Empty<AssetBundleCollectorPackage>()
                : packages
                    .Where(package => !package.Equals(null))
                    .Where(package => !package.Groups.Equals(null))
                    .Select(Convert);

        private static IEnumerable<AssetBundleCollectorGroup> Convert(IEnumerable<AssetCollectGroup> groups)
            => groups is null
                ? Array.Empty<AssetBundleCollectorGroup>()
                : groups
                    .Where(group => !group.Equals(null))
                    .Where(group => !group.Collectors.Equals(null))
                    .Select(Convert);

        private static IEnumerable<AssetBundleCollector> Convert(IEnumerable<AssetCollectItem> collects)
            => collects is null
                ? Array.Empty<AssetBundleCollector>()
                : collects
                    .Where(collect => !collect.Equals(null))
                    .Where(collect => !string.IsNullOrEmpty(collect.CollectPath))
                    .Where(collect => File.Exists(collect.CollectPath) || Directory.Exists(collect.CollectPath))
                    .Select(Convert);

        private static AssetBundleCollectorPackage Convert(AssetCollectPackage package) =>
            new AssetBundleCollectorPackage
            {
                PackageName = package.Name,
                PackageDesc = package.Description,
                Groups = Convert(package.Groups).ToList()
            };

        private static AssetBundleCollectorGroup Convert(AssetCollectGroup group)
            => new AssetBundleCollectorGroup
            {
                AssetTags = group.Tags,
                GroupDesc = group.Description,
                GroupName = group.Name,
                Collectors = Convert(group.Collectors).ToList()
            };

        private static AssetBundleCollector Convert(AssetCollectItem collect)
            => new AssetBundleCollector
            {
                CollectorGUID = collect.GUID,
                CollectPath = collect.CollectPath,
                CollectorType = Convert(collect.Type),
                AssetTags = collect.Tags,
                AddressRuleName = nameof(AIOAddressRule),
                FilterRuleName = nameof(AIOFilterRule),
                PackRuleName = nameof(AIOPackRule),
                UserData = collect.UserData,
            };

        private static ECollectorType Convert(EAssetCollectItemType type)
        {
            switch (type)
            {
                default:
                case EAssetCollectItemType.MainAssetCollector:
                    return ECollectorType.MainAssetCollector;
                case EAssetCollectItemType.DependAssetCollector:
                    return ECollectorType.DependAssetCollector;
                case EAssetCollectItemType.StaticAssetCollector:
                    return ECollectorType.StaticAssetCollector;
            }
        }

        private static void PackageRemoveRepeat(AssetCollectRoot asset)
        {
            if (asset.Packages is null) return;
            var YPackages = AssetBundleCollectorSettingData.Setting.Packages;
            foreach (var APackage in asset.Packages)
            {
                for (var YPIndex = YPackages.Count - 1; YPIndex >= 0; YPIndex--)
                {
                    if (YPackages[YPIndex].PackageName != APackage.Name) continue;
                    YPackages.RemoveAt(YPIndex);
                    break;
                }
            }
        }

        private static void PackageUpdateSetting(AssetCollectRoot asset)
        {
            AssetBundleCollectorSettingData.Setting.ShowPackageView = true;
            AssetBundleCollectorSettingData.Setting.ShowEditorAlias = true;
            AssetBundleCollectorSettingData.Setting.UniqueBundleName = asset.UniqueBundleName;
            AssetBundleCollectorSettingData.Setting.IncludeAssetGUID = asset.IncludeAssetGUID;
            AssetBundleCollectorSettingData.Setting.EnableAddressable = asset.EnableAddressable;
        }

        private static void PackageConvert(AssetCollectRoot asset)
        {
            foreach (var package in Convert(asset.Packages))
            {
                foreach (var group in package.Groups)
                {
                    group.GroupName = $"{package.PackageName}_{group.GroupName}";
                }

                AssetBundleCollectorSettingData.Setting.Packages.Add(package);
            }
        }

        private static void PackageRemoveEmpty()
        {
            var packages = AssetBundleCollectorSettingData.Setting.Packages;
            for (var i = packages.Count - 1; i >= 0; i--)
            {
                var package = packages[i];
                if (package.Groups is null)
                {
                    packages.RemoveAt(i);
                    continue;
                }

                for (var j = package.Groups.Count - 1; j >= 0; j--)
                {
                    if (package.Groups[j].Collectors is null)
                    {
                        package.Groups.RemoveAt(j);
                        continue;
                    }

                    for (var k = package.Groups[j].Collectors.Count - 1; k >= 0; k--)
                    {
                        var collector = package.Groups[j].Collectors[k];
                        if (!AHelper.IO.Exists(collector.CollectPath))
                        {
                            package.Groups[j].Collectors.RemoveAt(k);
                        }
                    }

                    if (package.Groups[j].Collectors.Count == 0) package.Groups.RemoveAt(j);
                }

                if (package.Groups.Count == 0) packages.RemoveAt(i);
            }

            AssetBundleCollectorSettingData.Setting.Packages = packages;
        }

        public static void Convert(AssetCollectRoot asset)
        {
            asset.Save();
            PackageRemoveRepeat(asset);
            PackageUpdateSetting(asset);
            PackageConvert(asset);
            PackageRemoveEmpty();
        }
    }
}
#endif