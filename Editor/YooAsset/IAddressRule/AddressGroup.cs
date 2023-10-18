/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-14
|||✩ Document: ||| ->
|||✩ - - - - - |*/
#if SUPPORT_YOOASSET

using System.IO;
using YooAsset.Editor;

namespace AIO.UEditor.YooAsset
{
    public class AddressGroup
    {
        [DisplayName("Group Collect + 后缀 = 定位地址")]
        public class GroupCollectWithSuffix : IAddressRule
        {
            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                return data.AssetPath.Replace(data.CollectPath + "/", Path.GetFileName(data.GroupName) + "/");
            }
        }

        [DisplayName("Group Collect + 后缀 = 定位地址 (全小写)")]
        public class GroupCollectWithSuffixToLower : IAddressRule
        {
            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                return data.AssetPath.Replace(data.CollectPath + "/", Path.GetFileName(data.GroupName) + "/").ToLower();
            }
        }

        [DisplayName("Group Collect + 后缀 = 定位地址 (全大写)")]
        public class GroupCollectWithSuffixToUpper : IAddressRule
        {
            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                return data.AssetPath.Replace(data.CollectPath + "/", Path.GetFileName(data.GroupName) + "/").ToUpper();
            }
        }
    }
}
#endif