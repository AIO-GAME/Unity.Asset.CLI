#region

using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AIO.UEngine;

#endregion

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
#if UNITY_EDITOR
            Parameter.SequenceRecord.Save();
#endif
            Proxy.Dispose();
        }

        /// <summary>
        ///     清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static async void ClearUnusedCache(Action<bool> completed = null)
            => await Proxy.ClearUnusedCacheTask(completed);

        /// <summary>
        ///     清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        /// <param name="completed">回调</param>
        public static IOperationAction<bool> CleanUnusedCacheTask(Action<bool> completed = null)
            => Proxy.ClearUnusedCacheTask(completed);

        /// <summary>
        ///     清理包裹全部缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static async void ClearAllCache(Action<bool> completed = null)
            => await Proxy.ClearAllCacheTask(completed);


        /// <summary>
        ///     清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static IOperationAction<bool> CleanAllCacheTask(Action<bool> completed = null)
            => Proxy.ClearAllCacheTask(completed);
    }
}