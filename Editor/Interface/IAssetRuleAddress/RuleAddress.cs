#region

using System;
using System.IO;
using UnityEditor;
using IAddressRule = AIO.UEditor.IAssetRuleAddress;
using Object = UnityEngine.Object;

#endregion

namespace AIO.UEditor
{
    internal class RuleAddress
    {
        #region Nested type: AddressRuleCollectFileName

        private class AddressRuleCollectFileName : IAddressRule
        {
            #region IAddressRule Members

            public bool AllowThread => true;

            public string DisplayAddressName => "寻址路径 = 收集器文件名 + 资源文件名";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return string.Concat(
                    Path.GetFileName(data.CollectPath), '/', Path.GetFileName(data.AssetPath)
                ).Replace('\\', '/');
            }

            #endregion
        }

        #endregion

        #region Nested type: AddressRuleCollectRelative

        private class AddressRuleCollectRelative : IAddressRule
        {
            #region IAddressRule Members

            public bool AllowThread => true;

            public string DisplayAddressName => "寻址路径 = 收集器文件名 + 资源文件相对收集器路径";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return data.AssetPath.Substring(
                                data.CollectPath.Replace('\\', '/').LastIndexOf('/') + 1).
                            Replace('\\', '/');
            }

            #endregion
        }

        #endregion

        #region Nested type: AddressRuleFileName

        private class AddressRuleFileName : IAddressRule
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

        private class AddressRuleGroupFileName : IAddressRule
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

        private class AddressRuleGroupRelative : IAddressRule
        {
            #region IAddressRule Members

            public bool AllowThread => true;

            public string DisplayAddressName => "寻址路径 = 组名 + 资源文件相对收集器路径";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                return data.AssetPath.
                            Replace('\\', '/').
                            Replace(data.CollectPath.Replace('\\', '/'), data.GroupName);
            }

            #endregion
        }

        #endregion

        #region Nested type: AddressRuleInstanceID

        private class AddressRuleInstanceID : IAddressRule
        {
            #region IAddressRule Members

            public bool AllowThread => false;

            public string DisplayAddressName => "寻址路径 = 资源 实例ID";

            string IAddressRule.GetAssetAddress(AssetRuleData data)
            {
                var path = string.Concat(data.AssetPath, '.', data.Extension);
                var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (obj) return obj.GetInstanceID().ToString();
                AssetSystem.LogException($"异常资源: {path}");
                return string.Empty;
            }

            #endregion
        }

        #endregion

        #region Nested type: AddressRuleRootRelative

        private class AddressRuleRootRelative : IAddressRule
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

        private class AddressRuleUserDataFileName : IAddressRule
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

        private class AddressRuleUserDataRelative : IAddressRule
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