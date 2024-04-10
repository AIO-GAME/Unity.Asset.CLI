﻿#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using YooAsset;
using Object = UnityEngine.Object;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        /// <inheritdoc />
        public override ILoaderHandle<TObject[]> LoadSubAssets<TObject>(string location, Type type, Action<TObject[]> completed = null)
        {
            return new LoaderHandleLoadSubAssetTask<TObject>(location, type, completed);
        }

        private class LoaderHandleLoadSubAssetTask<TObject> : YLoaderHandle<TObject[]>
        where TObject : Object
        {
            public LoaderHandleLoadSubAssetTask(string location, Type type, Action<TObject[]> completed) : base(location, type, completed) { }

            #region Sync

            protected override void CreateSync()
            {
                var operation = Instance.HandleGet<SubAssetsOperationHandle>(Address);
                if (operation is null)
                {
                    var package = Instance.GetAutoPackageSync(Address);
                    if (package is null) return;
                    operation = package.LoadSubAssetsSync(Address, AssetType);
                    if (!LoadCheckOPSync(operation)) return;
                    Instance.HandleAdd(Address, operation);
                }

                Result = operation.GetSubAssetObjects<TObject>();
            }

            #endregion

            #region Coroutine

            protected override IEnumerator CreateCoroutine()
            {
                var operation = Instance.HandleGet<SubAssetsOperationHandle>(Address);
                if (operation is null)
                {
                    ResPackage package = null;
                    yield return Instance.GetAutoPackageCO(Address, ya => package = ya);
                    if (package is null)
                    {
                        InvokeOnCompleted();
                        yield break;
                    }

                    operation = package.LoadSubAssetsAsync(Address, AssetType);
                    var check = false;
                    yield return LoadCheckOPCo(operation, ya => check = ya);
                    if (!check)
                    {
                        InvokeOnCompleted();
                        yield break;
                    }

                    Instance.HandleAdd(Address, operation);
                }

                Result = operation?.GetSubAssetObjects<TObject>();
                InvokeOnCompleted();
            }

            #endregion

            #region Task

            private void OnCompletedTaskGeneric()
            {
                Result = AwaiterGeneric.GetResult();
                InvokeOnCompleted();
            }

            private TaskAwaiter<TObject[]> AwaiterGeneric;

            private async Task<TObject[]> GetTask()
            {
                var operation = Instance.HandleGet<SubAssetsOperationHandle>(Address);
                if (operation is null)
                {
                    var package = await Instance.GetAutoPackageTask(Address);
                    if (package is null) return null;
                    operation = package.LoadSubAssetsAsync(Address, AssetType);
                    if (!await LoadCheckOPTask(operation)) return null;
                    Instance.HandleAdd(Address, operation);
                }

                return operation?.GetSubAssetObjects<TObject>();
            }

            protected override TaskAwaiter<TObject[]> CreateAsync()
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