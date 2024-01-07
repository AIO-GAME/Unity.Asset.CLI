/*|============|*|
|*|Author:     |*| xinan                
|*|Date:       |*| 2024-01-07               
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/

using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    /// 输出文件名称的样式
    /// </summary>
    public enum EOutputNameStyle
    {
        /// <summary>
        /// 哈希值名称
        /// </summary>
        [InspectorName("哈希值名称")] HashName = 1,

        /// <summary>
        /// 资源包名称 + 哈希值名称
        /// </summary>
        [InspectorName("资源包名称 + 哈希值名称")] BundleName_HashName = 4,
    }
}