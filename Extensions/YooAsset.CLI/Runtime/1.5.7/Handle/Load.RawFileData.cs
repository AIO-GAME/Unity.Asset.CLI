#if SUPPORT_YOOASSET

#region

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using YooAsset;

#endregion

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        private class LoadRawFileData : YLoaderHandle<byte[]>
        {
            public LoadRawFileData(string location, Action<byte[]> completed) : base(location, typeof(byte[]), completed) { }

            #region Sync

            protected override void CreateSync()
            {
                var operation = Instance.HandleGet<RawFileOperationHandle>(Address);
                if (operation is null)
                {
                    var package = Instance.AutoGetPackageSync(Address);
                    if (package is null)
                    {
                        Result = Array.Empty<byte>();
                        return;
                    }

                    operation = package.LoadRawFileSync(Address);
                    if (!operation.CheckSync())
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
                    yield return Instance.AutoGetPackageCoroutine(Address, ya => package = ya);
                    if (package is null)
                    {
                        Result = Array.Empty<byte>();
                        InvokeOnCompleted();
                        yield break;
                    }

                    operation = package.LoadRawFileAsync(Address);
                    var check = false;
                    yield return operation.CheckCoroutine(ya => check = ya);
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
            }

            private TaskAwaiter<byte[]> AwaiterGeneric;

            private async Task<byte[]> GetTask()
            {
                var operation = Instance.HandleGet<RawFileOperationHandle>(Address);
                if (operation is null)
                {
                    var package = await Instance.AutoGetPackageTask(Address);
                    if (package is null) return Array.Empty<byte>();
                    operation = package.LoadRawFileAsync(Address);
                    if (!await operation.CheckTask()) return Array.Empty<byte>();
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
        public override ILoaderHandle<byte[]> LoadRawFileDataTask(string location, Action<byte[]> cb = null)
            => new LoadRawFileData(location, cb);
    }
}
#endif