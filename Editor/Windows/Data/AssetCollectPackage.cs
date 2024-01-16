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
        /// 资源组配置
        /// </summary>
        public AssetCollectGroup[] Groups;

        /// <summary>
        /// 获取组数量
        /// </summary>
        public int Length
        {
            get
            {
                if (Groups is null) Groups = Array.Empty<AssetCollectGroup>();
                return Groups.Length;
            }
        }
        
        public AssetCollectGroup this[int index]
        {
            get => Groups[index];
            set => Groups[index] = value;
        }

        /// <summary>
        /// 获取资源收集组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns>收集组</returns>
        public AssetCollectGroup GetGroup(string groupName)
        {
            if (Groups is null) Groups = Array.Empty<AssetCollectGroup>();
            return Groups.Where(group => (group != null)).FirstOrDefault(group => group.Name == groupName);
        }

        /// <summary>
        /// 获取所有标签
        /// </summary>
        public string[] GetTags()
        {
            if (Groups is null)
            {
                Groups = Array.Empty<AssetCollectGroup>();
                return Array.Empty<string>();
            }

            var dictionary = new List<string>();
            foreach (var group in Groups)
            {
                dictionary.AddRange(group.Collectors
                    .Where(collect => !string.IsNullOrEmpty(collect.Tags))
                    .SelectMany(collect => collect.Tags.Split(';', ' ', ',')));

                if (string.IsNullOrEmpty(group.Tags)) continue;
                dictionary.AddRange(group.Tags.Split(';', ' ', ','));
            }

            return dictionary.Distinct().ToArray();
        }

        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            if (Groups is null) Groups = Array.Empty<AssetCollectGroup>();
            foreach (var group in Groups) group.Save();
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            if (Groups is null) Groups = Array.Empty<AssetCollectGroup>();
            foreach (var item in Groups) item.Dispose();
        }
    }
}