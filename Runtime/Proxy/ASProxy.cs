using System;
using UnityEngine.Scripting;

namespace AIO.UEngine
{
    /// <summary>
    ///     资源管理系统 - 资源代理
    /// </summary>
    [Preserve]
    public abstract partial class ASProxy : IDisposable
    {
        /// <summary>
        ///     是否已经初始化
        /// </summary>
        public abstract bool IsInitialize { get; }

        #region IDisposable Members

        /// <summary>
        ///     释放资源句柄
        /// </summary>
        public abstract void Dispose();

        #endregion

        /// <summary>
        ///    资源框架初始化
        /// </summary>
        /// <returns>Ture:成功 False:失败</returns>
        public abstract IOperationAction<bool> Initialize();

        /// <summary>
        ///     更新资源包
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <param name="completed">完成回调</param>
        /// <returns>Ture:成功 False:失败</returns>
        public abstract IOperationAction<bool> UpdatePackagesTask(ASConfig config, Action<bool> completed = null);

        /// <summary>
        ///     是否已经加载
        /// </summary>
        /// <param name="location">寻址地址</param>
        /// <returns>
        ///     Ture: 已经加载
        ///     False: 未加载
        /// </returns>
        public abstract bool AlreadyLoad(string location);

        /// <summary>
        ///     获取下载器
        /// </summary>
        public abstract IASNetLoading GetLoadingHandle();

        public sealed override bool   Equals(object obj) => false;
        public sealed override string ToString()         => string.Empty;
        public sealed override int    GetHashCode()      => 0;
    }
}