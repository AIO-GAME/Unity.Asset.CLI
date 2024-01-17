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
using UnityEngine;

namespace AIO
{
    /// <summary>
    /// 资源管理系统
    /// </summary>
    public static partial class AssetSystem
    {
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
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsAbstract) continue;
                    if (typeof(AssetProxy).IsAssignableFrom(type))
                    {
                        Proxy = (AssetProxy)Activator.CreateInstance(type);
                        break;
                    }
                }
            }

            if (Proxy is null)
                throw new Exception("Not Found Other Asset Proxy! Please Input Asset Proxy!");

            yield return Initialize(Proxy, ASConfig.GetOrCreate());
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
                    if (typeof(AssetProxy).IsAssignableFrom(type))
                    {
                        Proxy = (AssetProxy)Activator.CreateInstance(type);
                        break;
                    }
                }
            }

            if (Proxy is null)
                throw new Exception("Not Found Other Asset Proxy! Please Input Asset Proxy!");

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
        public static IEnumerator Initialize<T>(T proxy, ASConfig config) where T : AssetProxy
        {
            IsInitialized = false;
            Parameter = config;
            BuildInRootDirectory = Path.Combine(Application.streamingAssetsPath, Parameter.RuntimeRootDirectory);
            SandboxRootDirectory =
#if UNITY_EDITOR
                string.Concat(Directory.GetParent(Application.dataPath)?.FullName,
                    Path.DirectorySeparatorChar, "Sandbox", Path.DirectorySeparatorChar,
                    UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString());
#else
                Path.Combine(Application.persistentDataPath, Parameter.RuntimeRootDirectory);
#endif
            Proxy = proxy;
            if (Parameter.ASMode == EASMode.Remote)
                yield return Parameter.UpdatePackageRemote();
            else Parameter.UpdatePackage();

            yield return Proxy.Initialize();
            IsInitialized = true;
            
            MainDownloadHandle = Proxy.GetLoadingHandle();
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