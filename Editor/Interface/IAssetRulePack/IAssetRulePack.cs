namespace AIO.UEditor
{
    public struct AssetRulePackResult
    {
        public string BundleName      { get; }
        public string BundleExtension { get; }

        public AssetRulePackResult(string bundleName, string bundleExtension)
        {
            BundleName      = bundleName;
            BundleExtension = bundleExtension;
        }
    }

    /// <summary>
    ///     资源打包规则接口
    /// </summary>
    public interface IAssetRulePack
    {
        /// <summary>
        ///     显示打包规则名称
        /// </summary>
        string DisplayPackName { get; }

        /// <summary>
        ///     打包规则优先级
        /// </summary>
        int Priority { get; }

        /// <summary>
        ///     获取打包规则结果
        /// </summary>
        AssetRulePackResult GetPackRuleResult(AssetRuleData data);

        /// <summary>
        ///     是否为原生文件打包规则
        /// </summary>
        bool IsRawFilePackRule();
    }
}