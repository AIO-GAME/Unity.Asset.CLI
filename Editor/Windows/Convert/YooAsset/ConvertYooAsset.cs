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
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

namespace AIO.UEditor
{
    internal static class ConvertYooAsset
    {
        public static Dictionary<AssetCollectItem, bool> Collectors;

        private static AssetCollectRoot _Instance;

        private static AssetCollectRoot Instance
        {
            get
            {
                if (_Instance != null) return _Instance;
                _Instance = AssetCollectRoot.GetOrCreate();
                return _Instance;
            }
        }

        static ConvertYooAsset()
        {
            AssetCollectSetting.Initialize();
            Collectors = new Dictionary<AssetCollectItem, bool>();
        }

        [DisplayName("AIO Asset Filter Rule")]
        internal class AIOFilterRule : IFilterRule
        {
            public bool IsCollectAsset(FilterRuleData data)
            {
                if (!data.GroupName.Contains('_')) return false;
                if (Instance is null) return false;
                var info = data.GroupName.SplitOnce('_');
                var collector = Instance.GetPackage(info.Item1)?.GetGroup(info.Item2)?.GetCollector(data.CollectPath);
                if (collector is null) return false;

                if (Application.isPlaying)
                {
                    if (AssetSystem.Parameter.ASMode == EASMode.Editor &&
                        collector.LoadType == EAssetLoadType.Runtime)
                        return false;

                    if (AssetSystem.Parameter.ASMode != EASMode.Editor &&
                        collector.LoadType == EAssetLoadType.Editor)
                        return false;
                }
                else
                {
                    if (ASConfig.GetOrCreate().ASMode == EASMode.Editor &&
                        collector.LoadType == EAssetLoadType.Runtime)
                        return false;

                    if (ASConfig.GetOrCreate().ASMode != EASMode.Editor &&
                        collector.LoadType == EAssetLoadType.Editor)
                        return false;
                }

                if (!Collectors.ContainsKey(collector) || Collectors[collector] == false)
                {
                    collector.UpdateCollect();
                    collector.UpdateFilter();
                    Collectors[collector] = true;
                }

                var infoData = new AssetRuleData
                {
                    Tags = collector.Tags,
                    UserData = collector.UserData,
                    PackageName = info.Item1,
                    GroupName = info.Item2,
                    CollectPath = collector.CollectPath,
                    Extension = Path.GetExtension(data.AssetPath).Replace(".", "").ToLower()
                };
                infoData.AssetPath = data.AssetPath.Substring(0, data.AssetPath.Length - infoData.Extension.Length - 1);
                return collector.IsCollectAsset(infoData);
            }
        }

        [DisplayName("AIO Asset Address Rule")]
        internal class AIOAddressRule : IAddressRule
        {
            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                if (!data.GroupName.Contains('_')) return "Error : Rule mismatch";
                var info = data.GroupName.SplitOnce('_');
                var collector = Instance.GetPackage(info.Item1)?.GetGroup(info.Item2).GetCollector(data.CollectPath);
                if (collector is null) return "Error : Not found collector";
                if (!Collectors.ContainsKey(collector) || Collectors[collector] == false)
                {
                    collector.UpdateCollect();
                    collector.UpdateFilter();
                    Collectors[collector] = true;
                }

                var infoData = new AssetRuleData
                {
                    Tags = collector.Tags,
                    UserData = collector.UserData,
                    PackageName = info.Item1,
                    GroupName = info.Item2,
                    CollectPath = collector.CollectPath,
                    Extension = Path.GetExtension(data.AssetPath).Replace(".", "").ToLower()
                };
                infoData.AssetPath = data.AssetPath.Substring(0, data.AssetPath.Length - infoData.Extension.Length - 1);
                return collector.GetAssetAddress(infoData, ASConfig.GetOrCreate().LoadPathToLower);
            }
        }

        [DisplayName("AIO Asset Pack Rule")]
        internal class AIOPackRule : IPackRule
        {
            public PackRuleResult GetPackRuleResult(PackRuleData data)
            {
                if (!data.GroupName.Contains('_')) throw new Exception("Error : Rule mismatch");
                var info = data.GroupName.SplitOnce('_');
                var collector = Instance.GetPackage(info.Item1)?.GetGroup(info.Item2).GetCollector(data.CollectPath);
                if (collector is null) throw new Exception("Error : Not found collector");
                if (!Collectors.ContainsKey(collector) || Collectors[collector] == false)
                {
                    collector.UpdateCollect();
                    collector.UpdateFilter();
                    Collectors[collector] = true;
                }

                var infoData = new AssetRuleData
                {
                    Tags = collector.Tags,
                    UserData = collector.UserData,
                    PackageName = info.Item1,
                    GroupName = info.Item2,
                    CollectPath = collector.CollectPath,
                    Extension = Path.GetExtension(data.AssetPath).Replace(".", "").ToLower()
                };
                infoData.AssetPath = data.AssetPath.Substring(0, data.AssetPath.Length - infoData.Extension.Length - 1);
                var rule = collector.GetPackRule(infoData);
                return new PackRuleResult(rule.BundleName, rule.BundleExtension);
            }

            public bool IsRawFilePackRule() => false;
        }

        private static IEnumerable<AssetBundleCollectorPackage> Convert(IEnumerable<AssetCollectPackage> packages)
            => packages.Where(package => !package.Groups.Equals(null)).Select(Convert);

        private static IEnumerable<AssetBundleCollectorGroup> Convert(IEnumerable<AssetCollectGroup> groups)
            => groups.Where(group => !group.Collectors.Equals(null)).Select(Convert);

        private static IEnumerable<AssetBundleCollector> Convert(IEnumerable<AssetCollectItem> collects)
            => collects.Where(collect =>
                !string.IsNullOrEmpty(collect.CollectPath) &&
                (File.Exists(collect.CollectPath) || Directory.Exists(collect.CollectPath))
            ).Select(Convert);

        private static AssetBundleCollectorPackage Convert(AssetCollectPackage package)
            => new AssetBundleCollectorPackage
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

        public static void Convert(AssetCollectRoot asset)
        {
            asset.Save();
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

            AssetBundleCollectorSettingData.Setting.ShowPackageView = true;
            AssetBundleCollectorSettingData.Setting.ShowEditorAlias = true;
            AssetBundleCollectorSettingData.Setting.UniqueBundleName = asset.UniqueBundleName;
            AssetBundleCollectorSettingData.Setting.IncludeAssetGUID = asset.IncludeAssetGUID;
            AssetBundleCollectorSettingData.Setting.EnableAddressable = asset.EnableAddressable;
            foreach (var package in Convert(asset.Packages))
            {
                foreach (var group in package.Groups)
                {
                    group.GroupName = $"{package.PackageName}_{group.GroupName}";
                }

                AssetBundleCollectorSettingData.Setting.Packages.Add(package);
            }

            for (var i = AssetBundleCollectorSettingData.Setting.Packages.Count - 1; i >= 0; i--)
            {
                var package = AssetBundleCollectorSettingData.Setting.Packages[i];
                if (package.Groups is null)
                {
                    AssetBundleCollectorSettingData.Setting.Packages.RemoveAt(i);
                    continue;
                }

                for (var j = package.Groups.Count - 1; j >= 0; j--)
                {
                    var group = package.Groups[j];
                    if (group.Collectors is null)
                    {
                        package.Groups.RemoveAt(j);
                        continue;
                    }

                    for (var k = package.Groups[j].Collectors.Count - 1; k >= 0; k--)
                    {
                        var collector = package.Groups[j].Collectors[k];
                        if (string.IsNullOrEmpty(collector.CollectPath) ||
                            (!File.Exists(collector.CollectPath) && !Directory.Exists(collector.CollectPath)))
                        {
                            package.Groups[j].Collectors.RemoveAt(k);
                        }
                    }

                    if (group.Collectors.Count == 0) package.Groups.RemoveAt(j);
                }

                if (package.Groups.Count == 0)
                {
                    AssetBundleCollectorSettingData.Setting.Packages.RemoveAt(i);
                }
            }
        }
    }
}
#endif