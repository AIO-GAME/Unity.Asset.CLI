#if SUPPORT_YOOASSET

#region

using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using YooAsset;

#endregion

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        /// <inheritdoc />
        public override IOperationAction<bool> ClearUnusedCacheTask(Action<bool> completed = null)
        {
            return new ActionClearUnusedCache(completed);
        }

        private class ActionClearUnusedCache : OperationAction<bool>
        {
            public ActionClearUnusedCache(Action<bool> completed) : base(completed) { }

            #region Task

            private async Task<bool> GetTask()
            {
                try
                {
                    var enumerable = Instance.Dic.Values.Select(package => package.ClearUnusedCacheFilesAsync().Task);
#if UNITY_WEBGL
                    foreach (var task in enumerable) await task;
#else
                    await Task.WhenAll(enumerable);
#endif
                }
                catch (Exception e)
                {
                    AssetSystem.LogException(e);
                    return false;
                }

                return true;
            }

            private TaskAwaiter<bool> Awaiter;

            private void OnCompletedTask()
            {
                Result = Awaiter.GetResult();
            }

            /// <inheritdoc />
            protected override TaskAwaiter<bool> CreateAsync()
            {
                Awaiter = GetTask().GetAwaiter();
                Awaiter.OnCompleted(OnCompletedTask);
                return Awaiter;
            }

            #endregion

            /// <inheritdoc />
            protected override IEnumerator CreateCoroutine()
            {
                foreach (var operation in Instance.Dic.Values.Select(package => package.ClearUnusedCacheFilesAsync()))
                {
                    yield return operation;
                    if (operation.Status == EOperationStatus.Succeed) continue;
                    Result = false;
                    InvokeOnCompleted();
                    yield break;
                }

                Result = true;
                InvokeOnCompleted();
            }

            /// <inheritdoc />
            protected override void CreateSync()
            {
                Runner.StartCoroutine(CreateCoroutine);
            }
        }
    }
}
#endif