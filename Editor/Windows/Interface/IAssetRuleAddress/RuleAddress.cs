/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-04
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.IO;
using IAddressRule = AIO.UEditor.IAssetRuleAddress;

namespace AIO.UEditor
{
    public class RuleAddress
    {
        public class AddressRuleGroupRelative : IAddressRule
        {
            public string DisplayAddressName => "Location = Group + Relative File Path";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return data.AssetPath.Replace(
                    string.Concat(data.CollectPath, '/'),
                    string.Concat(Path.GetFileName(data.GroupName) + '/')
                );
            }
        }

        public class AddressRuleGroupFileName : IAddressRule
        {
            public string DisplayAddressName => "Location = Group + FileName";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return string.Concat(Path.GetFileName(data.GroupName), '/', Path.GetFileName(data.AssetPath));
            }
        }

        public class AddressRuleFileName : IAddressRule
        {
            public string DisplayAddressName => "Location = FileName";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return Path.GetFileName(data.AssetPath);
            }
        }

        public class AddressRuleInstanceID : IAddressRule
        {
            public string DisplayAddressName => "Location = InstanceID";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                try
                {
                    var obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(data.AssetPath);
                    return obj != null ? obj.GetInstanceID().ToString() : Path.GetFileName(data.AssetPath);
                }
                catch (Exception e)
                {
                    return Path.GetFileName(data.AssetPath);
                }
            }
        }

        public class AddressRuleCollectRelative : IAddressRule
        {
            public string DisplayAddressName => "Location = Collect + Relative File Path";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return data.AssetPath.Substring(data.CollectPath.LastIndexOf('/') + 1);
            }
        }

        public class AddressRuleCollectFileName : IAddressRule
        {
            public string DisplayAddressName => "Location = Collect + FileName";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return string.Concat(Path.GetFileName(data.CollectPath), '/', Path.GetFileName(data.AssetPath));
            }
        }

        public class AddressRuleRootRelative : IAddressRule
        {
            public string DisplayAddressName => "Location = Root Relative";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return data.AssetPath;
            }
        }

        public class AddressRuleUserDataRelative : IAddressRule
        {
            public string DisplayAddressName => "Location = UserData + Relative File Path";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return data.AssetPath.Replace(
                    string.Concat(data.CollectPath, '/'),
                    string.Concat(data.UserData, '/')
                );
            }
        }

        public class AddressRuleUserDataFileName : IAddressRule
        {
            public string DisplayAddressName => "Location = UserData + FileName";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return string.Concat(data.UserData, '/', Path.GetFileName(data.AssetPath));
            }
        }
    }
}