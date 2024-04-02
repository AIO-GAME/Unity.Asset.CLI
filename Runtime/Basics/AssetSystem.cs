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
    ///     资源管理系统
    /// </summary>
    public static partial class AssetSystem
    {
        private static ASException _Exception;

        /// <summary>
        ///     系统初始化异常
        /// </summary>
        public static event Action<ASException> OnException;

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
        public static IEnumerator Initialize<T>(ASConfig config)
        where T : ASProxy, new()
        {
            return Initialize(Activator.CreateInstance<T>(), config);
        }

        /// <summary>
        ///     系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize()
        {
            return Initialize(ASConfig.GetOrCreate());
        }

        /// <summary>
        ///     系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize(ASConfig config)
        {
            var proxyType = typeof(ASProxy);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
#if UNITY_EDITOR
                if (assembly.FullName.Contains("Editor")) continue;
#endif
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsAbstract) continue;
                    if (proxyType.IsAssignableFrom(type) == false) continue;
                    Proxy = (ASProxy)Activator.CreateInstance(type);
                    break;
                }
            }

            return Initialize(Proxy, config);
        }

        /// <summary>
        ///     系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize<T>(T proxy)
        where T : ASProxy
        {
            return Initialize(proxy, ASConfig.GetOrCreate());
        }

        /// <summary>
        ///     系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize<T>()
        where T : ASProxy, new()
        {
            return Initialize(Activator.CreateInstance<T>(), ASConfig.GetOrCreate());
        }

        /// <summary>
        ///     系统初始化
        /// </summary>
        [DebuggerNonUserCode, DebuggerHidden]
        public static IEnumerator Initialize<T>(T proxy, ASConfig config)
        where T : ASProxy
        {
            if (IsInitialized) yield break;
            _Exception = ASException.None;

            if (proxy is null)
            {
                ExceptionEvent(ASException.AssetProxyIsNull);
                yield break;
            }

            if (config is null)
            {
                ExceptionEvent(ASException.ASConfigIsNull);
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
            Proxy     = proxy;

            yield return Proxy.UpdatePackagesCO(Parameter);
            try
            {
                Parameter.Check();
            }
#if UNITY_EDITOR
            catch (Exception e)
            {
                throw new Exception($"ASConfig Check Error : {e.Message}");
            }
#else
            catch (Exception)
            {
                ExceptionEvent(ASException.ASConfigCheckError);
                yield break;
            }
#endif


            if (_Exception != ASException.None) yield break;
            yield return Proxy.InitializeCO();
            if (_Exception != ASException.None) yield break;
            DownloadHandle = Proxy.GetLoadingHandle();
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