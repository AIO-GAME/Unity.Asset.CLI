using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AIO;
using AIO.UEngine;

#if SUPPORT_WHOOTHOT

namespace Rol.Game
{
    public partial class WhootAssetProxy : AssetProxy
    {
        /// <summary>
        /// 获取下载器
        /// </summary>
        /// <returns></returns>
        public override IASDownloader GetDownloader()
        {
            // var dic = new Dictionary<string, YAssetPackage>();
            // if (GetPackages == null) return new YASDownloader(dic);
            //
            // foreach (var item in GetPackages.Invoke())
            //     dic.Add(item.Name, YAssetSystem.GetPackage(item.Name));
            // return new YASDownloader(dic);
            return null;
        }

        /// <summary>
        /// 释放资源句柄
        /// </summary>
        public override void FreeHandle(string location)
        {
            ResourceManager.ReleaseAsset(location);
        }

        public override void UnloadUnusedAssets()
        {
            throw new NotImplementedException();
        }

        public override void ForceUnloadALLAssets()
        {
            throw new NotImplementedException();
        }

        #region 资源信息

        /// <summary>
        /// 是否需要从远端更新下载
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public override bool IsNeedDownloadFromRemote(in string location)
        {
            return false;
        }

        /// <summary>
        /// 根据资源标签获取资源信息
        /// </summary>
        /// <param name="tag">资源标签</param>
        public override ICollection<string> GetAssetInfos(in string tag)
        {
            return Array.Empty<string>();
        }

        /// <summary>
        /// 根据资源标签获取资源信息
        /// </summary>
        /// <param name="tag">资源标签</param>
        public override ICollection<string> GetAssetInfos(ICollection<string> tag)
        {
            return Array.Empty<string>();
        }

        /// <summary>
        /// 检查资源定位地址是否有效
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public override bool CheckLocationValid(in string location)
        {
            return false;
        }

        /// <summary>
        /// 获取本地包裹的版本信息
        /// </summary>
        public override string GetPackageVersionDefault()
        {
            return string.Empty;
        }

        #endregion
    }
}
#endif