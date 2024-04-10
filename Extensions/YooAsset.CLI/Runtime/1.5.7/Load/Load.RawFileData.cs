#if SUPPORT_YOOASSET
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        private class LoaderHandleLoadRawFileDataTask : YLoaderHandle<byte[]>
        {
            public LoaderHandleLoadRawFileDataTask(string location, Action<byte[]> completed) : base(location, typeof(byte[]), completed) { }

            #region Sync

            protected override void CreateSync()
            {
                var operation = Instance.HandleGet<RawFileOperationHandle>(Address);
                if (operation is null)
                {
                    var package = Instance.GetAutoPackageSync(Address);
                    if (package is null)
                    {
                        Result = Array.Empty<byte>();
                        return;
                    }

                    operation = package.LoadRawFileSync(Address);
                    if (!LoadCheckOPSync(operation))
                    {
                        Result = Array.Empty<byte>();
                        return;
                    }

                    Instance.HandleAdd(Address, operation);
                }

                Result = operation?.GetRawFileData();
            }

            #endregion

            #region Coroutine

            protected override IEnumerator CreateCoroutine()
            {
                var operation = Instance.HandleGet<RawFileOperationHandle>(Address);
                if (operation is null)
                {
                    ResPackage package = null;
                    yield return Instance.GetAutoPackageCO(Address, ya => package = ya);
                    if (package is null)
                    {
                        Result = Array.Empty<byte>();
                        InvokeOnCompleted();
                        yield break;
                    }

                    operation = package.LoadRawFileAsync(Address);
                    var check = false;
                    yield return LoadCheckOPCo(operation, ya => check = ya);
                    if (!check)
                    {
                        Result = Array.Empty<byte>();
                        InvokeOnCompleted();
                        yield break;
                    }

                    Instance.HandleAdd(Address, operation);
                }

                Result = operation?.GetRawFileData();
                InvokeOnCompleted();
            }

            #endregion

            #region Task

            private void OnCompletedTaskGeneric()
            {
                Result = AwaiterGeneric.GetResult();
                InvokeOnCompleted();
            }

            private TaskAwaiter<byte[]> AwaiterGeneric;

            private async Task<byte[]> GetTask()
            {
                var operation = Instance.HandleGet<RawFileOperationHandle>(Address);
                if (operation is null)
                {
                    var package = await Instance.GetAutoPackageTask(Address);
                    if (package is null) return Array.Empty<byte>();
                    operation = package.LoadRawFileAsync(Address);
                    if (!await LoadCheckOPTask(operation)) return Array.Empty<byte>();
                    Instance.HandleAdd(Address, operation);
                }

                return operation?.GetRawFileData();
            }

            protected override TaskAwaiter<byte[]> CreateAsync()
            {
                AwaiterGeneric = GetTask().GetAwaiter();
                AwaiterGeneric.OnCompleted(OnCompletedTaskGeneric);
                return AwaiterGeneric;
            }

            #endregion
        }

        /// <inheritdoc />
        public override ILoaderHandle<byte[]> LoadRawFileData(string location, Action<byte[]> cb = null)
            => new LoaderHandleLoadRawFileDataTask(location, cb);
    }
}
#endif