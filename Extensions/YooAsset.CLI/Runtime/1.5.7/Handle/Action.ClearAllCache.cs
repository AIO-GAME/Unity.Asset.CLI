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
        public override IOperationAction<bool> ClearAllCacheTask(Action<bool> completed = null)
        {
            return new ActionClearAllCache(completed);
        }

        private class ActionClearAllCache : OperationAction<bool>
        {
            public ActionClearAllCache(Action<bool> completed) : base(completed) { }

            private async Task<bool> GetTask()
            {
                try
                {
                    var enumerable = Instance.Dic.Values.Select(package => package.ClearAllCacheFilesAsync().Task);
#if UNITY_WEBGL
                foreach (var task in enumerable) await task;
#else
                    await Task.WhenAll(enumerable);
#endif
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            }

            /// <inheritdoc />
            protected override TaskAwaiter<bool> CreateAsync()
            {
                var awaiter = GetTask().GetAwaiter();
                awaiter.OnCompleted(() => Result = awaiter.GetResult());
                return awaiter;
            }

            /// <inheritdoc />
            protected override IEnumerator CreateCoroutine()
            {
                var enumerable = Instance.Dic.Values.Select(package => package.ClearAllCacheFilesAsync());
                foreach (var operation in enumerable)
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