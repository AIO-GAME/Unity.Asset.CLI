/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2024-01-02
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using UnityEngine;

namespace AIO.UEditor
{
    public enum EAssetLoadType
    {
        [InspectorName("始终")] Always,
        [InspectorName("运行时")] Runtime,
        [InspectorName("编辑时")] Editor,
    }
}