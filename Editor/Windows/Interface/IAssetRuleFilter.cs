/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-04
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

namespace AIO.UEditor
{
    public interface IAssetRuleFilter
    {
        string DisplayFilterName { get; }
        
        /// <summary>
        /// 验证资源是否符合规则
        /// </summary>
        bool IsCollectAsset(AssetInfoData Data);
    }
}