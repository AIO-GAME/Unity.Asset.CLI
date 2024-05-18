/*|============|*|
|*|Author:     |*| Star fire
|*|Date:       |*| 2024-04-11
|*|E-Mail:     |*| xinansky99@gmail.com
|*|============|*/
#if SUPPORT_YOOASSET
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        /// <inheritdoc />
        public override IOperationAction<bool> Initialize()
        {
            return new ActionInitialize();
        }

        private class ActionInitialize : OperationAction<bool>
        {
            private async Task<bool> InitializeTask()
            {
                Instance.Initialize_Internal();
#if UNITY_WEBGL
                foreach (var operation in InitializationOperations) await operation;
#else
                await Task.WhenAll(Instance.InitializationOperations.Select(operation => operation.Task));
#endif
                return true;
            }

            /// <inheritdoc />
            protected override TaskAwaiter<bool> CreateAsync()
            {
                var awaiter = InitializeTask().GetAwaiter();
                awaiter.OnCompleted(() => Result = awaiter.GetResult());
                return awaiter;
            }

            /// <inheritdoc />
            protected override IEnumerator CreateCoroutine()
            {
                Instance.Initialize_Internal();
                foreach (var operation in Instance.InitializationOperations) yield return operation;
                InvokeOnCompleted();
            }

            /// <inheritdoc />
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