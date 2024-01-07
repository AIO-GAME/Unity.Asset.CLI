#if SUPPORT_YOOASSET
using System.Collections.Generic;
using System.Linq;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    internal partial class YAssetSystem
    {
        /// <summary>
        /// 是否需要从远端更新下载
        /// </summary>
        /// <param name="name">包名</param>
        /// <param name="location">资源的定位地址</param>
        public static bool IsNeedDownloadFromRemote(string name, string location)
        {
            if (AssetSystem.Parameter.ASMode != EASMode.Remote) return false;
            return Dic.TryGetValue(name, out var asset) && asset.IsNeedDownloadFromRemote(location);
        }

        /// <summary>
        /// 是否需要从远端更新下载
        /// </summary>
        /// <param name="name">包名</param>
        /// <param name="assetInfo">资源的定位地址</param>
        public static bool IsNeedDownloadFromRemote(string name, AssetInfo assetInfo)
        {
            if (AssetSystem.Parameter.ASMode != EASMode.Remote) return false;
            return Dic.TryGetValue(name, out var asset) && asset.IsNeedDownloadFromRemote(assetInfo);
        }

        /// <summary>
        /// 是否需要从远端更新下载
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static bool IsNeedDownloadFromRemote(string location)
        {
            if (AssetSystem.Parameter.ASMode != EASMode.Remote) return false;
            return (from package in Dic.Values
                where package.CheckLocationValid(location)
                select package.IsNeedDownloadFromRemote(location)).FirstOrDefault();
        }

        /// <summary>
        /// 获取资源信息列表
        /// </summary>
        /// <param name="tag">资源标签</param>
        public static ICollection<AssetInfo> GetAssetInfos(string tag)
        {
            var list = new List<AssetInfo>();
            foreach (var asset in Dic.Values) list.AddRange(asset.GetAssetInfos(tag));
            return list;
        }

        /// <summary>
        /// 获取资源信息列表
        /// </summary>
        /// <param name="tag">资源标签</param>
        public static ICollection<AssetInfo> GetAssetInfos(IEnumerable<string> tag)
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
        public static AssetInfo[] GetAssetInfos(string name, string tag)
        {
            return !Dic.TryGetValue(name, out var asset) ? null : asset.GetAssetInfos(tag);
        }

        /// <summary>
        /// 获取资源信息列表
        /// </summary>
        /// <param name="name">包名</param>
        /// <param name="tags">资源标签列表</param>
        public static AssetInfo[] GetAssetInfos(string name, string[] tags)
        {
            return !Dic.TryGetValue(name, out var asset) ? null : asset.GetAssetInfos(tags);
        }

        /// <summary>
        /// 获取资源信息
        /// </summary>
        /// <param name="name">包名</param>
        /// <param name="location">资源的定位地址</param>
        public static AssetInfo GetAssetInfo(string name, string location)
        {
            return !Dic.TryGetValue(name, out var asset) ? null : asset.GetAssetInfo(location);
        }

        /// <summary>
        /// 获取资源信息列表
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static AssetInfo GetAssetInfo(string location)
        {
            return (
                from asset in Dic.Values
                where asset.CheckLocationValid(location)
                select asset.GetAssetInfo(location)
            ).FirstOrDefault();
        }

        /// <summary>
        /// 检查资源定位地址是否有效
        /// </summary>
        /// <param name="name">包名</param>
        /// <param name="location">资源的定位地址</param>
        public static bool CheckLocationValid(string name, string location)
        {
            return Dic.TryGetValue(name, out var asset) && asset.CheckLocationValid(location);
        }

        /// <summary>
        /// 检查资源定位地址是否有效
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public static bool CheckLocationValid(string location)
        {
            return Dic.Values.Any(asset => asset.CheckLocationValid(location));
        }
    }
}
#endif