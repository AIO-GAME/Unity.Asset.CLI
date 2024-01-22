/*|✩ - - - - - |||
|||✩ Author:   ||| -> xi nan
|||✩ Date:     ||| -> 2023-08-22
|||✩ Document: ||| ->
|||✩ - - - - - |*/

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

        internal static void ExceptionEvent(AssetSystemException ex)
        {
            if (OnException is null) throw new SystemException($"Asset System Exception : {ex}");
            OnException.Invoke(ex);
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
        /// 收集类型过滤
        /// </summary>
        private static bool TypeFilter(Type type)
        {
            return !type.IsAbstract && typeof(AssetProxy).IsAssignableFrom(type);
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
                    if (!TypeFilter(type)) continue;
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
        public static IEnumerator Initialize<T>(T proxy, ASConfig config, IProgressEvent iEvent = default)
            where T : AssetProxy
        {
            if (!IsInitialized) IsInitialized = false;
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
                Path.Combine(Application.persistentDataPath, Parameter.RuntimeRootDirectory);
#endif
            Parameter = config;
            Proxy = proxy;
            yield return Proxy.UpdatePackages(Parameter);
            try
            {
                Parameter.Check();
            }
            catch (Exception)
            {
                ExceptionEvent(AssetSystemException.ASConfigCheckError);
                yield break;
            }

            yield return Proxy.Initialize();
            DownloadHandle = Proxy.GetLoadingHandle();
            IsInitialized = true;
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
        /// 清理缓存资源 (清空之后需要重新下载资源)
        /// </summary>
        public static void CleanCache(Action<bool> cb = null)
        {
            Proxy.CleanCache(cb);
        }
    }
}