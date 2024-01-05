using System;
using UnityEngine;

namespace AIO.UEngine
{
    public enum ECompressMode
    {
        [InspectorName("Uncompressed")] None = 0,
        [InspectorName("LZMA")] LZMA,
        [InspectorName("LZ4")] LZ4,
    }

    [Serializable]
    public class AssetsPackageConfig
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;

        /// <summary>
        /// 版本
        /// </summary>
        public string Version;

        /// <summary>
        /// 是否为默认包
        /// </summary>
        public bool IsDefault;

        /// <summary>
        /// 压缩方式
        /// </summary>
        public ECompressMode CompressMode;

        public override string ToString()
        {
            return string.Format(
                "Name : {0} , Version : {1} , IsDefault : {2} , CompressMode : {3}",
                Name,
                Version,
                IsDefault,
                CompressMode
            );
        }
    }
}