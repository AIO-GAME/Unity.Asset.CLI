#if SUPPORT_YOOASSET
using System.IO;
using YooAsset.Editor;

namespace AIO.UEditor.YooAsset
{
    [DisplayName("Asset + 后缀 = 定位地址")]
    public class AddressRuleRelativeAssetWithSuffix : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            return data.AssetPath;
        }
    }

    [DisplayName("Asset = 定位地址")]
    public class AddressRuleRelativeAssetNoSuffix : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            var path = data.AssetPath;
            var suffix = Path.GetExtension(path);
            if (string.IsNullOrEmpty(suffix)) return path;
            return path.Replace(Path.GetExtension(path), "");
        }
    }
}
#endif