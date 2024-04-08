using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AIO.UEngine;

namespace AIO
{
    /// <summary>
    ///     资源管理系统
    /// </summary>
    public static partial class AssetSystem
    {
        internal static void ExceptionEvent(ASException ex)
        {
            _Exception = ex;
            if (OnException is null)
            {
#if UNITY_EDITOR
                LogException($"Asset System Exception : {ex}");
#else
                throw new Exception($"Asset System Exception : {ex}");
#endif
            }
            else
            {
                OnException.Invoke(ex);
            }
        }

        /// <summary>
        ///     系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IOperationAction Initialize<T>(ASConfig config)
        where T : ASProxy, new()
        {
            return ASHandleActionInitializeTask.Create(Activator.CreateInstance<T>(), config);
        }

        /// <summary>
        ///     系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IOperationAction Initialize()
        {
            return Initialize(ASConfig.GetOrCreate());
        }

        /// <summary>
        ///     系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IOperationAction Initialize(ASConfig config)
        {
            return ASHandleActionInitializeTask.Create(config);
        }

        /// <summary>
        ///     系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IOperationAction Initialize<T>(T proxy)
        where T : ASProxy
        {
            return ASHandleActionInitializeTask.Create(proxy, ASConfig.GetOrCreate());
        }

        /// <summary>
        ///     系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IOperationAction Initialize<T>()
        where T : ASProxy, new()
        {
            return ASHandleActionInitializeTask.Create(Activator.CreateInstance<T>(), ASConfig.GetOrCreate());
        }

        /// <summary>
        ///     系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IOperationAction Initialize<T>(T proxy, ASConfig config)
        where T : ASProxy
        {
            return ASHandleActionInitializeTask.Create(proxy, config);
        }

        /// <summary>
        ///     销毁资源管理系统
        /// </summary>
        /// <returns></returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task DestroyTask()
        {
            Destroy();
            return Task.CompletedTask;
        }

        /// <summary>
        ///     销毁资源管理系统
        /// </summary>
        /// <returns></returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator DestroyCO()
        {
            Destroy();
            yield break;
        }

        /// <summary>
        ///     销毁资源管理系统
        /// </summary>
        /// <returns></returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static void Destroy()
        {
            foreach (var key in HandleDic.Keys.ToArray())
                HandleDic[key].Dispose();
            HandleDic.Clear();
#if UNITY_EDITOR
            Parameter.SequenceRecord.Save();
#endif
            Proxy.Dispose();
        }

        /// <summary>
        ///     清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static async void ClearUnusedCache(Action<bool> cb)
        {
            var result = await Proxy.ClearUnusedCacheTask();
            cb?.Invoke(result);
        }

        /// <summary>
        ///     清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static async void ClearUnusedCache()
        {
            await Proxy.ClearUnusedCacheTask();
        }

        /// <summary>
        ///     清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static Task<bool> CleanUnusedCacheTask()
        {
            return Proxy.ClearUnusedCacheTask();
        }

        /// <summary>
        ///     清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static IEnumerator CleanUnusedCacheCO(Action<bool> cb)
        {
            return Proxy.ClearUnusedCacheCO(cb);
        }

        /// <summary>
        ///     清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static IEnumerator CleanUnusedCacheCO()
        {
            return Proxy.ClearUnusedCacheCO(null);
        }

        /// <summary>
        ///     清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static async void ClearAllCache(Action<bool> cb)
        {
            var result = await Proxy.ClearAllCacheTask();
            cb?.Invoke(result);
        }

        /// <summary>
        ///     清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static async void ClearAllCache()
        {
            await Proxy.ClearAllCacheTask();
        }

        /// <summary>
        ///     清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static Task<bool> CleanAllCacheTask()
        {
            return Proxy.ClearAllCacheTask();
        }

        /// <summary>
        ///     清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static IEnumerator CleanAllCacheCO(Action<bool> cb)
        {
            return Proxy.ClearAllCacheCO(cb);
        }

        /// <summary>
        ///     清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static IEnumerator CleanAllCacheCO()
        {
            return Proxy.ClearAllCacheCO(null);
        }
    }
}