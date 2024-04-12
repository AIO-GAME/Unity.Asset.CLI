using System;
using System.Collections.Generic;
using System.Linq;

namespace AIO.UEditor
{
    /// <summary>
    ///     资源收集包
    /// </summary>
    [Serializable]
    public sealed class AssetCollectPackage : IDisposable, IEqualityComparer<AssetCollectPackage>
    {
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
        ///     获取组数量
        /// </summary>
        public int Length
        {
            get
            {
                if (Groups is null) Groups = Array.Empty<AssetCollectGroup>();
                return Groups.Length;
            }
        }

        public AssetCollectGroup this[int idxG]
        {
            get => Groups[idxG];
            set => Groups[idxG] = value;
        }

        public AssetCollectItem this[int idxG, int idxC]
        {
            get => Groups[idxG].Collectors[idxC];
            set => Groups[idxG].Collectors[idxC] = value;
        }

        public string[] AllTags
        {
            get
            {
                if (Groups is null)
                {
                    Groups = Array.Empty<AssetCollectGroup>();
                    return Array.Empty<string>();
                }

                var dictionary = new List<string>();
                foreach (var group in Groups)
                {
                    if (group is null) continue;
                    if (group.Collectors is null) group.Collectors = Array.Empty<AssetCollectItem>();

                    dictionary.AddRange(group.Collectors.
                                              Where(collect => !string.IsNullOrEmpty(collect.Tags)).
                                              SelectMany(collect => collect.Tags.Split(';', ' ', ',')));

                    if (string.IsNullOrEmpty(group.Tags)) continue;
                    dictionary.AddRange(group.Tags.Split(';', ' ', ','));
                }

                return dictionary.Distinct().ToArray();
            }
        }

        public void Dispose()
        {
            if (Groups is null) Groups = Array.Empty<AssetCollectGroup>();
            foreach (var item in Groups) item.Dispose();
        }

        public bool Equals(AssetCollectPackage x, AssetCollectPackage y)
        {
            if (x is null) return y is null;
            if (y is null) return false;
            return x.GetHashCode() == y.GetHashCode();
        }

        public int GetHashCode(AssetCollectPackage obj)
        {
            if (obj.Equals(null)) return 0;
            unchecked
            {
                var hashCode = (obj.Name.GetHashCode() * 397) ^ (!string.IsNullOrEmpty(obj.Name) ? obj.Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (!string.IsNullOrEmpty(obj.Description) ? obj.Description.GetHashCode() : 0);
                return obj.Groups is null
                    ? hashCode
                    : obj.Groups.Aggregate(hashCode, (current, item) => (current * 397) ^ item.GetHashCode());
            }
        }

        /// <summary>
        ///     获取资源收集组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns>收集组</returns>
        public AssetCollectGroup GetByGroupName(string groupName)
        {
            if (Groups is null)
            {
                Groups = Array.Empty<AssetCollectGroup>();
                return null;
            }

            return Groups.Where(group => group != null).FirstOrDefault(group => group.Name == groupName);
        }

        public void Save()
        {
            if (Groups is null) Groups = Array.Empty<AssetCollectGroup>();
            foreach (var group in Groups) group.Save();
        }

        public override int GetHashCode() => GetHashCode(this);
    }
}