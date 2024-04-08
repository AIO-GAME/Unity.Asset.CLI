using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

namespace AIO
{
    internal partial class LoaderHandleLoadSubAsset<TObject> : LoaderHandleList<TObject>
    where TObject : Object
    {
        #region Sync

        protected override void CreateSync()
        {
            Result = AssetSystem.Proxy.LoadSubAssetsSync<TObject>(Address);
        }

        #endregion

        #region CO

        protected override IEnumerator CreateCoroutine()
        {
            return AssetSystem.Proxy.LoadSubAssetsCO<TObject>(Address, OnCompletedCO);
        }

        private void OnCompletedCO(TObject[] asset)
        {
            Result = asset;
            InvokeOnCompleted();
        }

        #endregion

        #region Task

        private void OnCompletedTaskObject()
        {
            Result = AwaiterObject.GetResult();
            InvokeOnCompleted();
        }

        private TaskAwaiter<TObject[]> AwaiterObject;

        protected override TaskAwaiter<TObject[]> CreateAsync()
        {
            AwaiterObject = AssetSystem.Proxy.LoadSubAssetsTask<TObject>(Address).GetAwaiter();
            AwaiterObject.OnCompleted(OnCompletedTaskObject);
            return AwaiterObject;
        }

        #endregion

        #region Constructor

        private LoaderHandleLoadSubAsset(string location, Action<TObject[]> onCompleted) : this(location)
        {
            Completed += onCompleted;
        }

        #endregion

        private LoaderHandleLoadSubAsset(string location)
        {
            Address    = location;
            IsValidate = AssetSystem.CheckLocationValid(Address);
            if (IsValidate)
            {
                if (AssetSystem.ReferenceHandleCount.Increment(Address) == 1)
                {
                    AssetSystem.HandleDic[Address] = this;
                }
            }
            else AssetSystem.LogWarningFormat("资源地址无效: {0}", location);

            IsDone   = !IsValidate;
            Progress = 0;
        }

        protected override void OnDispose()
        {
            if (IsValidate)
            {
                var count = AssetSystem.ReferenceHandleCount.Decrement(Address);
                if (count <= 0)
                {
                    AssetSystem.HandleDic.Remove(Address);
                    AssetSystem.UnloadAsset(Address);
                    Result = default;
                }

                IsValidate = false;
            }

            Address = null;
        }
    }

    internal partial class LoaderHandleLoadSubAsset<TObject>
    {
        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandleList<TObject> Create(string location)
        {
            var key = AssetSystem.SettingToLocalPath(location);
            return AssetSystem.HandleDic.TryGetValue(key, out var handle) && handle is ILoaderHandleList<TObject> assetHandle
                ? assetHandle
                : new LoaderHandleLoadSubAsset<TObject>(key);
        }

        [DebuggerNonUserCode, DebuggerHidden]
        public static ILoaderHandleList<TObject> Create(string location, Action<TObject[]> completed)
        {
            if (completed is null) return Create(location);
            var key = AssetSystem.SettingToLocalPath(location);
            if (AssetSystem.HandleDic.TryGetValue(key, out var handle) && handle is ILoaderHandleList<TObject> assetHandle)
            {
                if (assetHandle.IsDone) completed.Invoke(assetHandle.Result);
                else assetHandle.Completed += completed;
                return assetHandle;
            }

            return new LoaderHandleLoadSubAsset<TObject>(key, completed);
        }
    }
}