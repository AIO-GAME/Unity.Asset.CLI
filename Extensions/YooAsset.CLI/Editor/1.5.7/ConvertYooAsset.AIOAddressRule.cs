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
        [DisplayName("AIO Asset Address Rule")]
        private class AIOAddressRule : IAddressRule
        {
            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                if (!data.GroupName.Contains('_')) return "Error : Rule mismatch";
                var info = data.GroupName.SplitOnce('_');
                var collector = Instance.GetPackage(info.Item1)?.GetByGroupName(info.Item2).GetByPath(data.CollectPath);
                if (collector is null) return "Error : Not found collector";
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
                return collector.GetAssetAddress(infoData, ASConfig.GetOrCreate().LoadPathToLower);
            }
        }

        [DisplayName("AIO Asset Address Record Rule")]
        private class AIOAddressRecordRule : IAddressRule
        {
            private static IAddressRule _rule;

            private static IAddressRule Rule
            {
                get
                {
                    if (_rule is null)
                    {
                        _rule = new AIOAddressRule();
                    }

                    return _rule;
                }
            }

            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                if (Application.isPlaying) return Rule.GetAssetAddress(data);
                if (!data.UserData.Contains('_')) return "Error : Rule mismatch";
                var info = data.UserData.SplitOnce('_');
                var collector = Instance.GetPackage(info.Item1)?.GetByGroupName(info.Item2).GetByPath(data.CollectPath);
                if (collector is null) return "Error : Not found collector";
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
                return collector.GetAssetAddress(infoData, ASConfig.GetOrCreate().LoadPathToLower);
            }
        }
    }
}
#endif