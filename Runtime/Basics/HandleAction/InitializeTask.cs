using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AIO.UEngine;
using UnityEngine;
using UnityEngine.Scripting;

namespace AIO
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [Preserve]
    internal static class ASHandleActionInitializeTask
    {
        public static IOperationAction Create<T>(T proxy, ASConfig config)
        where T : ASProxy
        {
            return new OperationActionInitializeTask<T>(proxy, config);
        }

        public static IOperationAction Create(ASConfig config)
        {
            var proxyType = typeof(ASProxy);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
#if UNITY_EDITOR
                if (assembly.FullName.Contains("Editor")) continue;
#endif
                foreach (var type in assembly.GetTypes().Where(type => !type.IsAbstract).Where(type => proxyType.IsAssignableFrom(type)))
                {
                    AssetSystem.Proxy = (ASProxy)Activator.CreateInstance(type);
                    break;
                }
            }

            return new OperationActionInitializeTask<ASProxy>(AssetSystem.Proxy, config);
        }
    }

    [StructLayout(LayoutKind.Auto)] [Preserve]
    internal partial class OperationActionInitializeTask<T> : OperationAction
    where T : ASProxy
    {
        #region CO

        protected override IEnumerator CreateCoroutine()
        {
            if (!IsValidate) throw new Exception("Initialize Error");
            yield return Proxy.UpdatePackagesTask(Config);
            if (AssetSystem._Exception != ASException.None)
            {
                IsValidate = false;
                yield break;
            }

            try
            {
                Config.Check();
            }
            catch (Exception)
            {
                AssetSystem.ExceptionEvent(ASException.ASConfigCheckError);
            }

            if (AssetSystem._Exception != ASException.None)
            {
                IsValidate = false;
                yield break;
            }

            yield return Proxy.Initialize();
            if (AssetSystem._Exception != ASException.None)
            {
                IsValidate = false;
            }
        }

        #endregion

        #region Sync

        protected override void CreateSync()
        {
            if (!IsValidate) throw new Exception("Initialize Error");
            Proxy.UpdatePackagesTask(Config).Invoke();
            if (AssetSystem._Exception != ASException.None)
            {
                IsValidate = false;
                return;
            }

            try
            {
                Config.Check();
            }
            catch (Exception)
            {
                AssetSystem.ExceptionEvent(ASException.ASConfigCheckError);
            }

            if (AssetSystem._Exception != ASException.None)
            {
                IsValidate = false;
                return;
            }

            Proxy.Initialize().Invoke();
            if (AssetSystem._Exception != ASException.None)
            {
                IsValidate = false;
            }

            IsDone = true;
        }

        #endregion

        #region Task

        private TaskAwaiter Awaiter;

        protected override TaskAwaiter CreateAsync()
        {
            if (!IsValidate) throw new Exception("Initialize Error");
            Awaiter = OnAwaiter2().GetAwaiter();
            Awaiter.OnCompleted(InvokeOnCompleted);
            return Awaiter;
        }

        protected async Task OnAwaiter2()
        {
            await Proxy.UpdatePackagesTask(Config);
            if (AssetSystem._Exception != ASException.None)
            {
                IsValidate = false;
                return;
            }

            try
            {
                Config.Check();
            }
            catch (Exception e)
            {
                AssetSystem.LogError(e.ToString());
                AssetSystem.ExceptionEvent(ASException.ASConfigCheckError);
                IsValidate = false;
                return;
            }

            try
            {
                await Proxy.Initialize();
            }
            catch (Exception e)
            {
                AssetSystem.LogError(e.ToString());
                AssetSystem.ExceptionEvent(ASException.AssetProxyInitializeError);
                IsValidate = false;
                return;
            }

            IsValidate = true;
        }

        #endregion

        protected override void OnCompleted()
        {
            if (!IsValidate) AssetSystem.LogError("Asset System Initialize Failed");
            IsDone = true;
        }

        protected override void OnReset() { IsValidate = !AssetSystem.IsInitialized; }

        private T        Proxy;
        private ASConfig Config;

        public OperationActionInitializeTask(T proxy, ASConfig config)
        {
            if (AssetSystem.IsInitialized)
            {
                IsValidate = false;
                return;
            }

            if (proxy is null)
            {
                AssetSystem.ExceptionEvent(ASException.AssetProxyIsNull);
                IsValidate = false;
                return;
            }

            if (!config)
            {
                AssetSystem.ExceptionEvent(ASException.ASConfigIsNull);
                IsValidate = false;
                return;
            }

            if (string.IsNullOrEmpty(config.RuntimeRootDirectory)) config.RuntimeRootDirectory = "BuiltinFiles";
            AssetSystem.BuildInRootDirectory = Path.Combine(Application.streamingAssetsPath, config.RuntimeRootDirectory);
            AssetSystem.SandboxRootDirectory =
#if UNITY_EDITOR
                Path.Combine(Directory.GetParent(Application.dataPath).FullName,
                             config.RuntimeRootDirectory,
                             EditorUserBuildSettings.activeBuildTarget.ToString());
#else
                Path.Combine(Application.persistentDataPath, config.RuntimeRootDirectory);
#endif

            AssetSystem.Proxy     = Proxy  = proxy;
            AssetSystem.Parameter = Config = config;
        }
    }
}