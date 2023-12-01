/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;

namespace AIO.UEditor
{
    /// <summary>
    /// nameof(AssetCollectPackage)
    /// </summary>
    [Serializable]
    public class AssetCollectPackage
    {
        /// <summary>
        /// 包名
        /// </summary>
        public string Name;

        /// <summary>
        /// 包描述
        /// </summary>
        public string Description;

        /// <summary>
        /// 资源收集配置
        /// </summary>
        public AssetCollectGroup[] Groups;
    }
}