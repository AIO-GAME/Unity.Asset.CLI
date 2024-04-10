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
        private class LoaderHandleLoadRawFileTextTask : YLoaderHandle<string>
        {
            public LoaderHandleLoadRawFileTextTask(string location, Action<string> completed) : base(location, typeof(string), completed) { }

            #region Sync

            protected override void CreateSync()
            {
                var operation = Instance.HandleGet<RawFileOperationHandle>(Address);
                if (operation is null)
                {
                    var package = Instance.GetAutoPackageSync(Address);
                    if (package is null)
                    {
                        Result = string.Empty;
                        return;
                    }

                    operation = package.LoadRawFileSync(Address);
                    if (!LoadCheckOPSync(operation))
                    {
                        Result = string.Empty;
                        return;
                    }

                    Instance.HandleAdd(Address, operation);
                }

                Result = operation?.GetRawFileText();
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
                        Result = string.Empty;
                        InvokeOnCompleted();
                        yield break;
                    }

                    operation = package.LoadRawFileAsync(Address);
                    var check = false;
                    yield return LoadCheckOPCo(operation, ya => check = ya);
                    if (!check)
                    {
                        Result = string.Empty;
                        InvokeOnCompleted();
                        yield break;
                    }

                    Instance.HandleAdd(Address, operation);
                }

                Result = operation?.GetRawFileText();
                InvokeOnCompleted();
            }

            #endregion

            #region Task

            private void OnCompletedTaskGeneric()
            {
                Result = AwaiterGeneric.GetResult();
                InvokeOnCompleted();
            }

            private TaskAwaiter<string> AwaiterGeneric;

            private async Task<string> GetTask()
            {
                var operation = Instance.HandleGet<RawFileOperationHandle>(Address);
                if (operation is null)
                {
                    var package = await Instance.GetAutoPackageTask(Address);
                    if (package is null) return string.Empty;
                    operation = package.LoadRawFileAsync(Address);
                    if (!await LoadCheckOPTask(operation)) return string.Empty;
                    Instance.HandleAdd(Address, operation);
                }

                return operation?.GetRawFileText();
            }

            protected override TaskAwaiter<string> CreateAsync()
            {
                AwaiterGeneric = GetTask().GetAwaiter();
                AwaiterGeneric.OnCompleted(OnCompletedTaskGeneric);
                return AwaiterGeneric;
            }

            #endregion
        }

        /// <inheritdoc />
        public override ILoaderHandle<string> LoadRawFileText(string location, Action<string> cb = null)
            => new LoaderHandleLoadRawFileTextTask(location, cb);
    }
}
#endif