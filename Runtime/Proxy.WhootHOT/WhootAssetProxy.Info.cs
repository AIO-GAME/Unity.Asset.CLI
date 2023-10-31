#if SUPPORT_WHOOTHOT

using System;
using System.Collections.Generic;
using AIO;
using AIO.UEngine;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
            throw new NotImplementedException();
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
            Addressables.CleanBundleCache();
        }

        public override void ForceUnloadALLAssets()
        {
            Addressables.ClearResourceLocators();
        }

        #region 资源信息

        /// <summary>
        /// 是否需要从远端更新下载
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public override bool IsNeedDownloadFromRemote(in string location)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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