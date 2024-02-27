/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2024-01-02
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    /// 资源收集状态类型
    /// </summary>
    public enum EAssetLoadType
    {
        /// <summary>
        /// 始终收集
        /// </summary>
        [InspectorName("始终")] Always,
        /// <summary>
        /// 运行时收集（发布版）
        /// </summary>
        [InspectorName("运行时")] Runtime,
        /// <summary>
        /// 编辑器时收集（编辑版）
        /// </summary>
        [InspectorName("编辑时")] Editor,
    }
}