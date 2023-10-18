#if SUPPORT_YOOASSET

using System.IO;
using YooAsset.Editor;

namespace AIO.UEditor.YooAsset
{
    [DisplayName("文件名 + 后缀 = 定位地址")]
    public class AddressRuleFileName : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            return Path.GetFileName(data.AssetPath);
        }
    }

    [DisplayName("文件名 + 后缀 = 定位地址 (全小写)")]
    public class AddressRuleFileNameToLower : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            return Path.GetFileName(data.AssetPath).ToLower();
        }
    }

    [DisplayName("文件名 + 后缀 = 定位地址 (全大写)")]
    public class AddressRuleFileNameToUpper : IAddressRule
    {
        string IAddressRule.GetAssetAddress(AddressRuleData data)
        {
            return Path.GetFileName(data.AssetPath).ToUpper();
        }
    }
}
#endif