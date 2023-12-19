/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Collections;

namespace AIO.UEngine
{
    /// <summary>
    /// 资源管理系统 - 资源代理
    /// </summary>
    public abstract partial class AssetProxy : IDisposable
    {
        public AssetProxy()
        {
        }

        /// <summary>
        /// 资源回收（卸载引用计数为零的资源）
        /// </summary>
        /// <param name="isForce">强制回收所有资源</param>
        public abstract void UnloadUnusedAssets(bool isForce = false);

        /// <summary>
        /// 初始化
        /// </summary>
        public abstract IEnumerator Initialize();

        /// <summary>
        /// 释放资源句柄
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// 是否已经加载
        /// </summary>
        /// <param name="location">寻址地址</param>
        /// <returns>
        /// Ture: 已经加载
        /// False: 未加载
        /// </returns>
        public abstract bool IsAlreadyLoad(string location);

        /// <summary>
        /// 清空资源缓存
        /// </summary>
        public abstract void CleanCache(Action<bool> cb);
    }
}