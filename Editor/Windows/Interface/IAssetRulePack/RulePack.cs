using System;
using System.IO;
using UnityEditor;
using PackRuleResult = AIO.UEditor.AssetRulePackResult;
using PackRuleData = AIO.UEditor.AssetRuleData;

namespace AIO.UEditor
{
    internal static class DefaultPackRule
    {
        /// <summary>
        ///     AssetBundle文件的后缀名
        /// </summary>
        public const string AssetBundleFileExtension = "bundle";

        /// <summary>
        ///     原生文件的后缀名
        /// </summary>
        public const string RawFileExtension = "rawfile";

        /// <summary>
        ///     Unity着色器资源包名称
        /// </summary>
        public const string ShadersBundleName = "unityshaders";

        public static PackRuleResult CreateShadersPackRuleResult()
        {
            return new PackRuleResult(ShadersBundleName, AssetBundleFileExtension);
        }
    }

    /// <summary>
    ///     以文件路径作为资源包名
    /// </summary>
    /// <remarks>
    ///     注意：每个文件独自打资源包
    ///     例如："Assets/UIPanel/Shop/Image/background.png" --> "assets_uipanel_shop_image_background.bundle"
    ///     例如："Assets/UIPanel/Shop/View/main.prefab" --> "assets_uipanel_shop_view_main.bundle"
    /// </remarks>
    internal class PackSeparately : IAssetRulePack
    {
        #region IAssetRulePack Members

        public int Priority => 0;

        public string DisplayPackName => "资源打包规则 = 文件路径";

        PackRuleResult IAssetRulePack.GetPackRuleResult(PackRuleData data)
        {
            var bundleName = AHelper.IO.GetFileName(data.AssetPath, false);
            var result = new PackRuleResult(bundleName, DefaultPackRule.AssetBundleFileExtension);
            return result;
        }

        bool IAssetRulePack.IsRawFilePackRule()
        {
            return false;
        }

        #endregion
    }

    /// <summary>
    ///     以父类文件夹路径作为资源包名
    ///     注意：文件夹下所有文件打进一个资源包
    ///     例如："Assets/UIPanel/Shop/Image/backgroud.png" --> "assets_uipanel_shop_image.bundle"
    ///     例如："Assets/UIPanel/Shop/View/main.prefab" --> "assets_uipanel_shop_view.bundle"
    /// </summary>
    internal class PackDirectory : IAssetRulePack
    {
        #region IAssetRulePack Members

        public string DisplayPackName => "资源打包规则 = 父类文件夹路径";

        public int Priority => 1;

        PackRuleResult IAssetRulePack.GetPackRuleResult(PackRuleData data)
        {
            var bundleName = Path.GetDirectoryName(data.AssetPath);
            var result = new PackRuleResult(bundleName, DefaultPackRule.AssetBundleFileExtension);
            return result;
        }

        bool IAssetRulePack.IsRawFilePackRule()
        {
            return false;
        }

        #endregion
    }

    /// <summary>
    ///     以收集器路径下顶级文件夹为资源包名
    ///     注意：文件夹下所有文件打进一个资源包
    ///     例如：收集器路径为 "Assets/UIPanel"
    ///     例如："Assets/UIPanel/Shop/Image/backgroud.png" --> "assets_uipanel_shop.bundle"
    ///     例如："Assets/UIPanel/Shop/View/main.prefab" --> "assets_uipanel_shop.bundle"
    /// </summary>
    internal class PackTopDirectory : IAssetRulePack
    {
        #region IAssetRulePack Members

        public string DisplayPackName => "资源打包规则 = 收集器下顶级文件夹路径";

        public int Priority => 2;

        PackRuleResult IAssetRulePack.GetPackRuleResult(PackRuleData data)
        {
            var assetPath = data.AssetPath.Replace(data.CollectPath, string.Empty);
            assetPath = assetPath.TrimStart('/');
            var splits = assetPath.Split('/');
            if (splits.Length > 0)
            {
                if (Path.HasExtension(splits[0]))
                    throw new Exception($"Not found root directory : {assetPath}");
                var bundleName = $"{data.CollectPath}/{splits[0]}";
                var result = new PackRuleResult(bundleName, DefaultPackRule.AssetBundleFileExtension);
                return result;
            }

            throw new Exception($"Not found root directory : {assetPath}");
        }

        bool IAssetRulePack.IsRawFilePackRule()
        {
            return false;
        }

        #endregion
    }

    /// <summary>
    ///     以收集器路径作为资源包名
    ///     注意：收集的所有文件打进一个资源包
    /// </summary>
    internal class PackCollector : IAssetRulePack
    {
        #region IAssetRulePack Members

        public string DisplayPackName => "资源打包规则 = 收集器路径";

        public int Priority => 3;

        PackRuleResult IAssetRulePack.GetPackRuleResult(PackRuleData data)
        {
            var collectPath = data.CollectPath;
            var bundleName = AssetDatabase.IsValidFolder(collectPath)
                ? collectPath
                : AHelper.IO.GetFileName(collectPath, false);
            var result = new PackRuleResult(bundleName, DefaultPackRule.AssetBundleFileExtension);
            return result;
        }

        bool IAssetRulePack.IsRawFilePackRule()
        {
            return false;
        }

        #endregion
    }

    /// <summary>
    ///     以分组名称作为资源包名
    ///     注意：收集的所有文件打进一个资源包
    /// </summary>
    internal class PackGroup : IAssetRulePack
    {
        #region IAssetRulePack Members

        public string DisplayPackName => "资源打包规则 = 分组名称";

        public int Priority => 4;

        PackRuleResult IAssetRulePack.GetPackRuleResult(PackRuleData data)
        {
            var bundleName = data.GroupName;
            var result = new PackRuleResult(bundleName, DefaultPackRule.AssetBundleFileExtension);
            return result;
        }

        bool IAssetRulePack.IsRawFilePackRule()
        {
            return false;
        }

        #endregion
    }

    /// <summary>
    ///     以整包名称作为资源包名
    ///     注意：收集的所有文件打进一个资源包
    /// </summary>
    internal class PackPackage : IAssetRulePack
    {
        #region IAssetRulePack Members

        public string DisplayPackName => "资源打包规则 = 整包名称";

        public int Priority => 5;

        PackRuleResult IAssetRulePack.GetPackRuleResult(PackRuleData data)
        {
            var bundleName = data.PackageName;
            var result = new PackRuleResult(bundleName, DefaultPackRule.AssetBundleFileExtension);
            return result;
        }

        bool IAssetRulePack.IsRawFilePackRule()
        {
            return false;
        }

        #endregion
    }

    /// <summary>
    ///     打包原生文件
    /// </summary>
    internal class PackRawFile : IAssetRulePack
    {
        #region IAssetRulePack Members

        public string DisplayPackName => "资源打包规则 = 打包原生文件";

        public int Priority => 6;

        PackRuleResult IAssetRulePack.GetPackRuleResult(PackRuleData data)
        {
            var bundleName = data.AssetPath;
            var result = new PackRuleResult(bundleName, DefaultPackRule.RawFileExtension);
            return result;
        }

        bool IAssetRulePack.IsRawFilePackRule()
        {
            return true;
        }

        #endregion
    }

    /// <summary>
    ///     打包着色器变种集合
    /// </summary>
    internal class PackShaderVariants : IAssetRulePack
    {
        #region IAssetRulePack Members

        public string DisplayPackName => "资源打包规则 = 打包着色器变种集合文件";

        public int Priority => 7;

        public PackRuleResult GetPackRuleResult(PackRuleData data)
        {
            return DefaultPackRule.CreateShadersPackRuleResult();
        }

        bool IAssetRulePack.IsRawFilePackRule()
        {
            return false;
        }

        #endregion
    }
}