using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AIO.UEngine;
using UnityEngine;

namespace AIO
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    partial class ASHandleActionInitializeTask
    {
        public static AssetSystem.IHandleAction Create<T>(T proxy, ASConfig config) where T : ASProxy
        {
            return new ASHandleActionInitializeTask<T>(proxy, config);
        }

        public static AssetSystem.IHandleAction Create(ASConfig config)
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
                    AssetSystem.Proxy = (ASProxy)Activator.CreateInstance(type);
                    break;
                }
            }

            return new ASHandleActionInitializeTask<ASProxy>(AssetSystem.Proxy, config);
        }
    }

    [StructLayout(LayoutKind.Auto)]
    internal partial class ASHandleActionInitializeTask<T> : ASHandleAction where T : ASProxy
    {
        #region CO

        protected override IEnumerator CreateCoroutine()
        {
            if (!IsValidate) throw new Exception("Initialize Error");
            yield return Proxy.UpdatePackagesCO(Config);
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

            yield return Proxy.InitializeCO();
            if (AssetSystem._Exception != ASException.None)
            {
                IsValidate = false;
            }
        }

        #endregion

        #region Sync

        protected override void CreateSync()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Task

        private TaskAwaiter _Awaiter1;
        private TaskAwaiter _Awaiter2;

        protected override TaskAwaiter CreateAsync()
        {
            if (!IsValidate) throw new Exception("Initialize Error");
            _Awaiter1 = OnAwaiter2().GetAwaiter();
            _Awaiter1.OnCompleted(InvokeOnCompleted);
            return _Awaiter1;
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
            catch (Exception)
            {
                AssetSystem.ExceptionEvent(ASException.ASConfigCheckError);
            }

            if (AssetSystem._Exception != ASException.None)
            {
                IsValidate = false;
            }

            await Proxy.InitializeTask();
            if (AssetSystem._Exception != ASException.None)
            {
                IsValidate = false;
            }
        }

        #endregion

        protected override void OnCompleted()
        {
            if (!IsValidate) throw new Exception("Initialize Error");
            if (Config.ASMode == EASMode.Remote)
                AssetSystem.DownloadHandle = Proxy.GetLoadingHandle();
        }

        protected override void OnReset()
        {
            IsValidate = !AssetSystem.IsInitialized;
        }

        private T        Proxy;
        private ASConfig Config;
        private bool     IsValidate = true;

        public ASHandleActionInitializeTask(T proxy, ASConfig config)
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

            if (config is null)
            {
                AssetSystem.ExceptionEvent(ASException.ASConfigIsNull);
                IsValidate = false;
                return;
            }

            if (string.IsNullOrEmpty(config.RuntimeRootDirectory)) config.RuntimeRootDirectory = "BuiltinFiles";
            AssetSystem.BuildInRootDirectory = Path.Combine(Application.streamingAssetsPath, config.RuntimeRootDirectory);
#if UNITY_EDITOR
            Path.Combine(Directory.GetParent(Application.dataPath).FullName,
                         config.RuntimeRootDirectory, EditorUserBuildSettings.activeBuildTarget.ToString());
#else
                Path.Combine(Application.persistentDataPath, config.RuntimeRootDirectory);
#endif
            AssetSystem.Proxy     = Proxy  = proxy;
            AssetSystem.Parameter = Config = config;
        }
    }
}