using System;
using System.IO;
using UnityEditor;
using IAddressRule = AIO.UEditor.IAssetRuleAddress;
using Object = UnityEngine.Object;

namespace AIO.UEditor
{
    public class RuleAddress
    {
        #region Nested type: AddressRuleCollectFileName

        public class AddressRuleCollectFileName : IAddressRule
        {
            #region IAddressRule Members

            public bool AllowThread => true;

            public string DisplayAddressName => "寻址路径 = 收集器文件名 + 资源文件名";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return string.Concat(
                                  Path.GetFileName(data.CollectPath), '/', Path.GetFileName(data.AssetPath)).
                              Replace('\\', '/');
            }

            #endregion
        }

        #endregion

        #region Nested type: AddressRuleCollectRelative

        public class AddressRuleCollectRelative : IAddressRule
        {
            #region IAddressRule Members

            public bool AllowThread => true;

            public string DisplayAddressName => "寻址路径 = 收集器文件名 + 资源文件相对收集器路径";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return data.AssetPath.Substring(data.CollectPath.Replace('\\', '/').LastIndexOf('/') + 1).
                            Replace('\\', '/');
            }

            #endregion
        }

        #endregion

        #region Nested type: AddressRuleFileName

        public class AddressRuleFileName : IAddressRule
        {
            #region IAddressRule Members

            public bool AllowThread => true;

            public string DisplayAddressName => "寻址路径 = 资源文件名";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return Path.GetFileName(data.AssetPath).Replace('\\', '/');
            }

            #endregion
        }

        #endregion

        #region Nested type: AddressRuleGroupFileName

        public class AddressRuleGroupFileName : IAddressRule
        {
            #region IAddressRule Members

            public bool AllowThread => true;

            public string DisplayAddressName => "寻址路径 = 组名 + 资源文件名";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                if (string.IsNullOrEmpty(data.AssetPath)) throw new ArgumentNullException(nameof(data.AssetPath));
                if (string.IsNullOrEmpty(data.GroupName)) throw new ArgumentNullException(nameof(data.GroupName));
                return Path.Combine(Path.GetFileName(data.GroupName), Path.GetFileName(data.AssetPath)).
                            Replace('\\', '/');
            }

            #endregion
        }

        #endregion

        #region Nested type: AddressRuleGroupRelative

        public class AddressRuleGroupRelative : IAddressRule
        {
            #region IAddressRule Members

            public bool AllowThread => true;

            public string DisplayAddressName => "寻址路径 = 组名 + 资源文件相对收集器路径";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return data.AssetPath.Replace(data.CollectPath, data.GroupName).Replace('\\', '/');
            }

            #endregion
        }

        #endregion

        #region Nested type: AddressRuleInstanceID

        public class AddressRuleInstanceID : IAddressRule
        {
            #region IAddressRule Members

            public bool AllowThread => false;

            public string DisplayAddressName => "寻址路径 = 资源 实例ID";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                var obj = AssetDatabase.LoadAssetAtPath<Object>(string.Concat(data.AssetPath, '.', data.Extension));
                return obj.GetInstanceID().ToString();
            }

            #endregion
        }

        #endregion

        #region Nested type: AddressRuleRootRelative

        public class AddressRuleRootRelative : IAddressRule
        {
            #region IAddressRule Members

            public bool AllowThread => true;

            public string DisplayAddressName => "寻址路径 = 资源文件相对Asset路径";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return data.AssetPath.Replace('\\', '/');
            }

            #endregion
        }

        #endregion

        #region Nested type: AddressRuleUserDataFileName

        public class AddressRuleUserDataFileName : IAddressRule
        {
            #region IAddressRule Members

            public bool AllowThread => true;

            public string DisplayAddressName => "寻址路径 = 自定义根路径 + 资源文件名";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return string.IsNullOrEmpty(data.UserData)
                    ? Path.GetFileName(data.AssetPath)
                    : string.Concat(data.UserData, '/', Path.GetFileName(data.AssetPath)).Replace('\\', '/');
            }

            #endregion
        }

        #endregion

        #region Nested type: AddressRuleUserDataRelative

        public class AddressRuleUserDataRelative : IAddressRule
        {
            #region IAddressRule Members

            public bool AllowThread => true;

            public string DisplayAddressName => "寻址路径 = 自定义根路径 + 资源文件相对收集器路径";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                var temp = data.AssetPath.Replace(data.CollectPath, "").Trim('/', '\\');
                return string.IsNullOrEmpty(data.UserData)
                    ? temp.Replace('\\', '/')
                    : string.Concat(data.UserData, '/', temp).Replace('\\', '/');
            }

            #endregion
        }

        #endregion
    }
}