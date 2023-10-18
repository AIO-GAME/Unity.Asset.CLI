#if SUPPORT_YOOASSET

using System.IO;
using YooAsset.Editor;

namespace AIO.UEditor.YooAsset
{
    [DisplayName("Collect + 后缀 = 定位地址")]
    public class AddressRuleRelativeCollectWithSuffix : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            return data.AssetPath.Replace(data.CollectPath + "/", "");
        }
    }

    [DisplayName("Collect + 后缀 = 定位地址  (全小写)")]
    public class AddressRuleRelativeCollectWithSuffixToLower : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            return data.AssetPath.Replace(data.CollectPath + "/", "").ToLower();
        }
    }

    [DisplayName("Collect + 后缀 = 定位地址  (全大写)")]
    public class AddressRuleRelativeCollectWithSuffixToUpper : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            return data.AssetPath.Replace(data.CollectPath + "/", "").ToUpper();
        }
    }

    [DisplayName("Collect = 定位地址")]
    public class AddressRuleRelativeCollectNoSuffix : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            var path = data.AssetPath.Replace(string.Concat(data.CollectPath, "/"), "");
            var suffix = Path.GetExtension(path);
            if (string.IsNullOrEmpty(suffix)) return path;
            return path.Replace(Path.GetExtension(path), "");
        }
    }

    [DisplayName("Collect = 定位地址 (全小写)")]
    public class AddressRuleRelativeCollectNoSuffixToLower : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            var path = data.AssetPath.Replace(string.Concat(data.CollectPath, "/"), "");
            var suffix = Path.GetExtension(path);
            if (string.IsNullOrEmpty(suffix)) return path;
            return path.Replace(Path.GetExtension(path), "").ToLower();
        }
    }

    [DisplayName("Collect = 定位地址  (全大写)")]
    public class AddressRuleRelativeCollectNoSuffixToUpper : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            var path = data.AssetPath.Replace(string.Concat(data.CollectPath, "/"), "");
            var suffix = Path.GetExtension(path);
            if (string.IsNullOrEmpty(suffix)) return path;
            return path.Replace(Path.GetExtension(path), "").ToUpper();
        }
    }

    [DisplayName("Root Collect + 后缀 = 定位地址")]
    public class AddressRuleRelativeRootCollectWithSuffix : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            return data.AssetPath.Replace(data.CollectPath + "/", Path.GetFileName(data.CollectPath) + "/");
        }
    }
}
#endif