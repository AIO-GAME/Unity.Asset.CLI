﻿#if SUPPORT_YOOASSET
using System.IO;
using AIO.UEngine;
using UnityEngine;
using YooAsset.Editor;

namespace AIO.UEditor.CLI
{
    internal static partial class ConvertYooAsset
    {
        #region Nested type: AIOFilterRecordRule

        [DisplayName("AIO Asset Filter Record Rule")]
        private class AIOFilterRecordRule : IFilterRule
        {
            private static ASConfig    _Config;
            private static IFilterRule _rule;

            private static ASConfig Config
            {
                get
                {
                    if (!_Config) _Config = ASConfig.GetOrCreate();
                    return _Config;
                }
            }

            private static IFilterRule Rule
            {
                get
                {
                    if (_rule is null) _rule = new AIOFilterRule();

                    return _rule;
                }
            }

            #region IFilterRule Members

            public bool IsCollectAsset(FilterRuleData data)
            {
                if (Application.isPlaying) return Rule.IsCollectAsset(data);
                if (!Instance || !data.UserData.Contains('#')) return false;
                var info      = data.UserData.SplitOnce('#');
                var collector = Instance.GetByName(info.Item1)?.GetByGroupName(info.Item2)?.GetByPath(data.CollectPath);
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
                    Tags        = collector.Tags,
                    UserData    = collector.UserData,
                    PackageName = info.Item1,
                    GroupName   = info.Item2,
                    CollectPath = collector.CollectPath,
                    Extension   = Path.GetExtension(data.AssetPath).Replace(".", "").ToLower()
                };
                infoData.AssetPath = data.AssetPath.Substring(0, data.AssetPath.Length - infoData.Extension.Length - 1);
                if (infoData.Extension.EndsWith("shadervariants")) return true;
                return collector.IsCollectAsset(infoData) &&
                       Config.SequenceRecord.ContainsAssetPath(data.AssetPath, info.Item1);
            }

            #endregion
        }

        #endregion

        #region Nested type: AIOFilterRule

        [DisplayName("AIO Asset Filter Rule")]
        private class AIOFilterRule : IFilterRule
        {
            private static ASConfig _Config;

            private static ASConfig Config
            {
                get
                {
                    if (!_Config) _Config = ASConfig.GetOrCreate();
                    return _Config;
                }
            }

            #region IFilterRule Members

            public bool IsCollectAsset(FilterRuleData data)
            {
                if (!Instance || !data.GroupName.Contains('#')) return false;
                var info = data.GroupName.SplitOnce('#');

                if (!Application.isPlaying &&
                    Config.EnableSequenceRecord &&
                    Config.SequenceRecord.ContainsAssetPath(data.AssetPath, info.Item1)
                   ) return false;

                var collector = Instance.GetByName(info.Item1)?.GetByGroupName(info.Item2)?.GetByPath(data.CollectPath);
                if (collector is null) return false;

                var mode = Application.isPlaying ? AssetSystem.Parameter.ASMode : Config.ASMode;
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
                    Tags        = collector.Tags,
                    UserData    = collector.UserData,
                    PackageName = info.Item1,
                    GroupName   = info.Item2,
                    CollectPath = collector.CollectPath,
                    Extension   = Path.GetExtension(data.AssetPath).Replace(".", "").ToLower()
                };
                infoData.AssetPath = data.AssetPath.Substring(0, data.AssetPath.Length - infoData.Extension.Length - 1);
                return collector.IsCollectAsset(infoData);
            }

            #endregion
        }

        #endregion
    }
}
#endif