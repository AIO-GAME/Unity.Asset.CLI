using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.Scripting;

namespace AIO.UEngine
{
    /// <summary>
    /// 资源管理系统 - 资源代理
    /// </summary>
    [Preserve]
    public abstract partial class AssetProxy : IDisposable
    {
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        public abstract bool IsInitialize { get; }

        /// <summary>
        /// 初始化协程
        /// </summary>
        public abstract IEnumerator InitializeCO();

        /// <summary>
        /// 初始化任务
        /// </summary>
        public abstract Task InitializeTask();

        /// <summary>
        /// 更新资源包
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator UpdatePackagesCO(ASConfig config);

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
        public abstract bool AlreadyLoad(string location);

        /// <summary>
        /// 获取下载器
        /// </summary>
        public abstract IASNetLoading GetLoadingHandle();

        public sealed override bool Equals(object obj)
        {
            return false;
        }

        public sealed override string ToString()
        {
            return string.Empty;
        }

        public sealed override int GetHashCode()
        {
            return 0;
        }
    }
}