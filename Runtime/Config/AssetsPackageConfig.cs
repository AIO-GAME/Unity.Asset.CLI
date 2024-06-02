#region

using System;
using UnityEngine;

#endregion

namespace AIO.UEngine
{
    public enum ECompressMode
    {
        [InspectorName("Uncompressed")]
        None = 0,

        [InspectorName("LZMA")]
        LZMA,

        [InspectorName("LZ4")]
        LZ4,
    }

    [Serializable]
    public class AssetsPackageConfig
    {
        /// <summary>
        ///     名称
        /// </summary>
        public string Name;

        /// <summary>
        ///     版本
        /// </summary>
        public string Version;

        /// <summary>
        ///     是否为默认包
        /// </summary>
        public bool IsDefault;

        /// <summary>
        ///     压缩方式
        /// </summary>
        public ECompressMode CompressMode;

        /// <summary>
        ///     当前版本
        /// </summary>
        public string CurrentVersion => IsLatest ? "Latest" : Version;

        /// <summary>
        ///     是否为最新版本
        /// </summary>
        public bool IsLatest { get; set; }

        public override string ToString() { return $"Name : {Name} , Version : {Version} , IsDefault : {IsDefault} , CompressMode : {CompressMode}"; }
    }
}