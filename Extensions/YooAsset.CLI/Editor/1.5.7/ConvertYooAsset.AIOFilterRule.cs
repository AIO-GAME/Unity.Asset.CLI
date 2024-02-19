/*|============|*|
|*|Author:     |*| USER
|*|Date:       |*| 2024-01-08
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/

#if SUPPORT_YOOASSET
using System.IO;
using AIO.UEngine;
using UnityEngine;
using YooAsset.Editor;

namespace AIO.UEditor.CLI
{
    internal static partial class ConvertYooAsset
    {
        [DisplayName("AIO Asset Filter Rule")]
        private class AIOFilterRule : IFilterRule
        {
            private static ASConfig Config
            {
                get
                {
                    if (_Config is null) _Config = ASConfig.GetOrCreate();
                    return _Config;
                }
            }

            private static ASConfig _Config;

            public bool IsCollectAsset(FilterRuleData data)
            {
                if (Instance is null || !data.GroupName.Contains('_')) return false;
                var info = data.GroupName.SplitOnce('_');

                if (!Application.isPlaying &&
                    Config.EnableSequenceRecord &&
                    Config.SequenceRecord.ContainsAssetPath(data.AssetPath, info.Item1))
                    return false;

                var collector = Instance.GetPackage(info.Item1)
                    ?.GetGroup(info.Item2)
                    ?.GetCollector(data.CollectPath);
                if (collector is null) return false;
      
                var mode = Application.isPlaying ? AssetSystem.Parameter.ASMode : ASConfig.GetOrCreate().ASMode;
                if (mode == EASMode.Editor &&
                    collector.LoadType == EAssetLoadType.Runtime)
                    return false;

                if (mode != EASMode.Editor &&
                    collector.LoadType == EAssetLoadType.Editor)
                    return false;

                if (!Collectors.ContainsKey(collector))
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

        [DisplayName("AIO Asset Filter Record Rule")]
        private class AIOFilterRecordRule : IFilterRule
        {
            private static ASConfig Config
            {
                get
                {
                    if (_Config is null) _Config = ASConfig.GetOrCreate();
                    return _Config;
                }
            }

            private static ASConfig _Config;
            private static IFilterRule _rule;

            private static IFilterRule Rule
            {
                get
                {
                    if (_rule is null)
                    {
                        _rule = new AIOFilterRule();
                    }

                    return _rule;
                }
            }

            public bool IsCollectAsset(FilterRuleData data)
            {
                if (Application.isPlaying) return Rule.IsCollectAsset(data);
                if (Instance is null || !data.UserData.Contains('_')) return false;
                var info = data.UserData.SplitOnce('_');
                var collector = Instance.GetPackage(info.Item1)
                    ?.GetGroup(info.Item2)
                    ?.GetCollector(data.CollectPath);
                if (collector is null) return false;

                var mode = Config.ASMode;
                if (mode == EASMode.Editor &&
                    collector.LoadType == EAssetLoadType.Runtime)
                    return false;

                if (mode != EASMode.Editor &&
                    collector.LoadType == EAssetLoadType.Editor)
                    return false;

                if (!Collectors.ContainsKey(collector))
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
                if (infoData.Extension.EndsWith("shadervariants")) return true;
                return collector.IsCollectAsset(infoData) &&
                       Config.SequenceRecord.ContainsAssetPath(data.AssetPath, info.Item1);
            }
        }
    }
}
#endif