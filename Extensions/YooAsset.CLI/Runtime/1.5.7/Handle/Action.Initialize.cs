#if SUPPORT_YOOASSET
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine.Scripting;
using YooAsset;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        /// <inheritdoc />
        [Preserve]
        public override IOperationAction<bool> Initialize() => new ActionInitialize();

        [Preserve]
        private class ActionInitialize : OperationAction<bool>
        {
            private async Task<bool> InitializeTask()
            {
                Instance.Initialize_Internal();
                foreach (var operation in Instance.InitializationOperations)
                {
                    await operation.Task;
                    if (operation.Status != EOperationStatus.Succeed)
                    {
                        AssetSystem.LOG.E($"Initialize Operation {operation.Status} | {operation.Error}");
                    }
                }

                return true;
            }

            protected override TaskAwaiter<bool> CreateAsync()
            {
                var awaiter = InitializeTask().GetAwaiter();
                awaiter.OnCompleted(() => Result = awaiter.GetResult());
                return awaiter;
            }

            protected override IEnumerator CreateCoroutine()
            {
                Instance.Initialize_Internal();
                foreach (var operation in Instance.InitializationOperations) yield return operation;
                InvokeOnCompleted();
            }

            protected override void CreateSync()
            {
                Instance.Initialize_Internal();
                foreach (var operation in Instance.InitializationOperations) operation.Task.RunSynchronously();
                Result = true;
                IsDone = true;
            }
        }
    }
}
#endif