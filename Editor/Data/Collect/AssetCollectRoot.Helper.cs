using System;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace AIO.UEditor
{
    partial class AssetCollectRoot
    {
        private static Tuple<string, string, string> Empty = new Tuple<string, string, string>(string.Empty, string.Empty, string.Empty);

        /// <summary>
        ///   排序
        /// </summary>
        /// <param name="isAscending">是否升序</param>
        public void Sort(bool isAscending = true)
        {
            if (Packages is null)
            {
                Packages = Array.Empty<AssetCollectPackage>();
                return;
            }

            Packages = isAscending
                ? Packages.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCulture))
                : Packages.Sort((a, b) => string.Compare(b.Name, a.Name, StringComparison.CurrentCulture));
        }

        /// <summary>
        ///   排序
        /// </summary>
        /// <param name="isAscending">是否升序</param>
        public void SortWithGroup(bool isAscending = true)
        {
            if (Packages is null)
            {
                Packages = Array.Empty<AssetCollectPackage>();
                return;
            }

            for (var i = 0; i < Packages.Length; i++)
            {
                if (Packages[i] is null) Packages[i] = new AssetCollectPackage();
                if (Packages[i].Groups is null)
                {
                    Packages[i].Groups = Array.Empty<AssetCollectGroup>();
                    continue;
                }

                if (Packages[i].Groups.Length == 0) continue;

                Packages[i].Groups = isAscending
                    ? Packages[i].Groups.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCulture))
                    : Packages[i].Groups.Sort((a, b) => string.Compare(b.Name, a.Name, StringComparison.CurrentCulture));
            }

            Packages = isAscending
                ? Packages.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCulture))
                : Packages.Sort((a, b) => string.Compare(b.Name, a.Name, StringComparison.CurrentCulture));
        }

        /// <summary>
        ///   排序
        /// </summary>
        /// <param name="isAscending">是否升序</param>
        public void SortWithGroupAndCollect(bool isAscending = true)
        {
            if (Packages is null)
            {
                Packages = Array.Empty<AssetCollectPackage>();
                return;
            }

            for (var i = 0; i < Packages.Length; i++)
            {
                if (Packages[i] is null) Packages[i] = new AssetCollectPackage();
                if (Packages[i].Groups is null)
                {
                    Packages[i].Groups = Array.Empty<AssetCollectGroup>();
                    continue;
                }

                if (Packages[i].Groups.Length == 0) continue;
                for (var j = 0; j < Packages[i].Groups.Length; j++)
                {
                    if (Packages[i].Groups[j].Collectors is null)
                    {
                        Packages[i].Groups[j].Collectors = Array.Empty<AssetCollectItem>();
                        continue;
                    }

                    if (Packages[i].Groups[j].Collectors.Length == 0) continue;
                    Packages[i].Groups[j].Collectors = isAscending
                        ? Packages[i].Groups[j].Collectors.Sort((a, b) => string.Compare(a.CollectPath, b.CollectPath, StringComparison.CurrentCulture))
                        : Packages[i].Groups[j].Collectors.Sort((a, b) => string.Compare(b.CollectPath, a.CollectPath, StringComparison.CurrentCulture));
                }

                Packages[i].Groups = isAscending
                    ? Packages[i].Groups.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCulture))
                    : Packages[i].Groups.Sort((a, b) => string.Compare(b.Name, a.Name, StringComparison.CurrentCulture));
            }

            Packages = isAscending
                ? Packages.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.CurrentCulture))
                : Packages.Sort((a, b) => string.Compare(b.Name, a.Name, StringComparison.CurrentCulture));
        }

        /// <summary>
        ///     获取资源收集配置
        /// </summary>
        public static AssetCollectRoot GetOrCreate()
        {
            if (_Instance) return _Instance;
            var objects = EHelper.IO.GetScriptableObjects<AssetCollectRoot>();
            if (objects != null && objects.Length > 0)
            {
                foreach (var asset in objects)
                {
                    if (asset is null) continue;
                    if (asset.Packages is null)
                        asset.Packages = Array.Empty<AssetCollectPackage>();
                    _Instance = asset;
                    return _Instance;
                }
            }

            _Instance          = CreateInstance<AssetCollectRoot>();
            _Instance.Packages = Array.Empty<AssetCollectPackage>();
            AssetDatabase.CreateAsset(_Instance, "Assets/Editor/AssetCollectRoot.asset");
            AssetDatabase.SaveAssets();
            return _Instance;
        }

        /// <summary>
        ///     根据资源路径查找资源可寻址路径
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        /// <param name="isLower">是否小写</param>
        /// <param name="hasExtension">是否包含后缀</param>
        /// <param name="limitPackage">限制包名 只查找指定包资源 空则忽略</param>
        /// <returns>
        ///     Item1 包名
        ///     Item2 组名
        ///     Item3 可寻址路径
        /// </returns>
        public static Tuple<string, string, string> AssetToAddress(string assetPath, bool isLower, bool hasExtension, string limitPackage = "")
        {
            if (string.IsNullOrEmpty(assetPath)) return Empty;
            foreach (var package in GetOrCreate().Packages)
            {
                if (package is null) continue;
                if (!string.IsNullOrEmpty(limitPackage))
                    if (limitPackage != package.Name)
                        continue;

                foreach (var group in package.Groups)
                {
                    if (group is null) continue;
                    foreach (var item in group.Collectors)
                    {
                        if (item is null) continue;
                        if (item.Type != EAssetCollectItemType.MainAssetCollector) continue;
                        if (!assetPath.StartsWith(item.CollectPath)) continue;
                        item.GroupName   = group.Name;
                        item.PackageName = package.Name;
                        var address = item.GetAddress(assetPath, isLower, hasExtension);
                        if (string.IsNullOrEmpty(address)) continue;
                        return new Tuple<string, string, string>(package.Name, group.Name, address);
                    }
                }
            }

            return Empty;
        }

        /// <summary>
        ///     根据资源路径查找资源可寻址路径
        /// </summary>
        /// <param name="guid">资源GUID</param>
        /// <param name="isLower">是否小写</param>
        /// <param name="hasExtension">是否包含后缀</param>
        /// <returns>
        ///     Item1 包名
        ///     Item2 组名
        ///     Item3 可寻址路径
        /// </returns>
        public static Tuple<string, string, string> GUIDToAddress(string guid, bool isLower, bool hasExtension)
        {
            if (string.IsNullOrEmpty(guid)) return Empty;
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            foreach (var package in GetOrCreate().Packages)
            {
                if (package is null) continue;
                foreach (var group in package.Groups)
                {
                    if (group is null) continue;
                    foreach (var item in group.Collectors)
                    {
                        if (item is null) continue;
                        if (item.Type != EAssetCollectItemType.MainAssetCollector) continue;
                        if (!assetPath.StartsWith(item.CollectPath)) continue;
                        var address = item.GetAddress(assetPath, isLower, hasExtension);
                        if (string.IsNullOrEmpty(address)) continue;
                        return new Tuple<string, string, string>(package.Name, group.Name, address);
                    }
                }
            }

            return Empty;
        }

        public static bool ObjToCollector(Object guid, out AssetCollectItem result)
        {
            if (!guid)
            {
                result = null;
                return false;
            }

            var assetPath = AssetDatabase.GetAssetPath(guid);
            if (!Directory.Exists(assetPath))
            {
                result = null;
                return false;
            }

            var root = GetOrCreate();
            foreach (var package in root.Packages)
            {
                if (package is null) continue;
                foreach (var group in package.Groups)
                {
                    if (group is null) continue;
                    foreach (var item in group.Collectors)
                    {
                        if (item is null) continue;
                        if (item.Type != EAssetCollectItemType.MainAssetCollector) continue;
                        if (assetPath != item.CollectPath) continue;
                        item.PackageName = package.Name;
                        item.GroupName   = group.Name;
                        result           = item;
                        return true;
                    }
                }
            }

            result = null;
            return false;
        }

        /// <summary>
        ///     根据资源路径查找资源可寻址路径
        /// </summary>
        /// <param name="guid">资源GUID</param>
        /// <param name="isLower">是否小写</param>
        /// <param name="hasExtension">是否包含后缀</param>
        /// <returns>
        ///     Item1 包名
        ///     Item2 组名
        ///     Item3 可寻址路径
        /// </returns>
        public static Tuple<string, string, string> ObjToAddress(Object guid, bool isLower, bool hasExtension)
        {
            if (!guid) return Empty;
            var assetPath = AssetDatabase.GetAssetPath(guid);
            if (!File.Exists(assetPath)) return Empty;
            foreach (var package in GetOrCreate().Packages)
            {
                if (package is null) continue;
                foreach (var group in package.Groups)
                {
                    if (group is null) continue;
                    foreach (var item in group.Collectors)
                    {
                        if (item is null) continue;
                        if (item.Type != EAssetCollectItemType.MainAssetCollector) continue;
                        if (!assetPath.StartsWith(item.CollectPath)) continue;
                        var address = item.GetAddress(assetPath, isLower, hasExtension);
                        if (string.IsNullOrEmpty(address)) continue;
                        return new Tuple<string, string, string>(package.Name, group.Name, address);
                    }
                }
            }

            return Empty;
        }
    }
}