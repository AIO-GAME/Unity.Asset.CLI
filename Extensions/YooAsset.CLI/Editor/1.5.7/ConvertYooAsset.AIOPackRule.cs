/*|============|*|
|*|Author:     |*| USER
|*|Date:       |*| 2024-01-08
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/

#if SUPPORT_YOOASSET
using System;
using System.IO;
using YooAsset.Editor;

namespace AIO.UEditor.CLI
{
    internal static partial class ConvertYooAsset
    {
        [DisplayName("AIO Asset Pack Rule")]
        internal class AIOPackRule : IPackRule
        {
            public PackRuleResult GetPackRuleResult(PackRuleData data)
            {
                if (!data.GroupName.Contains('_')) throw new Exception("Error : Rule mismatch");
                var info = data.GroupName.SplitOnce('_');
                var collector = Instance.GetPackage(info.Item1)?.GetGroup(info.Item2)?.GetCollector(data.CollectPath);
                if (collector is null) throw new Exception("Error : Not found collector");
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
                var rule = collector.GetPackRule(infoData);
                return new PackRuleResult(
                    rule.BundleName.Replace("#", "_"),
                    rule.BundleExtension);
            }

            public bool IsRawFilePackRule() => false;
        }
    }
}
#endif