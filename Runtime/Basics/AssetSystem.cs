/*|✩ - - - - - |||
|||✩ Author:   ||| -> XINAN
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AIO.UEngine;

namespace AIO
{
    /// <summary>
    /// 资源管理系统
    /// </summary>
    public static partial class AssetSystem
    {
        private static AssetProxy Proxy;

        /// <summary>
        /// 资源热更新配置
        /// </summary>
        public static ASConfig Parameter { get; private set; }

        /// <summary>
        /// 是否已经初始化
        /// </summary>
        public static bool IsInitialized { get; private set; }

        /// <summary>
        /// 系统初始化
        /// </summary>
        [DebuggerNonUserCode]
        public static Task Initialize<T>(ASConfig config) where T : AssetProxy, new()
        {
            return Initialize(Activator.CreateInstance<T>(), config);
        }

        /// <summary>
        /// 系统初始化
        /// </summary>
        [DebuggerNonUserCode]
        public static Task Initialize<T>(T proxy) where T : AssetProxy
        {
            return Initialize(proxy, new ASConfig());
        }

        /// <summary>
        /// 系统初始化
        /// </summary>
        [DebuggerNonUserCode]
        public static Task Initialize<T>() where T : AssetProxy, new()
        {
            return Initialize(Activator.CreateInstance<T>(), new ASConfig());
        }

        /// <summary>
        /// 系统初始化
        /// </summary>
        [DebuggerNonUserCode]
        public static async Task Initialize<T>(T proxy, ASConfig config) where T : AssetProxy
        {
            IsInitialized = false;
            Parameter = config;
            Proxy = proxy;
            await Proxy.Initialize();
            IsInitialized = true;
        }

        /// <summary>
        /// 销毁资源管理系统
        /// </summary>
        /// <returns></returns>
        public static Task Destroy()
        {
            Proxy.Dispose();
            return Task.CompletedTask;
        }
    }
}