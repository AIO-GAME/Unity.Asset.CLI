/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-04
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using IAddressRule = AIO.UEditor.IAssetRuleAddress;
using Object = UnityEngine.Object;

namespace AIO.UEditor
{
    public class RuleAddress
    {
        public class AddressRuleGroupRelative : IAddressRule
        {
            public bool AllowThread => true;

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
            public bool AllowThread => true;

            public string DisplayAddressName => "Location = Group + FileName";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return string.Concat(Path.GetFileName(data.GroupName), '/', Path.GetFileName(data.AssetPath));
            }
        }

        public class AddressRuleFileName : IAddressRule
        {
            public bool AllowThread => true;

            public string DisplayAddressName => "Location = FileName";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return Path.GetFileName(data.AssetPath);
            }
        }

        public class AddressRuleInstanceID : IAddressRule
        {
            public bool AllowThread => false;

            public string DisplayAddressName => "Location = InstanceID";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                var obj = AssetDatabase.LoadAssetAtPath<Object>(string.Concat(data.AssetPath, '.', data.Extension));
                return obj.GetInstanceID().ToString();
            }
        }

        public class AddressRuleCollectRelative : IAddressRule
        {
            public bool AllowThread => true;

            public string DisplayAddressName => "Location = Collect + Relative File Path";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return data.AssetPath.Substring(data.CollectPath.LastIndexOf('/') + 1);
            }
        }

        public class AddressRuleCollectFileName : IAddressRule
        {
            public bool AllowThread => true;

            public string DisplayAddressName => "Location = Collect + FileName";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return string.Concat(Path.GetFileName(data.CollectPath), '/', Path.GetFileName(data.AssetPath));
            }
        }

        public class AddressRuleRootRelative : IAddressRule
        {
            public bool AllowThread => true;

            public string DisplayAddressName => "Location = Root Relative";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return data.AssetPath;
            }
        }

        public class AddressRuleUserDataRelative : IAddressRule
        {
            public bool AllowThread => true;

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
            public bool AllowThread => true;

            public string DisplayAddressName => "Location = UserData + FileName";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return string.Concat(data.UserData, '/', Path.GetFileName(data.AssetPath));
            }
        }
    }
}