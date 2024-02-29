using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AIO.UEngine;
using UnityEditor;
using UnityEngine;

namespace AIO
{
    /// <summary>
    /// 资源管理系统
    /// </summary>
    public static partial class AssetSystem
    {
        /// <summary>
        /// 系统初始化异常
        /// </summary>
        public static event Action<AssetSystemException> OnException;

        private static AssetSystemException _Exception;

        internal static void ExceptionEvent(AssetSystemException ex)
        {
            _Exception = ex;
            if (OnException is null) LogException($"Asset System Exception : {ex}");
            else OnException.Invoke(ex);
        }

        /// <summary>
        /// 系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize<T>(ASConfig config) where T : AssetProxy, new()
        {
            return Initialize(Activator.CreateInstance<T>(), config);
        }

        /// <summary>
        /// 系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize()
        {
            yield return Initialize(ASConfig.GetOrCreate());
        }

        /// <summary>
        /// 系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize(ASConfig config)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsAbstract) continue;
                    if (!typeof(AssetProxy).IsAssignableFrom(type)) continue;
                    Proxy = (AssetProxy)Activator.CreateInstance(type);
                    break;
                }
            }

            yield return Initialize(Proxy, config);
        }

        /// <summary>
        /// 系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize<T>(T proxy) where T : AssetProxy
        {
            return Initialize(proxy, ASConfig.GetOrCreate());
        }

        /// <summary>
        /// 系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize<T>() where T : AssetProxy, new()
        {
            return Initialize(Activator.CreateInstance<T>(), ASConfig.GetOrCreate());
        }

        /// <summary>
        /// 系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize<T>(T proxy, ASConfig config)
            where T : AssetProxy
        {
            if (IsInitialized) yield break;
            _Exception = AssetSystemException.None;

            if (proxy is null)
            {
                ExceptionEvent(AssetSystemException.AssetProxyIsNull);
                yield break;
            }

            if (config is null)
            {
                ExceptionEvent(AssetSystemException.ASConfigIsNull);
                yield break;
            }

            if (string.IsNullOrEmpty(config.RuntimeRootDirectory)) config.RuntimeRootDirectory = "BuiltinFiles";
            BuildInRootDirectory = Path.Combine(Application.streamingAssetsPath, config.RuntimeRootDirectory);
            SandboxRootDirectory =
#if UNITY_EDITOR
                Path.Combine(Directory.GetParent(Application.dataPath).FullName,
                    config.RuntimeRootDirectory, EditorUserBuildSettings.activeBuildTarget.ToString());
#else
                Path.Combine(Application.persistentDataPath, config.RuntimeRootDirectory);
#endif
            Parameter = config;
            Proxy = proxy;

            yield return Proxy.UpdatePackagesCO(Parameter);
            try
            {
                Parameter.Check();
            }
            catch (Exception)
            {
                ExceptionEvent(AssetSystemException.ASConfigCheckError);
                yield break;
            }

            if (_Exception != AssetSystemException.None) yield break;
            yield return Proxy.InitializeCO();
            if (_Exception != AssetSystemException.None) yield break;
            DownloadHandle = Proxy.GetLoadingHandle();
        }

        /// <summary>
        /// 销毁资源管理系统
        /// </summary>
        /// <returns></returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static Task DestroyTask()
        {
            Destroy();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 销毁资源管理系统
        /// </summary>
        /// <returns></returns>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator DestroyCO()
        {
            Destroy();
            yield break;
        }

        /// <summary>
        /// 销毁资源管理系统
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
        /// 清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static async void ClearUnusedCache(Action<bool> cb)
        {
            var result = await Proxy.ClearUnusedCacheTask();
            cb?.Invoke(result);
        }

        /// <summary>
        /// 清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static async void ClearUnusedCache()
        {
            await Proxy.ClearUnusedCacheTask();
        }

        /// <summary>
        /// 清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static Task<bool> CleanUnusedCacheTask()
        {
            return Proxy.ClearUnusedCacheTask();
        }

        /// <summary>
        /// 清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static IEnumerator CleanUnusedCacheCO(Action<bool> cb)
        {
            return Proxy.ClearUnusedCacheCO(cb);
        }

        /// <summary>
        /// 清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static IEnumerator CleanUnusedCacheCO()
        {
            return Proxy.ClearUnusedCacheCO(null);
        }

        /// <summary>
        /// 清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static async void ClearAllCache(Action<bool> cb)
        {
            var result = await Proxy.ClearAllCacheTask();
            cb?.Invoke(result);
        }

        /// <summary>
        /// 清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static async void ClearAllCache()
        {
            await Proxy.ClearAllCacheTask();
        }

        /// <summary>
        /// 清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static Task<bool> CleanAllCacheTask()
        {
            return Proxy.ClearAllCacheTask();
        }

        /// <summary>
        /// 清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static IEnumerator CleanAllCacheCO(Action<bool> cb)
        {
            return Proxy.ClearAllCacheCO(cb);
        }

        /// <summary>
        /// 清理包裹未使用的缓存文件 (清空之后需要重新下载资源)
        /// </summary>
        public static IEnumerator CleanAllCacheCO()
        {
            return Proxy.ClearAllCacheCO(null);
        }
    }
}