#if SUPPORT_YOOASSET

#region

using System.IO;
using AIO.UEngine;
using UnityEngine;
using YooAsset.Editor;

#endregion

namespace AIO.UEditor.CLI
{
    internal static partial class ConvertYooAsset
    {
        #region Nested type: AIOAddressRecordRule

        [DisplayName("AIO Asset Address Record Rule")]
        private class AIOAddressRecordRule : IAddressRule
        {
            private static IAddressRule _rule;

            private static IAddressRule Rule
            {
                get
                {
                    if (_rule is null) _rule = new AIOAddressRule();

                    return _rule;
                }
            }

            #region IAddressRule Members

            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                if (Application.isPlaying) return Rule.GetAssetAddress(data);
                if (!data.UserData.Contains('_')) return "Error : Rule mismatch";
                var info = data.UserData.SplitOnce('_');
                var collector = Instance.GetByName(info.Item1, info.Item2, data.CollectPath);
                if (collector is null) return "Error : Not found collector";
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
                var config = ASConfig.GetOrCreate();
                var address = collector.GetAssetAddress(infoData, config.LoadPathToLower, config.HasExtension);
                return address;
            }

            #endregion
        }

        #endregion

        #region Nested type: AIOAddressRule

        [DisplayName("AIO Asset Address Rule")]
        private class AIOAddressRule : IAddressRule
        {
            #region IAddressRule Members

            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                if (!data.GroupName.Contains('_')) return "Error : Rule mismatch";
                var info = data.GroupName.SplitOnce('_');
                var collector = Instance.GetByName(info.Item1, info.Item2, data.CollectPath);
                if (collector is null) return "Error : Not found collector";
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
                var config = ASConfig.GetOrCreate();
                var address = collector.GetAssetAddress(infoData, config.LoadPathToLower, config.HasExtension);
                return address;
            }

            #endregion
        }

        #endregion
    }
}
#endif