/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2024-01-02
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using UnityEngine;

namespace AIO.UEditor
{
    public enum EAssetLocationFormat
    {
        [InspectorName("默认")] None,
        [InspectorName("小写")] ToLower,
        [InspectorName("大写")] ToUpper,
    }
}