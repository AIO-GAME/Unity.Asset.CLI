#region

using System.ComponentModel;
using UnityEngine;

#endregion

namespace AIO.UEngine
{
    [Description("资源加载模式")]
    public enum EASMode
    {
        /// <summary>
        ///     编辑器模式
        /// </summary>
        [InspectorName("编辑器模式")]
        Editor,

        /// <summary>
        ///     远端模式
        /// </summary>
        [InspectorName("远端模式")]
        Remote,

        /// <summary>
        ///     本地模式
        /// </summary>
        [InspectorName("本地模式")]
        Local,
    }
}