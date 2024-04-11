using System;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace AIO.UEditor
{
    partial class AssetCollectRoot
    {
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
        /// <param name="limitPackage">限制包名 只查找指定包资源 空则忽略</param>
        /// <returns>
        ///     Item1 包名
        ///     Item2 组名
        ///     Item3 可寻址路径
        /// </returns>
        public static Tuple<string, string, string> AssetToAddress(string assetPath, string limitPackage = "")
        {
            if (string.IsNullOrEmpty(assetPath))
                return new Tuple<string, string, string>(string.Empty, string.Empty, string.Empty);
            var root = GetOrCreate();
            foreach (var package in root.Packages)
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
                        var address = item.GetAddress(assetPath);
                        if (string.IsNullOrEmpty(address)) continue;
                        return new Tuple<string, string, string>(package.Name, group.Name, address);
                    }
                }
            }

            return new Tuple<string, string, string>(string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        ///     根据资源路径查找资源可寻址路径
        /// </summary>
        /// <returns>
        ///     Item1 包名
        ///     Item2 组名
        ///     Item3 可寻址路径
        /// </returns>
        public static Tuple<string, string, string> GUIDToAddress(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return new Tuple<string, string, string>(string.Empty, string.Empty, string.Empty);
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
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
                        if (!assetPath.StartsWith(item.CollectPath)) continue;
                        var address = item.GetAddress(assetPath);
                        if (string.IsNullOrEmpty(address)) continue;
                        return new Tuple<string, string, string>(package.Name, group.Name, address);
                    }
                }
            }

            return new Tuple<string, string, string>(string.Empty, string.Empty, string.Empty);
        }

        private static Tuple<string, string, string> Empty = new Tuple<string, string, string>(string.Empty, string.Empty, string.Empty);

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
        /// <returns>
        ///     Item1 包名
        ///     Item2 组名
        ///     Item3 可寻址路径
        /// </returns>
        public static Tuple<string, string, string> ObjToAddress(Object guid)
        {
            if (!guid) return Empty;
            var assetPath = AssetDatabase.GetAssetPath(guid);
            if (!File.Exists(assetPath)) return Empty;

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
                        if (!assetPath.StartsWith(item.CollectPath)) continue;
                        var address = item.GetAddress(assetPath); // 是否是否被过滤
                        if (string.IsNullOrEmpty(address)) continue;
                        return new Tuple<string, string, string>(package.Name, group.Name, address);
                    }
                }
            }

            return Empty;
        }
    }
}