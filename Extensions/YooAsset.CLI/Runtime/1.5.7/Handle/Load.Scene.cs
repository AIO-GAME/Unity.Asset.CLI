#if SUPPORT_YOOASSET

#region

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using YooAsset;

#endregion

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        /// <inheritdoc />
        public override ILoaderHandle<Scene> LoadSceneTask(
            string        location,
            Action<Scene> completed   = null,
            LoadSceneMode sceneMode   = LoadSceneMode.Single,
            bool          suspendLoad = false,
            int           priority    = 100
        ) => new LoadScene(location, completed, sceneMode, suspendLoad, priority);

        private class LoadScene : YLoaderHandle<Scene>
        {
            private LoadSceneMode sceneMode   { get; set; }
            private bool          suspendLoad { get; set; }
            private int           priority    { get; set; }

            public LoadScene(
                string        location,
                Action<Scene> completed,
                LoadSceneMode sceneMode   = LoadSceneMode.Single,
                bool          suspendLoad = false,
                int           priority    = 100
            ) : base(location, completed)
            {
                this.sceneMode   = sceneMode;
                this.suspendLoad = suspendLoad;
                this.priority    = priority;
            }

            #region Sync

            protected override void CreateSync()
            {
                Runner.StartCoroutine(CreateCoroutine);
            }

            #endregion

            #region Coroutine

            protected override IEnumerator CreateCoroutine()
            {
                var operation = Instance.HandleGet<SceneOperationHandle>(Address);
                if (operation != null) Instance.HandleFree(Address);

                ResPackage package = null;
                yield return Instance.AutoGetPackageCoroutine(Address, resPackage => package = resPackage);
                if (package is null) throw new Exception($"场景配置 异常错误 : {Address} {sceneMode}");

                operation = package.LoadSceneAsync(Address, sceneMode, suspendLoad, priority);
                yield return operation.CheckCoroutine(succeed =>
                {
                    if (succeed)
                    {
                        Instance.HandleAdd(Address, operation);
                        operation.ActivateScene();
                        Result = operation.SceneObject;
                        InvokeOnCompleted();
                    }
                    else
                    {
                        AssetSystem.LogException($"场景配置 异常错误 : {package.PackageName} {Address} {sceneMode}");
                        Result = SceneManager.GetActiveScene();
                        InvokeOnCompleted();
                    }
                });
            }

            #endregion

            #region Task

            private void OnCompletedTaskGeneric()
            {
                Result = AwaiterGeneric.GetResult();
            }

            private TaskAwaiter<Scene> AwaiterGeneric;

            private async Task<Scene> GetTask()
            {
                var operation = Instance.HandleGet<SceneOperationHandle>(Address);
                if (operation != null) Instance.HandleFree(Address);

                var package = await Instance.AutoGetPackageTask(Address);
                if (package is null)
                {
                    AssetSystem.LogExceptionFormat("场景配置 异常错误:{0} {1}", Address, sceneMode);
                    return SceneManager.GetActiveScene();
                }

                operation = package.LoadSceneAsync(Address, sceneMode, suspendLoad, priority);
                if (await operation.CheckTask())
                {
                    operation.ActivateScene();
                    Instance.HandleAdd(Address, operation);
                    return operation.SceneObject;
                }

                AssetSystem.LogExceptionFormat("加载场景 资源异常:{0} {1} {2}", package.PackageName, Address, sceneMode);
                return SceneManager.GetActiveScene();
            }

            protected override TaskAwaiter<Scene> CreateAsync()
            {
                AwaiterGeneric = GetTask().GetAwaiter();
                AwaiterGeneric.OnCompleted(OnCompletedTaskGeneric);
                return AwaiterGeneric;
            }

            #endregion
        }
    }
}
#endif