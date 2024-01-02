/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2023-12-01
|*|E-Mail:     |*| xinansky99@foxmail.com
|*|============|*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace AIO.UEditor
{
    /// <summary>
    /// 资源收集包
    /// </summary>
    [Serializable]
    public class AssetCollectPackage : IDisposable
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

        public string[] GetTags()
        {
            var dictionary = new Dictionary<string, bool>();
            foreach (var group in Groups)
            {
                foreach (var collect in group.Collectors)
                {
                    if (string.IsNullOrEmpty(collect.Tags)) continue;
                    foreach (var tag in collect.Tags.Split(';')) dictionary[tag] = true;
                }

                if (string.IsNullOrEmpty(group.Tags)) continue;
                foreach (var tag in group.Tags.Split(';')) dictionary[tag] = true;
            }

            return dictionary.Keys.ToArray();
        }

        public void Dispose()
        {
            if (Groups is null) return;
            foreach (var item in Groups) item.Dispose();
        }
    }
}