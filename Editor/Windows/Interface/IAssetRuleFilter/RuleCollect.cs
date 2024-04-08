using System.Collections.Generic;
using IFilterRule = AIO.UEditor.IAssetRuleFilter;

namespace AIO.UEditor
{
    internal static partial class RuleCollect
    {
        /// <summary>
        ///     判断是否为自定义收集规则
        /// </summary>
        internal static bool IsCollectAssetCustom(ICollection<string> filterCollect, string extension)
        {
            if (string.IsNullOrEmpty(extension)) return false;
            if (filterCollect is null) return false;
            if (filterCollect.Count == 0) return false;
            var extensionLower = extension.ToLower();
            foreach (var collect in filterCollect)
            {
                if (string.IsNullOrEmpty(collect)) continue;
                var collectLower = collect.ToLower();
                switch (collectLower[0])
                {
                    case '.':
                    {
                        if (collectLower.Substring(1).Equals(extensionLower)) return true;
                        break;
                    }
                    case '*':
                    {
                        if (collectLower[1] == '.')
                            if (collectLower.Substring(2).Equals(extensionLower))
                                return true;

                        if (collectLower.Substring(1).Equals(extensionLower)) return true;
                        break;
                    }
                }

                if (collectLower.Equals(extensionLower)) return true;
            }

            return false;
        }

        #region Nested type: CollectFBX

        private class CollectFBX : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "*.fbx";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "fbx";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectMaterial

        private class CollectMaterial : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "*.material";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "mat";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectPrefab

        private class CollectPrefab : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "*.prefab";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "prefab";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectShader

        private class CollectShader : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "*.shader";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "shader";
            }

            #endregion
        }

        #endregion

        #region Nested type: CollectUnity

        private class CollectUnity : IFilterRule
        {
            #region IFilterRule Members

            public string DisplayFilterName => "*.unity";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "unity";
            }

            #endregion
        }

        #endregion
    }
}