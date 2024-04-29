#if SUPPORT_YOOASSET

#region

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using YooAsset;

#endregion

namespace AIO.UEngine.YooAsset
{
    partial class Proxy
    {
        private class ActionUnloadScene : OperationAction
        {
            private readonly string               _location;
            private          SceneOperationHandle _handle;

            public ActionUnloadScene(string location, Action completed) : base(completed)
            {
                _location = AssetSystem.SettingToLocalPath(location);
                if (Instance.ReferenceOPHandle.TryGetValue(_location, out var operation) && operation is SceneOperationHandle handle)
                {
                    _handle    = handle;
                    IsValidate = true;
                    Instance.ReferenceOPHandle.Remove(_location);
                }
            }

            protected override IEnumerator CreateCoroutine()
            {
                yield return _handle.UnloadAsync();
                yield return Resources.UnloadUnusedAssets();

                ReleaseOperationHandle(_handle);
                AssetSystem.LogFormat("Free Scene Handle Release : {0}", _location);
                InvokeOnCompleted();
            }

            /// <inheritdoc />
            protected override void CreateSync()
            {
                _handle.UnloadAsync().Task.RunSynchronously();
                Runner.StartCoroutine(UnloadUnusedAssetsCo(_ =>
                {
                    ReleaseOperationHandle(_handle);
                    AssetSystem.LogFormat("Free Scene Handle Release : {0}", _location);
                    IsDone = true;
                }));
            }


            protected override TaskAwaiter CreateAsync()
            {
                var awaiter = _handle.UnloadAsync().Task.GetAwaiter();
                awaiter.OnCompleted(() =>
                {
                    Runner.StartCoroutine(UnloadUnusedAssetsCo(_ =>
                    {
                        ReleaseOperationHandle(_handle);
                        AssetSystem.LogFormat("Free Scene Handle Release : {0}", _location);
                        InvokeOnCompleted();
                    }));
                });
                return awaiter;
            }
        }

        public override IOperationAction UnloadSceneTask(string location, Action completed = null)
        {
            return new ActionUnloadScene(location, completed);
        }
    }
}
#endif