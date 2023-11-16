﻿/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

#if SUPPORT_YOOASSET
using System.Collections.Generic;
using AIO.UEngine.YooAsset;

namespace AIO.UEngine
{
    public partial class YAssetProxy
    {
        /// <summary>
        /// 获取下载器
        /// </summary>
        /// <returns></returns>
        public override IASDownloader GetDownloader()
        {
            var dic = new Dictionary<string, YAssetPackage>();
            var config = AssetSystem.PackageConfigs;
            if (config is null) return new YASDownloader(dic);
            foreach (var item in config)
                dic.Add(item.Name, YAssetSystem.GetPackage(item.Name));
            return new YASDownloader(dic);
        }

        /// <summary>
        /// 释放资源句柄
        /// </summary>
        public override void FreeHandle(string location)
        {
            YAssetSystem.FreeHandle(location);
        }

        public override void UnloadUnusedAssets()
        {
            YAssetSystem.UnloadUnusedALLAssets();
        }

        public override void ForceUnloadALLAssets()
        {
            YAssetSystem.ForceUnloadALLAssets();
        }

        #region 资源信息

        /// <summary>
        /// 是否需要从远端更新下载
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public override bool IsNeedDownloadFromRemote(in string location)
        {
            return YAssetSystem.IsNeedDownloadFromRemote(location);
        }

        /// <summary>
        /// 根据资源标签获取资源信息
        /// </summary>
        /// <param name="tag">资源标签</param>
        public override ICollection<string> GetAssetInfos(ICollection<string> tag)
        {
            var list2 = new List<string>();
            foreach (var info in YAssetSystem.GetAssetInfos(tag)) list2.Add(info.Address);
            return list2;
        }

        /// <summary>
        /// 检查资源定位地址是否有效
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public override bool CheckLocationValid(in string location)
        {
            return YAssetSystem.CheckLocationValid(location);
        }

        /// <summary>
        /// 获取本地包裹的版本信息
        /// </summary>
        public override string GetPackageVersionDefault()
        {
            return YAssetSystem.GetPackageVersionDefault();
        }

        #endregion
    }
}
#endif