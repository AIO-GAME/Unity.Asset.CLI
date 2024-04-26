using System;
using System.Linq;

namespace AIO.UEditor
{
    partial class AssetCollectPackage
    {
        /// <summary>
        ///    遍历操作
        /// </summary>
        public void ForEach(Action<AssetCollectGroup> action)
        {
            if (Groups is null) return;
            foreach (var package in Groups) action?.Invoke(package);
        }

        /// <summary>
        ///   排序
        /// </summary>
        /// <param name="isAscending">是否升序</param>
        public void Sort(bool isAscending = true)
        {
            if (Groups is null)
            {
                Groups = Array.Empty<AssetCollectGroup>();
                return;
            }

            if (isAscending)
                Array.Sort(Groups, (a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCulture));
            else
                Array.Sort(Groups, (a, b) => string.Compare(b.Name, a.Name, StringComparison.CurrentCulture));
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
    }
}