/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2024-02-27
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/

#if SUPPORT_YOOASSET

using System.Collections.Generic;
using System.Linq;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    /// <summary>
    /// YAssetProxy_Get
    /// </summary>
    partial class Proxy
    {
        public override ICollection<string> GetAddressByTag(IEnumerable<string> tags)
        {
            return GetAssetInfosByTag(tags).Select(info => info.Address).ToArray();
        }

        /// <summary>
        /// 获取本地包裹的版本信息
        /// </summary>
        public string GetPackageVersion(string packageName)
        {
            return Dic.TryGetValue(packageName, out var asset) ? asset.GetPackageVersion() : string.Empty;
        }

        public override string GetPackageVersionDefault()
        {
            return DefaultPackage?.GetPackageVersion();
        }

        /// <summary>
        /// 获取资源信息列表
        /// </summary>
        /// <param name="tag">资源标签</param>
        private IEnumerable<AssetInfo> GetAssetInfosByTag(IEnumerable<string> tag)
        {
            var list = new List<AssetInfo>();
            var tags = tag.ToArray();
            foreach (var asset in Dic.Values) list.AddRange(asset.GetAssetInfos(tags));
            return list;
        }

        /// <summary>
        /// 获取资源信息列表
        /// </summary>
        /// <param name="name">包名</param>
        /// <param name="tag">资源标签</param>
        private AssetInfo[] GetAssetInfosByNameWithTag(string name, string tag)
        {
            return Dic.TryGetValue(name, out var asset) ? asset.GetAssetInfos(tag) : null;
        }

        /// <summary>
        /// 获取资源信息列表
        /// </summary>
        /// <param name="name">包名</param>
        /// <param name="tags">资源标签列表</param>
        private AssetInfo[] GetAssetInfosByTag(string name, string[] tags)
        {
            return Dic.TryGetValue(name, out var asset) ? asset.GetAssetInfos(tags) : null;
        }

        /// <summary>
        /// 获取资源信息
        /// </summary>
        /// <param name="name">包名</param>
        /// <param name="location">资源的定位地址</param>
        private AssetInfo GetAssetInfoByNameWithAddress(string name, string location)
        {
            return Dic.TryGetValue(name, out var asset) ? asset.GetAssetInfo(location) : null;
        }

        /// <summary>
        /// 获取资源信息列表
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        private AssetInfo GetAssetInfoByAddress(string location)
        {
            return (
                from asset in Dic.Values
                where asset.CheckLocationValid(location)
                select asset.GetAssetInfo(location)
            ).FirstOrDefault();
        }
    }
}

#endif