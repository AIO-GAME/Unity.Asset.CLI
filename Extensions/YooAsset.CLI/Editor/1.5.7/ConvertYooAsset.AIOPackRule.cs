#if SUPPORT_YOOASSET
using System;
using System.IO;
using YooAsset.Editor;

namespace AIO.UEditor.CLI
{
    internal static partial class ConvertYooAsset
    {
        #region Nested type: AIOPackRule

        [DisplayName("AIO Asset Pack Rule")]
        internal class AIOPackRule : IPackRule
        {
            #region IPackRule Members

            public PackRuleResult GetPackRuleResult(PackRuleData data)
            {
                if (!data.GroupName.Contains('#')) throw new Exception("Error : Rule mismatch");
                var info      = data.GroupName.SplitOnce('#');
                var collector = Instance.GetByName(info.Item1, info.Item2, data.CollectPath);
                if (collector is null) throw new Exception("Error : Not found collector");
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
                var rule   = collector.GetPackRule();
                var result = rule.GetPackRuleResult(infoData);
                return new PackRuleResult(result.BundleName.Replace("#", "_"),
                                          result.BundleExtension);
            }

            public bool IsRawFilePackRule() { return false; }

            #endregion
        }

        #endregion

        [DisplayName("AIO Asset Raw Pack Rule")]
        internal class AIOPackRawRule : IPackRule
        {
            #region IPackRule Members

            public PackRuleResult GetPackRuleResult(PackRuleData data)
            {
                if (!data.GroupName.Contains('#')) throw new Exception("Error : Rule mismatch");
                var info      = data.GroupName.SplitOnce('#');
                var collector = Instance.GetByName(info.Item1, info.Item2, data.CollectPath);
                if (collector is null) throw new Exception("Error : Not found collector");
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
                var rule   = collector.GetPackRule();
                var result = rule.GetPackRuleResult(infoData);
                return new PackRuleResult(
                                          result.BundleName.Replace("#", "_"),
                                          result.BundleExtension);
            }

            public bool IsRawFilePackRule() { return true; }

            #endregion
        }
    }
}
#endif