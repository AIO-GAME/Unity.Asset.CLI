#if SUPPORT_YOOASSET

#region

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;

#endregion

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        private class LoaderHandleInstGameObjectTask : YLoaderHandle<GameObject>
        {
            private Transform parent { get; set; }
            public LoaderHandleInstGameObjectTask(string location, Action<GameObject> completed, Transform parent = null) : base(location, completed) => this.parent = parent;

            #region Sync

            protected override void CreateSync()
            {
                var operation = Instance.HandleGet<AssetOperationHandle>(Address);
                if (operation is null)
                {
                    var package = Instance.GetAutoPackageSync(Address);
                    if (package is null) return;
                    operation = package.LoadAssetSync<GameObject>(Address);
                    if (!LoadCheckOPSync(operation)) return;
                    Instance.HandleAdd(Address, operation);
                }

                Result = operation.InstantiateSync(parent);
            }

            #endregion

            #region Coroutine

            protected override IEnumerator CreateCoroutine()
            {
                var operation = Instance.HandleGet<AssetOperationHandle>(Address);
                if (operation is null)
                {
                    ResPackage package = null;
                    yield return Instance.GetAutoPackageCO(Address, ya => package = ya);
                    if (package is null)
                    {
                        InvokeOnCompleted();
                        yield break;
                    }

                    operation = package.LoadAssetAsync<GameObject>(Address);
                    var check = false;
                    yield return LoadCheckOPCo(operation, ya => check = ya);
                    if (!check)
                    {
                        InvokeOnCompleted();
                        yield break;
                    }

                    Instance.HandleAdd(Address, operation);
                }

                Result = operation?.InstantiateSync(parent);
                InvokeOnCompleted();
            }

            #endregion

            #region Task

            private void OnCompletedTaskGeneric()
            {
                Result = AwaiterGeneric.GetResult();
                InvokeOnCompleted();
            }

            private TaskAwaiter<GameObject> AwaiterGeneric;

            public async Task<GameObject> GetTask()
            {
                var operation = Instance.HandleGet<AssetOperationHandle>(Address);
                if (operation is null)
                {
                    var package = await Instance.GetAutoPackageTask(Address);
                    if (package is null) return null;
                    operation = package.LoadAssetAsync<GameObject>(Address);
                    if (!await LoadCheckOPTask(operation)) return null;
                    Instance.HandleAdd(Address, operation);
                }

                return operation.InstantiateSync(parent);
            }

            protected override TaskAwaiter<GameObject> CreateAsync()
            {
                AwaiterGeneric = GetTask().GetAwaiter();
                AwaiterGeneric.OnCompleted(OnCompletedTaskGeneric);
                return AwaiterGeneric;
            }

            #endregion
        }

        /// <inheritdoc />
        public override ILoaderHandle<GameObject> InstGameObject(string location, Action<GameObject> completed = null, Transform parent = null)
        {
            return new LoaderHandleInstGameObjectTask(location, completed, parent);
        }
    }
}
#endif