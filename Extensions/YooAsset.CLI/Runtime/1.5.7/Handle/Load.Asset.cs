#if SUPPORT_YOOASSET

#region

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using YooAsset;
using Object = UnityEngine.Object;

#endregion

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        private class LoadAsset<TObject> : YLoaderHandle<TObject>
        where TObject : Object
        {
            public LoadAsset(string location, Type type, Action<TObject> completed) : base(location, type, completed) { }

            #region Sync

            protected override void CreateSync()
            {
                var operation = Instance.HandleGet<AssetOperationHandle>(Address);
                if (operation is null)
                {
                    var package = Instance.AutoGetPackageSync(Address);
                    if (package is null) return;
                    operation = package.LoadAssetSync<TObject>(Address);
                    if (!operation.CheckSync()) return;
                    Instance.HandleAdd(Address, operation);
                }

                Result = operation?.GetAssetObject<TObject>();
            }

            #endregion

            #region Coroutine

            protected override IEnumerator CreateCoroutine()
            {
                var operation = Instance.HandleGet<AssetOperationHandle>(Address);
                if (operation is null)
                {
                    ResPackage package = null;
                    yield return Instance.AutoGetPackageCoroutine(Address, ya => package = ya);
                    if (package is null)
                    {
                        InvokeOnCompleted();
                        yield break;
                    }

                    operation = package.LoadAssetAsync(Address, AssetType);
                    var check = false;
                    yield return operation.CheckCoroutine(ya => check = ya);
                    if (!check)
                    {
                        InvokeOnCompleted();
                        yield break;
                    }

                    Instance.HandleAdd(Address, operation);
                }

                Result = operation?.GetAssetObject<TObject>();
                InvokeOnCompleted();
            }

            #endregion

            #region Task

            private void OnCompletedTaskGeneric()
            {
                Result = AwaiterGeneric.GetResult();
            }

            private TaskAwaiter<TObject> AwaiterGeneric;

            private async Task<TObject> GetTask()
            {
                var operation = Instance.HandleGet<AssetOperationHandle>(Address);
                if (operation is null)
                {
                    var package = await Instance.AutoGetPackageTask(Address);
                    if (package is null) return null;
                    operation = package.LoadAssetAsync(Address, AssetType);
                    if (!await operation.CheckTask()) return null;
                    Instance.HandleAdd(Address, operation);
                }

                return operation?.GetAssetObject<TObject>();
            }

            protected override TaskAwaiter<TObject> CreateAsync()
            {
                AwaiterGeneric = GetTask().GetAwaiter();
                AwaiterGeneric.OnCompleted(OnCompletedTaskGeneric);
                return AwaiterGeneric;
            }

            #endregion
        }

        /// <inheritdoc />
        public override ILoaderHandle<TObject> LoadAssetTask<TObject>(string location, Type type, Action<TObject> completed = null)
            => new LoadAsset<TObject>(location, type, completed);
    }
}

#endif