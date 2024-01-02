/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2024-01-02
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using UnityEngine;

namespace AIO.UEditor
{
    public enum EAssetCollectItemType
    {
        [InspectorName("动态资源")] MainAssetCollector,
        [InspectorName("依赖资源")] DependAssetCollector,
        [InspectorName("静态资源")] StaticAssetCollector,
    }
}