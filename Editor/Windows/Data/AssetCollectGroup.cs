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

        public AssetCollectItem this[int index]
        {
            get => Collectors[index];
            set => Collectors[index] = value;
        }

        public AssetCollectItem GetCollector(string collectPath)
        {
            return Collectors
                .Where(collectItem => !(collectItem is null))
                .FirstOrDefault(collectItem => collectItem.CollectPath == collectPath);
        }


        public string[] GetTags()
        {
            var dictionary = new Dictionary<string, bool>();
            foreach (var collect in Collectors)
            {
                if (collect.Type != EAssetCollectItemType.MainAssetCollector) continue;
                if (string.IsNullOrEmpty(collect.Tags)) continue;
                foreach (var tag in collect.Tags.Split(';')) dictionary[tag] = true;
            }

            if (string.IsNullOrEmpty(Tags)) return dictionary.Keys.ToArray();

            foreach (var tag in Tags.Split(';')) dictionary[tag] = true;
            return dictionary.Keys.ToArray();
        }
    }
}