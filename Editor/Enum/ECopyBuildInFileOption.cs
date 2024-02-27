/*|============|*|
|*|Author:     |*| xinan
|*|Date:       |*| 2024-01-07
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/

using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    /// 资源文件的拷贝方式
    /// </summary>
    public enum ECopyBuildInFileOption
    {
        /// <summary>
        /// 不拷贝任何文件
        /// </summary>
        [InspectorName("None")] None = 0,

        /// <summary>
        /// 先清空已有文件，然后拷贝所有文件
        /// </summary>
        [InspectorName("先清空已有文件，然后拷贝所有文件")] ClearAndCopyAll,

        /// <summary>
        /// 先清空已有文件，然后按照资源标签拷贝文件
        /// </summary>
        [InspectorName("先清空已有文件，然后按照资源标签拷贝文件")]
        ClearAndCopyByTags,

        /// <summary>
        /// 不清空已有文件，直接拷贝所有文件
        /// </summary>
        [InspectorName("不清空已有文件，直接拷贝所有文件")] OnlyCopyAll,

        /// <summary>
        /// 不清空已有文件，直接按照资源标签拷贝文件
        /// </summary>
        [InspectorName("不清空已有文件，直接按照资源标签拷贝文件")]
        OnlyCopyByTags,
    }
}