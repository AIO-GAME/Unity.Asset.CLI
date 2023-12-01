/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;

namespace AIO.UEditor
{
    [Serializable]
    public class AssetCollectGroup
    {
        /// <summary>
        /// 组名
        /// </summary>
        public string Name;

        /// <summary>
        /// 组描述
        /// </summary>
        public string Description;

        /// <summary>
        /// 资源标签 使用;分割
        /// </summary>
        public string Tags;

        /// <summary>
        /// 资源收集配置
        /// </summary>
        public AssetCollectItem[] Collectors;
    }
}