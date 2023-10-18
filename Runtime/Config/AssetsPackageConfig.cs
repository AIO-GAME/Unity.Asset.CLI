namespace AIO.UEngine
{
    using System;

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
        /// 资源包动态下载加载
        /// </summary>
        public bool IsSidePlayWithDownload = false;

        public override string ToString()
        {
            return string.Format("Name : {0} , Version : {1} , IsDefault : {2} , IsSidePlayWithDownload : {3}", Name,
                Version, IsDefault, IsSidePlayWithDownload);
        }
    }
}