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
            public bool IsCollectAsset(FilterRuleData data)
            {
                if (Instance is null || !data.GroupName.Contains('_')) return false;
                var info = data.GroupName.SplitOnce('_');
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
    }
}
#endif