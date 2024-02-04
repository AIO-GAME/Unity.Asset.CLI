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
using AIO.UEngine;
using UnityEngine;
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
            // 暂时直接全部清空 重新添加
            AssetBundleCollectorSettingData.Setting.Packages.Clear();

            // 可能会有重复的包名，所以需要去重
            // var YPackages = AssetBundleCollectorSettingData.Setting.Packages;
            // foreach (var APackage in asset.Packages)
            // {
            //     for (var YPIndex = YPackages.Count - 1; YPIndex >= 0; YPIndex--)
            //     {
            //         if (YPackages[YPIndex].PackageName != APackage.Name) continue;
            //         YPackages.RemoveAt(YPIndex);
            //         break;
            //     }
            // }
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
            if (!Application.isPlaying) asset.Save();
            PackageRemoveRepeat(asset);
            PackageUpdateSetting(asset);
            PackageConvert(asset);
            PackageRemoveEmpty();

            // 如果启用了序列记录，则需要给每个包新增一个序列记录组
            if (!ASConfig.GetOrCreate().EnableSequenceRecord) return;

            var bundle = new AssetBundleCollectorPackage
            {
                PackageName = AssetSystem.TagsRecord,
                PackageDesc = "序列记录包[首包专用](请勿删除)",
                Groups = new List<AssetBundleCollectorGroup>()
            };
            foreach (var package in AssetBundleCollectorSettingData.Setting.Packages)
            {
                if (package.Groups is null) continue;
                var recordGroup = new AssetBundleCollectorGroup
                {
                    GroupName = package.PackageName,
                    AssetTags = AssetSystem.TagsRecord,
                    Collectors = new List<AssetBundleCollector>()
                };
                foreach (var group in package.Groups)
                {
                    if (group.Collectors is null) continue;
                    foreach (var collector in group.Collectors
                                 // .Where(item => item.CollectorType == ECollectorType.MainAssetCollector)
                                 .Where(item => !recordGroup.Collectors.Exists(match =>
                                     match.CollectorGUID == item.CollectorGUID)))
                    {
                        recordGroup.Collectors.Add(new AssetBundleCollector
                        {
                            CollectorGUID = collector.CollectorGUID,
                            CollectPath = collector.CollectPath,
                            CollectorType = collector.CollectorType,
                            AssetTags = collector.AssetTags,
                            AddressRuleName = nameof(AIOAddressRecordRule),
                            FilterRuleName = nameof(AIOFilterRecordRule),
                            PackRuleName = nameof(PackGroup),
                            UserData = group.GroupName,
                        });
                    }
                }

                bundle.Groups.Insert(0, recordGroup);
            }

            AssetBundleCollectorSettingData.Setting.Packages.Insert(0, bundle);
        }
    }
}
#endif