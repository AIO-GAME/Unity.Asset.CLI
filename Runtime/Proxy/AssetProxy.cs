/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public abstract void UnloadUnusedAssets();

        /// <summary>
        /// 强制回收所有资源
        /// </summary>
        public abstract void ForceUnloadALLAssets();

        /// <summary>
        /// 初始化
        /// </summary>
        public abstract Task Initialize();

        /// <summary>
        /// 释放资源句柄
        /// </summary>
        public abstract void Dispose();
    }
}
