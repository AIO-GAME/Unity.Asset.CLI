/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-06
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

namespace AIO.UEditor
{
    public struct AssetRulePackResult
    {
        public string BundleName { get; }
        public string BundleExtension { get; }

        public AssetRulePackResult(string bundleName, string bundleExtension)
        {
            BundleName = bundleName;
            BundleExtension = bundleExtension;
        }
    }

    /// <summary>
    /// 资源打包规则接口
    /// </summary>
    public interface IAssetRulePack
    {
        string DisplayPackName { get; }

        /// <summary>
        /// 获取打包规则结果
        /// </summary>
        AssetRulePackResult GetPackRuleResult(AssetRuleData data);

        /// <summary>
        /// 是否为原生文件打包规则
        /// </summary>
        bool IsRawFilePackRule();
    }
}