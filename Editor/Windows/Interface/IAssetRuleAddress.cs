namespace AIO.UEditor
{
    /// <summary>
    ///     资源地址化
    /// </summary>
    public interface IAssetRuleAddress
    {
        /// <summary>
        ///     是否允许多线程
        /// </summary>
        bool AllowThread { get; }

        /// <summary>
        ///     显示的资源地址名称
        /// </summary>
        string DisplayAddressName { get; }

        /// <summary>
        ///     获取资源地址
        /// </summary>
        string GetAssetAddress(AssetRuleData Collect);
    }
}