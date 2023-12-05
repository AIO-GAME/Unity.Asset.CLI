/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Linq;

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

        public AssetCollectGroup this[int index]
        {
            get => Groups[index];
            set => Groups[index] = value;
        }

        public AssetCollectGroup GetGroup(string groupName)
        {
            return Groups.Where(group => !(group is null)).FirstOrDefault(group => group.Name == groupName);
        }
    }
}