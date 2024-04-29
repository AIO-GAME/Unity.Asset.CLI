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
        private class LoadRawFileText : YLoaderHandle<string>
        {
            public LoadRawFileText(string location, Action<string> completed) : base(location, typeof(string), completed) { }

            #region Sync

            protected override void CreateSync()
            {
                var operation = Instance.HandleGet<RawFileOperationHandle>(Address);
                if (operation is null)
                {
                    var package = Instance.AutoGetPackageSync(Address);
                    if (package is null)
                    {
                        Result = string.Empty;
                        return;
                    }

                    operation = package.LoadRawFileSync(Address);
                    if (!operation.CheckSync())
                    {
                        Result = string.Empty;
                        return;
                    }

                    Instance.HandleAdd(Address, operation);
                }

                if (operation != null)
                {
                    AssetPath = operation.GetRawFilePath();
                    Result    = operation.GetRawFileText();
                }

                IsDone = true;
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
                        Result = string.Empty;
                        InvokeOnCompleted();
                        yield break;
                    }

                    operation = package.LoadRawFileAsync(Address);
                    var check = false;
                    yield return operation.CheckCoroutine(ya => check = ya);
                    if (!check)
                    {
                        Result = string.Empty;
                        InvokeOnCompleted();
                        yield break;
                    }

                    Instance.HandleAdd(Address, operation);
                }


                if (operation != null)
                {
                    AssetPath = operation.GetRawFilePath();
                    Result    = operation.GetRawFileText();
                }

                InvokeOnCompleted();
            }

            #endregion

            #region Task

            private void OnCompletedTaskGeneric()
            {
                Result = AwaiterGeneric.GetResult();
            }

            private TaskAwaiter<string> AwaiterGeneric;

            private async Task<string> GetTask()
            {
                var operation = Instance.HandleGet<RawFileOperationHandle>(Address);
                if (operation is null)
                {
                    var package = await Instance.AutoGetPackageTask(Address);
                    if (package is null) return string.Empty;
                    operation = package.LoadRawFileAsync(Address);
                    if (!await operation.CheckTask()) return string.Empty;
                    Instance.HandleAdd(Address, operation);
                }

                if (operation == null) return string.Empty;
                AssetPath = operation.GetRawFilePath();
                return operation.GetRawFileText();
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
        public override ILoaderHandle<string> LoadRawFileTextTask(string location, Action<string> cb = null)
            => new LoadRawFileText(location, cb);
    }
}
#endif