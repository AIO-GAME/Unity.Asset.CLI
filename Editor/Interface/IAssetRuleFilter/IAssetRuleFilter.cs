namespace AIO.UEditor
{
    public interface IAssetRuleFilter
    {
        /// <summary>
        ///     显示的过滤器名称
        /// </summary>
        string DisplayFilterName { get; }

        /// <summary>
        ///    显示顺序
        /// </summary>
        int DisplayIndex { get; }
        
        /// <summary>
        ///     验证资源是否符合规则
        /// </summary>
        bool IsCollectAsset(AssetRuleData Data);
    }
}