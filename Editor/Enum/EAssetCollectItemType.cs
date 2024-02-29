using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    /// 资源收集类型
    /// </summary>
    public enum EAssetCollectItemType
    {
        /// <summary>
        /// 可寻址资源
        /// </summary>
        [InspectorName("动态资源")] MainAssetCollector,

        /// <summary>
        /// 动态资源的依赖资源 无法单独使用 无可寻址路径
        /// </summary>
        [InspectorName("依赖资源")] DependAssetCollector,

        /// <summary>
        /// 静态资源 无法单独使用 无可寻址路径
        /// </summary>
        [InspectorName("静态资源")] StaticAssetCollector,
    }
}