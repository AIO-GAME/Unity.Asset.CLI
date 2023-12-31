﻿/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-04
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System.Collections.Generic;
using IFilterRule = AIO.UEditor.IAssetRuleFilter;

namespace AIO.UEditor
{
    public partial class RuleCollect
    {
        public class CollectFBX : IFilterRule
        {
            public string DisplayFilterName => "*.fbx";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "fbx";
            }
        }

        public class CollectMaterial : IFilterRule
        {
            public string DisplayFilterName => "*.material";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "mat";
            }
        }

        public class CollectShader : IFilterRule
        {
            public string DisplayFilterName => "*.shader";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "shader";
            }
        }

        public class CollectPrefab : IFilterRule
        {
            public string DisplayFilterName => "*.prefab";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "prefab";
            }
        }

        public class CollectUnity : IFilterRule
        {
            public string DisplayFilterName => "*.unity";

            public bool IsCollectAsset(AssetRuleData data)
            {
                return data.Extension == "unity";
            }
        }

        /// <summary>
        /// 判断是否为自定义收集规则
        /// </summary>
        internal static bool IsCollectAssetCustom(IEnumerable<string> filterCollect, string extension)
        {
            if (filterCollect is null) return false;
            foreach (var collect in filterCollect)
            {
                if (collect[1] == '.' && collect.Substring(2) == extension) return true;
                switch (collect[0])
                {
                    case '.':
                    {
                        if (collect.Substring(1) == extension) return true;
                        break;
                    }
                    case '*':
                    {
                        if (collect.Substring(1) == extension) return true;
                        break;
                    }
                    default:
                    {
                        if (collect == extension) return true;
                        break;
                    }
                }
            }

            return false;
        }
    }
}