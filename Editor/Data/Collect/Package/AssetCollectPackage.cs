using System;
using System.Collections.Generic;
using System.Linq;

namespace AIO.UEditor
{
    /// <summary>
    ///     资源收集包
    /// </summary>
    [Serializable]
    public sealed partial class AssetCollectPackage
    {
        /// <summary>
        ///    是否启用
        /// </summary>
        public bool Enable = true;

        /// <summary>
        ///     包名
        /// </summary>
        public string Name;

        /// <summary>
        ///     包描述
        /// </summary>
        public string Description;

        /// <summary>
        ///     资源组配置
        /// </summary>
        public AssetCollectGroup[] Groups;

        /// <summary>
        ///    是否为默认包
        /// </summary>
        public bool Default;

        /// <summary>
        ///    资源标签列表
        /// </summary>
        public string[] Tags
        {
            get
            {
                if (Groups is null)
                {
                    Groups = Array.Empty<AssetCollectGroup>();
                    return Array.Empty<string>();
                }

                var dictionary = new List<string>();
                foreach (var group in Groups.Where(group => group != null))
                {
                    if (!string.IsNullOrEmpty(group.Tag))
                    {
                        dictionary.AddRange(group.Tag.Split(';', ' ', ','));
                    }

                    if (group.Collectors is null)
                    {
                        group.Collectors = Array.Empty<AssetCollectItem>();
                        continue;
                    }

                    dictionary.AddRange(group.Collectors.Where(collect => !string.IsNullOrEmpty(collect.Tags))
                                             .SelectMany(collect => collect.Tags.Split(';', ' ', ','))
                                       );
                }

                return dictionary.Distinct().ToArray();
            }
        }

        public void Save()
        {
            if (Groups is null) Groups = Array.Empty<AssetCollectGroup>();
            foreach (var group in Groups) group.Save();
        }

        public void Dispose()
        {
            if (Groups is null) return;
            foreach (var item in Groups) item.Dispose();
        }

        public override int GetHashCode() => (this as IEqualityComparer<AssetCollectPackage>).GetHashCode(this);
    }
}