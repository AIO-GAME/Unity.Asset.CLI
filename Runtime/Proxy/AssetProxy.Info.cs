/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System.Collections.Generic;
using System.Diagnostics;

namespace AIO.UEngine
{
    public partial class AssetProxy
    {
        /// <summary>
        /// 获取下载器
        /// </summary>
        /// <returns></returns>
        public abstract IASDownloader GetDownloader(IProgressEvent progress = null);

        /// <summary>
        /// 释放资源句柄
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public abstract void FreeHandle(string location);

        /// <summary>
        /// 释放资源句柄
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public virtual void FreeHandle(IEnumerable<string> locations)
        {
            foreach (var location in locations) FreeHandle(location);
        }

        #region 资源信息

        /// <summary>
        /// 是否需要从远端更新下载
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public abstract bool IsNeedDownloadFromRemote(in string location);

        /// <summary>
        /// 根据资源标签获取资源信息
        /// </summary>
        /// <param name="tag">资源标签</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public ICollection<string> GetAssetInfos(in string tag)
        {
            return GetAssetInfos(new string[] { tag });
        }

        /// <summary>
        /// 根据资源标签获取资源信息
        /// </summary>
        /// <param name="tag">资源标签</param>
        /// <param name="tags"></param>
        [DebuggerNonUserCode, DebuggerHidden]
        public ICollection<string> GetAssetInfos(in string tag, params string[] tags)
        {
            var list = new List<string> { tag };
            list.AddRange(tags);
            return GetAssetInfos(list);
        }

        /// <summary>
        /// 根据资源标签获取资源信息
        /// </summary>
        /// <param name="tags">资源标签</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public abstract ICollection<string> GetAssetInfos(ICollection<string> tags);

        /// <summary>
        /// 检查资源定位地址是否有效
        /// </summary>
        /// <param name="location">资源的定位地址</param>
        [DebuggerNonUserCode, DebuggerHidden]
        public abstract bool CheckLocationValid(in string location);

        /// <summary>
        /// 获取本地包裹的版本信息
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public abstract string GetPackageVersionDefault();

        #endregion
    }
}