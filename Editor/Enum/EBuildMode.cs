using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    ///     资源包流水线的构建模式
    /// </summary>
    public enum EBuildMode
    {
        /// <summary>
        ///     强制重建模式
        /// </summary>
        [InspectorName("强制重建模式")] ForceRebuild,

        /// <summary>
        ///     增量构建模式
        /// </summary>
        [InspectorName("增量构建模式")] IncrementalBuild,

        /// <summary>
        ///     演练构建模式
        /// </summary>
        [InspectorName("演练构建模式")] DryRunBuild,

        /// <summary>
        ///     模拟构建模式
        /// </summary>
        [InspectorName("模拟构建模式")] SimulateBuild
    }
}