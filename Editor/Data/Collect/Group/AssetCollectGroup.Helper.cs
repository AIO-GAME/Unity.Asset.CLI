using System;
using System.Linq;
using UnityEditor;

namespace AIO.UEditor
{
    partial class AssetCollectGroup
    {
        /// <summary>
        ///    遍历
        /// </summary>
        /// <param name="action">操作</param>
        public void ForEach(Action<AssetCollectItem> action)
        {
            if (Collectors is null) Collectors = Array.Empty<AssetCollectItem>();
            foreach (var collect in Collectors) action(collect);
        }

        /// <summary>
        ///   排序
        /// </summary>
        /// <param name="isAscending">是否升序</param>
        public void Sort(bool isAscending = true)
        {
            if (Collectors is null)
            {
                Collectors = Array.Empty<AssetCollectItem>();
                return;
            }

            if (isAscending)
                Array.Sort(Collectors, (a, b) => string.Compare(a.CollectPath, b.CollectPath, StringComparison.CurrentCulture));
            else
                Array.Sort(Collectors, (a, b) => string.Compare(b.CollectPath, a.CollectPath, StringComparison.CurrentCulture));
        }

        /// <param name="collectPath">收集器资源路径 会被转化成 GUID</param>
        /// <returns>收集器</returns>
        public AssetCollectItem GetByPath(string collectPath)
        {
            if (Collectors is null)
            {
                Collectors = Array.Empty<AssetCollectItem>();
                return null;
            }

            var guid = AssetDatabase.AssetPathToGUID(collectPath);
            return Collectors.Where(collectItem => collectItem != null).FirstOrDefault(collectItem => collectItem.GUID == guid);
        }

        /// <param name="guid">收集器资源路径GUID</param>
        /// <returns>收集器</returns>
        public AssetCollectItem GetByGUID(string guid)
        {
            if (Collectors is null)
            {
                Collectors = Array.Empty<AssetCollectItem>();
                return null;
            }

            return Collectors.Where(collectItem => collectItem != null).FirstOrDefault(collectItem => collectItem.GUID == guid);
        }
    }
}