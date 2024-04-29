using UnityEngine;

namespace AIO.UEditor
{
    /// <summary>
    ///     资源定位格式
    /// </summary>
    public enum EAssetLocationFormat
    {
        /// <summary>
        ///     默认
        /// </summary>
        [InspectorName("默认")]
        None,

        /// <summary>
        ///     全部 小写
        /// </summary>
        [InspectorName("小写")]
        ToLower,

        /// <summary>
        ///     全部 大写
        /// </summary>
        [InspectorName("大写")]
        ToUpper
    }
}