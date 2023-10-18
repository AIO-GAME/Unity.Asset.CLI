/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System.Collections.Generic;

namespace AIO.UEngine
{
    public partial class AssetProxy
    {
        /// <summary>
        /// 获取下载器
        /// </summary>
        /// <returns></returns>
        public abstract IASDownloader GetDownloader();

        /// <summary>
        /// 释放资源句柄
        /// </summary>
        public abstract void FreeHandle(string location);

        #region 资源信息

        /// <summary>
        /// 是否需要从远端更新下载
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public abstract bool IsNeedDownloadFromRemote(in string location);

        /// <summary>
        /// 根据资源标签获取资源信息
        /// </summary>
        /// <param name="tag">资源标签</param>
        public abstract ICollection<string> GetAssetInfos(in string tag);

        /// <summary>
        /// 根据资源标签获取资源信息
        /// </summary>
        /// <param name="tag">资源标签</param>
        public abstract ICollection<string> GetAssetInfos(ICollection<string> tag);

        /// <summary>
        /// 检查资源定位地址是否有效
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        public abstract bool CheckLocationValid(in string location);

        /// <summary>
        /// 获取本地包裹的版本信息
        /// </summary>
        public abstract string GetPackageVersionDefault();

        #endregion
    }
}
