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
    public class AddressUserData
    {
        [DisplayName("UserData > Collect + 后缀 = 定位地址")]
        public class UserDataCollectWithSuffix : IAddressRule
        {
            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                return data.AssetPath.Replace(data.CollectPath + "/", data.UserData + "/");
            }
        }

        [DisplayName("UserData > Collect + 后缀 = 定位地址 (全小写)")]
        public class UserDataCollectWithSuffixToLower : IAddressRule
        {
            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                return data.AssetPath.Replace(data.CollectPath + "/", data.UserData + "/").ToLower();
            }
        }

        [DisplayName("UserData > Collect + 后缀 = 定位地址 (全大写)")]
        public class UserDataCollectWithSuffixToUpper : IAddressRule
        {
            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                return data.AssetPath.Replace(data.CollectPath + "/", data.UserData + "/").ToUpper();
            }
        }


        [DisplayName("UserData > Collect = 定位地址")]
        public class UserDataCollect : IAddressRule
        {
            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                var path = data.AssetPath.Replace(data.CollectPath + "/", data.UserData + "/");
                return path.Replace(Path.GetExtension(path), "");
            }
        }

        [DisplayName("UserData > Collect = 定位地址 (全小写)")]
        public class UserDataCollectToLower : IAddressRule
        {
            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                var path = data.AssetPath.Replace(data.CollectPath + "/", data.UserData + "/").ToLower();
                return path.Replace(Path.GetExtension(path), "");
            }
        }

        [DisplayName("UserData > Collect = 定位地址 (全大写)")]
        public class UserDataCollectToUpper : IAddressRule
        {
            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                var path = data.AssetPath.Replace(data.CollectPath + "/", data.UserData + "/").ToUpper();
                return path.Replace(Path.GetExtension(path), "");
            }
        }


        [DisplayName("UserData + FileName = 定位地址")]
        public class UserDataFileName : IAddressRule
        {
            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                return Path.Combine(data.UserData, Path.GetFileNameWithoutExtension(data.AssetPath));
            }
        }

        [DisplayName("UserData + FileName = 定位地址 (全小写)")]
        public class UserDataFileNameToLower : IAddressRule
        {
            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                return Path.Combine(data.UserData, Path.GetFileNameWithoutExtension(data.AssetPath)).ToLower();
            }
        }

        [DisplayName("UserData + FileName = 定位地址 (全大写)")]
        public class UserDataFileNameToUpper : IAddressRule
        {
            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                return Path.Combine(data.UserData, Path.GetFileNameWithoutExtension(data.AssetPath)).ToUpper();
            }
        }

        [DisplayName("UserData + FileName + 后缀 = 定位地址")]
        public class UserDataFileNameWithSuffix : IAddressRule
        {
            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                return Path.Combine(data.UserData, Path.GetFileName(data.AssetPath));
            }
        }

        [DisplayName("UserData + FileName + 后缀 = 定位地址 (全小写)")]
        public class UserDataFileNameWithSuffixToLower : IAddressRule
        {
            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                return Path.Combine(data.UserData, Path.GetFileName(data.AssetPath)).ToLower();
            }
        }

        [DisplayName("UserData + FileName + 后缀 = 定位地址 (全大写)")]
        public class UserDataFileNameWithSuffixToUpper : IAddressRule
        {
            string IAddressRule.GetAssetAddress(AddressRuleData data)
            {
                return Path.Combine(data.UserData, Path.GetFileName(data.AssetPath)).ToUpper();
            }
        }
    }
}
#endif